using UnityEngine;

public class OrthographicZoom : MonoBehaviour
{
    private Camera _cam;
    public float sensitivity = 0.5f;
    public float maxZoom = 7;
    public float minZoom = 9;
    public float speed = 30;
    public Rigidbody2D followBody;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void Update()
    {
        var targetZoom = followBody.velocity.magnitude * sensitivity;
        targetZoom = Mathf.Clamp(targetZoom, maxZoom, minZoom);
        float newSize = Mathf.MoveTowards(_cam.orthographicSize, targetZoom, speed * Time.deltaTime);
        _cam.orthographicSize = newSize;
    }
}
