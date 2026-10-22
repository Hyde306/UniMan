using UnityEngine;

//ステージプレハブ高さ管理

[DisallowMultipleComponent]
public sealed class StagePiece : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float width = 17f;
    [SerializeField, Min(0.1f)] private float height = 10f;

    public float Width => Mathf.Max(0.1f, width);
    public float Height => Mathf.Max(0.1f, height);
    public float BottomY => transform.position.y;
    public float TopY => BottomY + Height;

    public void SetHeight(float value)
    {
        height = Mathf.Max(0.1f, value);
    }

    public void SetSize(float widthValue, float heightValue)
    {
        width = Mathf.Max(0.1f, widthValue);
        height = Mathf.Max(0.1f, heightValue);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.9f, 1f, 0.8f);
        Vector3 center = transform.position + Vector3.up * (Height * 0.5f);
        Gizmos.DrawWireCube(center, new Vector3(Width, Height, 0f));
    }
}
