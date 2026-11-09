using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private bool isPaused = false;
    private int move_distance = 0;

    public enum GameState { Title,Play,Paused,Over};

    public GameState currentState { get; private set; }

    public int Score {  get; private set; }

    private void Awake()
    {
        // インスタンスがまだ存在しない場合、このインスタンスを唯一のものとして設定
        if (instance == null)
        {
            instance = this;
            // シーンをまたいでこのオブジェクトを維持する
            DontDestroyOnLoad(gameObject);
        }
        // インスタンスが既に存在する場合、この新しいインスタンスは破棄する
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初期状態を設定
        currentState = GameState.Title;
    }

    public void AddScore(int amout)
    {
        if (currentState != GameState.Play) return;
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

    public void Distance(float _move)
    {
        move_distance += (int)_move;
    }

}