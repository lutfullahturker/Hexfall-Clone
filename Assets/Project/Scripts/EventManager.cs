using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Current;

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Current = this;
            }
        }

        public event Action<int> onScoreUpdated;

        public void ScoreUpdated(int score)
        {
            onScoreUpdated?.Invoke(score);
        }
        
        public event Action on1000Scored;

        public void ThousandScored()
        {
            on1000Scored?.Invoke();
        }
        
        public event Action onMoveCompleted;

        public void MoveCompleted()
        {
            onMoveCompleted?.Invoke();
        }
        
        public event Action onGameOver;

        public void GameOver()
        {
            onGameOver?.Invoke();
        }

        public event Action<GameObject> onHexagonTouched;

        public void HexagonTouched(GameObject selectedHex)
        {
            onHexagonTouched?.Invoke(selectedHex);
        }

        public event Action<InputManager.SwipeDirection, Vector2> onSwiped;

        public void Swiped(InputManager.SwipeDirection direction, Vector2 swipePosition)
        {
            onSwiped?.Invoke(direction, swipePosition);
        }
    }
}