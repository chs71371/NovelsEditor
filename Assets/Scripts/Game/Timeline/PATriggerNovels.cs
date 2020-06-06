using Novels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

 
 
public class PATriggerNovels : PlayableAsset
{
 
    public PBTriggerNovels Data = new PBTriggerNovels();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        Data.Director = go.GetComponent<PlayableDirector>();
   
        var playable = ScriptPlayable<PBTriggerNovels>.Create(graph, Data);
        return playable;
    }
}
