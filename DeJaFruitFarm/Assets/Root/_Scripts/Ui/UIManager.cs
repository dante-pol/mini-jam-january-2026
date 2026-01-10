using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager: MonoBehaviour
{
    
    public void LoadCollection()
    {
        SceneManager.LoadScene("Collection");
    }
    
    public void Exit()
    {
        Application.Quit();
    }
    
    
    public void LoadGarden()
    {
        SceneManager.LoadScene("NiceIlya");
    }
}
