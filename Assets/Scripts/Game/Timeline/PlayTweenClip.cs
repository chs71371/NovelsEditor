using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;

[System.Serializable]
public class PlayTweenClip : PlayableAsset
{
    public PlayTweenClipBehaviour Temp;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<PlayTweenClipBehaviour>.Create(graph, Temp);
        return playable;
    }
}


[System.Serializable]
public class PlayTweenClipBehaviour : PlayableBehaviour
{
    public ExposedReference<DOTweenAnimation> TweenAnim;

    public bool IsActive;
 
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        var anim = TweenAnim.Resolve(playable.GetGraph().GetResolver());
        if (IsActive)
        {
            anim.DOPlay();
        }
        else 
        {
            anim.DOKill();
        }
 
    }

 
}