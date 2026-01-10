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
    [Header("Анимация исчезновения")]
    public float animationDuration = 1.5f; // Длительность анимации
    public float scaleMultiplier = 1.3f; // Во сколько раз увеличиваем старый спрайт
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

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
        UpdateVisual();

        // Если выполнили все 4 действия - обновляем состояние
        if (actionsTaken.Count == REQUIRED_ACTIONS_COUNT)
        {
            Debug.Log($"[FRUIT] Все действия выполнены! Расчет финального результата...");
            CalculateFinalResults();
            OnGrowthComplete?.Invoke(this);

            // Вызываем EndGame из GameManager
            if (gameManager != null)
            {
                Debug.Log($"[FRUIT] Вызов EndGame из GameManager");
                gameManager.EndGame();
            }
            else
            {
                Debug.LogWarning($"[FRUIT] GameManager не найден!");
            }
        }

        Debug.Log($"[FRUIT] Текущая последовательность: [{string.Join(", ", actionsTaken)}]");

        return currentStep + 1;
    }

    // Обновить визуал после действия
    private void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        if (currentStage < growthSprites.Length && growthSprites[currentStage] != null)
        {
            StartCoroutine(ScaleUpOldSprite(growthSprites[currentStage]));
            Debug.Log($"[FRUIT] Спрайт обновлен: growthSprites[{currentStage}]");
        }
        else
        {
            Debug.LogWarning($"[FRUIT] Спрайт для стадии {currentStage} не найден!");
        }
    }

    public bool isAnimating = false;
    private IEnumerator ScaleUpOldSprite(Sprite newSprite)
    {
        // Если уже анимируется, выходим
        if (isAnimating) yield break;
        isAnimating = true;

        // Сохраняем старый спрайт
        Sprite oldSprite = spriteRenderer.sprite;

        // Если это самый первый спрайт (семя), просто показываем новый
        if (oldSprite == null || oldSprite == growthSprites[0])
        {
            spriteRenderer.sprite = newSprite;
            isAnimating = false;
            yield break;
        }

        Debug.Log($"[ANIMATION] Старт анимации. Старый спрайт: {oldSprite.name}, Новый: {newSprite?.name}");

        // 1. СОХРАНЯЕМ текущий sortingOrder основного спрайта
        int originalSortingOrder = spriteRenderer.sortingOrder;

        // 2. Создаем временный объект для старого спрайта
        GameObject oldSpriteObject = new GameObject("OldSpriteScaleUp");
        oldSpriteObject.transform.SetParent(transform, false);
        oldSpriteObject.transform.localPosition = Vector3.zero;
        oldSpriteObject.transform.localScale = Vector3.one;

        SpriteRenderer oldSpriteRenderer = oldSpriteObject.AddComponent<SpriteRenderer>();
        oldSpriteRenderer.sprite = oldSprite;
        oldSpriteRenderer.sortingOrder = originalSortingOrder + 1; // ВЫШЕ основного
        oldSpriteRenderer.color = Color.white; // Явно задаем цвет

        // 3. Сразу показываем новый спрайт в основном рендерере, но прозрачный
        spriteRenderer.sprite = newSprite;
        spriteRenderer.color = new Color(1, 1, 1, 0); // Полностью прозрачный
        spriteRenderer.sortingOrder = originalSortingOrder; // Сохраняем порядок

        // Анимация
        float timer = 0f;

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / animationDuration;

            // 4. Увеличиваем старый спрайт (линейно)
            float scale = 1f + (scaleMultiplier - 1f) * progress;
            oldSpriteObject.transform.localScale = Vector3.one * scale;

            // 5. ОЧЕНЬ МЕДЛЕННОЕ исчезновение старого спрайта
            // Старый спрайт виден первые 80%, потом медленно исчезает
            float fadeStart = 0.8f;
            if (progress < fadeStart)
            {
                // Держим старый спрайт полностью видимым
                oldSpriteRenderer.color = Color.white;
            }
            else
            {
                // Плавное исчезновение последние 20%
                float fadeProgress = (progress - fadeStart) / (1f - fadeStart);
                Color oldColor = oldSpriteRenderer.color;
                oldColor.a = 1f - fadeProgress;
                oldSpriteRenderer.color = oldColor;

                // Логируем для отладки
                if (Mathf.Approximately(fadeProgress, 0.5f))
                {
                    Debug.Log($"[ANIMATION] Старый спрайт альфа: {oldColor.a}");
                }
            }

            // 6. Новый спрайт постепенно появляется с начала
            // Начинаем сразу, но очень медленно
            float newAlpha = progress * 0.8f; // К концу анимации будет 80% прозрачности
            spriteRenderer.color = new Color(1, 1, 1, newAlpha);

            // 7. В конце анимации новый спрайт становится полностью видимым
            if (progress >= 0.95f)
            {
                spriteRenderer.color = Color.white;
            }

            yield return null;
        }

        // 8. Финальные настройки - новый спрайт полностью видим
        spriteRenderer.color = Color.white;
        spriteRenderer.sortingOrder = originalSortingOrder;

        // 9. Уничтожаем временный объект
        Destroy(oldSpriteObject);

        Debug.Log($"[ANIMATION] Анимация завершена");
        isAnimating = false;
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
        // Отрисовываем финальный спрайт
        if (spriteRenderer != null)
        {
            Sprite finalSprite = null;

            if (isPerfect && perfectFruitSprite != null)
            {
                finalSprite = perfectFruitSprite;
                Debug.Log($"[FRUIT] Отрисован идеальный фрукт!");
            }
            else if (mutationCount > 0 && mutationCount <= mutationSprites.Length)
            {
                int mutationIndex = mutationCount - 1;
                if (mutationSprites[mutationIndex] != null)
                {
                    finalSprite = mutationSprites[mutationIndex];
                    Debug.Log($"[FRUIT] Отрисован фрукт с мутацией: mutationSprites[{mutationIndex}]");
                }
            }

            if (finalSprite != null)
            {
                StartCoroutine(ScaleUpOldSprite(finalSprite));
            }
        }


        Debug.Log($"[FRUIT] РЕЗУЛЬТАТ ДЛЯ ФРУКТА '{fruitName}':");
        Debug.Log($"[FRUIT] Качество: {quality}%");
        Debug.Log($"[FRUIT] Мутации: {mutationCount}");
        Debug.Log($"[FRUIT] Идеальный: {isPerfect}");

        SaveManager.SaveFruitResult(fruitName, quality, isPerfect);
    }

    // Публичный метод для сброса фрукта
    public void ResetFruit()
    {
        Debug.Log($"[FRUIT] Сброс фрукта '{fruitName}'");
        Start();
    }
}
