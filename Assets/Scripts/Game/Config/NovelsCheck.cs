using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Novels;
using System.Linq;

namespace Novels
{

    public interface INovelsCheck
    {
        bool Check();

    }


    [LabelText("收到确认按键")]
    public class ClickConfireOver : INovelsCheck
    {
        public bool Check()
        {
            if (NovelsManager.Instance.IsAcceptConfirm)
            {
                return true;
            }

            return false;
        }
    }

    [LabelText("场景动画结束")]
    public class PlayableOver : INovelsCheck
    {
        public bool Check()
        {
            if (NovelsManager.Instance.IsPlayableOver)
            {
                return true;
            }

            return false;
        }
    }
}

 
 
