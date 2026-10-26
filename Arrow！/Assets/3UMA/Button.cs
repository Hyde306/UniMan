using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{

    [SerializeField] private string sceneName;
    
    public void ClickOnButton()
    {
        SceneManager.LoadScene(sceneName);
    }
}
