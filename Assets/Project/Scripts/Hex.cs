using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts
{
    public class Hex : MonoBehaviour
    {
        [HideInInspector]
        public Vector2Int tablePos;

        [HideInInspector] public Color color;
        
        [HideInInspector] public List<GameObject> neighbours;
    
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
