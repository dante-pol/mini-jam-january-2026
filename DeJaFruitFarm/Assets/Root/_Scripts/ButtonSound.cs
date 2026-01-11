using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        if (_button != null)
        {
            _button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        AudioManager.Instance?.PlayButtonClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Проверяем, что кнопка активна и интерактивна
        if (_button != null)
        {
            AudioManager.Instance?.PlayButtonHover();
        }
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
