using UnityEngine;

namespace Yoshida.TestScene
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AutoScroll : MonoBehaviour
    {
        private Rigidbody2D body;

        public float CenterY => body.position.y;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void SetSpeed(float speed)
        {
            body.linearVelocity = Vector2.down * speed;
        }
    }
}
