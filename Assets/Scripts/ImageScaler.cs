using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageScaler : MonoBehaviour
{
    //private Vector3 temp = new Vector3(0.6f, 0.6f, 1f);
    private Vector3 smallScale = new Vector3(0.03f, 0.03f, 0.03f);
    private Vector3 bigScale = new Vector3(0.045f, 0.045f, 0.045f);
    private float speed = 5f;

    private void FixedUpdate()
    {
        TappingScaler();
    }

    private void TappingScaler()
    {
        float lerpValue = (Mathf.Sin(speed * Time.time) + 1f) / 2f; // <0, 1> verecek hep
        transform.localScale = Vector3.Lerp(smallScale, bigScale, lerpValue); // burdan da baslangýç ile bitiþ deðeri arasýnda value alacaksýn
    }
}
