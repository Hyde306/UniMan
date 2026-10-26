using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{

    [SerializeField] private string sceneName;
    string str;
    
    public void ClickOnButton()
    {

        SceneManager.LoadScene(sceneName);
    }
}
