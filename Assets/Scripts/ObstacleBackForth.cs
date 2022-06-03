using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBackForth : MonoBehaviour
{
    [SerializeField] private Space _rotateSpace;
    [SerializeField] private float speed = 4f;
    [SerializeField] private Quaternion minAngle;
    [SerializeField] private Quaternion maxAngle;

    private void Start()
    {
        /*minHeight = transform.position;
        maxHeight = minHeight + new Vector3(0, 0.5f, 0);*/
    }

    void Update()
    {
        float lerpValue = (Mathf.Sin(speed * Time.time) + 1f) / 2f; // <0, 1> 
        transform.rotation = Quaternion.Lerp(minAngle, maxAngle, lerpValue);
    }
}
