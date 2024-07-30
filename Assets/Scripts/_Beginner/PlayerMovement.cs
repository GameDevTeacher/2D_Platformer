using UnityEngine;

namespace _Beginner
{
    public class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 4f;
        public float jumpSpeed = 10f;

        private PlayerInput _input;
        private PlayerCollision _collision;
        private Rigidbody2D _rigidbody2D;

        // Start is called before the first frame update
        private void Start()
        {
            _input = GetComponent<PlayerInput>();
            _collision = GetComponent<PlayerCollision>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_collision.IsGroundedBox()) return;

            if (_input.JumpPressed)
            {
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpSpeed);
            }
        }

        private void FixedUpdate()
        {
            _rigidbody2D.linearVelocity = new Vector2(_input.MoveDirection.x * moveSpeed, _rigidbody2D.linearVelocity.y);
        }
    }
}