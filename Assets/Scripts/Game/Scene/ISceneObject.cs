using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ESceneObjectType
{
    None,
    Link=1,
    Item=10,
}


public interface ISceneObject 
{
    ESceneObjectType GetType();

}
