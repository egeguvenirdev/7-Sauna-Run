using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LandingTriggers : MonoBehaviour
{
    [Header("Path Infos")]
    [SerializeField] private int number;
    [SerializeField] private float jumpHeight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManagement.Instance.SwitchPath(-jumpHeight, number);
        }
    }
}