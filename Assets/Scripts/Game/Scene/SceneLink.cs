using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SceneLink : MonoBehaviour, ISceneObject
{
    public SceneLink TargeLink;

    public ESceneType TargeScene;

    public UIImageSwitch ImageSwitch_Link;

    ESceneObjectType ISceneObject.GetType()
    {
        return ESceneObjectType.Link;
    }
}
