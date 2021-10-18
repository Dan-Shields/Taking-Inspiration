using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomTile : Tile
{
    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go)
    {

        return true;
    }

    #if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a CustomTile Asset
    [MenuItem("Assets/Create/CustomTile")]
    public static void CreateCustomTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Custom Tile", "New Custom Tile", "Asset", "Save Custom Tile", "Assets");
        if (path == "")
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CustomTile>(), path);
    }
    #endif
}
