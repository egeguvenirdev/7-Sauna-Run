using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    [SerializeField] private GameObject dirty;
    [SerializeField] private GameObject clean;
    [SerializeField] private Bermuda.Animation.SimpleAnimancer animancer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OpenCleanMesh()
    {
        dirty.SetActive(false);
        clean.SetActive(true);
    }

    private void PlayAnim(string animName)
    {
        animancer.PlayAnimation(animName);
    }
}
