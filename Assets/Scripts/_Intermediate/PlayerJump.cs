using System;
using UnityEngine;

namespace _Intermediate
{
    public class PlayerJump : MonoBehaviour
    {
        [Header("Components")]
        private Vector2 _velocity;
        private Rigidbody2D _rigidbody2D;
        private PlayerCollision _collision;
        private PlayerInput _input;
        
        
        [Header("Jumping Stats")]
        [SerializeField, Range(0, 1)] private int maxAirJumps = 0;
        [SerializeField, Range(2f, 5.5f)] private float jumpHeight = 7.3f;
        [SerializeField, Range(0.2f, 1.25f)] private float timeToJumpApex;
        [SerializeField, Range(0f, 5f)] private float upwardMovementMultiplier = 1f;
        [SerializeField, Range(1f, 10f)] private float downwardMovementMultiplier = 6.17f;
        
        
        [Header("Options")]
        [SerializeField] private float speedLimit;
        [SerializeField] private bool variableJumpHeight;
        [SerializeField, Range(1f, 10f)] private float jumpCutOff;
        [SerializeField, Range(1f, 10f)] private float coyoteTime = 0.15f;
        [SerializeField, Range(0f, 0.3f)] private float jumpBuffer = 0.15f;


        [Header("Calculations")] 
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float gravityMultiplier;
        private float _defaultGravityScale;

        [Header("Current State")]
        [SerializeField] private bool canJumpAgain = false;
        [SerializeField] private bool onGround;
        
         private bool _desiredJump;
         private float _jumpBufferCounter;
         private float _coyoteTimeCounter = 0;
         private bool _pressingJump;
         private bool _currentlyJumping;

         private void Awake()
         {
             _rigidbody2D = GetComponent<Rigidbody2D>();
             _input = GetComponent<PlayerInput>();
             _collision = GetComponent<PlayerCollision>();
             _defaultGravityScale = 1f;
         }

         private void Update()
         {
             
             onGround = _collision.IsGrounded();
             SetPhysics();
             
             if (_input.JumpHeld)
             {
                 _desiredJump = true;
                 _pressingJump = true;
             }
             else
             {
                 _pressingJump = false;
             }

             if (jumpBuffer > 0)
             {
                 if (_desiredJump)
                 {
                     jumpBuffer += Time.deltaTime;

                     if (_jumpBufferCounter > jumpBuffer)
                     {
                         _desiredJump = false;
                         _jumpBufferCounter = 0;
                     }
                 }
             }

             if (!_currentlyJumping && !onGround)
             {
                 _coyoteTimeCounter += Time.deltaTime;
             }
             else
             {
                 _coyoteTimeCounter = 0;
             }
         }

         public void SetPhysics()
         {
             Vector2 newGravity = new Vector2(0, (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex));
             _rigidbody2D.gravityScale = (newGravity.y / Physics2D.gravity.y) * gravityMultiplier;
         }

         private void FixedUpdate()
         {
             _velocity = _rigidbody2D.velocity;

             if (_desiredJump)
             {
                 Jump();
                 _rigidbody2D.velocity = _velocity;
                 return;
             }
             
             CalculateGravity();
         }

         private void CalculateGravity()
         {
             if (_rigidbody2D.velocity.y > 0.01f)
             {
                 if (onGround)
                 {
                     gravityMultiplier = _defaultGravityScale;
                 }
                 else
                 {
                     if (variableJumpHeight)
                     {
                         if (_pressingJump && _currentlyJumping)
                         {
                             gravityMultiplier = upwardMovementMultiplier;
                         }
                         else
                         {
                             gravityMultiplier = jumpCutOff;
                         }
                     }
                     else
                     {
                         gravityMultiplier = upwardMovementMultiplier;
                     }
                 }
             }
             else if (_rigidbody2D.velocity.y < -0.01f)
             {
                 if (onGround)
                     gravityMultiplier = _defaultGravityScale;
                 else
                     gravityMultiplier = downwardMovementMultiplier;
             }
             else
             {
                 if (onGround)
                 {
                     _currentlyJumping = false;
                 }

                 gravityMultiplier = _defaultGravityScale;
             }

             _rigidbody2D.velocity = new Vector2(_velocity.x, Mathf.Clamp(_velocity.y, -speedLimit, 100));
         }

         private void Jump()
         {
             if (onGround || (_coyoteTimeCounter > 0.03f && _coyoteTimeCounter < coyoteTime) || canJumpAgain)
             {
                 _desiredJump = false;
                 _jumpBufferCounter = 0;
                 _coyoteTimeCounter = 0;

                 canJumpAgain = (maxAirJumps == 1 && canJumpAgain == false);
                 
                 jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _rigidbody2D.gravityScale * jumpHeight);

                 if (_velocity.y > 0f)
                 {
                     jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0f);
                 }
                 else if (_velocity.y < 0f)
                 {
                     jumpSpeed += Mathf.Abs(_rigidbody2D.velocity.y);
                 }

                 _velocity.y += jumpSpeed;
                 _currentlyJumping = true;
             }

             if (jumpBuffer == 0)
             {
                 _desiredJump = false;
             }
         }
         
         public void BounceUp(float bounceAmount)
         {
             //Used by the springy pad
             _rigidbody2D.AddForce(Vector2.up * bounceAmount, ForceMode2D.Impulse);
         }
    }
}