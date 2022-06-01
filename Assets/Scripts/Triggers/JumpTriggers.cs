using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTriggers : MonoBehaviour
{
    [Header("Path Infos")]
    [SerializeField] private int number;
    [SerializeField] private float jumpHeight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManagement.Instance.SwitchPath(jumpHeight, number);
        }  
    }
}
