using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    [SerializeField] private GameObject dirty;
    [SerializeField] private GameObject clean;
    [SerializeField] private Bermuda.Animation.SimpleAnimancer animancer;
    [SerializeField] private int minSittingRange = 6;
    [SerializeField] private int maxSittingRange = 10;
    [SerializeField] private int minCheeringRange = 0;
    [SerializeField] private int maxCheeringRange = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManagement.Instance.AddCustomer(gameObject);
            PlaySittingAnim();
        }
    }

    private void OpenCleanMesh()
    {
        dirty.SetActive(false);
        clean.SetActive(true);
    }

    private void PlaySittingAnim()
    {
        animancer.PlayAnimation(minSittingRange, maxSittingRange);
        Invoke("OpenCleanMesh", 2f);
    }

    private void PlayCheeringAnim()
    {
        animancer.PlayAnimation(minCheeringRange, maxCheeringRange);
    }
}
