using System;
using Lean.Touch;
using UnityEngine;

namespace Project.Scripts
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private LeanFingerSwipe _leftSwipeDetector, _rightSwipeDetector;
        private LeanFingerTap _tapDetector;
        private Camera mainCam;

        public enum SwipeDirection
        {
            LeftSwipe,
            RightSwipe
        }

        // Start is called before the first frame update
        void Start()
        {
            mainCam = Camera.main;
            Input.multiTouchEnabled = false;
            Input.backButtonLeavesApp = true;
            _tapDetector = GetComponent<LeanFingerTap>();
            
            _tapDetector.OnFinger.AddListener(OnTap);
            _leftSwipeDetector.OnFinger.AddListener(OnLeftSwipe);
            _rightSwipeDetector.OnFinger.AddListener(OnRightSwipe);
        }

        private void OnDestroy()
        {
            _tapDetector.OnFinger.RemoveListener(OnTap);
            _leftSwipeDetector.OnFinger.RemoveListener(OnLeftSwipe);
            _rightSwipeDetector.OnFinger.RemoveListener(OnRightSwipe);
        }

        private void OnTap(LeanFinger finger)
        {
            //We transform the touch position into word space from screen space and store it.
            Vector3 touchWorldPos = mainCam.ScreenToWorldPoint(finger.ScreenPosition);

            Vector2 touchWorldPos2D = new Vector2(touchWorldPos.x, touchWorldPos.y);
            //We now raycast with this information. If we have hit something we can process it.
            RaycastHit2D hitInformation = Physics2D.Raycast(touchWorldPos2D, mainCam.transform.forward);

            if (hitInformation.collider != null)
            {
                //We should have hit something with a 2D Physics collider!
                GameObject touchedObject = hitInformation.transform.gameObject;
                //touchedObject should be the hexagon.
                if (touchedObject.CompareTag("hexagon"))
                {
                    // Notify all observer objects
                    EventManager.Current.HexagonTouched(touchedObject);
                }
            }
        }
        
        private void OnLeftSwipe(LeanFinger finger)
        {
            // Debug.Log("Left Swiped");
            EventManager.Current.Swiped(SwipeDirection.LeftSwipe, finger.GetStartWorldPosition(-10));
        }
        
        private void OnRightSwipe(LeanFinger finger)
        {
            // Debug.Log("Right Swiped");
            EventManager.Current.Swiped(SwipeDirection.RightSwipe, finger.GetStartWorldPosition(-10));
        }
    }
}