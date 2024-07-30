using UnityEngine;

namespace _IntermediatePlus
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private PlayerInput _input;
        private PlayerCollision _collision;
        private Rigidbody2D _rigidbody2D;

        private int _currentAnimationState;
        private float _lockedUntil;
        private float _landAnimationDuration;
        private float _attackAnimationDuration;

        

        #region Cached Animations

        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Fall = Animator.StringToHash("Fall");
        private static readonly int Land = Animator.StringToHash("Land");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Crouch = Animator.StringToHash("Crouch");

        #endregion

        
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }
        
        private void Update()
        {
            var _animationState = SetAnimationState();
           
            if (_animationState == _currentAnimationState) return;
            _animator.CrossFade(_animationState, 0, 0);
            _currentAnimationState = _animationState;
        }

        int SetAnimationState()
        {
            // Set a timer to make sure you cannot play a new animation until another is finished
            if (Time.time < _lockedUntil) return _currentAnimationState;
            
            if (_input.Attacking) return LockStateTime(Attack, _animator.GetCurrentAnimatorClipInfo(0).Length);
            if (_input.JumpPressed) return Jump;

            if (_collision.IsGroundedBox()) return _input.MoveDirection.x == 0 ? Idle : Walk;
            return _rigidbody2D.linearVelocity.y > 0 ? Jump : Fall;
            
            int LockStateTime(int s, float t) {
                _lockedUntil = Time.time + t;
                return s;
            }
        }
    }
}
