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
        AudioManager.Instance?.StopMusic();
        AudioManager.Instance?.PlayGameMusic();
        SceneManager.LoadScene("Alexzander");
    }

    public void LoadGardenSecondFruit()
    {
        AudioManager.Instance?.StopMusic();
        AudioManager.Instance?.PlayGameMusic();
        SceneManager.LoadScene("Alexzander2");
    }

    public void LoadGardenThirdFruit()
    {
        AudioManager.Instance?.StopMusic();
        AudioManager.Instance?.PlayGameMusic();
        SceneManager.LoadScene("Alexzander3");
    }

    public void LoadDragonInfo()
    {
        SceneManager.LoadScene("DragonPage");
    }

    public void LoadAkebiaInfo()
    {
        SceneManager.LoadScene("secondFruitPage");
    }

    public void LoadPomegranateInfo()
    {
        SceneManager.LoadScene("thirdFruitPage");
    }

    public void LoadGardenToSasha()
    {
        SceneManager.LoadScene("Alexzander");
    }

    public void LoadMainMenu()
    {
        AudioManager.Instance?.StopMusic();
        AudioManager.Instance?.PlayMenuMusic();
        SceneManager.LoadScene("MainMenu");
    }
}
