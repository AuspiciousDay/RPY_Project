using UnityEngine;
using UnityEditor;
using System.IO;

public static class GothicMapBuilder
{
    [MenuItem("Tools/Gothic Map/Build Prefab")]
    public static void Build()
    {
        string root = "Assets/GothicMap";
        Sprite floor = Load(root + "/Art/Floor.png");
        Sprite pillar = Load(root + "/Art/Pillar.png");
        Sprite altar = Load(root + "/Art/Altar.png");
        if (!floor || !pillar || !altar) { Debug.LogError("Place GothicMap directly under Assets."); return; }
        GameObject map = new GameObject("GothicCathedral");
        AddSprite(map.transform, "Ground", floor, Vector2.zero, -20);
        Wall(map.transform, "NorthWall", new Vector2(0,14.5f), new Vector2(40,1));
        Wall(map.transform, "SouthWall", new Vector2(0,-14.5f), new Vector2(40,1));
        Wall(map.transform, "WestWall", new Vector2(-19.5f,0), new Vector2(1,28));
        Wall(map.transform, "EastWall", new Vector2(19.5f,0), new Vector2(1,28));
        foreach (float x in new float[]{-12,12}) foreach(float y in new float[]{-9,-4,4,9})
        {
            GameObject p = AddSprite(map.transform,"Pillar",pillar,new Vector2(x,y),0);
            BoxCollider2D c = p.AddComponent<BoxCollider2D>(); c.size = new Vector2(1.15f,.65f); c.offset = new Vector2(0,-.45f);
        }
        GameObject a = AddSprite(map.transform,"Altar",altar,new Vector2(0,11),0);
        BoxCollider2D ac = a.AddComponent<BoxCollider2D>(); ac.size = new Vector2(3.2f,1f); ac.offset = new Vector2(0,-.25f);
        PrefabUtility.SaveAsPrefabAsset(map,root+"/GothicCathedral.prefab");
        Object.DestroyImmediate(map); AssetDatabase.SaveAssets();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(root+"/GothicCathedral.prefab");
    }
    static Sprite Load(string path)
    {
        TextureImporter t = AssetImporter.GetAtPath(path) as TextureImporter;
        if (!t) return null;
        t.textureType = TextureImporterType.Sprite; t.spriteImportMode = SpriteImportMode.Single; t.spritePixelsPerUnit = 32; t.filterMode = FilterMode.Point; t.textureCompression = TextureImporterCompression.Uncompressed; t.mipmapEnabled = false; t.maxTextureSize = 2048; t.npotScale = TextureImporterNPOTScale.None; t.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
    static GameObject AddSprite(Transform parent,string name,Sprite sprite,Vector2 pos,int order)
    {
        GameObject g = new GameObject(name); g.transform.SetParent(parent,false); g.transform.localPosition = pos; SpriteRenderer r = g.AddComponent<SpriteRenderer>(); r.sprite = sprite; r.sortingOrder = order; return g;
    }
    static void Wall(Transform parent,string name,Vector2 pos,Vector2 size)
    {
        GameObject g = new GameObject(name); g.transform.SetParent(parent,false); g.transform.localPosition = pos; g.AddComponent<BoxCollider2D>().size = size;
    }
}
