using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts.MainMenu
{
    public class MainMenuUIManager : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
}
