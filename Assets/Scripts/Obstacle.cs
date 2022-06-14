using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private Collider col;
    [SerializeField] private Collider col2;
    private void OnTriggerEnter(Collider other)
    {
        col.enabled = false;
        if (col2 != null)
        {
            col2.enabled = false;
        }

        if (other.CompareTag("Player"))
        {
            PlayerManagement.Instance.ThrowCustomer();
            Haptic.Instance.HapticFeedback(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
    }
}
