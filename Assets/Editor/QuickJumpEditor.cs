using Novels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Quick : Editor
{
   

    [MenuItem("快捷键/刷新表数据 &R")]
    static void FreshStaticData() 
    {
        GlobalConfig.Reload();
        NovelsConfig.Reload();
    }

    [MenuItem("快速场景/编辑/进入UI编辑场景", priority = 0)]
    static void EnterUIScence()
    {
        EditorSceneManager.OpenScene("Assets/Editor/EditorScene/UIEditor.unity");
    }

    [MenuItem("快速场景/编辑/进入剧编场景", priority = 0)]
    static void EnterTimeLineScence()
    {
        EditorSceneManager.OpenScene("Assets/Editor/EditorScene/TimeLineEditor.unity");
    }

    [MenuItem("快速场景/开始游戏", priority = 1)]
    static void EnterGame()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Demo.unity");
    }

    [MenuItem("Assets/项目/动画工具/合并AnimClips到控制器")]
    static public void NestAnimClips()
    {
        AnimatorController animController = null;
        AnimationClip[] clips = null;

        Object[] SelectedAsset = Selection.GetFiltered(typeof(AnimatorController), SelectionMode.Assets);

        SelectedAsset.ToList().ForEach(control =>
        {
            animController = control as AnimatorController;
            clips = animController.animationClips;

            if (animController != null && clips.Length > 0)
            {
                animController.layers.ToList().ForEach(o =>
                {

                    //状态机匹配该文件夹下的动画文件
                    for (int i = 0; i < o.stateMachine.states.Length; i++)
                    {
                        var state = o.stateMachine.states[i].state;

                        var curClip = state.motion;

                        BlendTree blend = curClip as BlendTree;

                        if (blend != null)
                        {
                            blend.children.ToList().ForEach(motion =>
                            {
                                AddMotionObjectToControl(animController, state, motion.motion);
                            });
                        }
                        else
                        {
                            AddMotionObjectToControl(animController, state, curClip);
                        }
                    }
                });
            }
        });


    }

    private static void AddMotionObjectToControl(AnimatorController animController, AnimatorState state, Motion curClip)
    {
        var path = AssetDatabase.GetAssetPath(curClip);
        if (path.EndsWith(".anim"))
        {
            var newClip = Object.Instantiate(curClip) as AnimationClip;
            newClip.name = curClip.name;

            state.motion = newClip;
            AssetDatabase.AddObjectToAsset(newClip, animController);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animController));
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(curClip));
        }
    }


    [MenuItem("GameObject/创建场景预制体",false,11)]
    static public void FastCreateScene()
    {
        var obj = new GameObject("Scene_1001");
        var res = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor/Prefabs/TimeLine.prefab");
        var tmpObj = GameObject.Instantiate(res);
        tmpObj.name = "TimeLine";
        var script = obj.AddComponent<StoryScenePrefab>();
        tmpObj.transform.RestTransform(obj.transform);
        script.PlayableDirector = tmpObj.GetComponent<PlayableDirector>();
    }

    [MenuItem("Assets/项目/创建剧情片段")]
    static public void CreatePlot()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        string path = AssetDatabase.GetAssetPath(arr[0]);
        AssetDatabase.CreateAsset( ScriptableObject.CreateInstance<NovelsSectionData>(), path+ "/NewPlot.asset");
        AssetDatabase.SaveAssets();
    }


}
