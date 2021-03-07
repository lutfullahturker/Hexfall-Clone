using UnityEngine;

namespace Project.Scripts
{
    public class InputManager : MonoBehaviour
    {
        private TouchPhase _touchPhase = TouchPhase.Ended;
        private Vector3 _touchWorldPos;
        private Camera mainCam;

        // Start is called before the first frame update
        void Start()
        {
            mainCam = Camera.main;
            Input.multiTouchEnabled = false;
            Input.backButtonLeavesApp = true;
        }

        // Update is called once per frame
        void Update()
        {
            //We check if the first touches phase is Ended (that the finger was lifted)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == _touchPhase)
            {
                //We transform the touch position into word space from screen space and store it.
                _touchWorldPos = mainCam.ScreenToWorldPoint(Input.GetTouch(0).position);

                Vector2 touchWorldPos2D = new Vector2(_touchWorldPos.x, _touchWorldPos.y);
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
        }
    }
}