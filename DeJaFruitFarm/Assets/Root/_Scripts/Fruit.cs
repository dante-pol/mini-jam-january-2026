using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Перечисление возможных типов действий
public enum ActionType
{
    Water,          // Полив
    Fertilizer,     // Удобрение
    Sun,            // Солнце
    SpecialLight,   // Специальный свет
    Prune,          // Обрезка
    Wait            // Ожидание
}

// Результат выполнения действия
public enum ActionResult
{
    Correct,        // Действие есть и выполнено в правильный момент
    WrongOrder,     // Действие есть, но не в правильный момент
    NotInCombo      // Действие отсутствует в комбинации
}

// Класс для передачи данных о результате выращивания
[System.Serializable]
public class FruitGrowthCompleteEvent : UnityEvent<Fruit> { }

public class Fruit : MonoBehaviour
{
    [Header("Настройки фрукта")]
    public string fruitName;
    public ActionType[] idealCombo = new ActionType[4]; // Идеальная комбинация из 4 действий
    public Sprite[] growthSprites = new Sprite[4];
    public Sprite[] mutationSprites = new Sprite[4];
    public Sprite perfectFruitSprite; // Отдельный спрайт для идеального фрукта

    [Header("Состояние")]
    public List<ActionType> actionsTaken = new List<ActionType>();
    public List<ActionResult> actionResults = new List<ActionResult>(); // Результаты каждого действия
    public int currentStage = 0;
    public int quality = 0;
    public bool isPerfect = false;
    public int mutationCount = 0;

    [Header("События")]
    public FruitGrowthCompleteEvent OnGrowthComplete = new FruitGrowthCompleteEvent();

    private const int REQUIRED_ACTIONS_COUNT = 4; // Нужно выполнить 4 действия

    // Инициализация фрукта (можно вызвать при создании)
    public void Start()
    {
        actionsTaken.Clear();
        actionResults.Clear();
        currentStage = 0;
        quality = 0;
        isPerfect = false;
        mutationCount = 0;
    }

    // Выполнить действие (вызывается при нажатии кнопки)
    public int PerformAction(ActionType action)
    {
        if (actionsTaken.Count >= REQUIRED_ACTIONS_COUNT)
        {
            Debug.LogWarning("Все действия уже выполнены!");
            return -1; // Возвращаем -1, если все действия уже выполнены
        }

        int currentStep = actionsTaken.Count; // Номер текущего действия (0-3)
        actionsTaken.Add(action);

        // Проверяем результат действия
        ActionResult result = CheckActionResult(action, currentStep);

        // Сохраняем результат
        actionResults.Add(result);

        // Если выполнили все 4 действия - обновляем состояние и уведомляем
        if (actionsTaken.Count == REQUIRED_ACTIONS_COUNT)
        {
            CalculateFinalResults();

            // Уведомляем всех подписчиков о завершении выращивания
            OnGrowthComplete?.Invoke(this);
        }

        // Возвращаем номер выполненного действия (начиная с 0)
        return currentStep;
    }

    // Проверяем результат выполненного действия
    private ActionResult CheckActionResult(ActionType action, int step)
    {
        // Проверяем, есть ли действие в идеальной комбинации
        bool actionInCombo = false;
        int correctIndex = -1;

        for (int i = 0; i < idealCombo.Length; i++)
        {
            if (idealCombo[i] == action)
            {
                actionInCombo = true;
                if (i == step)
                {
                    correctIndex = i;
                    break;
                }
            }
        }

        if (!actionInCombo)
        {
            return ActionResult.NotInCombo; // Действия нет в комбинации
        }

        if (correctIndex == step)
        {
            return ActionResult.Correct; // Действие в правильном месте и порядке
        }

        return ActionResult.WrongOrder; // Действие есть, но не на своем месте
    }

    // Рассчитываем финальные результаты после выполнения всех действий
    private void CalculateFinalResults()
    {
        // Подсчитываем количество правильных действий
        int correctCount = 0;
        int wrongOrderCount = 0;
        int notInComboCount = 0;

        foreach (var result in actionResults)
        {
            if (result == ActionResult.Correct)
                correctCount++;
            else if (result == ActionResult.WrongOrder)
                wrongOrderCount++;
            else
                notInComboCount++;

        }

        // Определяем качество (0-100)
        quality = correctCount * 25 + wrongOrderCount * 10;
        quality = Mathf.Clamp(quality, 0, 100);

        // Определяем количество мутаций (0-4)
        mutationCount = 4 - correctCount;

        // Проверяем, идеален ли фрукт (все 4 действия выполнены правильно)
        isPerfect = (correctCount == REQUIRED_ACTIONS_COUNT);

        Debug.Log($"Фрукт '{fruitName}' выращен! " +
                  $"Правильных действий: {correctCount}/4, " +
                  $"Качество: {quality}%, " +
                  $"Мутации: {mutationCount}, " +
                  $"Идеальный: {isPerfect}");
    }

}
