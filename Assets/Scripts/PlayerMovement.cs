using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float walkingSpeed;
    public float jumpHeight;
    public int numAirJumps = 1;

    public Transform groundCheckPos;
    public LayerMask groundLayers;
    private Vector2 groundCheckSize = new(0.37f,0.05f);

    private Rigidbody2D _rigidbody;

    private Vector2 _directionalInput;

    private bool _jumpPressed;
    private int _remainingAirJumps;
    private bool _canJump;
    private bool _isGrounded;

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
        HandleJump(ref velocity);
        
        _rigidbody.velocity = velocity;
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
                velocity.y = Mathf.Sqrt(2 * -Physics2D.gravity.y * _rigidbody.gravityScale * jumpHeight);
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
        velocity.x = horizontal * walkingSpeed;
    }
}