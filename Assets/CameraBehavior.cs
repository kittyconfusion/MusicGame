using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Rigidbody2D player;
    public Vector2 offset;
    public float speed;
    

    void Update()
    {
        //transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z); // Camera follows the player with specified offset position

        float Speed = speed * Time.deltaTime * Mathf.Clamp(player.velocity.magnitude, 1, 5);
        var actualTargetPosition = new Vector3(player.position.x + player.velocity.x + offset.x, player.position.y + player.velocity.y + offset.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, actualTargetPosition, Speed);
    }
}

