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
    private Collider col;

    private void Start()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerManagement.Instance.CanSit())
            {
                PlaySittingAnim();
            }
            PlayerManagement.Instance.AddCustomer(gameObject);
            CloseCollider();
        }
    }

    private void OpenCleanMesh()
    {
        dirty.SetActive(false);
        clean.SetActive(true);

        var particle = ObjectPooler.Instance.GetPooledObject("Bubble");
        particle.transform.position = transform.position + new Vector3(0, 0f, 1f);
        particle.transform.rotation = Quaternion.identity;
        particle.SetActive(true);
        particle.GetComponent<ParticleSystem>().Play();
    }

    public void PlaySittingAnim()
    {
        animancer.PlayAnimation(minSittingRange, maxSittingRange);
        Invoke("OpenCleanMesh", 2f);
    }

    public void PlayCheeringAnim()
    {
        animancer.PlayAnimation(minCheeringRange, maxCheeringRange);
    }

    public void PlayFlyingAnim()
    {
        animancer.PlayAnimation("Fly");
    }

    private void CloseCollider()
    {
        col.enabled = false;
    }
}
