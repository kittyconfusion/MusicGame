using Unity.VisualScripting;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private Camera _cam;
    private int _layermask;
    public Rigidbody2D player;
    public Vector2 offset = new(0,0);
    public float motionSensitivity = 0.25f;
    public float maxFollowSpeed = 20;
    private Vector3 _velocity = new(0,0,0);
    public float movementSmoothing;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _layermask = LayerMask.GetMask("Camera Collider");
    }
    void Update()
    {
        //transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z); // Camera follows the player with specified offset position
        
        var position = transform.position;
        var targetPosition = new Vector3(player.position.x + player.velocity.x * motionSensitivity + offset.x, player.position.y + player.velocity.y * motionSensitivity + offset.y, position.z);

        bool hitvertical = false;
        bool hithorizontal = false;
        
        CameraCollision(player.position, ref targetPosition, ref hitvertical, ref hithorizontal);

        position = Vector3.SmoothDamp(position, targetPosition, ref _velocity, movementSmoothing, maxFollowSpeed);
        if (!(hitvertical || hithorizontal))
        {
            position.x = Mathf.Clamp(position.x, player.position.x - (_cam.orthographicSize * _cam.aspect) + 2, player.position.x + (_cam.orthographicSize * _cam.aspect) - 2);
            position.y = Mathf.Clamp(position.y, player.position.y - _cam.orthographicSize + 2, player.position.y + _cam.orthographicSize - 2);
        }
        transform.position = position;
    }

    private void CameraCollision(Vector3 playerPosition, ref Vector3 targetPosition, ref bool hitvertical, ref bool hithorizontal)
    {

        var centerHit = Physics2D.Linecast(playerPosition,targetPosition,_layermask);
        Vector2 newTarget = targetPosition;
        if (centerHit.collider && centerHit.collider.OverlapPoint(playerPosition))
        {
            newTarget = centerHit.collider.ClosestPoint(playerPosition);
            newTarget += (newTarget-(Vector2)playerPosition).normalized * 0.1f;
        }
        else if (centerHit.collider)
        {
            newTarget = Vector2.Lerp(playerPosition, targetPosition,Mathf.Max(0,centerHit.fraction-0.01f));
            
        }
        targetPosition.x = newTarget.x;
        targetPosition.y = newTarget.y;

        var hitleft = Physics2D.Raycast(targetPosition, Vector2.left, _cam.orthographicSize * _cam.aspect, _layermask);
        var hitright = Physics2D.Raycast(targetPosition, Vector2.right, _cam.orthographicSize * _cam.aspect, _layermask);

        if (hitright.collider && !hitleft.collider)
        {
            hithorizontal = true;
            targetPosition.x = hitright.point.x - (_cam.orthographicSize * _cam.aspect);
        }
        else if (hitleft.collider && !hitright.collider)
        {
            hithorizontal = true;
            targetPosition.x = hitleft.point.x + (_cam.orthographicSize * _cam.aspect);
        }
        
        var hitup = Physics2D.Raycast(targetPosition, Vector2.up, _cam.orthographicSize, _layermask);
        var hitdown = Physics2D.Raycast(targetPosition, Vector2.down, _cam.orthographicSize, _layermask);
        
        if (hitup.collider && !hitdown.collider)
        {
            hitvertical = true;
            targetPosition.y = hitup.point.y - _cam.orthographicSize;
        }
        else if (hitdown.collider && !hitup.collider)
        {
            hitvertical = true;
            targetPosition.y = hitdown.point.y + _cam.orthographicSize;
        }
    }
}

