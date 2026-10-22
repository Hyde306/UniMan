#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/// <summary>
/// Yoshidaフォルダ内にプレハブと実行シーンを作るセットアップツールです。
/// 通常は作成済みのVerticalRunner.unityを使えばよく、再生成したい場合だけ
/// Tools/Yoshida/Build Vertical Runner Stageを実行してください。
/// </summary>
public static class YoshidaVerticalRunnerSetup
{
    private const string Root = "Assets/4Yoshida";
    private const string ArtFolder = Root + "/Art";
    private const string PrefabFolder = Root + "/StagePrefabs";
    private const string ScenePath = Root + "/VerticalRunner.unity";
    private const float BaseStageWidth = 10f;
    private const float BaseStageHeight = 10f;
    private const float StageScale = 1.7f;
    private const float StageWidth = BaseStageWidth * StageScale;
    private const float StageHeight = BaseStageHeight * StageScale;

    private static readonly Color BackgroundColor = new Color(0.045f, 0.055f, 0.09f, 1f);
    private static readonly Color WallColor = new Color(0.15f, 0.85f, 1f, 1f);
    private static readonly Color ObstacleColor = new Color(1f, 0.24f, 0.42f, 1f);

    [MenuItem("Tools/Yoshida/Build Vertical Runner Stage")]
    public static void Build()
    {
        EnsureFolder(Root);
        EnsureFolder(ArtFolder);
        EnsureFolder(PrefabFolder);

        Sprite squareSprite = CreateSquareSprite();
        GameObject startStage = CreateStagePrefab("Stage_Start", 0, squareSprite);
        var randomStages = new List<GameObject>();

        for (int pattern = 1; pattern <= 5; pattern++)
        {
            randomStages.Add(CreateStagePrefab($"Stage_{pattern:00}", pattern, squareSprite));
        }

        CreatePlayableScene(squareSprite, startStage, randomStages);
        AddSceneToBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Yoshida縦スクロールステージの作成完了: {ScenePath}");
    }

    // -executeMethod用。バッチモードからも同じセットアップを呼べます。
    public static void BuildFromCommandLine()
    {
        Build();
        ValidateFromCommandLine();
    }

    [MenuItem("Tools/Yoshida/Apply 1.7x Stage And Movement Fix")]
    public static void ApplyRequestedAdjustments()
    {
        ResizeExistingStagePrefabs();

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Stage generator = Object.FindFirstObjectByType<Stage>();
        Player player = Object.FindFirstObjectByType<Player>();

        if (generator == null || player == null)
        {
            throw new InvalidDataException("実行シーンにStageGeneratorまたはPlayerがありません。");
        }

        var generatorProperties = new SerializedObject(generator);
        generatorProperties.FindProperty("initialBottomY").floatValue = -StageHeight * 0.5f;
        generatorProperties.FindProperty("segmentLength").floatValue = StageHeight;
        generatorProperties.FindProperty("initialSegmentCount").intValue = 3;
        generatorProperties.FindProperty("segmentsAheadOfPlayer").intValue = 3;
        generatorProperties.FindProperty("segmentsBehindPlayer").intValue = 3;
        generatorProperties.ApplyModifiedPropertiesWithoutUndo();

        var playerProperties = new SerializedObject(player);
        playerProperties.FindProperty("visualTurnSpeed").floatValue = 2400f;
        playerProperties.FindProperty("snapVisualRotation").boolValue = true;
        playerProperties.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("ステージ1.7倍化・区間結合・プレイヤー方向転換の調整完了");
    }

    public static void ApplyRequestedAdjustmentsFromCommandLine()
    {
        ApplyRequestedAdjustments();
        ValidateFromCommandLine();
    }

    public static void ValidateFromCommandLine()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject[] roots = scene.GetRootGameObjects();

        int missingScriptCount = roots
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
            .Sum(item => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(item.gameObject));

        if (missingScriptCount != 0)
        {
            throw new InvalidDataException($"{ScenePath}に{missingScriptCount}個のMissing Scriptがあります。");
        }

        Stage generator = Object.FindFirstObjectByType<Stage>();
        Player player = Object.FindFirstObjectByType<Player>();
        Camera cameraFollow = Object.FindFirstObjectByType<Camera>();

        if (generator == null || player == null || cameraFollow == null || UnityEngine.Camera.main == null)
        {
            throw new InvalidDataException("実行シーンに必要なコンポーネントが足りません。");
        }

        var generatorProperties = new SerializedObject(generator);
        SerializedProperty startPrefab = generatorProperties.FindProperty("startingStagePrefab");
        SerializedProperty stagePrefabs = generatorProperties.FindProperty("stagePrefabs");
        if (startPrefab.objectReferenceValue == null || stagePrefabs.arraySize < 1)
        {
            throw new InvalidDataException("ステージプレハブの参照設定が不正です。");
        }

        for (int index = 0; index < stagePrefabs.arraySize; index++)
        {
            var stageObject = stagePrefabs.GetArrayElementAtIndex(index).objectReferenceValue as GameObject;
            StagePiece stage = stageObject != null ? stageObject.GetComponent<StagePiece>() : null;
            bool hasExpectedSize = stage != null &&
                Mathf.Approximately(stage.Width, StageWidth) &&
                Mathf.Approximately(stage.Height, StageHeight);
            if (!hasExpectedSize || stageObject.GetComponentsInChildren<Collider2D>(true).Length < 2)
            {
                throw new InvalidDataException($"ステージプレハブ{index}の設定が不正です。");
            }
        }

        Debug.Log($"YOSHIDA_VALIDATION_OK: Missing Script 0 / 登録ステージ{stagePrefabs.arraySize}種 / 1.7倍サイズ / プレイヤー / カメラ確認済み");
    }

    private static Sprite CreateSquareSprite()
    {
        const string assetPath = ArtFolder + "/WhiteSquare.png";
        string absolutePath = Path.Combine(Application.dataPath, "4Yoshida/Art/WhiteSquare.png");

        var texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        var pixels = Enumerable.Repeat(Color.white, 32 * 32).ToArray();
        texture.SetPixels(pixels);
        texture.Apply();
        File.WriteAllBytes(absolutePath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 32f;
        importer.filterMode = FilterMode.Point;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = false;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }

    private static GameObject CreateStagePrefab(string name, int pattern, Sprite sprite)
    {
        var root = new GameObject(name);
        root.transform.localScale = new Vector3(StageScale, StageScale, 1f);
        StagePiece stagePiece = root.AddComponent<StagePiece>();
        stagePiece.SetSize(StageWidth, StageHeight);

        CreateBlock(root.transform, "Background", sprite, new Vector2(0f, BaseStageHeight * 0.5f),
            new Vector2(BaseStageWidth, BaseStageHeight), BackgroundColor, false, -20);

        CreateBlock(root.transform, "Wall_Left", sprite, new Vector2(-4.75f, BaseStageHeight * 0.5f),
            new Vector2(0.5f, BaseStageHeight), WallColor, true, 0);
        CreateBlock(root.transform, "Wall_Right", sprite, new Vector2(4.75f, BaseStageHeight * 0.5f),
            new Vector2(0.5f, BaseStageHeight), WallColor, true, 0);

        AddPattern(root.transform, pattern, sprite);

        string path = $"{PrefabFolder}/{name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        GameObject savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        return savedPrefab;
    }

    private static void ResizeExistingStagePrefabs()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder });
        int resizedCount = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            StagePiece stagePiece = root.GetComponent<StagePiece>();

            if (stagePiece == null)
            {
                PrefabUtility.UnloadPrefabContents(root);
                continue;
            }

            root.transform.localScale = new Vector3(StageScale, StageScale, 1f);
            stagePiece.SetSize(StageWidth, StageHeight);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
            resizedCount++;
        }

        if (resizedCount == 0)
        {
            throw new InvalidDataException("サイズを変更できるステージプレハブがありません。");
        }
    }

    private static void AddPattern(Transform root, int pattern, Sprite sprite)
    {
        switch (pattern)
        {
            case 1:
                CreateObstacle(root, sprite, "Lower_Left", new Vector2(-2.7f, 3f), new Vector2(3.6f, 0.55f));
                CreateObstacle(root, sprite, "Upper_Right", new Vector2(2.7f, 7.1f), new Vector2(3.6f, 0.55f));
                break;

            case 2:
                CreateObstacle(root, sprite, "Lower_Right", new Vector2(2.5f, 2.4f), new Vector2(4f, 0.55f));
                CreateObstacle(root, sprite, "Middle_Left", new Vector2(-2.5f, 5.3f), new Vector2(4f, 0.55f));
                CreateObstacle(root, sprite, "Upper_Center", new Vector2(0f, 8.2f), new Vector2(3f, 0.55f));
                break;

            case 3:
                CreateObstacle(root, sprite, "Gate_Left_Lower", new Vector2(-3.25f, 2.5f), new Vector2(2.5f, 0.55f));
                CreateObstacle(root, sprite, "Gate_Right_Lower", new Vector2(3.25f, 2.5f), new Vector2(2.5f, 0.55f));
                CreateObstacle(root, sprite, "Gate_Left_Upper", new Vector2(-3.25f, 7f), new Vector2(2.5f, 0.55f));
                CreateObstacle(root, sprite, "Gate_Right_Upper", new Vector2(3.25f, 7f), new Vector2(2.5f, 0.55f));
                break;

            case 4:
                CreateObstacle(root, sprite, "Pillar_Left", new Vector2(-1.8f, 3f), new Vector2(0.6f, 3f));
                CreateObstacle(root, sprite, "Pillar_Right", new Vector2(1.8f, 7f), new Vector2(0.6f, 3f));
                break;

            case 5:
                CreateObstacle(root, sprite, "Zig_1", new Vector2(-3.15f, 2f), new Vector2(2.7f, 0.5f));
                CreateObstacle(root, sprite, "Zig_2", new Vector2(1.5f, 4.3f), new Vector2(5.5f, 0.5f));
                CreateObstacle(root, sprite, "Zig_3", new Vector2(-1.5f, 6.6f), new Vector2(5.5f, 0.5f));
                CreateObstacle(root, sprite, "Zig_4", new Vector2(3.15f, 8.8f), new Vector2(2.7f, 0.5f));
                break;
        }
    }

    private static void CreateObstacle(Transform root, Sprite sprite, string name, Vector2 position, Vector2 size)
    {
        CreateBlock(root, name, sprite, position, size, ObstacleColor, true, 1);
    }

    private static GameObject CreateBlock(
        Transform parent,
        string name,
        Sprite sprite,
        Vector2 localPosition,
        Vector2 size,
        Color color,
        bool addCollider,
        int sortingOrder)
    {
        var block = new GameObject(name);
        block.transform.SetParent(parent, false);
        block.transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
        block.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        if (addCollider)
        {
            block.tag = "Wall";
            BoxCollider2D collider = block.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        return block;
    }

    private static void CreatePlayableScene(Sprite sprite, GameObject startStage, IReadOnlyList<GameObject> randomStages)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 3f, -10f);

        UnityEngine.Camera camera = cameraObject.AddComponent<UnityEngine.Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.015f, 0.02f, 0.04f, 1f);
        cameraObject.AddComponent<AudioListener>();
        cameraObject.AddComponent<UniversalAdditionalCameraData>();

        var player = new GameObject("Player");
        player.transform.position = Vector3.zero;

        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D playerCollider = player.AddComponent<CircleCollider2D>();
        playerCollider.radius = 0.42f;

        var visual = new GameObject("Visual");
        visual.transform.SetParent(player.transform, false);
        SpriteRenderer playerRenderer = visual.AddComponent<SpriteRenderer>();
        playerRenderer.sprite = sprite;
        playerRenderer.color = new Color(0.98f, 0.86f, 0.2f, 1f);
        playerRenderer.sortingOrder = 10;
        visual.transform.localScale = new Vector3(0.78f, 0.78f, 1f);

        var directionMarker = new GameObject("DirectionMarker");
        directionMarker.transform.SetParent(visual.transform, false);
        directionMarker.transform.localPosition = new Vector3(0f, 0.7f, 0f);
        directionMarker.transform.localScale = new Vector3(0.22f, 0.5f, 1f);
        SpriteRenderer markerRenderer = directionMarker.AddComponent<SpriteRenderer>();
        markerRenderer.sprite = sprite;
        markerRenderer.color = Color.white;
        markerRenderer.sortingOrder = 11;

        Player playerController = player.AddComponent<Player>();
        var playerProperties = new SerializedObject(playerController);
        playerProperties.FindProperty("visual").objectReferenceValue = visual.transform;
        playerProperties.FindProperty("gameplayCamera").objectReferenceValue = camera;
        playerProperties.FindProperty("visualTurnSpeed").floatValue = 2400f;
        playerProperties.FindProperty("snapVisualRotation").boolValue = true;
        playerProperties.ApplyModifiedPropertiesWithoutUndo();

        Camera cameraFollow = cameraObject.AddComponent<Camera>();
        var cameraProperties = new SerializedObject(cameraFollow);
        cameraProperties.FindProperty("target").objectReferenceValue = player.transform;
        cameraProperties.ApplyModifiedPropertiesWithoutUndo();

        var stageGeneratorObject = new GameObject("StageGenerator");
        Stage generator = stageGeneratorObject.AddComponent<Stage>();
        var generatorProperties = new SerializedObject(generator);
        generatorProperties.FindProperty("startingStagePrefab").objectReferenceValue = startStage;
        generatorProperties.FindProperty("player").objectReferenceValue = player.transform;
        generatorProperties.FindProperty("targetCamera").objectReferenceValue = camera;
        generatorProperties.FindProperty("initialBottomY").floatValue = -StageHeight * 0.5f;
        generatorProperties.FindProperty("segmentLength").floatValue = StageHeight;
        generatorProperties.FindProperty("initialSegmentCount").intValue = 3;
        generatorProperties.FindProperty("segmentsAheadOfPlayer").intValue = 3;
        generatorProperties.FindProperty("segmentsBehindPlayer").intValue = 3;

        SerializedProperty prefabArray = generatorProperties.FindProperty("stagePrefabs");
        prefabArray.arraySize = randomStages.Count;
        for (int index = 0; index < randomStages.Count; index++)
        {
            prefabArray.GetArrayElementAtIndex(index).objectReferenceValue = randomStages[index];
        }

        generatorProperties.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
    }

    private static void AddSceneToBuildSettings()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.path != ScenePath)
            .ToList();

        scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static void EnsureFolder(string assetPath)
    {
        if (AssetDatabase.IsValidFolder(assetPath))
        {
            return;
        }

        string parent = Path.GetDirectoryName(assetPath)?.Replace('\\', '/');
        string name = Path.GetFileName(assetPath);
        if (!string.IsNullOrEmpty(parent))
        {
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
