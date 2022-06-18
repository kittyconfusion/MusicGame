using UnityEngine;

namespace Camera
{
    public class CameraBehavior : MonoBehaviour
    {
        private UnityEngine.Camera _cam;
        private int _layermask;
        public Rigidbody2D player;
        public Vector2 offset = new(0,0);
        public float motionSensitivity = 0.25f;
        public float maxFollowSpeed = 20;
        private Vector3 _velocity = new(0,0,0);
        public float movementSmoothing;

        private void Awake()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            _layermask = LayerMask.GetMask("Camera Collider");
        }
        void Update()
        {
            //transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z); // Camera follows the player with specified offset position
        
            var position = transform.position;
            var targetPosition = new Vector3(player.position.x + player.velocity.x * motionSensitivity + offset.x, player.position.y + player.velocity.y * motionSensitivity + offset.y, position.z);

            bool hitvertical = false;
            bool hithorizontal = false;

            CameraCollision(ref targetPosition, ref hitvertical, ref hithorizontal);

            position = Vector3.SmoothDamp(position, targetPosition, ref _velocity, movementSmoothing, maxFollowSpeed);
            if (!(hitvertical || hithorizontal))
            {
                position.x = Mathf.Clamp(position.x, player.position.x - (_cam.orthographicSize * _cam.aspect) + 2, player.position.x + (_cam.orthographicSize * _cam.aspect) - 2);
                position.y = Mathf.Clamp(position.y, player.position.y - _cam.orthographicSize + 2, player.position.y + _cam.orthographicSize - 2);
            }
            transform.position = position;
        }

        private void CameraCollision(ref Vector3 position, ref bool hitvertical, ref bool hithorizontal)
        {


            RaycastHit2D hitup = Physics2D.Raycast(position, Vector2.up, _cam.orthographicSize, _layermask);
            RaycastHit2D hitdown = Physics2D.Raycast(position, Vector2.down, _cam.orthographicSize, _layermask);
            RaycastHit2D hitleft = Physics2D.Raycast(position, Vector2.left, _cam.orthographicSize * _cam.aspect, _layermask);
            RaycastHit2D hitright = Physics2D.Raycast(position, Vector2.right, _cam.orthographicSize * _cam.aspect, _layermask);

            if (hitup.collider != null)
            {
                hitvertical = true;
                position.y = hitup.point.y - _cam.orthographicSize;
            }
            if (hitdown.collider != null)
            {
                hitvertical = true;
                position.y = hitup.point.y + _cam.orthographicSize;
            }
            if (hitleft.collider != null)
            {
                hithorizontal = true;
                position.x = hitup.point.x + (_cam.orthographicSize * _cam.aspect);
            }
            if (hitright.collider != null)
            {
                hithorizontal = true;
                position.x = hitup.point.x - (_cam.orthographicSize * _cam.aspect);
            }
        }
    }
}

