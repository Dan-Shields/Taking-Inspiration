using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomTile : Tile
{
    public Sprite overlaySprite;
    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go)
    {
        if (go && this.overlaySprite)
        {
            SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();

            if (sr) sr.sprite = this.overlaySprite;
        }

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
