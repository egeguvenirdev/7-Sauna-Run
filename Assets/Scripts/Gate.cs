using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Gate : MonoBehaviour
{
    [SerializeField] private ObjectType objectType;
    [SerializeField] private int timeLine;
    [SerializeField] private TMP_Text objectText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider thisTrigger = null;
    [SerializeField] private Collider otherTrigger = null;
    [SerializeField] private Transform gate;

    private int dateToDay;
    private int randomTime;
    private Vector3 swerveVector;

    public enum ObjectType
    {
        DebuffGate,
        BuffGate,
        RngGate
    }

    private void Start()
    {
        DOTween.Init();
        SetTexts();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CloseTriggers();

            if (objectType == ObjectType.BuffGate)
            {
                //PlayerManagement.Instance.AddHealth(dateToDay);
                TurnToGrey();
            }

            else if (objectType == ObjectType.DebuffGate)
            {
                //PlayerManagement.Instance.AddHealth(dateToDay);
                TurnToGrey();
            }

            else
            {
                //MODELI ACTIRT
                //PlayerManagement.Instance.OpenSpeacialMouse();
                TurnToGrey();
            }
        }
    }

    private void SetTexts()
    {
        if (objectType == ObjectType.BuffGate)
        {
            objectText.text = "+" + timeLine;
        }

        if (objectType == ObjectType.DebuffGate)
        {
            objectText.text = "-" + timeLine;
        }

        if (objectType == ObjectType.RngGate)
        {
            swerveVector = new Vector3(-1.7f, gate.position.y, gate.position.z);
            gate.DOMove(swerveVector, 2).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void TurnToGrey()
    {
        spriteRenderer.color = new Color32(118, 118, 118, 255);
    }

    private void CloseTriggers()
    {
        thisTrigger.enabled = false;
        if (otherTrigger != null)
        {
            otherTrigger.enabled = false;
        }
    }
}
