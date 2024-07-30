using UnityEngine;

namespace _IntermediatePlus
{
    public class PlayerMovement : MonoBehaviour
    {
        #region VARIABLES

        [Header("Movement")]
        public float maxVelocityX = 6f, maxVelocityY = 16;
        public float groundAcceleration = 1f, airAcceleration = 0.5f;
        public float groundFriction = .3f, airFriction = 0.005f;
        private float _horizontalMoveSpeed;
        private Vector2 _currentVelocity;
         
        [Header("Jumping")]
        public float jumpSpeed = 10f;
        
        public float coyoteTime = 0.15f;
        public float _coyoteTimeCounter;
        
        public float jumpTime = 0.25f;
        public float _jumpTimeCounter;

        public int maxDoubleJumpValue = 1;
        public int _doubleJumpValue;
        
        public bool _isJumping;
     
        
        [Header("Components")]
        private _Intermediate.PlayerInput _input;
        private _Intermediate.PlayerCollision _collision;
        private Rigidbody2D _rigidbody2D;

        #endregion
        

        #region EVENT FUNCTIONS

        private void Start()
        {
            _input = GetComponent<_Intermediate.PlayerInput>();
            _collision = GetComponent<_Intermediate.PlayerCollision>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        private void Update()
        {
            // Added CoyoteTime
            
            UpdateJumping();
            
            
        }
        
        private void FixedUpdate()
        {
            // Acceleration & Friction & Air Friction && Double Jump
            UpdateMovement();
        }

        #endregion


        #region GENERAL FUNCTIONS

        private void UpdateMovement()
        {
            // Store the Players Velocity in Separate Vector2
            _currentVelocity = _rigidbody2D.linearVelocity;
            UpdateGravity();

            #region UPDATE MOVESPEED 
            if (_input.MoveDirection.x != 0 /* // Only used if we have air control && _collision.IsGroundedBox()*/)
            {
                _horizontalMoveSpeed += _input.MoveDirection.x * groundAcceleration;
                _horizontalMoveSpeed = Mathf.Clamp(_horizontalMoveSpeed, -maxVelocityX, maxVelocityX);
            }
            else
            {
                _horizontalMoveSpeed = Mathf.Lerp(_horizontalMoveSpeed, 0, _collision.IsGroundedBox() ? groundFriction : airFriction);
            }

            _currentVelocity.x = _horizontalMoveSpeed;
            #endregion
            
            _rigidbody2D.linearVelocity = _currentVelocity;
        }
        
        private void UpdateGravity()
        {
            _currentVelocity.y = Mathf.Clamp(_rigidbody2D.linearVelocity.y, -maxVelocityY, maxVelocityY);
        }

        private void UpdateJumping()
        {
            // Double Jump & CoyoteTime
            if (_collision.IsGroundedBox() && _rigidbody2D.linearVelocity.y <= 0f )
            {
                _isJumping = false;
                _doubleJumpValue = maxDoubleJumpValue;
            }

            // TODO: Add More Fine Tuned Gravity
            VariableJumpHeight();
            
            // Set CoyoteTimer
            if (!_isJumping && !_collision.IsGroundedBox())
            {
                _coyoteTimeCounter += Time.deltaTime;
            }
            else { _coyoteTimeCounter = 0; }


            if (!_input.JumpPressed) return;
            
            if (_collision.IsGroundedBox() || (_coyoteTimeCounter > 0.03f && _coyoteTimeCounter < coyoteTime))
            {
                _rigidbody2D.linearVelocity = Vector2.up * jumpSpeed;
                _jumpTimeCounter = jumpTime;
                _isJumping = true;
            }
            else if (_doubleJumpValue > 0)
            {
                _rigidbody2D.linearVelocity = Vector2.up * jumpSpeed;
                _doubleJumpValue--;
                //_isJumping = false;
            }
        }

        private void VariableJumpHeight()
        {
            if (_input.JumpHeld && _isJumping)
            {
                if (_jumpTimeCounter > 0)
                {
                    _rigidbody2D.linearVelocity = Vector2.up * jumpSpeed;
                    _jumpTimeCounter -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeCounter = 0;
            }

            /*
            if (_input.JumpValue > 0f && _isJumping)
            {
                if (_jumpTimeCounter > 0)
                {
                    _rigidbody2D.velocity = Vector2.up * jumpSpeed;
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
                _jumpTimeCounter = 0;
            }
            */
        }
        #endregion
    }
}