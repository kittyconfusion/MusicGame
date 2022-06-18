using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
    
        public float walkingSpeed = 5;
        public float airSpeedScale = 0.8f;
        public float horizontalDrag = 0.1f;
        public float walkSmoothing = 0.2f;
        public float minJumpHeight = 1f;
        public float maxJumpHeight = 2.5f;
        public int numAirJumps = 1;
        public int wallJumpXForce = 10;

        public int coyoteBufferTicks = 5;
        private int _coyoteBuffer;
        public int jumpBufferTicks = 5;
        private int _jumpBuffer;

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
        private bool _isJumping;
        private int _remainingAirJumps;
        private bool _wasJumpPressed;
        private bool _isGrounded;
        private bool _isSliding;
        private bool _isSlidingRight;

        private float _gravity;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
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

            HandleGravity(ref velocity);
            
            _rigidbody.velocity = velocity;
        }
        
        // Sets gravity to 0 whilst on ground as to not slide down ramps and remove friction (handled separately).
        private void HandleGravity(ref Vector2 velocity)
        {
            _gravity = _rigidbody.gravityScale > 0 ? _rigidbody.gravityScale : _gravity;
            
            _rigidbody.gravityScale = _isGrounded ? 0 : _gravity;
        }

        private void CheckGrounded()
        {
            _isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayers);
            
            if (_isGrounded) _coyoteBuffer = coyoteBufferTicks;
            else if (_coyoteBuffer > 0) _coyoteBuffer--;
        }

        private void CheckWallSliding()
        {
            if (Physics2D.OverlapBox(wallCheckLeftPos.position, wallCheckSize, 0, groundLayers) && _directionalInput.x < 0)
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

            if (_isGrounded) _remainingAirJumps = numAirJumps;

            if (!_isGrounded && _remainingAirJumps == 0 && _jumpPressed && !_wasJumpPressed) _jumpBuffer = jumpBufferTicks;

            if (velocity.y <= 0) _isJumping = false;
            
            if (_jumpBuffer > 0)
            {
                if (_isGrounded || _isSliding)
                {
                    _wasJumpPressed = false;
                    _jumpPressed = true;
                    _jumpBuffer = 0;
                }
                _jumpBuffer--;
            }

            if (_jumpPressed)
            {
                if (_isJumping)
                {
                    velocity.y += Time.deltaTime * (Physics2D.gravity.y * _gravity) * (minJumpHeight / maxJumpHeight - 1);
                }
                else if (!_wasJumpPressed)
                {
                    var toJump = false;

                    if (!_wasJumpPressed && _isSliding && !_isGrounded)
                    {
                        velocity.x += wallJumpXForce * (_isSlidingRight ? -1 : 1);
                        toJump = true;
                    }

                    else if (!_wasJumpPressed && (_isGrounded || _remainingAirJumps > 0 || _coyoteBuffer > 0))
                    {
                        toJump = true;
                        if (!_isGrounded && _coyoteBuffer == 0)
                        {
                            _remainingAirJumps--;
                        }

                        _isGrounded = false;
                        _coyoteBuffer = 0;
                    }

                    if (toJump)
                    {
                        velocity.y = Mathf.Sqrt(2 * -Physics2D.gravity.y * _gravity * minJumpHeight);
                        _isJumping = true;
                    }
                }

                _wasJumpPressed = true;
            }
            else
            {
                _wasJumpPressed = false;
                _isJumping = false;
            }
        }
        
        private void HandleMoveHorizontal(ref Vector2 velocity)
        {
            // TODO: make this use acceleration and stuff
            
            velocity.x *= 1 - horizontalDrag;
            
            float inputMovement = _directionalInput.x * walkingSpeed;
            bool movingInSameDirectionAsMotion = inputMovement * velocity.x > 0;
            inputMovement *= (!_isGrounded && !movingInSameDirectionAsMotion) ? airSpeedScale : 1;

            float targetMovement;
            if (movingInSameDirectionAsMotion)
            {
                // If moving in same direction as existing motion, take whichever value is bigger
                targetMovement = Math.Abs(inputMovement) >= Math.Abs(velocity.x) ? inputMovement : velocity.x;
            }
            else
            {
                // If moving in opposite direction as existing motion, just add
                targetMovement = velocity.x + inputMovement;
            }

            velocity.x = Vector2.SmoothDamp(velocity, new Vector2(targetMovement, velocity.y), ref _horizontalAcceleration, walkSmoothing).x;
        }
    }
}