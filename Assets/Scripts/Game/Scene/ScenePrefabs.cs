using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;


public enum ESceneType
{
    None=0,
    [LabelText("客厅")]
    LiveRoom=1,
    [LabelText("厨房")]
    KitchenRoom=2,
    [LabelText("卧室")]
    BedRoom=3,

    [LabelText("三途河")]
    River=6,

    [LabelText("奈何")]
    Brige=11,
    [LabelText("地狱")]
    Hell =12,
    [LabelText("小游戏")]
    MiniGame = 13,

    [LabelText("梦境")]
    Dream = 21,
}

public class ScenePrefabs : MonoBehaviour
{
    public ESceneType Type;
    public CinemachineVirtualCamera CV_Cam;
    public GameObject FollowObj;

    public Transform Border_Left;
    public Transform Border_Right;
}
