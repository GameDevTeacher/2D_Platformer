using UnityEngine;
using UnityEngine.SceneManagement;

namespace _BeginnerPlus
{
    public class PlayerCollision : MonoBehaviour
    {
        [SerializeField] private float groundCheckRange = 1.1f;
        [SerializeField] private LayerMask whatIsGround;
        private BoxCollider2D _boxCollider2D;

        private void Start()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag($"Death"))
            {
                RestartScene();
            }
        }

        private void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public bool IsNearGround()
        {
            var position = transform.position;
            
            UnityEngine.Debug.DrawRay(position, Vector2.down, new Color(1f, 0f, 1f));
            var hit = Physics2D.Raycast(position, Vector2.down, groundCheckRange, whatIsGround);
            return hit.collider != null;
        }
    
        public bool IsGroundedRay()
        {
            var position = transform.position;
            
            UnityEngine.Debug.DrawRay(position, Vector2.down, new Color(1f, 0f, 1f));
            var hit = Physics2D.Raycast(position, Vector2.down, groundCheckRange, whatIsGround);
            return hit.collider != null;
        }

        public bool IsGroundedBox()
        {
            
            var bounds = _boxCollider2D.bounds;
            return Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, .1f, whatIsGround);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawCube(transform.position, Vector3.down);
        }
    }
}
