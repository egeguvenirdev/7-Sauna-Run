using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LandingTriggers : MonoBehaviour
{
    [Header("Path Infos")]
    [SerializeField] private int number;
    [SerializeField] private float jumpHeight;
    [SerializeField] private bool isBumpy = false;
    [SerializeField] private Collider col;

    private void Start()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManagement.Instance.SwitchPath(-jumpHeight, number, isBumpy);
            col.enabled = false;
        }
    }
}
