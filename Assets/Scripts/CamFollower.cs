using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CamFollower : MonoBehaviour
{
    [SerializeField] private Transform targetTransform = null;
    [SerializeField] private Transform localTransform = null;
    [SerializeField] private Transform cam;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float playerFollowSpeed = 0.125f;
    [SerializeField] private float clampLocalX = 1.5f;

    [SerializeField] private Vector3 finisPosition;
    [SerializeField] private Vector3 finisRotation;

    private void Start()
    {
        DOTween.Init();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = localTransform.localPosition;
        targetPosition.x = Mathf.Clamp(targetPosition.x, -clampLocalX, clampLocalX);

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, playerFollowSpeed);
    }

    public void SwitchTarget(Transform newTargetTransform)
    {
        targetTransform = newTargetTransform;
    }

    public void FinisPosition()
    {
        cam.DOLocalMove(finisPosition, 1.5f);
        cam.DOLocalRotate(finisRotation, 1.5f);
    }
    public void StopMovement()
    {
        targetTransform = null;
    }
}
