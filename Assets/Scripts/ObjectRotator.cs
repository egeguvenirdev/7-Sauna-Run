using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private Vector3 _rotateVelocity;
    [SerializeField] private Space _rotateSpace;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    private Vector3 minVec;
    private Vector3 maxVec;

    private void Start()
    {
        minVec = new Vector3(transform.position.x, minHeight, transform.position.z);
        maxVec = new Vector3(transform.position.x, maxHeight, transform.position.z);
    }

    void Update()
    {
        transform.Rotate(_rotateVelocity * Time.deltaTime, _rotateSpace);

        float lerpValue = (Mathf.Sin(speed * Time.time) + 1f) / 2f; // <0, 1> 
        transform.position = Vector3.Lerp(minVec, maxVec, lerpValue);
    }
}

