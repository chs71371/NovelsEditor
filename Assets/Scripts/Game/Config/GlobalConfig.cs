using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Novels;
using System.Linq;


[InlineEditor]
public class GlobalConfig : SerializedScriptableObject
{
    public static GlobalConfig _instance;

    public static GlobalConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                Reload();
            }
            return _instance;
        }
    }

    public static void Reload()
    {
        var res = Resources.Load<GlobalConfig>("Config/GlobalConfig");

        if (res != null)
        {
            _instance = res;
        }
    }

#if UNITY_EDITOR
    [Button("刷新")]
    public void Fresh() 
    {
        Reload();

        //刷新动画文件
        var objs = UnityEditor.AssetDatabase.FindAssets("t:AnimationClip", new string[] { "Assets/Resources/Animation" });
        AnimationList = objs.Select(o => UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(o), typeof(AnimationClip)).name).ToList();
    }
#endif


    [LabelText("角色数据")]
    [InlineEditor]
    public class NovelsCharaData
    {
        [LabelText("标识")]
        public string Key;
        [LabelText("名字")]
        public string Name;
        [LabelText("角色立绘列表")]
        public List<CharaSprite> Sprites;


        public class CharaSprite
        {
            [LabelText("贴图尺寸")]
            public Vector2 TextureSize;
            [LabelText("表情类型")]
            public DialogueContent.EExpressionType Express;
            [PreviewField(200)]
            [HideLabel]
            public Sprite Sprite;
        }
    }

    [BoxGroup("立绘图配置")]
    public NovelsCharaData[] NovelsCharas = new NovelsCharaData[0];

    [BoxGroup("文本配置")]
    [LabelText("字符出现间隔")]
    public float CharSpeed = 0.02f;
    [LabelText("对话间隔强制等待时间")]
    public float ForceTextWait = 0.5f;

    [BoxGroup("音效配置")]
    [LabelText("字体出现声音")]
    public AudioClipData SFX_TextAppear = new AudioClipData();


    [BoxGroup("事件配置")]
    [LabelText("角色显示字典")]
    public Dictionary<string, ShowCharaSet> CharaMap = new Dictionary<string, ShowCharaSet>();
    [BoxGroup("事件配置")]
    [LabelText("事件字典")]
    public Dictionary<string, INovelsSet> EventMap = new Dictionary<string, INovelsSet>();

    [LabelText("角色动画列表")]
    public List<string> AnimationList = new List<string>();
}
