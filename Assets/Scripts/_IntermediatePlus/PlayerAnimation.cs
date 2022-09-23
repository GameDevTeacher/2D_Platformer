using System;
using UnityEngine;
using UnityEngine.Animations;

namespace _IntermediatePlus
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        [SerializeField] private float _minImpactForce = 20;
        private int _currentAnimationState;

        public event Action<bool, float> GroundChanged;
        private bool _grounded;
        private bool _landed;
        
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Fall = Animator.StringToHash("Fall");
        private static readonly int Land = Animator.StringToHash("Land");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Crouch = Animator.StringToHash("Crouch");
        
        private void Start()
        {
            _animator = GetComponent<Animator>();

            GroundChanged += (grounded, impactForce) =>
            {
                _grounded = grounded;
                _landed = impactForce >= _minImpactForce;
            };
        }
        
        private void Update()
        {
            _animator.Play("State1");
            
        }

       
    }
}
