using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts.MainMenu
{
    public class MainMenuUIManager : MonoBehaviour
    {
        void Start()
        {
            Input.multiTouchEnabled = false;
            Input.backButtonLeavesApp = true;
        }

        /// <summary>
        /// Called when new game button clicked.
        /// </summary>
        public void StartGame()
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
}
