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
using Sirenix.Utilities.Editor;
using Novels;
using Sirenix.Utilities;
using System.Linq.Expressions;

public class NoverlsWindow : OdinMenuEditorWindow
{
    [MenuItem("Tools/NoverlsEditor")]
    private static void OpenWindow()
    {
        GetWindow<NoverlsWindow>().Show();
    }

    private CreateNovelsScene _CreateData;

    private static int _currentId;

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();

        _CreateData = new CreateNovelsScene();

        var check=  new CheckBatchNovels();


        tree.Add("创建新场景", new CreateNovelsScene());
        tree.Add("批量处理文本", check);

        var temp = tree.AddAllAssetsAtPath("场景列表", "Assets/Resources/Config/", typeof(NovelsSectionData),true);
        _currentId = temp.ToList().Count;
        check.Novels = temp.Select(o => o.Value).Cast<NovelsSectionData>().ToList(); ;


        return tree;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_CreateData != null)
        {
            DestroyImmediate(_CreateData.EntityData);
        }
    }

    protected override void OnBeginDrawEditors()
    {
        OdinMenuTreeSelection selected = this.MenuTree.Selection;
        var asset = selected.SelectedValue as NovelsSectionData;
        if (asset != null)
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();
                if (SirenixEditorGUI.ToolbarButton("删除"))
                {
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
       

    }


    public class CreateNovelsScene
    {
        public CreateNovelsScene()
        {
            EntityData= ScriptableObject.CreateInstance<NovelsSectionData>();
        }


        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public NovelsSectionData EntityData;

        [Button("添加事件场景")]
        private void _CreateData()
        {
            AssetDatabase.CreateAsset(EntityData, "Assets/Resources/Config/NovelsChapters/" + (_currentId + 101) + ".asset");
            AssetDatabase.SaveAssets();
        }

    }

    public class CheckBatchNovels
    {
        [HideInInspector]
        public List<NovelsSectionData> Novels;
        [LabelText("检测字符串")]
        public string CheckStr;
        [LabelText("替换内容")]
        public string ReplaceStr;

        public CheckBatchNovels()
        {
           
        }

 
        [Button("校正")]
        private void CheckText()
        {
            if (Novels == null) 
            {
                return;
            }

            Novels.ForEach(o =>
            {
                if (o != null)
                {
                    bool isExitReplace = false;

                    o.EventNodes.ForEach(p =>
                    {
                        var nodeData = p as NovelsNodeData;
                        if (nodeData != null)
                        {
                            nodeData.Contents.ForEach(r =>
                            {
                                var content = r as DialogueContent;

                          
                                if (content != null && !string.IsNullOrEmpty(content.Str))
                                {
                                    
                                    if (content.Str.Contains(CheckStr))
                                    {
                                        content.Str = content.Str.Replace(CheckStr, ReplaceStr); 
                                        Debug.Log(content.Str + "\n校正成功" + o.name);
                                        isExitReplace = true;
                                    }
                                }


                                var blackContent = r as BlackScreenContent;

                                if (blackContent != null && !string.IsNullOrEmpty(blackContent.Str))
                                {
                                    if (blackContent.Str.Contains(CheckStr))
                                    {
                                        blackContent.Str = blackContent.Str.Replace(CheckStr, ReplaceStr);
                                        Debug.Log(blackContent.Str + "\n校正成功" + o.name);
                                        isExitReplace = true;
                                    }
                                }

                              
                            });
                        }
                    });

                    if (isExitReplace)
                    {
                        var data = ScriptableObject.CreateInstance<NovelsSectionData>();
                        data.EventNodes = new INovelsNode[o.EventNodes.Length];
                        Array.Copy(o.EventNodes, data.EventNodes, o.EventNodes.Length);
                        var path = AssetDatabase.GetAssetPath(o);
                      
                        AssetDatabase.CreateAsset(data, path);
                    }
                }
            });
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

    }

}
