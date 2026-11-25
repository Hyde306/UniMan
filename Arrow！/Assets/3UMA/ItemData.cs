using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public Sprite icon;
    public string id;
    public string itemName;
    public GameObject prefab;

    public abstract void Use();
}