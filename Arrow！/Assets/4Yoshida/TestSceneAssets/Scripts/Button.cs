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
                Scene scene = gameObject.scene;
#if UNITY_EDITOR
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                    scene.path, new LoadSceneParameters(LoadSceneMode.Single));
#else
                SceneManager.LoadScene(scene.buildIndex);
#endif
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
