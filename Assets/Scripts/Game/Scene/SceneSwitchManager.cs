using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;

public class SceneSwitchManager : SerializedMonoBehaviour
{
    public static SceneSwitchManager Instance;
    public SpriteRenderer Mask;
    [LabelText("场景")]
    public Dictionary<ESceneType, ScenePrefabs> SceneMap = new Dictionary<ESceneType, ScenePrefabs>();
    [LabelText("当前场景")]
    [OnValueChanged("_SwitchScene")]
    public ESceneType CurrentScene = ESceneType.LiveRoom;

    public void Awake()
    {
        Instance = this;
    }


    public void OnDestroy()
    {
        Instance = null;
    }

    public Vector2 MoveRange
    {
        get
        {
            return new Vector2(SceneMap[CurrentScene].Border_Left.position.x, SceneMap[CurrentScene].Border_Right.position.x);
        }
    }
 
    public void SetScene(ESceneType tp)
    {
        CurrentScene = tp;
        _SwitchScene();
    }

    private void _SwitchScene()
    {
        foreach (var info in SceneMap)
        {
            if (info.Key == CurrentScene)
            {
                info.Value.CV_Cam.gameObject.SetActive(true);
            }
            else
            {
                info.Value.CV_Cam.gameObject.SetActive(false);
            }
        }

       
    }
}
