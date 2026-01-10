using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager: MonoBehaviour
{
    // Метод 1: Переход на сцену коллекцию
    public void LoadCollection()
    {
        SceneManager.LoadScene("Collection");
    }
    
    // Метод 2: Выход из игры
    public void Exit()
    {
        Application.Quit();
    }
}
