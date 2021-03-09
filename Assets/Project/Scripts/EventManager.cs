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

        public event Action onScoreUpdated;

        public void ScoreUpdated()
        {
            onScoreUpdated?.Invoke();
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