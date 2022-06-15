using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ramp : MonoBehaviour
{
    [SerializeField] private Material black;
    [SerializeField] private Material green;
    [SerializeField] private float blockSize;
    [SerializeField] private Transform tableTransform;
    private ModelManager modelManager;

    Sequence seqRamp;

    private void Start()
    {
        DOTween.Init();
        seqRamp = DOTween.Sequence();
    }

    private void OnTriggerEnter(Collider other)
    {
        TurnGreen();
        PullCustomer();
        Haptic.Instance.HapticFeedback(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
    }

    private void OnTriggerExit(Collider other)
    {
        TurnBlack();
    }

    public void TurnGreen()
    {
        gameObject.GetComponent<Renderer>().material = green;
    }

    public void TurnBlack()
    {
        gameObject.GetComponent<Renderer>().material = black;
    }

    private void CalculateRange()
    {
        int range = (int)(transform.localPosition.z / blockSize);
        int currentMoney = GameManager.Instance.ReturnCurrentMoney();
        int money = range * currentMoney - currentMoney;
        Debug.Log(money);
        GameManager.Instance.AddMultipliedMoney(money);
    }

    private void PullCustomer()
    {
        if (PlayerManagement.Instance.ReturnCustomerCount() <= 0) 
        {
            CalculateRange();
            return; 
        }
        GameObject customer = PlayerManagement.Instance.ReturnCustomer();
        customer.transform.parent = null;
        customer.transform.rotation = tableTransform.rotation;
        customer.transform.DOScale(customer.transform.localScale.x + 0.01f, 0.5f);
        modelManager = customer.GetComponent<ModelManager>();
        modelManager.PlayFlyingAnim();

        Vector3 targetPoint = tableTransform.position;
        if (modelManager.isMale) targetPoint += new Vector3(0, -0.048f, 0);
        seqRamp.Append(customer.transform.DOMove(targetPoint, 0.2f).OnComplete(() => { modelManager.PlayLyingAnim(); }));
    }
}
