using System;
using UnityEngine;

namespace Project.Scripts
{
    public class GameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            EventManager.Current.onHexagonTouched += OnTouchedHexagon;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnDestroy()
        {
            EventManager.Current.onHexagonTouched -= OnTouchedHexagon;
        }

        private void OnTouchedHexagon(GameObject touchedHex)
        {
            Debug.Log("Touched GO: " + touchedHex.transform.name);

        }
    }
}
