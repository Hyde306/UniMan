using System.Collections.Generic;
using UnityEngine;

//区間プレハブをランダムに上へ接続　古い区間を自動削除

[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public sealed class Stage : MonoBehaviour
{
    [Header("ステージプレハブ")]
    [SerializeField] private GameObject startingStagePrefab;
    [SerializeField] private GameObject[] stagePrefabs;
    [SerializeField] private bool avoidImmediateRepeats = true;

    [Header("追従対象")]
    [SerializeField] private Transform player;
    [SerializeField] private UnityEngine.Camera targetCamera;

    [Header("区間結合")]
    [SerializeField] private float initialBottomY = -8.5f;
    [SerializeField, Min(0.1f)] private float segmentLength = 17f;
    [SerializeField, Min(1)] private int initialSegmentCount = 3;
    [SerializeField, Min(1)] private int segmentsAheadOfPlayer = 3;
    [SerializeField, Min(0)] private int segmentsBehindPlayer = 3;

    private readonly Queue<StagePiece> activePieces = new Queue<StagePiece>();
    private float nextSpawnY;
    private int lastRandomIndex = -1;
    private bool startingStageSpawned;

    public int ActivePieceCount => activePieces.Count;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = UnityEngine.Camera.main;
        }

        nextSpawnY = initialBottomY;

        // 開始直後に足場が無くならないよう、最初の数区間を先に作る。
        for (int index = 0; index < initialSegmentCount; index++)
        {
            if (!SpawnNextPiece())
            {
                return;
            }
        }
    }

    private void Update()
    {
        GenerateAheadOfPlayer();
        RemoveOldPieces();
    }

    private void GenerateAheadOfPlayer()
    {
        float requiredTopY = GetReferenceY() + segmentLength * segmentsAheadOfPlayer;
        int safetyCounter = 0;

        while (nextSpawnY < requiredTopY && safetyCounter < 128)
        {
            if (!SpawnNextPiece())
            {
                return;
            }

            safetyCounter++;
        }

        if (safetyCounter >= 128)
        {
            Debug.LogError("StageGenerator: 生成処理の安全上限に達しました。区間長を確認してください。", this);
            enabled = false;
        }
    }

    private bool SpawnNextPiece()
    {
        GameObject prefab = SelectNextPrefab();
        if (prefab == null)
        {
            Debug.LogError("StageGenerator: 使用できるステージプレハブが登録されていません。", this);
            enabled = false;
            return false;
        }

        return Spawn(prefab);
    }

    private GameObject SelectNextPrefab()
    {
        if (!startingStageSpawned && startingStagePrefab != null)
        {
            startingStageSpawned = true;
            return startingStagePrefab;
        }

        startingStageSpawned = true;

        if (stagePrefabs == null || stagePrefabs.Length == 0)
        {
            return null;
        }

        int index = Random.Range(0, stagePrefabs.Length);

        if (avoidImmediateRepeats && stagePrefabs.Length > 1 && index == lastRandomIndex)
        {
            index = (index + Random.Range(1, stagePrefabs.Length)) % stagePrefabs.Length;
        }

        lastRandomIndex = index;
        return stagePrefabs[index];
    }

    private bool Spawn(GameObject prefab)
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, nextSpawnY, transform.position.z);
        GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        StagePiece instance = spawnedObject.GetComponent<StagePiece>();

        if (instance == null)
        {
            Debug.LogError($"StageGenerator: {prefab.name}にStagePieceが付いていません。", prefab);
            Destroy(spawnedObject);
            enabled = false;
            return false;
        }

        spawnedObject.name = prefab.name;
        activePieces.Enqueue(instance);

        // すべての区間を同じ長さで接続し、隙間や重なりを防ぐ。
        nextSpawnY += segmentLength;
        return true;
    }

    private void RemoveOldPieces()
    {
        float deleteBelowY = GetReferenceY() - segmentLength * segmentsBehindPlayer;

        while (activePieces.Count > 0 && activePieces.Peek().TopY < deleteBelowY)
        {
            StagePiece oldPiece = activePieces.Dequeue();
            if (oldPiece != null)
            {
                Destroy(oldPiece.gameObject);
            }
        }
    }

    private float GetReferenceY()
    {
        if (player != null)
        {
            return player.position.y;
        }

        if (targetCamera != null)
        {
            return targetCamera.transform.position.y;
        }

        return transform.position.y;
    }

    private void OnValidate()
    {
        segmentLength = Mathf.Max(0.1f, segmentLength);
        initialSegmentCount = Mathf.Max(1, initialSegmentCount);
        segmentsAheadOfPlayer = Mathf.Max(1, segmentsAheadOfPlayer);
        segmentsBehindPlayer = Mathf.Max(0, segmentsBehindPlayer);
    }
}
