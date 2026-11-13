using UnityEngine;

namespace Yoshida.TestScene
{
    public class GameOverTrigger : MonoBehaviour
    {
        [SerializeField] private GameOverManager gameOverManager;
        [SerializeField] private string gameOverTag = "GameOver";

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag(gameOverTag))
            {
                gameOverManager.GameOver();
            }
        }
    }
}
