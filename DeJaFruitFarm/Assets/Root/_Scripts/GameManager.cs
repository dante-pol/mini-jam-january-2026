using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Панели")]
    [SerializeField] private GameObject mainGamePanel;
    [SerializeField] private GameObject victoryPanel;
    
    [Header("Итоговый плод")]
    [SerializeField] private UnityEngine.UI.Image finalFruitImage;
    
    [Header("Спрайты действий (6 штук)")]
    [SerializeField] private Sprite waterActionSprite;
    [SerializeField] private Sprite fertilizerSprite;
    [SerializeField] private Sprite sunSprite;
    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite pruneSprite;
    [SerializeField] private Sprite waitSprite;
    
    [Header("4 объекта для показа комбинации")]
    [SerializeField] private Image[] actionResultSlots = new Image[4];  // Уже существующие 4 Image
    
    [Header("UI результаты")]
    [SerializeField] private TMPro.TextMeshProUGUI resultText;
    [SerializeField] private TMPro.TextMeshProUGUI fruitNameText;
    
    private Fruit currentFruit;
    private Sprite[] actionSprites;
    
    void Awake()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (mainGamePanel != null) mainGamePanel.SetActive(true);
        
        currentFruit = FindObjectOfType<Fruit>();
        
        // Массив спрайтов действий
        actionSprites = new Sprite[6] {
            waterActionSprite,      // Water
            fertilizerSprite,       // Fertilizer
            sunSprite,              // Sun
            lightSprite,            // SpecialLight
            pruneSprite,            // Prune
            waitSprite              // Wait
        };
    }
    
    public void EndGame()
    {
        if (currentFruit == null) return;
        
        // ★ Итоговый спрайт как было ★
        if (finalFruitImage != null && currentFruit != null)
        {
            SpriteRenderer plantSprite = currentFruit.GetComponent<SpriteRenderer>();
            finalFruitImage.sprite = plantSprite.sprite;
        }
        
        // ★ НОВОЕ: Показ комбинации действий ★
        ShowPlayerActionCombo();
        
        // Имя и качество
        if (fruitNameText != null) fruitNameText.text = currentFruit.fruitName;
        if (resultText != null) resultText.text = $"{currentFruit.quality}%";
        
        // Переключение панелей
        if (mainGamePanel != null) mainGamePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }
    
    // ★ ГЛАВНЫЙ МЕТОД: показывает комбинацию в 4 слота ★
    void ShowPlayerActionCombo()
    {
        if (currentFruit == null || actionResultSlots == null) return;
        
        for (int i = 0; i < 4 && i < actionResultSlots.Length; i++)
        {
            Image slot = actionResultSlots[i];
            if (slot == null) continue;
            
            if (i < currentFruit.actionsTaken.Count)
            {
                // Устанавливаем спрайт действия
                ActionType action = currentFruit.actionsTaken[i];
                Sprite actionSprite = GetActionSprite(action);
                slot.sprite = actionSprite;
                
                // Цвет по результату
                Color slotColor = GetActionColor(currentFruit.actionResults[i]);
                slot.color = slotColor;
                
                slot.gameObject.SetActive(true);
            }
            else
            {
                // Пустой слот если действий меньше 4
                slot.gameObject.SetActive(false);
            }
        }
    }
    
    Sprite GetActionSprite(ActionType action)
    {
        int index = (int)action;
        return index < actionSprites.Length ? actionSprites[index] : null;
    }
    
    Color GetActionColor(ActionResult result)
    {
        return result switch
        {
            ActionResult.Correct => Color.green,      // ✅ Правильно
            ActionResult.WrongOrder => Color.yellow,  // ⚠️ Есть, но не там
            ActionResult.NotInCombo => Color.red,     // ❌ Нет в комбинации
            _ => Color.white
        };
    }
    
    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    public void NextFruit()
    {
        if (currentFruit != null) currentFruit.ResetFruit();
        
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (mainGamePanel != null) mainGamePanel.SetActive(true);
    }
}
