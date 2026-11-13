using System.Collections.Generic;
using UnityEngine;

namespace Yoshida.TestScene
{
    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField] private GameObject blankField;
        [SerializeField] private GameObject[] obstaclePrefabs;
        [SerializeField] private BoxCollider2D leftWall;
        [SerializeField] private BoxCollider2D rightWall;
        [SerializeField, Min(1f)] private float blockHeight = 10f;
        [SerializeField, Min(0f)] private float scrollSpeed = 5f;
        [SerializeField, Range(0, 3)] private int maxObstacles = 3;
        [SerializeField, Min(0f)] private float obstacleGap = 1.5f;
        [SerializeField, Min(0f)] private float wallGap = 2f;

        private readonly Queue<AutoScroll> blocks = new Queue<AutoScroll>();
        private readonly List<Rect> occupied = new List<Rect>(3);
        private float nextCenterY;

        public float BlockHeight => blockHeight;
        public int BlockCount => blocks.Count;
        public float FirstCenterY => blocks.Peek().CenterY;
        public float NextCenterY => nextCenterY;

        public void SpawnBackground(bool clear)
        {
            GameObject block = new GameObject("Stage Section");
            block.transform.SetParent(transform);
            block.transform.position = new Vector3(0f, nextCenterY, 0f);

            GameObject field = Instantiate(blankField, block.transform);
            field.transform.localPosition = Vector3.zero;
            field.transform.localScale = Vector3.Scale(field.transform.localScale, new Vector3(blockHeight, blockHeight, 1f));

            AutoScroll scroll = block.AddComponent<AutoScroll>();
            scroll.SetSpeed(scrollSpeed);

            if (!clear)
            {
                PlaceObstacles(block.transform);
            }

            blocks.Enqueue(scroll);
            nextCenterY += blockHeight;
        }

        public void Scroll()
        {
            foreach (AutoScroll block in blocks)
            {
                block.SetSpeed(scrollSpeed);
            }

            if (blocks.Count > 0)
            {
                nextCenterY = FirstCenterY + blocks.Count * blockHeight;
            }
        }

        public void RemoveFirst()
        {
            AutoScroll block = blocks.Dequeue();
            block.gameObject.SetActive(false);
            Destroy(block.gameObject);
        }

        private void PlaceObstacles(Transform block)
        {
            if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
            {
                return;
            }

            occupied.Clear();
            int count = Random.Range(0, Mathf.Clamp(maxObstacles, 0, 3) + 1);
            float left = leftWall.bounds.max.x + wallGap;
            float right = rightWall.bounds.min.x - wallGap;
            float verticalEdge = (blockHeight - obstacleGap) * 0.5f;

            for (int i = 0; i < count; i++)
            {
                GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                if (prefab == null)
                {
                    continue;
                }

                SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
                if (renderer == null || renderer.sprite == null)
                {
                    continue;
                }

                Bounds bounds = renderer.sprite.bounds;
                Vector2 scale = prefab.transform.localScale;
                Vector2 min = Vector2.Scale(bounds.min, scale);
                Vector2 max = Vector2.Scale(bounds.max, scale);
                if (renderer.TryGetComponent<InfiniteRotate>(out _))
                {
                    Vector2 extentsFromPivot = new Vector2(
                        Mathf.Max(Mathf.Abs(min.x), Mathf.Abs(max.x)),
                        Mathf.Max(Mathf.Abs(min.y), Mathf.Abs(max.y)));
                    float radius = extentsFromPivot.magnitude;
                    min = Vector2.one * -radius;
                    max = Vector2.one * radius;
                }

                const float padding = 0.05f;
                Rect footprint = Rect.MinMaxRect(min.x - padding, min.y - padding, max.x + padding, max.y + padding);
                if (TryPickPosition(footprint, left, right, verticalEdge, out Vector2 position))
                {
                    GameObject obstacle = Instantiate(prefab, block);
                    obstacle.transform.localPosition = position;
                    obstacle.transform.localRotation = Quaternion.identity;
                }
            }
        }

        private bool TryPickPosition(Rect footprint, float left, float right, float verticalEdge, out Vector2 position)
        {
            float minX = left - footprint.xMin;
            float maxX = right - footprint.xMax;
            float minY = -verticalEdge - footprint.yMin;
            float maxY = verticalEdge - footprint.yMax;
            position = Vector2.zero;
            if (minX > maxX || minY > maxY)
            {
                return false;
            }

            for (int attempt = 0; attempt < 24; attempt++)
            {
                position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                Rect placed = footprint;
                placed.position += position;
                Rect clearance = Rect.MinMaxRect(
                    placed.xMin - obstacleGap, placed.yMin - obstacleGap,
                    placed.xMax + obstacleGap, placed.yMax + obstacleGap);

                bool overlaps = false;
                foreach (Rect previous in occupied)
                {
                    if (clearance.Overlaps(previous))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    occupied.Add(placed);
                    return true;
                }
            }

            return false;
        }
    }
}
