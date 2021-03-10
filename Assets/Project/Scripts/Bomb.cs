using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts
{
    public class Bomb : MonoBehaviour
    {
        private int counterToExplode;
        /// <summary>
        /// Counter Text inside the bomb
        /// </summary>
        private TextMesh countText;
            
        // Start is called before the first frame update
        void Start()
        {
            EventManager.Current.onMoveCompleted += onMoveCompleted;
            counterToExplode = Random.Range(5, 9);
            countText = GetComponentInChildren<TextMesh>();
            countText.text = counterToExplode.ToString();
        }

        private void OnDestroy()
        {
            EventManager.Current.onMoveCompleted -= onMoveCompleted;
        }
        
        /// <summary>
        /// Called per Successful rotation and decreases the bombs count.
        /// Triggers Bomb animation.
        /// </summary>
        private void onMoveCompleted()
        {
            --counterToExplode;
            countText.text = counterToExplode.ToString();
            transform.DOShakeScale(4);
            transform.DOShakePosition(4, Vector3.zero);
            if (counterToExplode <= 0)
            {
                EventManager.Current.GameOver();
            }
        }
    }
}
