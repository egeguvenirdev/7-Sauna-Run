using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManagement.Instance.SwitchPath();
        }  
    }
}
