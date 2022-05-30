using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManagement : MonoSingleton<PlayerManagement>
{
    [Header("Scripts")]
    [SerializeField] private RunnerScript runnerScript;

    private bool canRun = true;
    Sequence sequence;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && canRun)
        {
            runnerScript.StartToRun(true);
        }
    }

    public void SwitchPath()
    {
        runnerScript.SwitchPathLine();
    }
}
