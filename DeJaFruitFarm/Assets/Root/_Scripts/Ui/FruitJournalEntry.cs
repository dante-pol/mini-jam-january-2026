using UnityEngine;

public class FruitJournalEntry : MonoBehaviour
{
    [SerializeField] private string fruitCode;           // Код фрукта из Fruit.cs
    [SerializeField] private Sprite silhouetteSprite;    // Силуэт
    [SerializeField] private Sprite perfectSprite;       // Идеальный фрукт

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError($"Image не найден на {gameObject.name}!");
            return;
        }

        bool isPerfect = SaveManager.IsPerfectUnlocked(fruitCode);
        spriteRenderer.sprite = isPerfect ? perfectSprite : silhouetteSprite;
    }
}
