using Unity.VisualScripting;
using UnityEngine;

namespace CameraScripts
{
    public class CameraBehavior : MonoBehaviour
    {
        private Camera _cam;
        private int _layerMask;
        public Rigidbody2D player;
        public Vector2 offset = new(0, 0);
        public float motionSensitivity = 0.25f;
        public float maxFollowSpeed = 20;
        private Vector3 _velocity = new(0, 0, 0);
        public float movementSmoothing;

        private void Awake()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            _layerMask = LayerMask.GetMask("Camera Collider");
        }
        void Update()
        {
            //transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z); // Camera follows the player with specified offset position
            
            var position = transform.position;
            var targetPosition = new Vector3(player.position.x + player.velocity.x * motionSensitivity + offset.x, player.position.y + player.velocity.y * motionSensitivity + offset.y, position.z);

            bool hitVertical = false;
            bool hitHorizontal = false;
            
            CameraCollision(player.position, ref targetPosition, ref hitVertical, ref hitHorizontal);

            position = Vector3.SmoothDamp(position, targetPosition, ref _velocity, movementSmoothing, maxFollowSpeed);
            if (!(hitVertical || hitHorizontal))
            {
                position.x = Mathf.Clamp(position.x, player.position.x - (_cam.orthographicSize * _cam.aspect) + 2, player.position.x + (_cam.orthographicSize * _cam.aspect) - 2);
                position.y = Mathf.Clamp(position.y, player.position.y - _cam.orthographicSize + 2, player.position.y + _cam.orthographicSize - 2);
            }
            transform.position = position;
        }

        private void CameraCollision(Vector3 playerPosition, ref Vector3 targetPosition, ref bool hitVertical, ref bool hitHorizontal)
        {

            var centerHit = Physics2D.Linecast(playerPosition, targetPosition, _layerMask);
            Vector2 newTarget = targetPosition;
            if (centerHit.collider && centerHit.collider.OverlapPoint(playerPosition))
            {
                newTarget = centerHit.collider.ClosestPoint(playerPosition);
                newTarget += (newTarget - (Vector2) playerPosition).normalized * 0.1f;
            }
            else if (centerHit.collider)
            {
                newTarget = Vector2.Lerp(playerPosition, targetPosition, Mathf.Max(0, centerHit.fraction - 0.01f));
                
            }
            targetPosition.x = newTarget.x;
            targetPosition.y = newTarget.y;

            var hitLeft = Physics2D.Raycast(targetPosition, Vector2.left, _cam.orthographicSize * _cam.aspect, _layerMask);
            var hitRight = Physics2D.Raycast(targetPosition, Vector2.right, _cam.orthographicSize * _cam.aspect, _layerMask);

            if (hitRight.collider && !hitLeft.collider)
            {
                hitHorizontal = true;
                targetPosition.x = hitRight.point.x - (_cam.orthographicSize * _cam.aspect);
            }
            else if (hitLeft.collider && !hitRight.collider)
            {
                hitHorizontal = true;
                targetPosition.x = hitLeft.point.x + (_cam.orthographicSize * _cam.aspect);
            }
            
            var hitUp = Physics2D.Raycast(targetPosition, Vector2.up, _cam.orthographicSize, _layerMask);
            var hitDown = Physics2D.Raycast(targetPosition, Vector2.down, _cam.orthographicSize, _layerMask);
            
            if (hitUp.collider && !hitDown.collider)
            {
                hitVertical = true;
                targetPosition.y = hitUp.point.y - _cam.orthographicSize;
            }
            else if (hitDown.collider && !hitUp.collider)
            {
                hitVertical = true;
                targetPosition.y = hitDown.point.y + _cam.orthographicSize;
            }
        }
    }
}


