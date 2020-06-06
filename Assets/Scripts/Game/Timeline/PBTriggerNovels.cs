using Novels;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

 


[Serializable]
public class PBTriggerNovels : PlayableBehaviour
{
  
    private NovelsNodeData Node= new NovelsNodeData();
    [HideInInspector]
    public PlayableDirector Director;


    private bool _waitPlay=false;
    [LabelText("对话列表")]
    public List<DialogueContent> Contents = new List<DialogueContent>();

    enum EPlay 
    {
        None,
        Play,
        Over,
    }

    private EPlay _playState= EPlay.None;
 
    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
 
        if (_playState != EPlay.None)
        {
            return;
        }
       
        if (!NovelsManager.Instance)
        {
            return;
        }


        if (NovelsManager.Instance.TimeLineCoroutine != null)
        {
            NovelsManager.Instance.StopCoroutine(NovelsManager.Instance.TimeLineCoroutine);
            NovelsManager.Instance.TimeLineCoroutine = null;
        }
      
        if (Contents.Count!=0 && NovelsManager.Instance)
        {
            Node.Contents = Contents.Cast<IContent>().ToList();
            NovelsManager.Instance.TimeLineCoroutine = NovelsManager.Instance.StartCoroutine(Play());
            
        }
    }


    IEnumerator Play()
    {
        _playState = EPlay.Play;
        yield return Node.Run();
        _playState = EPlay.Over;
        if (_waitPlay) 
        {
            _waitPlay = false;
            Director.Resume();
        }
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (_playState == EPlay.Play) 
        {
            _waitPlay = true;
            Director.Pause();
        }
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
      
    }
}
