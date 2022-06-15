using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private Camera _cam;
    public Rigidbody2D player;
    public Vector2 offset = new(0,0);
    public float motionSensitivity = 0.25f;
    public float maxFollowSpeed = 20;
    private Vector3 _velocity = new(0,0,0);
    public float movementSmoothing;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }
    void Update()
    {
        //transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z); // Camera follows the player with specified offset position
        
        var position = transform.position;
        var targetPosition = new Vector3(player.position.x + player.velocity.x * motionSensitivity + offset.x, player.position.y + player.velocity.y * motionSensitivity + offset.y, position.z);
        position = Vector3.SmoothDamp(position, targetPosition, ref _velocity, movementSmoothing, maxFollowSpeed);
        position.x = Mathf.Clamp(position.x, player.position.x - (_cam.orthographicSize * _cam.aspect) + 2, player.position.x + (_cam.orthographicSize * _cam.aspect) - 2);
        position.y = Mathf.Clamp(position.y, player.position.y -  _cam.orthographicSize + 2, player.position.y + _cam.orthographicSize - 2);
        transform.position = position;
    }
}

