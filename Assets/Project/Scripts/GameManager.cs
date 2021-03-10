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
        private int _score = 0;
        private Vector2Int? _selectedHexPosition;
        private HexGroup _selectedHexGroup = null;
        private RotationHandler _rotationHandler;
        private int divisionBy1000;
        private bool stopInput;
        private List<List<GameObject>> gameTable => GridManager.Instance.gameTable;

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                if (divisionBy1000 != _score / 1000)
                {
                    EventManager.Current.ThousandScored();
                }

                divisionBy1000 = _score / 1000;
                EventManager.Current.ScoreUpdated(_score);
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            _rotationHandler = GetComponent<RotationHandler>();
            EventManager.Current.onGameOver += OnGameOver;
            EventManager.Current.onHexagonTouched += OnTouchedHexagon;
            EventManager.Current.onSwiped += OnSwiped;
        }

        void OnDestroy()
        {
            EventManager.Current.onGameOver -= OnGameOver;
            EventManager.Current.onHexagonTouched -= OnTouchedHexagon;
            EventManager.Current.onSwiped -= OnSwiped;
        }

        private IEnumerator RotateCoroutine(InputManager.SwipeDirection swipeDirection, Vector2 swipePos)
        {
            stopInput = true;
            highlightGameObject.SetActive(false);
            var rotateDirection =
                GetRotateDirection(swipeDirection, _selectedHexGroup.GetCenterPositionOfGroup(), swipePos);

            if (_selectedHexPosition.HasValue)
            {
                yield return StartCoroutine(_rotationHandler.Rotate(_selectedHexGroup, rotateDirection));
            }

            highlightGameObject.SetActive(true);
            stopInput = false;
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

        private void OnGameOver()
        {
            stopInput = true;
            DOTween.Sequence()
                .Append(highlightGameObject.transform.DOScale(Vector3.zero, 1))
                .OnComplete(() =>
                {
                    highlightGameObject.SetActive(false);
                    stopInput = true;
                });

            foreach (var gameObjects in gameTable)
            {
                foreach (var hex in gameObjects)
                {
                    DOTween.Sequence()
                        .Append(hex.transform.DOShakePosition(1))
                        .Append(hex.transform.DOMoveY(-4, 1))
                        .OnComplete(() =>
                        {
                            Destroy(hex);
                            Time.timeScale = 0;
                        });
                }
            }
        }

        private void OnTouchedHexagon(GameObject touchedHex)
        {
            // Debug.Log("Touched GO: " + touchedHex.transform.name);
            if (stopInput)
                return;

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
            if (stopInput || _selectedHexGroup == null || !_selectedHexPosition.HasValue)
                return;

            StartCoroutine(RotateCoroutine(swipeDirection, swipePosition));
        }

        #endregion
    }
}