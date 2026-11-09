
using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    [Header("ゲームオーバー設定")]
    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private string gameOverTag = "GameOver";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(gameOverTag))
        {
            gameOverManager.GameOver();
        }
    }
}
