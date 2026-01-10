using UnityEngine;
using System.Collections;

public class ActionAnimator : MonoBehaviour
{
    [Header("Объект лейки")]
    [SerializeField] private GameObject _wateringCanObject;
    [SerializeField] private Animator _wateringCanAnimator;

    [Header("Настройки")]
    [SerializeField] private string _animationName = "WateringCan_Pour";
    [SerializeField] private float _animationDuration = 2f;

    private void Start()
    {
        if (_wateringCanObject != null)
        {
            _wateringCanObject.SetActive(false);
        }

        // Отключаем автоплей анимации
        if (_wateringCanAnimator != null)
        {
            _wateringCanAnimator.enabled = false;
        }
    }

    public void PlayWateringAnimation()
    {
        _wateringCanObject.SetActive(true);
        StartCoroutine(WateringAnimationCoroutine());
    }

    private IEnumerator WateringAnimationCoroutine()
    {
        if (_wateringCanObject == null || _wateringCanAnimator == null) yield break;

        // Показываем объект
        _wateringCanObject.SetActive(true);

        // Включаем аниматор и запускаем анимацию
        _wateringCanAnimator.enabled = true;
        _wateringCanAnimator.Play(_animationName, 0, 0f);

        // Проигрываем звук
        AudioManager.Instance?.PlayActionSound(0);

        // Ждём завершения анимации
        yield return new WaitForSeconds(_animationDuration);

        // Отключаем аниматор и скрываем объект
        _wateringCanAnimator.enabled = false;
        _wateringCanObject.SetActive(false);
    }
}
