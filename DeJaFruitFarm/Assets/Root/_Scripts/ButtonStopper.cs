using UnityEngine;
using UnityEngine.UI;

public class ButtonStopper : MonoBehaviour
{
    [Header("Action buttons")]
    [SerializeField] private Button _wateringButton;
    [SerializeField] private Button _sunlightButton;
    [SerializeField] private Button _uvLampButton;
    [SerializeField] private Button _pruningButton;
    [SerializeField] private Button _fertilizingButton;
    [SerializeField] private Button _waitingButton;

    [Header("Ref to anim")]
    [SerializeField] private ActionAnimator _actionAnimator;

    private Button[] _allButtons;
    private bool[] _previousButtonStates;
    private bool _isAnimationPlaying = false;

    private void Awake()
    {
        _allButtons = new Button[]
        {
            _wateringButton,
            _sunlightButton,
            _uvLampButton,
            _pruningButton,
            _fertilizingButton,
            _waitingButton
        };

        _previousButtonStates = new bool[_allButtons.Length];

        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {
        if (_wateringButton != null)
            _wateringButton.onClick.AddListener(() => PlayAnimation(ActionType.Watering));

        if (_sunlightButton != null)
            _sunlightButton.onClick.AddListener(() => PlayAnimation(ActionType.Sunlight));

        if (_uvLampButton != null)
            _uvLampButton.onClick.AddListener(() => PlayAnimation(ActionType.UVLamp));

        if (_pruningButton != null)
            _pruningButton.onClick.AddListener(() => PlayAnimation(ActionType.Pruning));

        if (_fertilizingButton != null)
            _fertilizingButton.onClick.AddListener(() => PlayAnimation(ActionType.Fertilizing));

        if (_waitingButton != null)
            _waitingButton.onClick.AddListener(() => PlayAnimation(ActionType.Waiting));
    }

    private void PlayAnimation(ActionType actionType)
    {
        if (_isAnimationPlaying || _actionAnimator == null) return;

        SaveButtonStates();

        SetButtonsInteractable(false);
        _isAnimationPlaying = true;

        switch (actionType)
        {
            case ActionType.Watering:
                StartCoroutine(PlayAnimationWithUnlock(() => _actionAnimator.PlayWateringAnimation()));
                break;
            case ActionType.Sunlight:
                StartCoroutine(PlayAnimationWithUnlock(() => _actionAnimator.PlaySunlightAnimation()));
                break;
            case ActionType.UVLamp:
                StartCoroutine(PlayAnimationWithUnlock(() => _actionAnimator.PlayUVLampAnimation()));
                break;
            case ActionType.Pruning:
                StartCoroutine(PlayAnimationWithUnlock(() => _actionAnimator.PlayPruningAnimation()));
                break;
            case ActionType.Fertilizing:
                StartCoroutine(PlayAnimationWithUnlock(() => _actionAnimator.PlayFertilizingAnimation()));
                break;
            case ActionType.Waiting:
                StartCoroutine(PlayAnimationWithUnlock(() => _actionAnimator.PlayWaitingAnimation()));
                break;
        }
    }

    private System.Collections.IEnumerator PlayAnimationWithUnlock(System.Action animationMethod)
    {
        animationMethod?.Invoke();

        yield return new WaitForSeconds(GetMaxAnimationDuration());

        RestoreButtonStates();
        _isAnimationPlaying = false;
    }

    private void SaveButtonStates()
    {
        for (int i = 0; i < _allButtons.Length; i++)
        {
            if (_allButtons[i] != null)
            {
                _previousButtonStates[i] = _allButtons[i].interactable;
            }
        }
    }

    private void RestoreButtonStates()
    {
        for (int i = 0; i < _allButtons.Length; i++)
        {
            if (_allButtons[i] != null)
            {
                ActionButton actionButton = _allButtons[i].GetComponent<ActionButton>();

                // Проверяем через свойство IsUsed
                bool isUsed = actionButton != null && actionButton.IsUsed;

                // Восстанавливаем только не использованные кнопки
                if (_previousButtonStates[i] && !isUsed)
                {
                    _allButtons[i].interactable = true;
                }
            }
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Button button in _allButtons)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }

    private float GetMaxAnimationDuration()
    {
        return 3f;
    }

    private enum ActionType
    {
        Watering,
        Sunlight,
        UVLamp,
        Pruning,
        Fertilizing,
        Waiting
    }
}
