using UnityEngine;

namespace _BeginnerPlus
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;
        private PlayerInput _input;
        private PlayerCollision _collision;

        /* Used if we Have Attacks
        private bool attack;
        private float animationTimer;
        */

        private static readonly int Idle = Animator.StringToHash("Player_Idle");
        private static readonly int Walk = Animator.StringToHash("Player_Walk");
        private static readonly int Jump = Animator.StringToHash("Player_Jump");
        private static readonly int Fall = Animator.StringToHash("Player_Fall");
        
        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInput>();
            _collision = GetComponent<PlayerCollision>();
        }

        private void Update()
        {
            /* Used if we Have Attacks  
            if (animationTimer > Time.time) return;
            
            if (_input.Attack)
            {
                _animator.Play("Player_Attack");
                animationTimer = Time.time + _animator.GetCurrentAnimatorClipInfo(0).Length;     
                return;
            }
            */

            if (_input.MoveVector.x != 0)
            {
                _spriteRenderer.flipX = _input.MoveVector.x < 0;
            }

            if (_collision.IsGroundedBox())
            {
                _animator.Play(_input.MoveVector.x == 0 ? Idle : Walk);
            }
            else
            {
                _animator.Play(_rigidbody2D.linearVelocity.y > 0 ? Jump : Fall);
            }
        }
    }
}