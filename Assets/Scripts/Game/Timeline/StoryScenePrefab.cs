using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StoryScenePrefab : SerializedMonoBehaviour
{
    public static StoryScenePrefab Instance;

    public Dictionary<string, PlaySpriteAnimation> CharaMap = new Dictionary<string, PlaySpriteAnimation>();

    public PlayableDirector PlayableDirector;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}
