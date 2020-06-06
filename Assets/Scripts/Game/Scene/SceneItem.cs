using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Novels;

public enum ESceneTriggerType
{
    [LabelText("确认键")]
    Confire=0,
    [LabelText("碰撞触发")]
    Auto =1
}

public class SceneItem : SerializedMonoBehaviour, ISceneObject
{
    [LabelText("触发方式")]
    public ESceneTriggerType TiggerType;

    [LabelText("触发剧情")]
    public NovelsSectionData TriggerPlot;

 
    [LabelText("关闭后激活节点")]
    public GameObject NextPlot;

    [LabelText("触发是否隐去模型")]
    public bool IsHideModel=false;

    public void Get()
    {
        gameObject.SetActive(false);
        if (NextPlot != null)
        {
            NextPlot.gameObject.SetActive(true);
        }
    }


    ESceneObjectType ISceneObject.GetType()
    {
        return ESceneObjectType.Item;
    }
}
