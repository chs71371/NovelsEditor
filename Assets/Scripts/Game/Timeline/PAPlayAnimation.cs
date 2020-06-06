using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[System.Serializable]
public class PAPlayAnimation : PlayableAsset
{
 
    public PBPlayAnimation PB;


    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<PBPlayAnimation>.Create(graph, PB);
        var behaviour = playable.GetBehaviour();

        return playable;
    }
}
