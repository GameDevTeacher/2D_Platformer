using System;
using UnityEngine;

namespace _Advanced
{
    public class PlayerCollision : MonoBehaviour
    {
        [SerializeField] private float groundLength = 0.95f;
        [SerializeField] private Vector3 colliderOffset;

        [SerializeField] private LayerMask WhatIsGround;
        
        public bool IsGrounded()
        {
            return Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, WhatIsGround) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, WhatIsGround);
        }

        private void OnDrawGizmos()
        {
            if (IsGrounded()){ Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }

            var position = transform.position;
            Gizmos.DrawLine(position + colliderOffset, position + colliderOffset + Vector3.down * groundLength);
            Gizmos.DrawLine(position - colliderOffset, position - colliderOffset + Vector3.down * groundLength);
        }
    }
}
