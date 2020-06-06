using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Sirenix.Serialization;
using System.IO;
using System;

public class SheetWindow : OdinEditorWindow
{
    [MenuItem("Tools/全局配置",priority =10)]
    private static void OpenWindow()
    {
        var window = GetWindow<SheetWindow>();
        window.Show();
        window.LoadAll();
    }


    [TabGroup("全局配置")]
    public GlobalConfig GlobalCfg;
    [TabGroup("剧本配置")]
    public NovelsConfig NovelsConfig;

    public void LoadAll()
    {
        GlobalCfg = _LoadSheet<GlobalConfig>("Assets/Resources/Config/GlobalConfig.Asset");
        NovelsConfig = _LoadSheet<NovelsConfig>("Assets/Resources/Config/NovelsConfig.Asset");
    }


    private T _LoadSheet<T>(string path) where T:ScriptableObject
    {
        var cfg = AssetDatabase.LoadAssetAtPath<T>(path);
        if (cfg == null)
        {
            var battleCfg = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(battleCfg, path);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        return cfg;
    }
}
