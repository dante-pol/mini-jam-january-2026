using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimationData
{
    public GameObject animatedObject;
    public Animator animator;
    public float duration;
}

public class ActionAnimator : MonoBehaviour
{
    [Header("Анимации действий")]
    [SerializeField] private AnimationData _wateringAnimation;
    [SerializeField] private AnimationData _sunlightAnimation;
    [SerializeField] private AnimationData _uvLampAnimation;
    [SerializeField] private AnimationData _pruningAnimation;
    [SerializeField] private AnimationData _fertilizingAnimation;
    [SerializeField] private AnimationData _waitingAnimation;

    private void Start()
    {
        HideAllObjects();
        DisableAllAnimators();
    }

    private void HideAllObjects()
    {
        HideAnimation(_wateringAnimation);
        HideAnimation(_sunlightAnimation);
        HideAnimation(_uvLampAnimation);
        HideAnimation(_pruningAnimation);
        HideAnimation(_fertilizingAnimation);
        HideAnimation(_waitingAnimation);
    }

    private void DisableAllAnimators()
    {
        DisableAnimator(_wateringAnimation);
        DisableAnimator(_sunlightAnimation);
        DisableAnimator(_uvLampAnimation);
        DisableAnimator(_pruningAnimation);
        DisableAnimator(_fertilizingAnimation);
        DisableAnimator(_waitingAnimation);
    }

    private void HideAnimation(AnimationData data)
    {
        if (data.animatedObject != null)
        {
            data.animatedObject.SetActive(false);
        }
    }

    private void DisableAnimator(AnimationData data)
    {
        if (data.animator != null)
        {
            data.animator.enabled = false;
        }
    }

    #region Public Methods for Buttons

    public void PlayWateringAnimation()
    {
        StartCoroutine(PlaySimpleAnimation(_wateringAnimation, 0));
    }

    public void PlaySunlightAnimation()
    {
        StartCoroutine(PlaySimpleAnimation(_sunlightAnimation, 2));
    }

    public void PlayUVLampAnimation()
    {
        StartCoroutine(PlaySimpleAnimation(_uvLampAnimation, 3));
    }

    public void PlayPruningAnimation()
    {
        StartCoroutine(PlaySimpleAnimation(_pruningAnimation, 4));
    }

    public void PlayFertilizingAnimation()
    {
        StartCoroutine(PlaySimpleAnimation(_fertilizingAnimation, 5));
    }

    public void PlayWaitingAnimation()
    {
        StartCoroutine(PlaySimpleAnimation(_waitingAnimation, 1));
    }

    #endregion

    #region Animation Coroutines

    private IEnumerator PlaySimpleAnimation(AnimationData data, int soundIndex)
    {
        if (data.animatedObject == null) yield break;

        // Показываем объект
        data.animatedObject.SetActive(true);

        // Запускаем аниматор если есть
        if (data.animator != null)
        {
            data.animator.enabled = true;
        }

        // Звук
        AudioManager.Instance?.PlayActionSound(soundIndex);

        // Ждем
        yield return new WaitForSeconds(data.duration);

        // Выключаем
        if (data.animator != null)
        {
            data.animator.enabled = false;
        }

        data.animatedObject.SetActive(false);
    }

    #endregion
}
