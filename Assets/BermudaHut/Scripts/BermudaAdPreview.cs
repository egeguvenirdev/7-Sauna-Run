using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BermudaAdPreview : MonoBehaviour
{
    [SerializeField] private GameObject _panelGO;
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private Text _adText;
    [SerializeField] private Image _bar;
    [Space]
    [SerializeField] private float _interstitialDuration = 1f;
    [SerializeField] private float _rewardedDuration = 2f;

    public static Action OnResumeGame;
    public static Action OnPauseGame;
    public static Action OnRewardedVideoSuccess;

    private bool _rewarded = false;

    public static BermudaAdPreview Instance;
    
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ResetView() 
    {
        _closeButton.SetActive(false);
        _bar.DOKill();
        _bar.fillAmount = 0f;
    }

    public void OnCloseButtonClick()
    {
        Debug.Log("naber");
        if (_rewarded)
        {
            OnRewardedVideoSuccess?.Invoke();
        }
        else
        {
            OnResumeGame?.Invoke();
        }
        ResetView();
        _panelGO.SetActive(false);
    }

    public void ShowInterstitial()
    {
        OnPauseGame?.Invoke();
        _adText.text = "INTERSTITIAL";
        _rewarded = false;
        _panelGO.SetActive(true);
        StartCoroutine(ShowProcess());
    }

    public void ShowRewarded()
    {
        OnPauseGame?.Invoke();
        _adText.text = "REWARDED";
        _rewarded = true;
        _panelGO.SetActive(true);
        StartCoroutine(ShowProcess());
    }

    IEnumerator ShowProcess()
    {
        if(!_rewarded)
        {
            _bar.DOFillAmount(1f, _interstitialDuration).SetUpdate(true);
            yield return new WaitForSecondsRealtime(0.3f);
            _closeButton.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(_interstitialDuration - 0.3f);
        }
        else
        {
            _bar.DOFillAmount(1f, _rewardedDuration).SetUpdate(true);
            yield return new WaitForSecondsRealtime(_rewardedDuration);
            _closeButton.gameObject.SetActive(true);
        }
    }

}
