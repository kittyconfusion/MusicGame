using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float walkingSpeed = 5;
    public float walkSmoothing = 0.2f;
    public float jumpHeight = 2.5f;
    public int numAirJumps = 1;

    public Transform groundCheckPos;
    public LayerMask groundLayers;
    private Vector2 groundCheckSize = new(0.37f,0.05f);

    private Rigidbody2D _rigidbody;

    private Vector2 _directionalInput;
    private Vector2 _horizontalAcceleration = Vector2.zero;

    private bool _jumpPressed;
    private int _remainingAirJumps;
    private bool _canJump;
    private bool _isGrounded;

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
        _isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0,groundLayers);
    }

    private void HandleJump(ref Vector2 velocity)
    {
        if (_isGrounded)
        {
            _remainingAirJumps = numAirJumps;
        }
        if (_jumpPressed)
        {
            if (_canJump && (_isGrounded || _remainingAirJumps > 0))
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
        var horizontal = _directionalInput.x;
        velocity.x = Vector2.SmoothDamp(velocity,new Vector2(horizontal * walkingSpeed,velocity.y),ref _horizontalAcceleration,walkSmoothing).x;
    }
}