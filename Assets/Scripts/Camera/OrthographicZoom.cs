using UnityEngine;

public class OrthographicZoom : MonoBehaviour
{
    private Camera _cam;
    public float minZoom = 7;
    public float maxZoom = 9;
    public float speedThreshold = 7.5f;
    public float smooothness = 0.5f;
    public Rigidbody2D followBody;
    private float _zooooomSpeed = 0;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void Update()
    {
        var targetZoom = minZoom + (maxZoom - minZoom) / (1 + Mathf.Exp(-0.5f * (followBody.velocity.magnitude - speedThreshold)));
        float newSize = Mathf.SmoothDamp(_cam.orthographicSize, targetZoom, ref _zooooomSpeed, smooothness, Mathf.Infinity,(targetZoom < _cam.orthographicSize ? 2 : 1) * Time.deltaTime);
        _cam.orthographicSize = newSize;
    }
}
