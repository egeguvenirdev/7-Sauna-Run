using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private int moneyWorth = 1;
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.SetTotalMoney(moneyWorth);
        gameObject.SetActive(false);
    }
}
