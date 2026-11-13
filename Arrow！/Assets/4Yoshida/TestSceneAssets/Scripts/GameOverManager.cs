using UnityEngine;

namespace Yoshida.TestScene
{
    public class GameOverManager : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;

        private bool pausedByThisPanel;

        private void Awake()
        {
            gameOverPanel.SetActive(false);
        }

        public void GameOver()
        {
            if (Time.timeScale == 0f)
            {
                return;
            }

            pausedByThisPanel = true;
            Time.timeScale = 0f;
            gameOverPanel.SetActive(true);
        }

        private void OnDestroy()
        {
            if (pausedByThisPanel && Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
            }
        }
    }
}
