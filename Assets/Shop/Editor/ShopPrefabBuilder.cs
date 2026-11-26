using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class ShopPrefabBuilder
{
    static Font font;
    static Color gold = new Color32(218,177,75,255), dark = new Color32(48,32,30,255), panel = new Color32(73,73,108,255), gray = new Color32(143,143,143,255), cream = new Color32(255,244,210,255);
    static RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
    {
        var g = new GameObject(name, typeof(RectTransform)); if(parent != null) g.transform.SetParent(parent,false);
        var r = g.GetComponent<RectTransform>(); r.anchorMin = r.anchorMax = r.pivot = new Vector2(.5f,.5f); r.anchoredPosition = new Vector2(x,y); r.sizeDelta = new Vector2(w,h); return r;
    }
    static Image Fill(RectTransform r, Color c) { var i = r.gameObject.AddComponent<Image>(); i.color=c; i.raycastTarget=false; return i; }
    static RectTransform Frame(string name, Transform p,float x,float y,float w,float h,Color c)
    {
        var r=Rect(name,p,x,y,w,h); Fill(r,dark); Fill(Rect("GoldBorder",r,0,0,w-4,h-4),gold); Fill(Rect("InnerShadow",r,0,0,w-10,h-10),dark); Fill(Rect("Surface",r,0,0,w-14,h-14),c); return r;
    }
    static Text Label(string name,Transform p,string value,float x,float y,float w,float h,int size,Color c,TextAnchor align=TextAnchor.MiddleCenter)
    {
        var t=Rect(name,p,x,y,w,h).gameObject.AddComponent<Text>(); t.font=font;t.text=value;t.fontSize=size;t.color=c;t.alignment=align;t.raycastTarget=false;return t;
    }
    static Button ButtonOn(RectTransform r) { var b=r.gameObject.AddComponent<Button>(); var i=r.GetComponent<Image>();i.raycastTarget=true;b.targetGraphic=r.Find("Surface").GetComponent<Image>();return b; }
    static void Icon(Transform p,int index,float x,float y,float size)
    {
        var r=Frame("Icon",p,x,y,size,size,new Color32(20,31,43,255));
        var sprite=AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Shop/Icons/Icon{index}.png");
        var i=Fill(Rect("Artwork",r,0,0,size-16,size-16),Color.white);i.sprite=sprite;i.preserveAspect=true;
    }
    [MenuItem("Tools/Shop/Create PowerUp Shop Prefab")]
    public static void Build()
    {
        for(int n=0;n<12;n++) { var importer=AssetImporter.GetAtPath($"Assets/Shop/Icons/Icon{n}.png") as TextureImporter; if(importer!=null) { importer.textureType=TextureImporterType.Sprite; importer.spriteImportMode=SpriteImportMode.Single; importer.filterMode=FilterMode.Point; importer.textureCompression=TextureImporterCompression.Uncompressed; importer.mipmapEnabled=false; importer.SaveAndReimport(); } }
        font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if(font==null) { Debug.LogError("LegacyRuntime font unavailable.");return; }
        var root=Rect("PowerUpShop",null,0,0,1920,1080); var canvas=root.gameObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=50;
        var scaler=root.gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=.5f;root.gameObject.AddComponent<GraphicRaycaster>();
        var shade=Rect("Backdrop",root,0,0,1920,1080);shade.anchorMin=Vector2.zero;shade.anchorMax=Vector2.one;shade.sizeDelta=Vector2.zero;Fill(shade,new Color(.10f,.025f,.04f,.72f)).raycastTarget=true;
        var main=Frame("ShopPanel",root,0,-48,800,950,panel);
        Label("Title",main,"PowerUp Selection",0,425,760,64,42,cream);
        var wallet=Frame("Wallet",root,0,490,300,76,Color.black);Label("CoinSymbol",wallet,"G",-112,0,52,60,34,gold);Label("GoldAmount",wallet,"0",48,0,180,60,36,cream,TextAnchor.MiddleRight);
        var back=Frame("BackButton",root,320,490,146,76,new Color32(159,54,22,255));ButtonOn(back);Label("Label",back,"BACK",0,0,130,60,26,cream);
        string[] names={"Might","Max Health","Armor","Amount","Cooldown","Area","Velocity","Duration","Speed","Magnet","Luck","Growth"};int[] levels={5,3,3,1,2,2,2,2,2,2,3,4};
        for(int n=0;n<12;n++) { var slot=Frame(names[n].Replace(" ","")+"Slot",main,-294+(n%4)*196,280-(n/4)*194,174,174,gray);ButtonOn(slot);Label("Name",slot,names[n],0,62,160,30,22,dark);Icon(slot,n,0,-3,76);for(int j=0;j<levels[n];j++) Frame("Level"+(j+1),slot,(j-(levels[n]-1)*.5f)*23,-61,22,22,new Color32(57,37,49,255)); }
        var detail=Frame("Details",main,0,-390,766,152,gray);Label("SelectedName",detail,"Might",-255,51,210,30,24,cream);Icon(detail,0,-285,-20,76);Label("Description",detail,"Raises inflicted damage\nby 5% per level (max +25%).",-20,-14,390,100,23,cream,TextAnchor.MiddleLeft);Label("Price",detail,"G 200",280,48,150,30,24,cream);var buy=Frame("BuyButton",detail,280,-25,144,76,new Color32(83,166,78,255));ButtonOn(buy);Label("Label",buy,"Buy",0,0,120,60,28,cream);
        const string path="Assets/Shop/PowerUpShop.prefab";PrefabUtility.SaveAsPrefabAsset(root.gameObject,path);Object.DestroyImmediate(root.gameObject);AssetDatabase.SaveAssets();Selection.activeObject=AssetDatabase.LoadAssetAtPath<GameObject>(path);EditorGUIUtility.PingObject(Selection.activeObject);Debug.Log("Created "+path+". Add an EventSystem to the scene if needed.");
    }
}
