using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthographicZoom : MonoBehaviour
{
    public Camera cam;
    public float maxZoom = 7;
    public float minZoom = 9;
    public float sensitivity = 1;
    public float speed = 30;
    float targetZoom;
    public Rigidbody2D followBody;
    void Update()
    {
        targetZoom = followBody.velocity.magnitude;
        targetZoom = Mathf.Clamp(targetZoom, maxZoom, minZoom);
        float newSize = Mathf.MoveTowards(cam.orthographicSize, targetZoom, speed * Time.deltaTime);
        cam.orthographicSize = newSize;
    }
}
