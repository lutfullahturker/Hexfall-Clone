using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts
{
    public class GameManager : MonoBehaviour
    {
        #region Private

        [SerializeField] private GameObject highlightGameObject;
        private int _score = 0;
        private Vector2Int? _selectedHexPosition;
        private HexGroup _selectedHexGroup = null;
        private RotationHandler _rotationHandler;
        private int divisionBy1000;
        private bool stopInput;
        private List<List<GameObject>> gameTable => GridManager.Instance.gameTable;

        #endregion

        #region Public

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

        #endregion

        #region Unity Methods

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

        #endregion

        /// <summary>
        /// Starts the rotation process in the given direction and checks the matches. 
        /// Called when user swiped.
        /// </summary>
        /// <param name="swipeDirection"></param>
        /// <param name="swipePos">The position which swipe starts on the screen</param>
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

        /// <summary>
        /// Calculates rotation direction with using selected hexGroups center and swipe start position.
        /// Rotation direction depends on these positions.
        /// </summary>
        /// <param name="swipeDirection"></param>
        /// <param name="centerPosOfGroup"></param>
        /// <param name="swipePos">The position which swipe starts on the screen</param>
        /// <returns></returns>
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

        /// <summary>
        /// Called when game ends. Triggers game over animation and destroys all Hexes.
        /// </summary>
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
                        });
                }
            }
        }

        /// <summary>
        /// Selects touched hexagon and highlight selected hexGroup.
        /// Every tap on same hexagon switches the selected hexGroups.
        /// Called when user taps a hexagon.
        /// </summary>
        /// <param name="touchedHex">Hexagon gameObject which user touched</param>
        private void OnTouchedHexagon(GameObject touchedHex)
        {
            Debug.Log("Touched GO: " + touchedHex.transform.name);
            
            if (stopInput)
                return;

            var hexComponent = touchedHex.GetComponent<Hex>();
            // if user taps same hex more than once, selected hexGroup will be changed
            _selectedHexGroup = hexComponent.GetNextHexGroup();
            var centerPosition = _selectedHexGroup.GetCenterPositionOfGroup();

            // if touched hex is different from last selected hex, reset selected hexGroup of last selected hex
            if (_selectedHexPosition != null && hexComponent.tablePos != _selectedHexPosition)
            {
                gameTable[_selectedHexPosition.Value.x][_selectedHexPosition.Value.y].GetComponent<Hex>()
                    .ResetSelectedHexGroup();
            }

            _selectedHexPosition = hexComponent.tablePos;
            highlightGameObject.SetActive(true);
            
            // Rotation of highlight sprite depends on which side has 2 hex
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

        /// <summary>
        /// Triggers rotation process of selected hexGroup.
        /// Called when user swipes to any rotation.
        /// </summary>
        /// <param name="swipeDirection"></param>
        /// <param name="swipePosition"></param>
        private void OnSwiped(InputManager.SwipeDirection swipeDirection, Vector2 swipePosition)
        {
            if (stopInput || _selectedHexGroup == null || !_selectedHexPosition.HasValue)
                return;

            StartCoroutine(RotateCoroutine(swipeDirection, swipePosition));
        }

        #endregion
    }
}