using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace Yoshida.TestScene
{
    public class GameOverManager : MonoBehaviour
    {
        [SerializeField] private GameObject      gameOverPanel;
        [SerializeField] private TextMeshProUGUI WinText;
        [SerializeField] private SpriteRenderer[] CharImage;
        [SerializeField] private  Image           WinnerImage;

        private bool pausedByThisPanel;

        private void Awake()
        {
            gameOverPanel.SetActive(false);
        }

        public void GameOver(GameOverTrigger Loser)
        {
            if (Time.timeScale == 0f)
            {
                return;
            }


            int Winner = 1;
            if (int.Parse(Regex.Match(Loser.name, @"\d+").Value) == 1)
            {
                Winner = 2;
                WinText.color = Color.red;
            }
            else if (int.Parse(Regex.Match(Loser.name, @"\d+").Value) == 2)
            {
                Winner = 1;
                WinText.color = Color.blue;
            }

            if (Winner - 1 >= 0 && CharImage[Winner - 1] != null)
            WinnerImage.sprite = CharImage[Winner - 1].sprite;

            WinText.text = "Player" + Winner + "èüóòÅI";
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
