using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCoin : MonoBehaviour
{
    public float rotationspeed = 100f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationspeed * Time.deltaTime);
    }
}
