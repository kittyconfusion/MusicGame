using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
    
        public float walkingSpeed = 5;
        public float horizontalDrag = 0.1f;
        public float walkSmoothing = 0.2f;
        public float jumpHeight = 2.5f;
        public int numAirJumps = 1;
        public int wallJumpXForce = 10;

        public Transform groundCheckPos;
        public Transform wallCheckLeftPos;
        public Transform wallCheckRightPos;
        public LayerMask groundLayers;
        private Vector2 groundCheckSize = new(0.37f, 0.05f);
        private Vector2 wallCheckSize = new(0.05f, 0.55f);

        private Rigidbody2D _rigidbody;

        private Vector2 _directionalInput;
        private Vector2 _horizontalAcceleration = Vector2.zero;

        private bool _jumpPressed;
        private int _remainingAirJumps;
        private bool _canJump;
        private bool _isGrounded;
        private bool _isSliding;
        private bool _isSlidingRight;

        private float _gravity;
    
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _gravity = _rigidbody.gravityScale;
        }

        void Update()
        {

            _jumpPressed = Input.GetAxis("Jump") > 0;
            _directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        }

        private void FixedUpdate()
        {
            var velocity = _rigidbody.velocity;

            HandleMoveHorizontal(ref velocity);

            CheckGrounded();
            CheckWallSliding();
            HandleJump(ref velocity);

            HandleGravity();
        
            _rigidbody.velocity = velocity;
        }
    
        // Sets gravity to 0 whilst on ground as to not slide down ramps and remove friction (handled separately).
        private void HandleGravity()
        {
            _gravity = _rigidbody.gravityScale > 0 ? _rigidbody.gravityScale : _gravity;

            if (!_isGrounded && _rigidbody.gravityScale == 0)
            {
                _rigidbody.gravityScale = _gravity;
            }
        
            if (_isGrounded && _rigidbody.gravityScale > 0)
            {
                _rigidbody.gravityScale = 0;
            }
        }

        private void CheckGrounded()
        {
            _isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayers);
        }

        private void CheckWallSliding()
        {
            if (Physics2D.OverlapBox(wallCheckLeftPos.position,wallCheckSize, 0, groundLayers) && _directionalInput.x < 0)
            {
                _isSliding = true;
                _isSlidingRight = false;
            }
            else if (Physics2D.OverlapBox(wallCheckRightPos.position, wallCheckSize, 0, groundLayers) && _directionalInput.x > 0)
            {
                _isSliding = true;
                _isSlidingRight = true;
            }
            else
            {
                _isSliding = false;
            }

        }

        private void HandleJump(ref Vector2 velocity)
        {
            if (_isGrounded)
            {
                _remainingAirJumps = numAirJumps;
            }
            if (_jumpPressed)
            {
                if (_canJump && _isSliding && !_isGrounded)
                {
                    velocity.x += wallJumpXForce * (_isSlidingRight ? -1 : 1);
                    velocity.y = Mathf.Sqrt(2 * -Physics2D.gravity.y * _gravity * jumpHeight);
                }

                else if (_canJump && (_isGrounded || _remainingAirJumps > 0))
                {
                    velocity.y = Mathf.Sqrt(2 * -Physics2D.gravity.y * _gravity * jumpHeight);
                    if (!_isGrounded) {_remainingAirJumps--;}
                    _isGrounded = false;
                }
            
                _canJump = false;
            }
            else
            {
                _canJump = true;
            }
        }
    
        private void HandleMoveHorizontal(ref Vector2 velocity)
        {
            velocity.x *= 1-horizontalDrag;
            var inputMovement = _directionalInput.x * walkingSpeed;
            float targetMovement = inputMovement * velocity.x > 0 ? (Math.Abs(inputMovement) >= Math.Abs(velocity.x) ? inputMovement : velocity.x) : (velocity.x + inputMovement);
            velocity.x = Vector2.SmoothDamp(velocity,new Vector2(targetMovement,velocity.y),ref _horizontalAcceleration,walkSmoothing).x;
        }
    }
}