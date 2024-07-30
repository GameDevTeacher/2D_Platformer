using UnityEngine;

namespace _IntermediatePlus
{
    public class MovementWIP : MonoBehaviour
    {
    #region VARIABLES

    [Header("Ground Movement")]
    public float maxMoveSpeed = 6f;
    public float acceleration = 1f;
    public float groundFriction = .3f;

    private float _desiredAcceleration;
    private float _moveSpeed;
    private Vector2 _velocity;
    
    [Header("Air Movement")]
    public float airFriction = 0.005f;
    public float airAcceleration = 0.5f;
    public bool AirAccelerated;
    public float maxVelocityY = 16;
    
    [Header("Jumping")]
    public float jumpSpeed = 10f;
    public float coyoteTime = 0.15f;
    
    public int maxDoubleJumpValue = 2;
    public float _jumpTimeCounter;
    public float jumpTime = 0.25f;
    
    public bool _isJumping;
    
    private int _doubleJumpValue;
    private float _coyoteTimeCounter;

    
    [Header("Components")]
    private PlayerInput _input;
    private _Intermediate.PlayerCollision _collision;
    private Rigidbody2D _rigidbody2D;

    #endregion
    

    #region EVENT FUNCTIONS

    private void Start()
    {
        _input = GetComponent<PlayerInput>();
        _collision = GetComponent<_Intermediate.PlayerCollision>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        if (_input.MoveDirection == Vector2.zero && _collision.IsGroundedBox())
        {
            //_rigidbody2D.velocity = new Vector2(0f, 0f);
            _rigidbody2D.gravityScale = 0f;
        }
        else
        {
            _rigidbody2D.gravityScale = 3f;
        }
        // Added CoyoteTime
        LongJump();
        UpdateJumping();
        
    }
    
    private void FixedUpdate()
    {
        // Acceleration & Friction & Air Friction && Double Jump
        UpdateMovement();
        
        
        // CoyoteTime
        if (_collision.IsGroundedBox() && _rigidbody2D.linearVelocity.y < 0f )
        {
            _isJumping = false;
            _doubleJumpValue = maxDoubleJumpValue;
        }
    }

    #endregion


    #region GENERAL FUNCTIONS

    
    private void UpdateGravity()
    {
        _velocity.y = Mathf.Clamp(_rigidbody2D.linearVelocity.y, -maxVelocityY, maxVelocityY);
    }

    private void UpdateJumping()
    {

        // TODO: Add More Fine Tuned Gravity
        
        
        // Set CoyoteTimer
        if (!_isJumping && !_collision.IsGroundedBox())
        {
            _coyoteTimeCounter += Time.deltaTime;
        }
        else
        {
            _coyoteTimeCounter = 0;
        }
        

        // Check if We are able to jump
        if (_input.JumpPressed && (_collision.IsGroundedBox() || (_coyoteTimeCounter > 0.03f && _coyoteTimeCounter < coyoteTime) || _doubleJumpValue > 0))
        {
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpSpeed);
            _doubleJumpValue--;
            _jumpTimeCounter = jumpTime;
            _isJumping = true;
        }
    }


    private void UpdateMovement()
    {
        // Store the Players Velocity in Separate Vector2
        _velocity = _rigidbody2D.linearVelocity;
        UpdateGravity();

        #region UPDATE MOVESPEED 
        if (_input.MoveDirection.x != 0)
        {
            _moveSpeed += _input.MoveDirection.x * acceleration;
            _moveSpeed = Mathf.Clamp(_moveSpeed, -maxMoveSpeed, maxMoveSpeed);
        }
        else
        {
            _moveSpeed = Mathf.Lerp(_moveSpeed, 0, _collision.IsGroundedBox() ? groundFriction : airFriction);
        }
        /*
        if (_input.MoveDirection.x != 0)
        {
            _desiredAcceleration = _collision.IsGroundedBox() ? acceleration : airAcceleration;
            
            _moveSpeed += _input.MoveDirection.x * _desiredAcceleration;
            _moveSpeed = Mathf.Clamp(_moveSpeed, -maxMoveSpeed, maxMoveSpeed);
        }
        else
        {
            _moveSpeed = Mathf.Lerp(_moveSpeed, 0, _collision.IsGroundedBox() ? groundFriction : airFriction);
        }*/


        _velocity.x = _moveSpeed;
        #endregion
        
        _rigidbody2D.linearVelocity = _velocity;
    }

    private void LongJump()
    {
        if (_input.JumpValue > 0f && _isJumping)
        {
            if (_jumpTimeCounter > 0)
            {
                _rigidbody2D.linearVelocity = Vector2.up *jumpSpeed;
                _jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                _isJumping = false;
            }
        }
        if (_input.JumpValue <= 0f)
        {
            _isJumping = false;
        }
    }

    #endregion
    }
}
