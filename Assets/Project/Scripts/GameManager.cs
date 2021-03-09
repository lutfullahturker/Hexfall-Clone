using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject highlightGameObject;

        public static int Score
        {
            get => _score;
            set
            {
                _score = value;
                EventManager.Current.ScoreUpdated();
            }
        }

        private static int _score = 0;
        private List<List<GameObject>> gameTable => GridManager.Instance.gameTable;
        private Vector2Int? _selectedHexPosition;
        private HexGroup _selectedHexGroup = null;
        private bool _isHexGroupRotating = false;
        private RotationHandler _rotationHandler;

        // Start is called before the first frame update
        void Start()
        {
            _rotationHandler = GetComponent<RotationHandler>();
            EventManager.Current.onHexagonTouched += OnTouchedHexagon;
            EventManager.Current.onSwiped += OnSwiped;
        }

        // Update is called once per frame
        void Update()
        {
        }

        void OnDestroy()
        {
            EventManager.Current.onHexagonTouched -= OnTouchedHexagon;
            EventManager.Current.onSwiped -= OnSwiped;
        }

        private IEnumerator RotateCoroutine(InputManager.SwipeDirection swipeDirection, Vector2 swipePos)
        {
            _isHexGroupRotating = true;
            var rotateDirection =
                GetRotateDirection(swipeDirection, _selectedHexGroup.GetCenterPositionOfGroup(), swipePos);

            if (_selectedHexPosition.HasValue)
            {
                yield return StartCoroutine(_rotationHandler.Rotate(_selectedHexGroup, rotateDirection));
            }

            _isHexGroupRotating = false;
        }

        private HexGroup.RotateDirection GetRotateDirection(
            InputManager.SwipeDirection swipeDirection,
            Vector2 centerPosOfGroup,
            Vector2 swipePos)
        {
            var isSwipeAboveCenter = swipePos.y > centerPosOfGroup.y;

            return swipeDirection switch
            {
                InputManager.SwipeDirection.LeftSwipe => isSwipeAboveCenter
                    ? HexGroup.RotateDirection.CounterClockwise
                    : HexGroup.RotateDirection.Clockwise,
                InputManager.SwipeDirection.RightSwipe => isSwipeAboveCenter
                    ? HexGroup.RotateDirection.Clockwise
                    : HexGroup.RotateDirection.CounterClockwise,
                _ => throw new ArgumentOutOfRangeException(nameof(swipeDirection), swipeDirection, null)
            };
        }

        #region Event Callbacks

        private void OnTouchedHexagon(GameObject touchedHex)
        {
            // Debug.Log("Touched GO: " + touchedHex.transform.name);

            var hexComponent = touchedHex.GetComponent<Hex>();
            _selectedHexGroup = hexComponent.GetNextHexGroup();
            var centerPosition = _selectedHexGroup.GetCenterPositionOfGroup();

            if (_selectedHexPosition != null && hexComponent.tablePos != _selectedHexPosition)
            {
                gameTable[_selectedHexPosition.Value.x][_selectedHexPosition.Value.y].GetComponent<Hex>()
                    .ResetSelectedHexGroup();
            }

            _selectedHexPosition = hexComponent.tablePos;
            highlightGameObject.SetActive(true);
            if (_selectedHexGroup.highlightSpriteRotation == HexGroup.HighlightSpriteRotation.LeftTwo)
            {
                highlightGameObject.transform.DORotate(new Vector3(0f, 0), 0);
                centerPosition += new Vector3(0.138f, 0, 0);
            }
            else
            {
                highlightGameObject.transform.DORotate(new Vector3(0f, 180f), 0);
                centerPosition -= new Vector3(0.138f, 0, 0);
            }

            highlightGameObject.transform.position = centerPosition;
        }

        private void OnSwiped(InputManager.SwipeDirection swipeDirection, Vector2 swipePosition)
        {
            if (_isHexGroupRotating || _selectedHexGroup == null || !_selectedHexPosition.HasValue)
            {
                return;
            }

            StartCoroutine(RotateCoroutine(swipeDirection, swipePosition));
        }

        #endregion
    }
}