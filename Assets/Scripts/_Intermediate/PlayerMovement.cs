using UnityEngine;

namespace _Intermediate
{
    public class PlayerMovement : MonoBehaviour
    {
        #region VARIABLES

        [Header("Movement")]
        public float maxMoveSpeed = 6f;
        public float acceleration = 1f;
        public float groundFriction = .3f;
        private float _moveSpeed;
        private Vector2 _velocity;
         
        [Header("Jumping")]
        public float jumpSpeed = 10f;
        public float airFriction = 0.005f;
        public float coyoteTime = 0.15f;
        public float maxVelocityY = 16;
        public int maxDoubleJumpValue = 2;
        private int _doubleJumpValue;
        
        private bool _isJumping;
        private float _coyoteTimeCounter;

        
        [Header("Components")]
        private PlayerInput _input;
        private PlayerCollision _collision;
        private Rigidbody2D _rigidbody2D;

        #endregion
        

        #region EVENT FUNCTIONS

        private void Start()
        {
            _input = GetComponent<PlayerInput>();
            _collision = GetComponent<PlayerCollision>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        private void Update()
        {
            // Added CoyoteTime
            UpdateJumping();
            
            print(_rigidbody2D.velocity.y);
        }
        
        private void FixedUpdate()
        {
            // Acceleration & Friction & Air Friction && Double Jump
            UpdateMovement();
            
            
            // CoyoteTime
            if (_collision.IsGroundedBox())
            {
                _isJumping = false;
                _doubleJumpValue = maxDoubleJumpValue;
            }
        }

        #endregion


        #region GENERAL FUNCTIONS

        
        private void UpdateGravity()
        {
            _velocity.y = Mathf.Clamp(_rigidbody2D.velocity.y, -maxVelocityY, maxVelocityY);
        }

        private void UpdateJumping()
        {
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
            if (_input.JumpPressed && (_collision.IsGroundedBox() 
                                       || (_coyoteTimeCounter > 0.03f && _coyoteTimeCounter < coyoteTime) 
                                       || _doubleJumpValue > 0))
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpSpeed);

                _doubleJumpValue--;
                _isJumping = true;
            }
        }


        private void UpdateMovement()
        {
            // Store the Players Velocity in Separate Vector2
            _velocity = _rigidbody2D.velocity;
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

            _velocity.x = _moveSpeed;
            #endregion
            
            _rigidbody2D.velocity = _velocity;
        }

        #endregion
    }
}