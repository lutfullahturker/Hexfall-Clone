using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject highlightGameObject;

        private Vector2Int? _selectedHexPosition;

        // Start is called before the first frame update
        void Start()
        {
            EventManager.Current.onHexagonTouched += OnTouchedHexagon;
            EventManager.Current.onSwiped += OnSwiped;
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnDestroy()
        {
            EventManager.Current.onHexagonTouched -= OnTouchedHexagon;
            EventManager.Current.onSwiped -= OnSwiped;
        }

        private void OnTouchedHexagon(GameObject touchedHex)
        {
            // Debug.Log("Touched GO: " + touchedHex.transform.name);

            var hexComponent = touchedHex.GetComponent<Hex>();
            var hexGroup = hexComponent.GetNextHexGroup();
            var centerPosition = hexGroup.GetCenterPositionOfGroup();

            if (_selectedHexPosition != null && hexComponent.tablePos != _selectedHexPosition)
            {
                GridCreator.gameTable[_selectedHexPosition.Value.x][_selectedHexPosition.Value.y].GetComponent<Hex>()
                    .ResetSelectedHexGroup();
            }

            _selectedHexPosition = hexComponent.tablePos;
            highlightGameObject.SetActive(true);
            if (hexGroup.rotation == HexGroup.Rotation.LeftTwo)
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
            
        }
    }
}