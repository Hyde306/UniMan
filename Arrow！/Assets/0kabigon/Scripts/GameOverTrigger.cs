
using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    [Header("ゲームオーバー設定")]
    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private string gameOverTag = "GameOver";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(gameOverTag))
        {
            gameOverManager.GameOver();
        }
    }
}
