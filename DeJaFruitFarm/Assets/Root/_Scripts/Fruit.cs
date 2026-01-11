using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ActionType
{
    Water,
    Fertilizer,
    Sun,
    SpecialLight,
    Prune,
    Wait
}

public enum ActionResult
{
    Correct,
    WrongOrder,
    NotInCombo
}

[System.Serializable]
public class FruitGrowthCompleteEvent : UnityEvent<Fruit> { }

public class Fruit : MonoBehaviour
{
    [Header("Настройки фрукта")]
    public string fruitName;
    public ActionType[] idealCombo = new ActionType[4];
    public Sprite[] growthSprites = new Sprite[4];
    public Sprite[] mutationSprites = new Sprite[4];
    public Sprite perfectFruitSprite;

    [Header("Визуал")]
    public SpriteRenderer spriteRenderer;

    [Header("Менеджер")]
    public GameManager gameManager;

    [Header("Состояние")]
    public List<ActionType> actionsTaken = new List<ActionType>();
    public List<ActionResult> actionResults = new List<ActionResult>();
    public int currentStage = 0;
    public int quality = 0;
    public bool isPerfect = false;
    public int mutationCount = 0;

    [Header("События")]
    public FruitGrowthCompleteEvent OnGrowthComplete = new FruitGrowthCompleteEvent();

    private const int REQUIRED_ACTIONS_COUNT = 4;

    public void Start()
    {
        actionsTaken.Clear();
        actionResults.Clear();
        currentStage = 0;
        quality = 0;
        isPerfect = false;
        mutationCount = 0;

        // Получаем SpriteRenderer если не назначен
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Получаем GameManager если не назначен
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Показываем начальный спрайт (семя)
        if (spriteRenderer != null && growthSprites.Length > 0 && growthSprites[0] != null)
        {
            spriteRenderer.sprite = growthSprites[0];
            Debug.Log($"[FRUIT] Установлен начальный спрайт: growthSprites[0]");
        }

        Debug.Log($"[FRUIT] Инициализация фрукта: {fruitName}");
        Debug.Log($"[FRUIT] Идеальная комбинация: [{string.Join(", ", idealCombo)}]");
    }

    // Выполнить действие (вызывается при нажатии кнопки)
    public int PerformAction(ActionType action)
    {
        Debug.Log($"[FRUIT] Попытка выполнить действие: {action}");

        if (actionsTaken.Count >= REQUIRED_ACTIONS_COUNT)
        {
            Debug.LogWarning($"[FRUIT] Все действия уже выполнены!");
            return -1;
        }

        int currentStep = actionsTaken.Count;
        actionsTaken.Add(action);

        Debug.Log($"[FRUIT] Действие добавлено. Шаг: {currentStep + 1}/{REQUIRED_ACTIONS_COUNT}");

        // Проверяем результат действия
        ActionResult result = CheckActionResult(action, currentStep);
        actionResults.Add(result);

        Debug.Log($"[FRUIT] Результат: {result}");

        // Обновляем стадию роста
        currentStage = currentStep + 1;

        // Показываем следующий спрайт роста
        StartCoroutine(UpdateVisualAfterDelay());

        // Если выполнили все 4 действия - обновляем состояние
        if (actionsTaken.Count == REQUIRED_ACTIONS_COUNT)
        {
            Debug.Log($"[FRUIT] Все действия выполнены! Расчет финального результата...");
            CalculateFinalResults();
            OnGrowthComplete?.Invoke(this);

            // Запускаем корутину с задержкой
            StartCoroutine(ShowVictoryScreenAfterFinalAnimation());
        }

        Debug.Log($"[FRUIT] Текущая последовательность: [{string.Join(", ", actionsTaken)}]");

        return currentStep + 1;
    }

    private IEnumerator UpdateVisualAfterDelay()
    {
        Debug.Log($"[FRUIT] Ожидание завершения анимации действия...");

        // Ждём 3 секунды (пока анимация действия проиграется)
        yield return new WaitForSeconds(3.4f);

        Debug.Log($"[FRUIT] Обновление визуала растения");
        UpdateVisual();
    }

    private IEnumerator ShowVictoryScreenAfterFinalAnimation()
    {
        Debug.Log($"[FRUIT] Ожидание анимации действия (3 сек)...");

        // Ждём анимацию действия (лейка, солнце и т.д.)
        yield return new WaitForSeconds(3f);

        Debug.Log($"[FRUIT] Показ финального спрайта");
        ShowFinalSprite(); // Показываем финальный спрайт ЗДЕСЬ

        Debug.Log($"[FRUIT] Ожидание анимации роста (0.5 сек)...");

        // Ждём анимацию роста (ScaleAnimation)
        yield return new WaitForSeconds(0f);

        Debug.Log($"[FRUIT] Показ экрана победы");

        if (gameManager != null)
        {
            gameManager.EndGame();
        }
        else
        {
            Debug.LogWarning($"[FRUIT] GameManager не найден!");
        }
    }

    // Обновить визуал после действия
    private void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        // Просто показываем следующий спрайт из growthSprites
        if (currentStage < growthSprites.Length && growthSprites[currentStage] != null)
        {
            spriteRenderer.sprite = growthSprites[currentStage];
            StartCoroutine(ScaleAnimation()); // Запускаем анимацию
            Debug.Log($"[FRUIT] Спрайт обновлен: growthSprites[{currentStage}]");
        }
        else
        {
            Debug.LogWarning($"[FRUIT] Спрайт для стадии {currentStage} не найден!");
        }
    }

    // Проверяем результат выполненного действия
    private ActionResult CheckActionResult(ActionType action, int step)
    {
        Debug.Log($"[FRUIT] Проверка действия '{action}' на шаге {step}");
        Debug.Log($"[FRUIT] Ожидаемое действие: {idealCombo[step]}");

        bool actionInCombo = false;
        int correctIndex = -1;

        for (int i = 0; i < idealCombo.Length; i++)
        {
            if (idealCombo[i] == action)
            {
                actionInCombo = true;
                correctIndex = i;
                Debug.Log($"[FRUIT] Действие найдено в комбинации на позиции {i}");
                break;
            }
        }

        if (!actionInCombo)
        {
            Debug.Log($"[FRUIT] Действие отсутствует в идеальной комбинации");
            return ActionResult.NotInCombo;
        }

        if (correctIndex == step)
        {
            Debug.Log($"[FRUIT] Действие выполнено в правильный момент");
            return ActionResult.Correct;
        }

        Debug.Log($"[FRUIT] Действие есть, но не в правильном порядке");
        return ActionResult.WrongOrder;
    }

    // Рассчитываем финальные результаты после выполнения всех действий
    private void CalculateFinalResults()
    {
        Debug.Log($"[FRUIT] РАСЧЕТ ФИНАЛЬНОГО РЕЗУЛЬТАТА");

        int correctCount = 0;
        int wrongOrderCount = 0;
        int notInComboCount = 0;

        for (int i = 0; i < actionResults.Count; i++)
        {
            Debug.Log($"[FRUIT] Шаг {i + 1}: {actionsTaken[i]} -> {actionResults[i]}");

            if (actionResults[i] == ActionResult.Correct)
                correctCount++;
            else if (actionResults[i] == ActionResult.WrongOrder)
                wrongOrderCount++;
            else
                notInComboCount++;
        }

        Debug.Log($"[FRUIT] Правильных действий: {correctCount}/4");
        Debug.Log($"[FRUIT] Неправильный порядок: {wrongOrderCount}");
        Debug.Log($"[FRUIT] Не в комбинации: {notInComboCount}");

        // Определяем качество (0-100)
        quality = correctCount * 25;
        quality = Mathf.Clamp(quality, 0, 100);

        // Определяем количество мутаций (0-4)
        mutationCount = 4 - correctCount;

        // Проверяем, идеален ли фрукт
        isPerfect = (correctCount == REQUIRED_ACTIONS_COUNT);

        /*
        // Отрисовываем финальный спрайт
        if (spriteRenderer != null)
        {
            if (isPerfect && perfectFruitSprite != null)
            {
                // Все 4 действия правильные - показываем идеальный фрукт
                spriteRenderer.sprite = perfectFruitSprite;
                Debug.Log($"[FRUIT] Отрисован идеальный фрукт!");
            }
            else if (mutationCount > 0 && mutationCount <= mutationSprites.Length)
            {
                // Есть мутации - показываем соответствующий спрайт
                int mutationIndex = mutationCount - 1;
                if (mutationSprites[mutationIndex] != null)
                {
                    spriteRenderer.sprite = mutationSprites[mutationIndex];
                    Debug.Log($"[FRUIT] Отрисован фрукт с мутацией: mutationSprites[{mutationIndex}] (Correct действий: {correctCount})");
                }
            }
        }
        */

        Debug.Log($"[FRUIT] РЕЗУЛЬТАТ ДЛЯ ФРУКТА '{fruitName}':");
        Debug.Log($"[FRUIT] Качество: {quality}%");
        Debug.Log($"[FRUIT] Мутации: {mutationCount}");
        Debug.Log($"[FRUIT] Идеальный: {isPerfect}");

        SaveManager.SaveFruitResult(fruitName, quality, isPerfect);
    }

    private void ShowFinalSprite()
    {
        if (spriteRenderer == null) return;

        if (isPerfect && perfectFruitSprite != null)
        {
            // Все 4 действия правильные - показываем идеальный фрукт
            spriteRenderer.sprite = perfectFruitSprite;
            StartCoroutine(ScaleAnimation());
            Debug.Log($"[FRUIT] Отрисован идеальный фрукт!");
        }
        else if (mutationCount > 0 && mutationCount <= mutationSprites.Length)
        {
            // Есть мутации - показываем соответствующий спрайт
            int mutationIndex = mutationCount - 1;
            if (mutationSprites[mutationIndex] != null)
            {
                spriteRenderer.sprite = mutationSprites[mutationIndex];
                StartCoroutine(ScaleAnimation());
                Debug.Log($"[FRUIT] Отрисован фрукт с мутацией: mutationSprites[{mutationIndex}]");
            }
        }
    }

    // Публичный метод для сброса фрукта
    public void ResetFruit()
    {
        Debug.Log($"[FRUIT] Сброс фрукта '{fruitName}'");
        Start();
    }

    // Добавьте в класс Fruit
    private IEnumerator ScaleAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        // Увеличение
        float duration = 0.2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        // Уменьшение обратно
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
