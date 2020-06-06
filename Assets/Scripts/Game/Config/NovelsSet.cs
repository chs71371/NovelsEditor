using Novels;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public interface INovelsSet 
{
    IEnumerator Run();
}


 

[Serializable]
public class LeaveTitle : INovelsSet
{
    public float Speed = 0.5f;

    public IEnumerator Run()
    {
        while (true)
        {
            UITitlePanel.Instance.Group.alpha -= Time.deltaTime * Speed;
            if (UITitlePanel.Instance.Group.alpha == 0)
            {
                break;
            }
            yield return null;
        }

        UINovelsPanel.Instance.gameObject.SetActive(true);
 
        yield break;
    }

}



[Serializable]
public class BackToTitle : INovelsSet
{
  //  public float Speed=0.5f;

    public IEnumerator Run()
    {
        //while (true)
        //{
        //    UITitlePanel.Instance.Group.alpha -= Time.deltaTime * Speed;
        //    if (UITitlePanel.Instance.Group.alpha == 0)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}
        GameManager.Instance.BackTitle();
        yield break;
    }

}


[Serializable]
public class CurrentControlPlayAnim : INovelsSet
{
    
    public string CharaKey;
    public string AnimName;
    public bool IsLoop;
    public float WaitTime;

    public IEnumerator Run() {

        if (StoryScenePrefab.Instance != null && StoryScenePrefab.Instance.CharaMap.ContainsKey(CharaKey)) 
        {
            StoryScenePrefab.Instance.CharaMap[CharaKey].Play(AnimName, IsLoop);
        }
        yield return new WaitForSeconds(WaitTime);
    }
}


[Serializable]
public class ShowCharaSet : INovelsSet
{
   
    [ValueDropdown("_dropItemList")]
    [LabelText("提示Key")]
    [HideIf("@State== CharaShowData.EEffType.Close")]
    [InlineButton("FreshItem")]
    public string CharaKey;
    [LabelText("位置")]
    public CharaShowData.EPosType PosType;
    [LabelText("X翻转")]
    public bool IsFlipX=false;
    [LabelText("图片效果")]
    public CharaShowData.EEffType State = CharaShowData.EEffType.Show;

    private IEnumerable<string> _dropItemList = GlobalConfig.Instance.NovelsCharas.Select(o => o.Key);

    public void FreshItem()
    {
        _dropItemList = GlobalConfig.Instance.NovelsCharas.Select(o => o.Key);
    }


    public IEnumerator Run()
    {
        yield break;
    }

}

[Serializable]
public class AudioSet: INovelsSet
{
    [LabelText("设置BGM")]
    public bool IsSetBgm = false;
    [LabelText("背景音乐")]
    [ShowIf(@"IsSetBgm")]
    public AudioClipData Bgm = new AudioClipData();
    [LabelText("音效列表")]
    public List<AudioClipData> AudioDatas = new List<AudioClipData>();

    public IEnumerator Run()
    {
        if (IsSetBgm)
        {
            if (Bgm == null || string.IsNullOrEmpty(Bgm.AudioRes))
            {
                AudioManager.Instance.SetBgmFadeOut(1);
            }
            else
            {
                if (AudioManager.Instance.CurrentBgm != Bgm.AudioRes)
                {
                    AudioManager.Instance.SetBgmFadeInOut(Bgm, 3);
                }
            }
        }

        if (AudioDatas != null)
        {
            for (int i = 0; i < AudioDatas.Count; i++)
            {
                AudioManager.Instance.PlaySound(AudioDatas[i], AudioGroupType.Effect);
            }
        }
        yield break;
    }

}


[Serializable]
public class OverConditionSet : INovelsSet
{ 
    [LabelText("结束检测")]
    public List<INovelsCheck> OverCheck = new List<INovelsCheck>();

    public virtual IEnumerator Run()
    {
        bool isOver = true;
        do
        {
            isOver = true;
            for (int i = 0; i < OverCheck.Count; i++)
            {
                if (!OverCheck[i].Check())
                {
                    isOver = false;
                }
            }

            if (isOver)
            {
                break;
            }

            yield return null;

        } while (!isOver);
    }
}


[Serializable]
public class PlayTimeline : INovelsSet
{
    [LabelText("加载的剧编")]
    public PlayableDirector Playable;
    [LabelText("加载后播放")]
    [ShowIf(("@Playable!=null"))]
    public bool IsAwakePlay=false;

    public OverConditionSet OverCheck;

    public PlayTimeline()
    {
        OverCheck = new OverConditionSet();
        OverCheck.OverCheck.Add(new PlayableOver());
    }

    public IEnumerator Run()
    {
        if (Playable != null)
        {
            if (NovelsManager.Instance.CurrentPlayable != null)
            {
                GameHelper.Recycle(NovelsManager.Instance.CurrentPlayable.gameObject);
            }

            var obj = GameObject.Instantiate(Playable.gameObject);
            NovelsManager.Instance.CurrentPlayable = obj.GetComponent<PlayableDirector>();
        }

        if (Playable == null || IsAwakePlay)
        {
            NovelsManager.Instance.ContineTimeLine();
        }
 
        if (OverCheck != null)
        {
            yield return OverCheck.Run();
        }
    }
}



[LabelText("加载场景")]
[Serializable]
public class LoadScene : INovelsSet
{
    [LabelText("加载的场景")]
    public string SceneId;
 
    public IEnumerator Run()
    {
        yield return UINovelsPanel.Instance.BlackEnter(EBlackType.Black, 0);

        if (GameManager.Instance.CurrentScene != null)
        {
            GameHelper.Recycle(GameManager.Instance.CurrentScene.gameObject);
        }

        var obj = GameHelper.Alloc<GameObject>("EventScene/Scene_" + SceneId);
        GameManager.Instance.CurrentScene = obj.GetComponent<StoryScenePrefab>();
        NovelsManager.Instance.CurrentPlayable = GameManager.Instance.CurrentScene.PlayableDirector;
    }
}

[LabelText("离开场景")]
[Serializable]
public class LeaveScene : INovelsSet
{
    [LabelText("离场时间")]
    public float FadeTime = 0.5f;

    public IEnumerator Run()
    {
        yield return UINovelsPanel.Instance.BlackEnter(EBlackType.Black, FadeTime);

        if (GameManager.Instance.CurrentScene != null)
        {
            GameHelper.Recycle(GameManager.Instance.CurrentScene.gameObject);
        }

    }
}

[LabelText("设置当前场景切换舞台")]
[Serializable]
public class SetSceneStage : INovelsSet
{
    public bool IsBlackFade = false;
    [ShowIf("@IsBlackFade")]
    public float FadeTime = 0.5f;

    public ESceneType Stage;

    public IEnumerator Run()
    {
        if (IsBlackFade) 
        {
            yield return UINovelsPanel.Instance.BlackEnter(EBlackType.Black, FadeTime);
        }

        if (SceneSwitchManager.Instance!=null)
        {
            SceneSwitchManager.Instance.SetScene(Stage);
        }

        if (IsBlackFade)
        {
            yield return UINovelsPanel.Instance.BlackLeave(FadeTime);
        }
        yield break;
    }
}


[LabelText("角色进门")]
[Serializable]
public class SetPlayerEnterDoor: INovelsSet
{
    public IEnumerator Run()
    {
        if (CharacterControl.Instance != null) 
        {
            yield return CharacterControl.Instance.EnterDoor();
        }
    }
}


[Serializable]
public class PlayEvent : INovelsSet
{
    [LabelText("加载的剧编")]
    public PlayableAsset Asset;
    [LabelText("加载后播放")]
    [ShowIf(("@Asset!=null"))]
    public bool IsAwakePlay = false;

    public OverConditionSet OverCheck;

    public PlayEvent()
    {
        OverCheck = new OverConditionSet();
        OverCheck.OverCheck.Add(new PlayableOver());
    }

    public IEnumerator Run()
    {
        if (Asset != null)
        {
            GameManager.Instance.CurrentScene.PlayableDirector.playableAsset = Asset;
        }

        if (Asset == null || IsAwakePlay)
        {
            NovelsManager.Instance.ContineTimeLine();
        }

        if (OverCheck != null)
        {
            yield return OverCheck.Run();
        }
    }
}


[Serializable]
public class ClearTimeLine: INovelsSet
{
    public IEnumerator Run()
    {
        if (NovelsManager.Instance.CurrentPlayable != null)
        {
            GameHelper.Recycle(NovelsManager.Instance.CurrentPlayable.gameObject);
        }
        yield break;
    }
}

[Serializable]
public class CloseNovelsPanel : INovelsSet
{
    public IEnumerator Run()
    {
        yield return UINovelsPanel.Instance.DialogLeave();
    }
}


[Serializable]
public class BlackScreen : INovelsSet
{
    [LabelText("是否开启")]
    public bool IsShow;
    [LabelText("过渡时间")]
    public float FadeTime;

    public IEnumerator Run()
    {
        if (IsShow)
        {
            yield return UINovelsPanel.Instance.BlackEnter( EBlackType.Black,FadeTime);
        }
        else
        {
            yield return UINovelsPanel.Instance.BlackLeave( FadeTime);
        }
    }
}


[Serializable]
public class Delay : INovelsSet
{
    [LabelText("延迟时间")]
    public float DelayTime = 0;

    public IEnumerator Run()
    {
        yield return new WaitForSeconds(DelayTime);
    }
}


[Serializable]
public class PopupClose : INovelsSet
{
    public IEnumerator Run()
    {
        UIPopupWindow.Instance.CloseAll();
        yield break;
    }
}


[Serializable]
public class CheckItem : INovelsSet
{
    public int CheckCount;

    public IEnumerator Run()
    {
        while (true)
        {
            if (CheckCount <= CharaterData.CollectItemCount)
            {
                break;
            }

            yield return null;
        }
    }
}


[Serializable]
public class SetCharaterControl : INovelsSet
{
    public bool IsActive = false;

    public IEnumerator Run()
    {
        if (CharacterControl.Instance != null) 
        {
            if (IsActive)
            {
                CharacterControl.Instance.State = CharacterControl.EState.Move;
            }
            else 
            {
                CharacterControl.Instance.State = CharacterControl.EState.Stop;
            }
        }

        yield break;
    }
}


[Serializable]
public class CharaExploreActive : INovelsSet
{
    public bool IsActive;

    public IEnumerator Run()
    {
        CharaterData.IsExplore = IsActive;

        if (CharacterControl.Instance!=null)
        {
            if (IsActive)
            {
                CharacterControl.Instance.State = CharacterControl.EState.Move;
            }
            else
            {
                CharacterControl.Instance.State = CharacterControl.EState.Stop;
            }
        }
        yield break;
    }

}

[Serializable]
public class WaitCharaExploreOver : INovelsSet
{
    public IEnumerator Run()
    {
        CharaterData.IsExplore = true;

        if (CharacterControl.Instance != null)
        {
            CharacterControl.Instance.State = CharacterControl.EState.Move;
        }

        while (CharaterData.IsExplore) 
        {
            yield return null;
        }

        if (CharacterControl.Instance!=null)
        {
            CharacterControl.Instance.State = CharacterControl.EState.Stop;
        }
    }
}

[Serializable]
public class EnterTitlePanel : INovelsSet
{
    public IEnumerator Run()
    {
        yield return UINovelsPanel.Instance.BlackEnter(EBlackType.Black, 1f);

        if (GameManager.Instance.CurrentScene != null)
        {
            GameHelper.Recycle(GameManager.Instance.CurrentScene.gameObject);
        }

        GameManager.Instance.EnterTitle();
    }
}

 