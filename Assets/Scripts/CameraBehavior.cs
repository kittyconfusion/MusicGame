using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Rigidbody2D player;
    public Vector2 offset = new(0,0);
    public float followSpeed = 5;
    public float motionSensitivity = 0.25f;
    public float maxFollowSpeed = 5;
    

    void Update()
    {
        //transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z); // Camera follows the player with specified offset position

        float speed = followSpeed * Time.deltaTime * Mathf.Clamp(player.velocity.magnitude * motionSensitivity, 1, maxFollowSpeed);
        var position = transform.position;
        var actualTargetPosition = new Vector3(player.position.x + player.velocity.x * motionSensitivity + offset.x, player.position.y + player.velocity.y * motionSensitivity + offset.y, position.z);
        position = Vector3.MoveTowards(position, actualTargetPosition, speed);
        transform.position = position;
    }
}

