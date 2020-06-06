using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
 
[System.Serializable]
public class PBPlayAnimation : PlayableBehaviour
{
    public enum EMoveSet 
    {
        None,
        Left=-1,
        Right=1,
    }

    [TabGroup("动画")]
    public ExposedReference<PlaySpriteAnimation> SpriteAnim;
    [TabGroup("动画")]
    public bool IsLoop;


 
    [TabGroup("动画")]
    [LabelText("序列帧动画")]
    public string SpriteAnimName;
    [TabGroup("动画")]
    [LabelText("序列帧动画速度")]
    public float AnimSpeed = 1;
    [TabGroup("动画")]
    [LabelText("暂停动画")]
    public bool IsPause=false;


    [TabGroup("Animator动画")]
    [LabelText("播放动画名")]
    public string AnimName;
    [TabGroup("Animator动画")]
    [LabelText("参数名字")]
    public string AnimParamName;
    [TabGroup("Animator动画")]
    [LabelText("参数值")]
    public int AnimParamValue;
    [TabGroup("Animator动画")]
    public ExposedReference<Animator> Anim;



    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {

    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {

    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
       
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {

        var spriteAnim = SpriteAnim.Resolve(playable.GetGraph().GetResolver());

       
        if (spriteAnim != null)
        {
            if (IsPause)
            {
                spriteAnim.IsStop(false);
                return;
            }

            spriteAnim.PlayDefaultAnim();
        }
    }

    private float _cacheTime;
    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
         var anim = Anim.Resolve(playable.GetGraph().GetResolver());
        if (anim != null)
        {
            if (!string.IsNullOrEmpty(AnimName)) 
            {
                anim.Play(AnimName);
            }

            if (!string.IsNullOrEmpty(AnimParamName))
            {
                anim.SetInteger(AnimParamName, AnimParamValue);
            }
        }


        var spriteAnim = SpriteAnim.Resolve(playable.GetGraph().GetResolver());

       
        if (spriteAnim != null)
        {
            if (IsPause)
            {
                spriteAnim.IsStop(true);
                return;
            }

            spriteAnim.Play(SpriteAnimName, IsLoop, AnimSpeed == 0 ? 1 : AnimSpeed);
        }
    }
}
