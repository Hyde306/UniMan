
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("ゲームオーバー設定")]
    [SerializeField] private GameObject gameOverPanel;

    private bool isGameOver = false;

    private void Start()
    {
        Time.timeScale = 1f;
        gameOverPanel.SetActive(false);
    }

    // ゲームオーバー処理
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // ゲーム全体を停止
        Time.timeScale = 0f;

        // パネルを表示
        gameOverPanel.SetActive(true);
    }

    // リスタート処理
    public void Restart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
