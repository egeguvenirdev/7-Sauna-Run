using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Gate : MonoBehaviour
{
    [SerializeField] private ObjectType objectType;
    [SerializeField] private int capacity;
    [SerializeField] private TMP_Text objectText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider thisTrigger = null;
    [SerializeField] private Collider otherTrigger = null;
    [SerializeField] private Transform gate;

    public enum ObjectType
    {
        DebuffGate,
        BuffGate,
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
                PlayerManagement.Instance.AddCap(capacity);
                TurnToGrey();
            }

            if (objectType == ObjectType.DebuffGate)
            {
                PlayerManagement.Instance.AddCap(-capacity);
                TurnToGrey();
            }
        }
    }

    private void SetTexts()
    {
        if (objectType == ObjectType.BuffGate)
        {
            objectText.text = "+" + capacity;
        }

        if (objectType == ObjectType.DebuffGate)
        {
            objectText.text = "-" + capacity;
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
