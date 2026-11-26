using UnityEngine;
using UnityEngine.SceneManagement;

namespace Yoshida.TestScene
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private bool restartCurrentScene;

        public void ClickOnButton()
        {
            Time.timeScale = 1f;

            if (restartCurrentScene)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }

            SceneManager.LoadScene("title");
        }
    }
}
