using UnityEngine;

namespace Yoshida.TestScene
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class DestroyBackground : MonoBehaviour
    {
        [SerializeField] private BackgroundManager manager;
        [SerializeField, Min(1)] private int startCount = 2;

        private UnityEngine.Camera stageCamera;

        private void Start()
        {
            stageCamera = GetComponent<UnityEngine.Camera>();
            for (int i = 0; i < startCount; i++)
            {
                manager.SpawnBackground(i == 0);
            }
        }

        private void FixedUpdate()
        {
            manager.Scroll();
            float halfBlock = manager.BlockHeight * 0.5f;
            float top = stageCamera.transform.position.y + stageCamera.orthographicSize;
            float bottom = stageCamera.transform.position.y - stageCamera.orthographicSize;

            while (manager.NextCenterY - halfBlock <= top + manager.BlockHeight)
            {
                manager.SpawnBackground(false);
            }

            while (manager.BlockCount > 1 && manager.FirstCenterY + halfBlock < bottom - manager.BlockHeight)
            {
                manager.RemoveFirst();
            }
        }
    }
}
