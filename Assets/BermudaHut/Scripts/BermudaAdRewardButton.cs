using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BermudaAdRewardButton : MonoBehaviour
{
    [SerializeField] private GameObject _videoIcon;
    [SerializeField] private GameObject _loading;
    [Space]
    [SerializeField] private UnityEvent OnRewarded;
    void Update()
    {
        if (WebAdManager.Instance.IsRewardedAdAvailable() && _loading.activeInHierarchy)
        {
            _loading.SetActive(false);
            _videoIcon.SetActive(true);
        }
    }

    public void OnClick()
    {
        if(_videoIcon.activeInHierarchy)
        {
            StartCoroutine(RewardProcess());
        }
    }

    IEnumerator RewardProcess()
    {
        WebAdManager.Instance.ShowRewardedAd();
        yield return new WaitWhile(() => WebAdManager.Instance.IsRewardWaiting());
        if(WebAdManager.Instance.IsRewardSuccess())
        {
            OnRewarded?.Invoke();
        }
    }
}
