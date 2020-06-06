using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
  
    public Animator Anim;

    public PlaySpriteAnimation SpriteAnim;

    public bool IsLoop;
 
    public string AnimName;

 

    public void Start()
    {
        if (Anim != null)
        {
            Anim.Play(AnimName);
        }

        if (SpriteAnim != null) 
        {
            SpriteAnim.Play(AnimName, IsLoop);
        }

    }
}
