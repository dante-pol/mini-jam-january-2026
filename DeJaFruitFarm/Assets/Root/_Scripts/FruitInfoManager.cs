using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FruitInfoManager : MonoBehaviour
{
    [Header("Код фрукта")]
    [SerializeField] private string fruitCode;

    [SerializeField] private Image perfectImage;
    [SerializeField] private Image mutation75Image;
    [SerializeField] private Image mutation50Image;
    [SerializeField] private Image mutation25Image;
    [SerializeField] private Image mutation0Image;

    [Header("Спрайты - Силуэты (заблокированные)")]
    [SerializeField] private Sprite perfectSilhouette;
    [SerializeField] private Sprite mutation75Silhouette;
    [SerializeField] private Sprite mutation50Silhouette;
    [SerializeField] private Sprite mutation25Silhouette;
    [SerializeField] private Sprite mutation0Silhouette;

    [Header("Спрайты - Открытые")]
    [SerializeField] private Sprite perfectUnlocked;
    [SerializeField] private Sprite mutation75Unlocked;
    [SerializeField] private Sprite mutation50Unlocked;
    [SerializeField] private Sprite mutation25Unlocked;
    [SerializeField] private Sprite mutation0Unlocked;

    void Start()
    {
        LoadFruitInfo();
    }

    private void LoadFruitInfo()
    {
        Debug.Log($"[FRUIT INFO] === Загрузка информации о фрукте: {fruitCode} ===");

        bool isPerfectUnlocked = SaveManager.IsPerfectUnlocked(fruitCode);
        List<int> unlockedMutations = SaveManager.GetUnlockedMutations(fruitCode);

        Debug.Log($"[FRUIT INFO] Идеальный фрукт открыт: {isPerfectUnlocked}");
        Debug.Log($"[FRUIT INFO] Открытые мутации: {string.Join(", ", unlockedMutations)}");

        Debug.Log("[FRUIT INFO] --- Обновление Perfect ---");
        UpdateMutation(perfectImage, perfectSilhouette, perfectUnlocked, isPerfectUnlocked, "Perfect");

        Debug.Log("[FRUIT INFO] --- Обновление Mutation75 ---");
        UpdateMutation(mutation75Image, mutation75Silhouette, mutation75Unlocked, unlockedMutations.Contains(75), "Mutation75");

        Debug.Log("[FRUIT INFO] --- Обновление Mutation50 ---");
        UpdateMutation(mutation50Image, mutation50Silhouette, mutation50Unlocked, unlockedMutations.Contains(50), "Mutation50");

        Debug.Log("[FRUIT INFO] --- Обновление Mutation25 ---");
        UpdateMutation(mutation25Image, mutation25Silhouette, mutation25Unlocked, unlockedMutations.Contains(25), "Mutation25");

        Debug.Log("[FRUIT INFO] --- Обновление Mutation0 ---");
        UpdateMutation(mutation0Image, mutation0Silhouette, mutation0Unlocked, unlockedMutations.Contains(0), "Mutation0");
    }

    private void UpdateMutation(Image image, Sprite silhouette, Sprite unlocked, bool isUnlocked, string name)
    {
        if (image == null)
        {
            Debug.LogError($"[FRUIT INFO] {name}: Image НЕ НАЗНАЧЕН!");
            return;
        }

        if (silhouette == null)
        {
            Debug.LogWarning($"[FRUIT INFO] {name}: Silhouette спрайт не назначен!");
        }

        if (unlocked == null)
        {
            Debug.LogWarning($"[FRUIT INFO] {name}: Unlocked спрайт не назначен!");
        }

        Debug.Log($"[FRUIT INFO] {name}: isUnlocked={isUnlocked}");

        if (isUnlocked && unlocked != null)
        {
            image.sprite = unlocked;
            Debug.Log($"[FRUIT INFO] {name}: ✅ Установлен ОТКРЫТЫЙ спрайт '{unlocked.name}'");
        }
        else if (silhouette != null)
        {
            image.sprite = silhouette;
            Debug.Log($"[FRUIT INFO] {name}: 🔒 Установлен СИЛУЭТ '{silhouette.name}'");
        }
    }
}
