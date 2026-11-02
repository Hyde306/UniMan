using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private bool isPaused = false;

    public enum GameState { Title,Play,Paused,Over};

    public GameState currentState { get; private set; }

    public int Score {  get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 初期状態を設定
        currentState = GameState.Title;
    }

    public void AddScore(int amout)
    {

    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        // 状態に応じた処理を実行
        switch (newState)
        {
            case GameState.Title:
                // タイトル画面の準備
                break;
            case GameState.Play:
                // ゲームプレイ開始の準備
                Time.timeScale = 1f; // 時間の進行を再開
                break;
            case GameState.Paused:
                // ポーズ処理
                Time.timeScale = 0f; // 時間の進行を停止
                break;
            case GameState.Over:
                // ゲームオーバー処理
                break;
        }
    }
}
