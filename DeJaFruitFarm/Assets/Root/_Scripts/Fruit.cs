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
    public float fadeOutDuration = 0.3f; // Длительность исчезновения старого спрайта
    public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Coroutine currentAnimation;


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

        // Просто показываем следующий спрайт из growthSprites
        if (currentStage < growthSprites.Length && growthSprites[currentStage] != null)
        {
            StartCoroutine(FadeOutOldSprite(growthSprites[currentStage]));
            // ИЛИ Вариант 2: Уменьшение с вращением
            //StartCoroutine(ShrinkOldSprite(growthSprites[currentStage]));

            // ИЛИ Вариант 3: Эффект пузырьков
            //StartCoroutine(BubbleFadeOldSprite(growthSprites[currentStage]));
            Debug.Log($"[FRUIT] Спрайт обновлен: growthSprites[{currentStage}]");
        }
        else
        {
            Debug.LogWarning($"[FRUIT] Спрайт для стадии {currentStage} не найден!");
        }
    }

    // Анимация исчезновения старого спрайта
    private IEnumerator FadeOutOldSprite(Sprite newSprite)
    {
        // Останавливаем предыдущую анимацию, если она есть
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        // Сохраняем старый спрайт
        Sprite oldSprite = spriteRenderer.sprite;

        // Если это самый первый спрайт (семя), просто показываем новый
        if (oldSprite == null || currentStage == 0)
        {
            spriteRenderer.sprite = newSprite;
            yield break;
        }

        // Создаем временный объект для старого спрайта
        GameObject oldSpriteObject = new GameObject("OldSpriteFadeOut");
        oldSpriteObject.transform.SetParent(transform, false);
        oldSpriteObject.transform.localPosition = Vector3.zero;

        SpriteRenderer oldSpriteRenderer = oldSpriteObject.AddComponent<SpriteRenderer>();
        oldSpriteRenderer.sprite = oldSprite;
        oldSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        oldSpriteRenderer.color = spriteRenderer.color;

        // Немного смещаем старый спрайт для эффекта
        oldSpriteObject.transform.localPosition = new Vector3(0, 0.1f, 0);

        // Сразу показываем новый спрайт (полупрозрачный)
        spriteRenderer.sprite = newSprite;
        Color newColor = spriteRenderer.color;
        newColor.a = 0.7f; // Полупрозрачный в начале
        spriteRenderer.color = newColor;

        // Анимация исчезновения старого спрайта
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeOutDuration;
            float curveValue = fadeOutCurve.Evaluate(progress);

            // Старый спрайт исчезает и смещается вверх
            Color oldColor = oldSpriteRenderer.color;
            oldColor.a = curveValue;
            oldSpriteRenderer.color = oldColor;

            // Слегка поднимаем старый спрайт
            Vector3 pos = oldSpriteObject.transform.localPosition;
            pos.y = 0.1f + progress * 0.2f; // Поднимаем вверх
            oldSpriteObject.transform.localPosition = pos;

            // Новый спрайт становится полностью видимым
            if (progress > 0.5f)
            {
                newColor.a = 0.7f + (progress - 0.5f) * 0.6f; // От 0.7 до 1.0
                spriteRenderer.color = newColor;
            }

            yield return null;
        }

        // Финальные значения
        spriteRenderer.color = Color.white;

        // Уничтожаем временный объект
        Destroy(oldSpriteObject);

        currentAnimation = null;
    }

    // Вариант 2: Анимация уменьшения старого спрайта
    private IEnumerator ShrinkOldSprite(Sprite newSprite)
    {
        Sprite oldSprite = spriteRenderer.sprite;

        if (oldSprite == null || currentStage == 0)
        {
            spriteRenderer.sprite = newSprite;
            yield break;
        }

        // Создаем временный объект
        GameObject oldSpriteObject = new GameObject("OldSpriteShrink");
        oldSpriteObject.transform.SetParent(transform, false);
        oldSpriteObject.transform.localPosition = Vector3.zero;
        oldSpriteObject.transform.localScale = Vector3.one;

        SpriteRenderer oldSpriteRenderer = oldSpriteObject.AddComponent<SpriteRenderer>();
        oldSpriteRenderer.sprite = oldSprite;
        oldSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

        // Сразу показываем новый спрайт
        spriteRenderer.sprite = newSprite;

        // Анимация уменьшения старого спрайта
        float timer = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeOutDuration;

            // Плавное уменьшение
            oldSpriteObject.transform.localScale = Vector3.Lerp(startScale, endScale, progress);

            // Немного вращаем
            oldSpriteObject.transform.Rotate(0, 0, 45 * Time.deltaTime);

            yield return null;
        }

        Destroy(oldSpriteObject);
    }

    // Вариант 3: Анимация "всплывания пузырьков" для старого спрайта
    private IEnumerator BubbleFadeOldSprite(Sprite newSprite)
    {
        Sprite oldSprite = spriteRenderer.sprite;

        if (oldSprite == null || currentStage == 0)
        {
            spriteRenderer.sprite = newSprite;
            yield break;
        }

        GameObject oldSpriteObject = new GameObject("OldSpriteBubble");
        oldSpriteObject.transform.SetParent(transform, false);

        SpriteRenderer oldSpriteRenderer = oldSpriteObject.AddComponent<SpriteRenderer>();
        oldSpriteRenderer.sprite = oldSprite;
        oldSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

        // Показываем новый спрайт
        spriteRenderer.sprite = newSprite;

        // Создаем эффект "пузырьков" через несколько маленьких объектов
        int bubbleCount = 8;
        GameObject[] bubbles = new GameObject[bubbleCount];

        for (int i = 0; i < bubbleCount; i++)
        {
            bubbles[i] = new GameObject($"Bubble_{i}");
            bubbles[i].transform.SetParent(oldSpriteObject.transform, false);

            SpriteRenderer bubbleRenderer = bubbles[i].AddComponent<SpriteRenderer>();
            bubbleRenderer.sprite = oldSprite;
            bubbleRenderer.sortingOrder = oldSpriteRenderer.sortingOrder;

            // Маска для отображения только части спрайта
            // (здесь нужен шейдер для маскирования, но для простоты сделаем через scale)
            bubbles[i].transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            bubbles[i].transform.localPosition = new Vector3(
                UnityEngine.Random.Range(-0.5f, 0.5f),
                UnityEngine.Random.Range(-0.5f, 0.5f),
                0
            );
        }

        // Анимация "всплывания" пузырьков
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeOutDuration;

            // Старый спрайт становится прозрачным
            Color color = oldSpriteRenderer.color;
            color.a = 1f - progress;
            oldSpriteRenderer.color = color;

            // Пузырьки разлетаются
            for (int i = 0; i < bubbleCount; i++)
            {
                if (bubbles[i] != null)
                {
                    Vector3 pos = bubbles[i].transform.localPosition;
                    pos.y += Time.deltaTime * 2f; // Всплывают вверх
                    pos.x += Mathf.Sin(Time.time * 10f + i) * 0.01f; // Легкое колебание
                    bubbles[i].transform.localPosition = pos;

                    // Уменьшаются
                    float scale = 0.3f * (1f - progress);
                    bubbles[i].transform.localScale = new Vector3(scale, scale, 1f);

                    // Становятся прозрачными
                    SpriteRenderer br = bubbles[i].GetComponent<SpriteRenderer>();
                    Color bubbleColor = br.color;
                    bubbleColor.a = 1f - progress;
                    br.color = bubbleColor;
                }
            }

            yield return null;
        }

        // Уничтожаем все объекты
        for (int i = 0; i < bubbleCount; i++)
        {
            if (bubbles[i] != null)
                Destroy(bubbles[i]);
        }
        Destroy(oldSpriteObject);
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
                StartCoroutine(FadeOutOldSprite(finalSprite));
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
