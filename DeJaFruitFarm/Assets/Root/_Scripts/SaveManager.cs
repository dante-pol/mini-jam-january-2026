    using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class FruitSaveData
{
    public string fruitCode;
    public bool perfectVersionUnlocked;
    public List<int> unlockedMutations; // 0, 25, 50, 75, 100

    public FruitSaveData(string code)
    {
        fruitCode = code;
        perfectVersionUnlocked = false;
        unlockedMutations = new List<int>();
    }
}

[System.Serializable]
public class GameSaveData
{
    public List<FruitSaveData> fruits = new List<FruitSaveData>();

    public FruitSaveData GetOrCreateFruit(string code)
    {
        foreach (var fruit in fruits)
        {
            if (fruit.fruitCode == code)
                return fruit;
        }

        var newFruit = new FruitSaveData(code);
        fruits.Add(newFruit);
        return newFruit;
    }
}

public static class SaveManager
{
    private static string SAVE_PATH => Path.Combine(Application.persistentDataPath, "fruit_save.json");
    private static GameSaveData _saveData;
    private static bool _isLoaded = false;

    private static void Load()
    {
        if (_isLoaded) return;

        Debug.Log($"[SAVE] Загрузка из: {SAVE_PATH}");

        if (File.Exists(SAVE_PATH))
        {
            try
            {
                string json = File.ReadAllText(SAVE_PATH);
                _saveData = JsonUtility.FromJson<GameSaveData>(json);
                Debug.Log($"[SAVE] Загружено фруктов: {_saveData.fruits.Count}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SAVE] Ошибка загрузки: {e.Message}");
                _saveData = new GameSaveData();
            }
        }
        else
        {
            _saveData = new GameSaveData();
            Debug.Log("[SAVE] Созданы новые сохранения");
        }

        _isLoaded = true;
    }

    private static void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(_saveData, true);
            File.WriteAllText(SAVE_PATH, json);
            Debug.Log($"[SAVE] Сохранено в: {SAVE_PATH}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SAVE] Ошибка сохранения: {e.Message}");
        }
    }

    // Сохранение результатов
    public static void SaveFruitResult(string fruitCode, int quality, bool isPerfect)
    {
        Load();

        var fruit = _saveData.GetOrCreateFruit(fruitCode);

        // Идеальная версия
        if (isPerfect && !fruit.perfectVersionUnlocked)
        {
            fruit.perfectVersionUnlocked = true;
            Debug.Log($"[SAVE] Открыт идеальный фрукт: {fruitCode}");
        }

        // Добавляем мутацию если её нет
        int mutationType = CalculateMutationType(quality);
        if (!fruit.unlockedMutations.Contains(mutationType))
        {
            fruit.unlockedMutations.Add(mutationType);
            fruit.unlockedMutations.Sort();
        }

        Save();
    }

    // Проверка открыта ли идеальная версия
    public static bool IsPerfectUnlocked(string fruitCode)
    {
        Load();

        var fruit = _saveData.GetOrCreateFruit(fruitCode);
        return fruit.perfectVersionUnlocked;
    }

    // Получить открытые мутации
    public static List<int> GetUnlockedMutations(string fruitCode)
    {
        Load();

        var fruit = _saveData.GetOrCreateFruit(fruitCode);
        return new List<int>(fruit.unlockedMutations);
    }

    // Проверить конкретную мутацию
    public static bool IsMutationUnlocked(string fruitCode, int mutationPercent)
    {
        Load();

        var fruit = _saveData.GetOrCreateFruit(fruitCode);
        return fruit.unlockedMutations.Contains(mutationPercent);
    }

    // Получить статистику
    public static int GetPerfectFruitsCount()
    {
        Load();

        int count = 0;
        foreach (var fruit in _saveData.fruits)
        {
            if (fruit.perfectVersionUnlocked)
                count++;
        }
        return count;
    }

    public static void DeleteAllSaves()
    {
        if (File.Exists(SAVE_PATH))
        {
            File.Delete(SAVE_PATH);
            _saveData = new GameSaveData();
            _isLoaded = true;
            Debug.Log("[SAVE] Все сохранения удалены");
        }
    }

    private static int CalculateMutationType(int quality)
    {
        if (quality == 100) return 100;
        if (quality >= 75) return 75;
        if (quality >= 50) return 50;
        if (quality >= 25) return 25;
        return 0;
    }
}