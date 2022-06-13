using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerManagement.Instance.FinishAction();
        Debug.Log("finish");
    }
}
