using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private int moneyWorth = 1;
    private void OnTriggerEnter(Collider other)
    {
        var particle = ObjectPooler.Instance.GetPooledObject("MoneyParticle");
        particle.transform.position = transform.position + new Vector3(0, .25f, 1f);
        particle.transform.rotation = Quaternion.identity;
        particle.SetActive(true);
        particle.GetComponent<ParticleSystem>().Play();

        Haptic.Instance.HapticFeedback(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        GameManager.Instance.SetTotalMoney(moneyWorth);
        gameObject.SetActive(false);
    }
}
