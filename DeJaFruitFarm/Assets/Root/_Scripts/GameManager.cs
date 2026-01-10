using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI Панели")]
    [SerializeField] private GameObject mainGamePanel;     // Панель геймплея
    [SerializeField] private GameObject victoryPanel;       // Панель победы
    
    [Header("Итоговый плод")]
    [SerializeField] private UnityEngine.UI.Image finalFruitImage;  // Картинка результата
    
    private Fruit currentFruit;
    
    void Awake()
    {
        // Делаем активной панель победы (скрываем геймплей)
        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (mainGamePanel != null) mainGamePanel.SetActive(false);
        
        // Находим текущий фрукт
        currentFruit = FindObjectOfType<Fruit>();
    }
    
    // Метод конца игры (вызывается из Fruit после 100%)
    public void EndGame()
    {
        // Показываем картинку итогового плода
        if (finalFruitImage != null && currentFruit != null)
        {
            SpriteRenderer plantSprite = currentFruit.GetComponent<SpriteRenderer>();
            finalFruitImage.sprite = plantSprite.sprite;
        }
        
        // Переключаем панели
        if (mainGamePanel != null) mainGamePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }
    
    // Метод для кнопки "Главное меню"
    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
