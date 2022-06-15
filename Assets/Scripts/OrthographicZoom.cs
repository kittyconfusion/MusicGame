using UnityEngine;

public class OrthographicZoom : MonoBehaviour
{
    private Camera _cam;
    public float sensitivity = 0.5f;
    public float minZoom = 7;
    public float maxZoom = 9;
    public float smooothness = 0.5f;
    public Rigidbody2D followBody;
    private float _zoooomSpeed = 0;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void Update()
    {
        var targetZoom = followBody.velocity.magnitude * sensitivity;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        float newSize = Mathf.SmoothDamp(_cam.orthographicSize, targetZoom, ref _zoooomSpeed, smooothness);
        _cam.orthographicSize = newSize;
    }
}
