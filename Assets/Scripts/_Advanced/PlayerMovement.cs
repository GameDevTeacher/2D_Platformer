using System;
using UnityEngine;

namespace _Advanced
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Stats")]
        [SerializeField] [Range(0f, 20f)]  private float maxSpeed = 10f;
        [SerializeField] [Range(0f, 100f)] private float maxAcceleration = 52f;
        [SerializeField] [Range(0f, 100f)] private float maxDeceleration = 52f;
        [SerializeField] [Range(0f, 100f)] private float maxTurnSpeed = 80f;
        [SerializeField] [Range(0f, 100f)] private float maxAirAcceleration;
        [SerializeField] [Range(0f, 100f)] private float maxAirDeceleration;
        [SerializeField] [Range(0f, 100f)] private float maxAirTurnSpeed = 80f;
        [SerializeField] private float friction;

        [Header("Options")]
        public Vector2 velocity;
        private Vector2 _desiredVelocity;
        private float _maxSpeedChange;
        private float _acceleration;
        private float _deceleration;
        private float _turnSpeed;
        
        /* COMPONENTS */
        private Rigidbody2D _rigidbody2D;
        private PlayerInput _input;
        private PlayerCollision _collision;

        public bool onGround;
        public bool pressingKey;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInput>();
            _collision = GetComponent<PlayerCollision>();
        }

        private void Update()
        {
            if (_input.MoveDirection.x != 0)
            {
                transform.localScale = new Vector3(_input.MoveDirection.x > 0f ? 1f : -1f, 1f, 1f);
                pressingKey = true;
            }
            else
            {
                pressingKey = false;
            }
            _desiredVelocity = new Vector2(_input.MoveDirection.x, 0f) * Mathf.Max(maxSpeed - friction, 0f);
        }

        private void FixedUpdate()
        {
            onGround = _collision.IsGrounded();
            velocity = _rigidbody2D.velocity;

            if (onGround)
            {
                RunWithoutAcceleration();
            }
            else
            {
                RunWithAcceleration();
            }
        }

        private void RunWithAcceleration()
        {
            _acceleration = onGround ? maxAcceleration : maxAirAcceleration;
            _deceleration = onGround ? maxDeceleration : maxAirDeceleration;
            _turnSpeed = onGround ? maxTurnSpeed : maxAirTurnSpeed;

            if (pressingKey)
            {
                if (Mathf.Sign(_input.MoveDirection.x) != Mathf.Sign(velocity.x))
                {
                    _maxSpeedChange = _turnSpeed * Time.deltaTime;
                }
                else
                {
                    _maxSpeedChange = _acceleration * Time.deltaTime;
                }
            }
            else
            {
                _maxSpeedChange = _deceleration * Time.deltaTime;
            }

            velocity.x = Mathf.MoveTowards(velocity.x, _desiredVelocity.x, _maxSpeedChange);
            _rigidbody2D.velocity = velocity;
        }
        private void RunWithoutAcceleration()
        {
            velocity.x = _desiredVelocity.x;

            _rigidbody2D.velocity = velocity;
        }
    }
}