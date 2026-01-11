using UnityEngine;
using TMPro;

public class ActionCounter : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private Fruit fruit;

    [Header("Формат отображения")]
    [SerializeField] private string textFormat = "{0}/4";

    void Start()
    {
        // Автоматически находим фрукт если не назначен
        if (fruit == null)
        {
            fruit = FindObjectOfType<Fruit>();
            if (fruit != null)
            {
                Debug.Log($"[COUNTER] Фрукт найден автоматически: {fruit.fruitName}");
            }
            else
            {
                Debug.LogError("[COUNTER] Фрукт не найден на сцене!");
            }
        }

        // Автоматически берем текст с этого же объекта
        if (counterText == null)
        {
            counterText = GetComponent<TextMeshProUGUI>();
            if (counterText != null)
            {
                Debug.Log("[COUNTER] TextMeshProUGUI найден на этом объекте");
            }
            else
            {
                Debug.LogError("[COUNTER] TextMeshProUGUI не найден! Добавьте компонент или назначьте вручную.");
            }
        }

        UpdateCounter();
    }

    void Update()
    {
        UpdateCounter();
    }

    private void UpdateCounter()
    {
        if (fruit == null || counterText == null) return;

        int current = fruit.actionsTaken.Count;
        counterText.text = string.Format(textFormat, current);
    }
}
