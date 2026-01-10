using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour
{
    [Header("Визуал")]
    [SerializeField] public Image mainImage;           // Основное изображение кнопки
    [SerializeField] public Image usedImage;           // Изображение после использования
    [SerializeField] public TextMeshProUGUI numberText; // Текст с номером действия
    
    [Header("Логика")]
    [SerializeField] public ActionType action;         // Enum действия
    [SerializeField] public Fruit plant;               // Ссылка на растение
    
    private bool isUsed = false;
    private int actionNumber = 0;
    
    void Start()
    {
        // Настройка начального вида
        if (mainImage != null) mainImage.gameObject.SetActive(true);
        if (usedImage != null) usedImage.gameObject.SetActive(false);
        if (numberText != null) numberText.gameObject.SetActive(false);
        
        // Подписка на клик
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }
    
    // Метод для OnClick 
    public void OnButtonClick()
    {
        if (isUsed || plant == null) return;
        
        // Вызываем PerformAction на растении
        actionNumber = plant.PerformAction(action);
        
        if (actionNumber > 0)
        {
            // Меняем вид кнопки
            UseButton();
            
            // Показываем номер действия (1-5)
            ShowActionNumber(actionNumber);
        }
    }
    
    // Приватный метод смены вида кнопки
    void UseButton()
    {
        isUsed = true;
        
        if (mainImage != null) mainImage.gameObject.SetActive(false);
        if (usedImage != null) usedImage.gameObject.SetActive(true);
    }
    
    // Показать номер действия
    void ShowActionNumber(int number)
    {
        if (numberText != null)
        {
            numberText.gameObject.SetActive(true);
            numberText.text = number.ToString();
        }
    }
    
    // Публичный метод для сброса 
    public void ResetButton()
    {
        isUsed = false;
        actionNumber = 0;
        
        if (mainImage != null) mainImage.gameObject.SetActive(true);
        if (usedImage != null) usedImage.gameObject.SetActive(false);
        if (numberText != null) numberText.gameObject.SetActive(false);
    }
}
