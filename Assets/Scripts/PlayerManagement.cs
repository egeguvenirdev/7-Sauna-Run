using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManagement : MonoSingleton<PlayerManagement>
{
    [Header("Scripts")]
    [SerializeField] private RunnerScript runnerScript;

    private bool canRun = false;
    Sequence seq;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();
        runnerScript.Init();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartMovement()
    {
        runnerScript.StartToRun(true);
    }

    public void StopMovement()
    {
        runnerScript.StartToRun(false);
    }

    public void SwitchPath(float targetPoint, int number)
    {
        //INIS CIKIS ICIN AYRI YAZ TARGET POINT KULLASN
        seq = DOTween.Sequence();
        if (number == 0)
        {
            seq.Append(transform.DOMoveY(targetPoint + 2, 1.5f));
            seq.Append(transform.DOMoveY(targetPoint, 0.375f)
                .OnComplete( () => { runnerScript.SetPlayerHeight(targetPoint); } ));
        }
        else
        {
            runnerScript.SwitchPathLine(number);
        }
    }
}
