using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour
{
    [Header("Визуал")]
    [SerializeField] public Sprite mainSprite;
    [SerializeField] public Sprite usedSprite;
    [SerializeField] public TextMeshProUGUI numberText;

    private Image buttonImage;

    [Header("Логика")]
    [SerializeField] public ActionType action;
    [SerializeField] public Fruit plant;

    private bool isUsed = false;
    private int actionNumber = 0;

    void Start()
    {
        // Получаем компонент Image кнопки
        buttonImage = GetComponent<Image>();

        // Настройка начального вида
        if (buttonImage != null && mainSprite != null)
        {
            buttonImage.sprite = mainSprite;
        }

        if (numberText != null)
        {
            numberText.gameObject.SetActive(false);
        }

        // Подписка на клик
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    // Метод для OnClick
    public void OnButtonClick()
    {
        if (isUsed || plant == null) return;

        Debug.Log($"[BUTTON] Клик по кнопке: {action}");

        // Вызываем PerformAction на растении
        actionNumber = plant.PerformAction(action);

        if (actionNumber > 0)
        {
            // Меняем вид кнопки
            UseButton();
            // Показываем номер действия (1-4)
            ShowActionNumber(actionNumber);
        }
    }

    // Приватный метод смены вида кнопки
    void UseButton()
    {
        isUsed = true;

        if (buttonImage != null && usedSprite != null)
        {
            buttonImage.sprite = usedSprite;
        }
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

        if (buttonImage != null && mainSprite != null)
        {
            buttonImage.sprite = mainSprite;
        }

        if (numberText != null)
        {
            numberText.gameObject.SetActive(false);
        }
    }
}
