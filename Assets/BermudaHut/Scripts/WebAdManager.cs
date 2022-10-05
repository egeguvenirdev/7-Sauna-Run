using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WebAdManager : MonoBehaviour
{

    private static bool _sdkReady = false;
    private bool _adblock = false;
    private static bool _adPreview = false;
    private bool _rewardedAdLoaded = false;
    private bool _rewardWaiting = false;
    private bool _rewardSuccess = false;

    [DllImport("__Internal")]
    private static extern bool IsAdBlock();

    public event Action OnAdStarted;
    public event Action OnAdEnded;
    public event Action OnRewardedSuccess;
    public event Action OnRewardedFail;

    public static WebAdManager Instance;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Web Ad Manager Awake");
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    bool IsAdPreview()
    {
#if UNITY_EDITOR
        return true;
#endif
        return _adPreview; 
    }

    public void ActivateAdPreview()
    {
        Debug.Log("activated ad preview");
        _adPreview = true;
        _sdkReady = true;
    }


    private IEnumerator Start()
    {
#if UNITY_EDITOR
        _sdkReady = true;
#endif
        
        yield return new WaitWhile(() => !_sdkReady);
        Debug.Log("method assign");
        if (!IsAdPreview())
        {
            ExternalAdManager.OnResumeGame += OnResumeGame;
            ExternalAdManager.OnPauseGame += OnPauseGame;
            ExternalAdManager.OnRewardedVideoSuccess += OnRewardSuccess;
            ExternalAdManager.OnRewardedVideoFailure += OnRewardFailed;
            ExternalAdManager.OnPreloadRewardedVideo += OnPreloadRewardedVideo;
            ExternalAdManager.Instance.PreloadRewardedAd();
        }
        else
        {
            BermudaAdPreview.OnResumeGame += OnResumeGame;
            BermudaAdPreview.OnPauseGame += OnPauseGame;
            BermudaAdPreview.OnRewardedVideoSuccess += OnRewardSuccess;
        }
        //yield return null;
        SceneManager.LoadScene(1);
    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.U))
        //{
        //    Debug.Log(IsAdBlock());
        //}
    }

    private void OnPreloadRewardedVideo(int loaded)
    {
        Debug.Log("Rewarded Loaded : " + loaded);
        _rewardedAdLoaded = loaded == 1;
        // Feedback about preloading ad after called GameDistribution.Instance.PreloadRewardedAd
        // 0: SDK couldn't preload ad
        // 1: SDK preloaded ad
    }

    public void PromiseException(string error)
    {
        Debug.Log("Print : " + error);
        if (error.Contains("Failed to load") && error.Contains("ima3.js"))
        {
            _adblock = true;
        }
        else if (error.Contains("advertisement was requested too soon") || error.Contains("VAST"))
        {
            OnResumeGameFail();
        }
        else if (error.Contains("The user skipped the advertisement"))
        {
            OnResumeGameFail();
        }
    }

    void OnResumeGame()
    {
        if(_adblock)
        {
            return;
        }
        Time.timeScale = 1f;
        if (!_rewardedAdLoaded && !IsAdPreview())
        {
            ExternalAdManager.Instance.PreloadRewardedAd();
        }
        OnAdEnded?.Invoke();
    }

    void OnResumeGameFail()
    {
        if(_adblock)
        {
            return;
        }
        Time.timeScale = 1f;
        if(!_rewardedAdLoaded && !IsAdPreview())
        {
            ExternalAdManager.Instance.PreloadRewardedAd();
        }
        
        OnAdEnded?.Invoke();

    }

    void OnPauseGame()
    {
        if(_adblock)
        {
            return;
        }
        Time.timeScale = 0f;
    }

    public void OnEventCatch(string eventName)
    {
        Debug.Log("Caught event : " + eventName);
        if (eventName.Equals("SDK_READY"))
        {
            _sdkReady = true;
        }
    }

    public bool IsSDKReady()
    {
        return _sdkReady;
    }

    public void ShowAd()
    {
        if(_adblock)
        {
            return;
        }
        try
        {
            OnAdStarted?.Invoke();
            Debug.Log("Real showing");
            if (IsAdPreview())
            {
                BermudaAdPreview.Instance.ShowInterstitial();
            }
            else
            {
                ExternalAdManager.Instance.ShowAd();
            }
        }
        catch
        {
            OnResumeGameFail();

        }
    }

    public void ShowRewardedAd()
    {
        if(_adblock || (!IsAdPreview() && !_rewardedAdLoaded))
        {
            return;
        }
        try
        {
            OnAdStarted?.Invoke();
            _rewardWaiting = true;
            Debug.Log("Showing rewarded ad");
            if (IsAdPreview())
            {
                BermudaAdPreview.Instance.ShowRewarded();
            }
            else
            {
                ExternalAdManager.Instance.ShowRewardedAd();
            }
        }
        catch(Exception ex)
        {
            Debug.Log("Show Rewarded Exception" + ex.Message);
            OnResumeGameFail();
        }
    }

    public bool IsRewardedAdAvailable()
    {
        if(IsAdPreview())
        {
            return true;
        }
        return _rewardedAdLoaded;
    }

    public bool IsRewardWaiting()
    {
        return _rewardWaiting;
    }

    public bool IsRewardSuccess()
    {
        return _rewardSuccess;
    }

    void OnRewardSuccess()
    {
        _rewardSuccess = true;
        _rewardWaiting = false;
        _rewardedAdLoaded = false;
        Time.timeScale = 1f;
        if (!IsAdPreview())
        {
            ExternalAdManager.Instance.PreloadRewardedAd();
        }
        OnRewardedSuccess?.Invoke();
    }

    void OnRewardFailed()
    {
        _rewardSuccess = false;
        _rewardWaiting = false;
        _rewardedAdLoaded = false;
        Time.timeScale = 1f;
        if(!IsAdPreview())
        {
            ExternalAdManager.Instance.PreloadRewardedAd();
        }
        OnRewardedFail?.Invoke();
    }

    private void OnApplicationFocus(bool focus)
    {
        AudioListener.volume = focus ? 1 : 0;
    }

}
