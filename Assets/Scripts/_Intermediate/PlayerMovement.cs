using UnityEngine;

namespace _Intermediate
{
    public class PlayerMovement : MonoBehaviour
    {
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
        private bool _isJumping;
        private float _coyoteTimeCounter;

        
        [Header("Components")]
        private PlayerInput _input;
        private PlayerCollision _collision;
        private Rigidbody2D _rigidbody2D;
        
        private void Start()
        {
            _input = GetComponent<PlayerInput>();
            _collision = GetComponent<PlayerCollision>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

       
        private void Update()
        {

            if (!_isJumping && !_collision.IsGroundedBox())
            {
                _coyoteTimeCounter += Time.deltaTime;
            }
            else
            {
                _coyoteTimeCounter = 0;
            }
            
            
            if (_input.JumpPressed && (_collision.IsGroundedBox() || (_coyoteTimeCounter > 0.03f && _coyoteTimeCounter < coyoteTime)))
            {
                print($" I am grounded? {_collision.IsGroundedBox()}");
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpSpeed);
                _isJumping = true;
            }
        }

        private void FixedUpdate()
        {
            UpdateMovement();
            
            if (_collision.IsGroundedBox())
            {
                _isJumping = false;
            }
        }

        private void UpdateMovement()
        {
            _velocity = _rigidbody2D.velocity;

            #region Update MoveSpeed
                if (_input.MoveVector.x != 0)
                {
                    _moveSpeed += _input.MoveVector.x * acceleration;
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
    }
}