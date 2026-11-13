using UnityEngine;

namespace Yoshida.TestScene
{
    public class InfiniteRotate : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 180f;

        private void Update()
        {
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
    }
}
