using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Novels;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Playables;
////using UnityEditor;

namespace Novels
{
 
    public interface INovelsNode
    {
        IEnumerator Run();
    }


    public interface IContent
    {
        EContentType GetType();
        string GetStr();
    }

    public enum EContentType
    {
        None=-1,
        Black,
        Dialog,
        Event,
        Audio,
        Popup,
    }

    [LabelText("黑屏")]
    [Serializable]
    public class BlackScreenContent: IContent
    {
        [TabGroup("展示内容")]
        [TextArea]
        [LabelText("内容文本")]
        public string Str;
        [TabGroup("功能设置")]
        [LabelText("点击下一步")]
        public bool IsNeedClick = true;
        [TabGroup("功能设置")]
        [LabelText("结束后关闭")]
        public bool IsClose = false;
        [TabGroup("功能设置")]
        [LabelText("进入时间")]
        public float EnterFade = 0.5f;
        [TabGroup("功能设置")]
        [LabelText("停留时间")]
        public float StayTime = 1;
        [TabGroup("功能设置")]
        [LabelText("结束时间")]
        public float LeaveFade = 0.5f;

        [TabGroup("文本设置")]
        [LabelText("字体出现效果")]
        public ETextEffect TextEffect = ETextEffect.Fade;
        [TabGroup("文本设置")]
        [LabelText("效果时间")]
        public float EffectTime = 0.5f;
        [TabGroup("文本设置")]
        [LabelText("背景类型")]
        public EBlackType BGType = EBlackType.Black;

        public string GetStr()
        {
            return Str;
        }

        EContentType IContent.GetType()
        {
            return EContentType.Black;
        }


#if UNITY_EDITOR
        [Button("添加新节点")]
        public void AddNewContent()
        {
            var item = UnityEditor.Selection.GetFiltered<SceneItem>(UnityEditor.SelectionMode.Unfiltered);

            if (item.Length > 0)
            {
                var sceneItem = item[0] as SceneItem;
                for (int i = 0; i < sceneItem.TriggerPlot.EventNodes.Length; i++)
                {
                    var nodeData = sceneItem.TriggerPlot.EventNodes[i] as NovelsNodeData;
                    if (nodeData != null && nodeData.Contents.Contains(this))
                    {
                        nodeData.AddNewContent(this, MemberwiseClone());
                        return;
                    }
                }
            }

            var section = UnityEditor.Selection.GetFiltered<NovelsSectionData>(UnityEditor.SelectionMode.Unfiltered);

            if (section.Length > 0)
            {
                var data = section[0] as NovelsSectionData;

                for (int i = 0; i < data.EventNodes.Length; i++)
                {
                    var nodeData = data.EventNodes[i] as NovelsNodeData;
                    if (nodeData != null && nodeData.Contents.Contains(this))
                    {
                        nodeData.AddNewContent(this, MemberwiseClone());
                        return;
                    }
                }
            }

            for (int i = 0; i < NovelsConfig.Instance.Chapters.Count; i++)
            {
                for (int j = 0; j < NovelsConfig.Instance.Chapters[i].SectionNodes.Count; j++)
                {
                    var config = Resources.Load<NovelsSectionData>(NovelsConfig.Instance.Chapters[i].SectionNodes[j]);
                    for (int k = 0; k < config.EventNodes.Length; k++)
                    {
                        var nodeData = config.EventNodes[k] as NovelsNodeData;

                        if (nodeData != null && nodeData.Contents.Contains(this))
                        {
                            nodeData.AddNewContent(this, MemberwiseClone());
                            return;
                        }
                    }
                }
            }

        }
#endif
    }

    
    [LabelText("对话")]
    [Serializable]
    public class DialogueContent : IContent
    {
        [TabGroup("展示内容")]
        [ValueDropdown("_dropItemList")]
        [LabelText("显示")]
        [InlineButton("FreshItem")]
        public string ShowStr;
        [TabGroup("展示内容")]
        [LabelText("表情设置")]
        public EExpressionType Expression= EExpressionType.Normal;
        [TabGroup("展示内容")]
        [LabelText("名字显示")]
        public string CharaName;
        [TabGroup("展示内容")]
        [LabelText("关闭显示")]
        public EClose Close;

        [TabGroup("文本设置")]
        [LabelText("字体出现效果")]
        public ETextEffect TextEffect = ETextEffect.Step;
        [TabGroup("文本设置")]
        [LabelText("效果时间")]
        public float EffectTime = 0;

        [TabGroup("文本设置")]
        [LabelText("离开等待时间")]
        public float LeaveWait = 0;

        [Flags]
        public enum EClose
        {
            None=0,
            Left=1<<1,
            Right=1<<2,
            Center=1<<3,
            All=Left|Right|Center,
        }

        public enum EExpressionType
        {
            [LabelText("通常")]
            Normal=0,
            [LabelText("哭泣")]
            Cry=1,
            [LabelText("生气")]
            Angry =2,
            [LabelText("失神")]
            Absence=3,
            [LabelText("难过")]
            Sad=4,
            [LabelText("惊讶")]
            surprised=5,
            [LabelText("撒谎")]
            Shy=6,
            [LabelText("快乐")]
            Happy=7,
            [LabelText("叹气")]
            Sigh = 8,
            [LabelText("幸福哭泣")]
            WeepingWithJoy = 9,
        }

        [TabGroup("展示内容")]
        [TextArea]
        [LabelText("内容文本")]
        public string Str;
        [TabGroup("功能设置")]
        [LabelText("对话框类型")]
        public EDialogType DialogBoxType = EDialogType.DialogBox;
        [TabGroup("功能设置")]
        [LabelText("换行显示")]
        public bool IsNextLine = true;
        [TabGroup("功能设置")]
        [LabelText("点击下一步")]
        public bool IsNeedClick=true;
        [TabGroup("功能设置")]
        [LabelText("进入清理文本")]
        public bool IsClearCache = true;
 
        [TabGroup("功能设置")]
        [LabelText("结束后关闭对话框")]
        public bool IsCloseDialog = false;
        [TabGroup("功能设置")]
        [LabelText("结束后关闭黑屏框")]
        public bool IsCloseBlack = false;


        IEnumerable<string> _dropItemList;

 
        private void FreshItem()
        {
           
             _dropItemList = GlobalConfig.Instance.CharaMap.Select(o => o.Key);
        }

        public string GetStr()
        {
            return Str;
        }

         EContentType IContent.GetType()
        {
            return EContentType.Dialog;
        }

#if UNITY_EDITOR
        [Button("添加新节点")]
        public void AddNewContent()
        {
            var item=  UnityEditor.Selection.GetFiltered<SceneItem>(UnityEditor.SelectionMode.Unfiltered);

            if (item.Length>0)
            {
                var sceneItem = item[0] as SceneItem;
                for (int i = 0; i < sceneItem.TriggerPlot.EventNodes.Length; i++)
                {
                    var nodeData = sceneItem.TriggerPlot.EventNodes[i] as NovelsNodeData;
                    if (nodeData != null && nodeData.Contents.Contains(this))
                    {
                        nodeData.AddNewContent(this, MemberwiseClone());
                        return;
                    }
                }
            }

            var section = UnityEditor.Selection.GetFiltered<NovelsSectionData>(UnityEditor.SelectionMode.Unfiltered);
 
            if (section.Length > 0)
            {
                var data = section[0] as NovelsSectionData;

                for (int i = 0; i < data.EventNodes.Length; i++)
                {
                    var nodeData = data.EventNodes[i] as NovelsNodeData;
                    if (nodeData != null && nodeData.Contents.Contains(this))
                    {
                        nodeData.AddNewContent(this, MemberwiseClone());
                        return;
                    }
                }
            }

            for (int i = 0; i < NovelsConfig.Instance.Chapters.Count; i++)
            {
                for (int j = 0; j < NovelsConfig.Instance.Chapters[i].SectionNodes.Count; j++)
                {
                    var config = Resources.Load<NovelsSectionData>(NovelsConfig.Instance.Chapters[i].SectionNodes[j]);
                    for (int k = 0; k < config.EventNodes.Length; k++)
                    {
                        var nodeData = config.EventNodes[k] as NovelsNodeData;

                        if (nodeData != null && nodeData.Contents.Contains(this))
                        {
                            nodeData.AddNewContent(this, MemberwiseClone());
                            return;
                        }
                    }
                }
            }

        }
#endif



    }

    [LabelText("音效")]
    [Serializable]
    public class SEAudioContent : IContent
    {

       
        [LabelText("等待音效结束")]
        public bool IsWait=false;
        [LabelText("音效配置")]
        public AudioClipData Data;

        public string GetStr()
        {
            return "";
        }

        EContentType IContent.GetType()
        {
            return EContentType.Audio;
        }
    }

    [LabelText("事件")]
    [Serializable]
    public class EventContent : IContent
    {
        [Title("事件文本")]
        [GUIColor("@Color.red")]
        [ValueDropdown("_dropItemList")]
        [InlineButton("FreshItem")]
        public string Str;

        IEnumerable<string> _dropItemList = GlobalConfig.Instance.EventMap.Select(o => o.Key);

        public void FreshItem()
        {
            _dropItemList = GlobalConfig.Instance.EventMap.Select(o => o.Key);
        }

        public string GetStr()
        {
            return Str;
        }

        EContentType IContent.GetType()
        {
            return EContentType.Event;
        }

        [Button("添加新节点")]
        public void AddNewContent()
        {
            for (int i = 0; i < NovelsConfig.Instance.Chapters.Count; i++)
            {
                for (int j = 0; j < NovelsConfig.Instance.Chapters[i].SectionNodes.Count; j++)
                {
                    var config = Resources.Load<NovelsSectionData>(NovelsConfig.Instance.Chapters[i].SectionNodes[j]);
                    for (int k = 0; k < config.EventNodes.Length; k++)
                    {
                        var nodeData = config.EventNodes[k] as NovelsNodeData;
 
                        if (nodeData != null && nodeData.Contents.Contains(this))
                        {
                            nodeData.Contents.Insert(nodeData.Contents.IndexOf(this), MemberwiseClone() as EventContent);
                            return;
                        }
                    }
                }
            }
        }
    }

    [LabelText("弹窗")]
    [Serializable]
    public class PopupContent : IContent
    {
        [LabelText("弹窗数据")]
        public PopupWindowData Data;

        public string GetStr()
        {
            return "";
        }

        EContentType IContent.GetType()
        {
            return EContentType.Popup;
        }

#if UNITY_EDITOR
        [Button("添加新节点")]
        public void AddNewContent()
        {
            var item = UnityEditor.Selection.GetFiltered<SceneItem>(UnityEditor.SelectionMode.Unfiltered);

            if (item.Length > 0)
            {
                var sceneItem = item[0] as SceneItem;
                for (int i = 0; i < sceneItem.TriggerPlot.EventNodes.Length; i++)
                {
                    var nodeData = sceneItem.TriggerPlot.EventNodes[i] as NovelsNodeData;
                    if (nodeData != null && nodeData.Contents.Contains(this))
                    {
                        nodeData.AddNewContent(this, MemberwiseClone());
                        return;
                    }
                }
            }

            var section = UnityEditor.Selection.GetFiltered<NovelsSectionData>(UnityEditor.SelectionMode.Unfiltered);

            if (section.Length > 0)
            {
                var data = section[0] as NovelsSectionData;

                for (int i = 0; i < data.EventNodes.Length; i++)
                {
                    var nodeData = data.EventNodes[i] as NovelsNodeData;
                    if (nodeData != null && nodeData.Contents.Contains(this))
                    {
                        nodeData.AddNewContent(this, MemberwiseClone());
                        return;
                    }
                }
            }

            for (int i = 0; i < NovelsConfig.Instance.Chapters.Count; i++)
            {
                for (int j = 0; j < NovelsConfig.Instance.Chapters[i].SectionNodes.Count; j++)
                {
                    var config = Resources.Load<NovelsSectionData>(NovelsConfig.Instance.Chapters[i].SectionNodes[j]);
                    for (int k = 0; k < config.EventNodes.Length; k++)
                    {
                        var nodeData = config.EventNodes[k] as NovelsNodeData;

                        if (nodeData != null && nodeData.Contents.Contains(this))
                        {
                            nodeData.AddNewContent(this, MemberwiseClone());
                            return;
                        }
                    }
                }
            }

        }
#endif
    }



    [Title("段落")]
    [Serializable]
    public class NovelsNodeData : INovelsNode
    {
        [TabGroup("文本段落")]
        public List<IContent> Contents = new List<IContent>();

        public void AddNewContent(IContent content, System.Object newContent)
        {
            Contents.Insert(Contents.IndexOf(content), newContent as IContent);
        }

        public IEnumerator Run()
        {
            while (NovelsManager.Instance == null || UINovelsPanel.Instance == null) 
            {
                yield return null;
            }
 
            var cacheContent = NovelsManager.Instance.CacheContent;

            var lastType = EContentType.None;
            for (int i = 0; i < Contents.Count; i++)
            {
                var currentType = Contents[i].GetType();

                //全屏文本或文本内容类型切换时清理上一次的文本
                if (currentType== EContentType.Black||(lastType != EContentType.None && currentType != lastType)) 
                {
                    NovelsManager.Instance.CacheContent = cacheContent = "";
                    UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);
                }
                lastType = currentType;

                var str = Contents[i].GetStr()+"";

                switch (currentType)
                {
                    case EContentType.Dialog:
                        {
                            var dialogContent = Contents[i] as DialogueContent;
                            if (dialogContent == null)
                            {
                                continue;
                            }

                            yield return UINovelsPanel.Instance.DialogEnter(dialogContent.DialogBoxType);
 
                            //关闭立绘
                            if ((dialogContent.Close & DialogueContent.EClose.Left) == DialogueContent.EClose.Left)
                            {
                                UINovelsPanel.Instance.SetCharaClose( CharaShowData.EPosType.Left);
                            }

                            if ((dialogContent.Close & DialogueContent.EClose.Right) == DialogueContent.EClose.Right)
                            {
                                UINovelsPanel.Instance.SetCharaClose(CharaShowData.EPosType.Right);
                            }

                            if ((dialogContent.Close & DialogueContent.EClose.Center) == DialogueContent.EClose.Center)
                            {
                                UINovelsPanel.Instance.SetCharaClose(CharaShowData.EPosType.Center);
                            }

                            //寻找显示数据
                            ShowCharaSet showData = null;
                            if (!string.IsNullOrEmpty(dialogContent.ShowStr)
                                && GlobalConfig.Instance.CharaMap.ContainsKey(dialogContent.ShowStr))
                            {
                                showData = GlobalConfig.Instance.CharaMap[dialogContent.ShowStr];
                              
                            }

                            //清理文本
                            if (dialogContent.IsClearCache ||
                               (showData != null && showData.PosType != UINovelsPanel.Instance.LastShowPos)
                               || dialogContent.DialogBoxType!= UINovelsPanel.Instance.CurrentDialogType)
                            {
                                NovelsManager.Instance.CacheContent = cacheContent = "";
                                UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);
                            }

                            //显示立绘
                            if (showData != null)
                            {
                                   yield return UINovelsPanel.Instance.SetCharaState(showData, (int)dialogContent.Expression,dialogContent.CharaName);
                            }

                            if (dialogContent.IsNextLine)
                            {
                                if (!string.IsNullOrEmpty(cacheContent))
                                {
                                    cacheContent += "\n";
                                }
                            }

                            //文本特效
                            switch (dialogContent.TextEffect)
                            {
                                case ETextEffect.None:
                                    {
                                        cacheContent += str;
                                        UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);
                                    }
                                    break;
                                case ETextEffect.Step:
                                    {
                                        if (!string.IsNullOrEmpty(str))
                                        {
                                            var charArr = str.ToCharArray();
                                            var charIndex = 0;
                                            while (charIndex < charArr.Length)
                                            {
                                                UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent + str.Substring(0, charIndex));
                                                charIndex += 1;
                                                AudioManager.Instance.PlaySound(GlobalConfig.Instance.SFX_TextAppear, AudioGroupType.Effect);
                                                if (dialogContent.EffectTime != 0)
                                                {
                                                    yield return new WaitForSeconds(dialogContent.EffectTime);
                                                }
                                                else 
                                                {
                                                    yield return new WaitForSeconds((float)SaveManager.Instance.Cfg.CharSpeed/1000);
                                                }
                                            }
                                            cacheContent += str;
                                            UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);
                                        }
                                    }
                                    break;
                                case ETextEffect.Fade:
                                    cacheContent += str;
                                    UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);
                                    yield return UINovelsPanel.Instance.TextFadeIn( UINovelsPanel.EShowType.Dialog, dialogContent.EffectTime);
                                    break;
                            }

                        
                            //点击下一步
                            if (dialogContent != null && dialogContent.IsNeedClick && !SaveManager.Instance.Cfg.IsSkip)
                            {
                                NovelsManager.Instance.IsAcceptConfirm = false;
                                UINovelsPanel.Instance.Image_NextStepTip.gameObject.SetActive(true);
 
                                while (true)
                                {
                                    if (NovelsManager.Instance.IsAcceptConfirm)
                                    {
                                        NovelsManager.Instance.IsAcceptConfirm = false;
                                        UINovelsPanel.Instance.Image_NextStepTip.gameObject.SetActive(false);
                                        break;
                                    }
                                    yield return null;
                                }
                            }

                            var tmpWait = dialogContent.LeaveWait;
                            if (tmpWait == 0)
                            {
                                tmpWait = (float)SaveManager.Instance.Cfg.ForceTextWait/1000;
                            } 

                            yield return new WaitForSeconds(tmpWait);

                            //关闭对话框
                            if (dialogContent.IsCloseDialog)
                            {
                                yield return UINovelsPanel.Instance.DialogLeave();
                            }

                            //关闭黑屏框
                            if (dialogContent.IsCloseBlack)
                            {
                                yield return UINovelsPanel.Instance.BlackLeave(0.5f);
                            }
                        }
                        break;
                    case EContentType.Black:
                        {
                            var blackContent = Contents[i] as BlackScreenContent;
                            if (blackContent == null)
                            {
                                continue;
                            }
 
                            yield return UINovelsPanel.Instance.BlackEnter(blackContent.BGType, blackContent.EnterFade);

                            switch (blackContent.TextEffect)
                            {
                                case ETextEffect.None:
                                    {
                                        cacheContent += str;
                                        UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.BlackScreen, cacheContent);
                                    }
                                    break;
                                case ETextEffect.Step:
                                    {
                                        if (!string.IsNullOrEmpty(str))
                                        {
                                            var charArr = str.ToCharArray();
                                            var charIndex = 0;
                                            while (charIndex < charArr.Length)
                                            {
                                                UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.BlackScreen, str.Substring(0, charIndex));
                                                charIndex += 1;
                                                AudioManager.Instance.PlaySound(GlobalConfig.Instance.SFX_TextAppear, AudioGroupType.Effect);
                                                yield return new WaitForSeconds((float)SaveManager.Instance.Cfg.CharSpeed/1000);
                                            }
                                        }
                                    }
                                    break;
                                case ETextEffect.Fade:
                                    cacheContent += str;
                                    UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.BlackScreen, cacheContent);
                                    yield return UINovelsPanel.Instance.TextFadeIn(UINovelsPanel.EShowType.BlackScreen, blackContent.EffectTime);
                                    break;
                            }
 
                            UINovelsPanel.Instance.SetContent( UINovelsPanel.EShowType.BlackScreen, str);

                            if (blackContent.IsNeedClick&&!SaveManager.Instance.Cfg.IsSkip)
                            {
                                while (true)
                                {
                                    if (NovelsManager.Instance.IsAcceptConfirm)
                                    {
                                        NovelsManager.Instance.IsAcceptConfirm = false;
                                        break;
                                    }
                                    yield return null;
                                }
                            }

                            if (blackContent.StayTime > 0)
                            {
                                yield return new WaitForSeconds(blackContent.StayTime);
                            }

                            if (blackContent.IsClose)
                            {
                                yield return UINovelsPanel.Instance.BlackLeave(blackContent.LeaveFade);
                            }

                            yield return new WaitForSeconds((float)SaveManager.Instance.Cfg.ForceTextWait/1000);
                            NovelsManager.Instance.IsAcceptConfirm = false;
                        }
                        break;
                    case EContentType.Event:
                        {
                            if (GlobalConfig.Instance.EventMap.ContainsKey(str))
                            {
                                yield return GlobalConfig.Instance.EventMap[str].Run();
                            }
                        }
                        break;
                    case EContentType.Audio:
                        {
                            var audioContent = Contents[i] as SEAudioContent;
                            var lenth=  AudioManager.Instance.PlaySound(audioContent.Data, AudioGroupType.Effect);
                            if (audioContent.IsWait) 
                            {
                                yield return new WaitForSeconds(lenth);
                            }
                        }
                        break;
                    case EContentType.Popup:
                        {
                            var popupContent = Contents[i] as PopupContent;
                            yield return UIPopupWindow.Instance.Open(popupContent.Data);
                        }
                        break;
                }
            }
        }


#if UNITY_EDITOR
        [Button("添加段落")]
        public void AddNewContent()
        {
            var item = UnityEditor.Selection.GetFiltered<SceneItem>(UnityEditor.SelectionMode.Unfiltered);

            if (item.Length > 0)
            {
                var sceneItem = item[0] as SceneItem;

                var list = sceneItem.TriggerPlot.EventNodes.ToList();
                list.Insert(list.IndexOf(this), MemberwiseClone() as NovelsNodeData);
                sceneItem.TriggerPlot.EventNodes = list.ToArray();
                return;
            }

            var section = UnityEditor.Selection.GetFiltered<NovelsSectionData>(UnityEditor.SelectionMode.Unfiltered);

            if (section.Length > 0)
            {
                var data = section[0] as NovelsSectionData;
                var list = data.EventNodes.ToList();
                list.Insert(list.IndexOf(this), MemberwiseClone() as NovelsNodeData);
                data.EventNodes = list.ToArray();
                return;
            }

            for (int i = 0; i < NovelsConfig.Instance.Chapters.Count; i++)
            {
                for (int j = 0; j < NovelsConfig.Instance.Chapters[i].SectionNodes.Count; j++)
                {
                    var config = Resources.Load<NovelsSectionData>(NovelsConfig.Instance.Chapters[i].SectionNodes[j]);
                    for (int k = 0; k < config.EventNodes.Length; k++)
                    {
                        var list = config.EventNodes.ToList();

                        if (list.Contains(this))
                        {
                            list.Insert(list.IndexOf(this), MemberwiseClone() as NovelsNodeData);
                            config.EventNodes= list.ToArray();
                            return;
                        }
                    }
                }
            }

        }
#endif
    }


    [Title("属性修改")]
    [InlineEditor]
    [Serializable]
    public class SetNodeData : INovelsNode
    {
        public INovelsSet SetData;

        public IEnumerator Run()
        {
            yield return SetData.Run();
        }

#if UNITY_EDITOR
        [Button("添加段落")]
        public void AddNewContent()
        {
            var item = UnityEditor.Selection.GetFiltered<SceneItem>(UnityEditor.SelectionMode.Unfiltered);

            if (item.Length > 0)
            {
                var sceneItem = item[0] as SceneItem;

                var list = sceneItem.TriggerPlot.EventNodes.ToList();
                list.Insert(list.IndexOf(this), MemberwiseClone() as SetNodeData);
                sceneItem.TriggerPlot.EventNodes = list.ToArray();
                return;
            }

            var section = UnityEditor.Selection.GetFiltered<NovelsSectionData>(UnityEditor.SelectionMode.Unfiltered);

            if (section.Length > 0)
            {
                var data = section[0] as NovelsSectionData;
                var list = data.EventNodes.ToList();
                list.Insert(list.IndexOf(this), MemberwiseClone() as SetNodeData);
                data.EventNodes = list.ToArray();
                return;
            }

            for (int i = 0; i < NovelsConfig.Instance.Chapters.Count; i++)
            {
                for (int j = 0; j < NovelsConfig.Instance.Chapters[i].SectionNodes.Count; j++)
                {
                    var config = Resources.Load<NovelsSectionData>(NovelsConfig.Instance.Chapters[i].SectionNodes[j]);
                    for (int k = 0; k < config.EventNodes.Length; k++)
                    {
                        var list = config.EventNodes.ToList();

                        if (list.Contains(this))
                        {
                            list.Insert(list.IndexOf(this), MemberwiseClone() as SetNodeData);
                            config.EventNodes = list.ToArray();
                            return;
                        }
                    }
                }
            }

        }
#endif
    }

    [LabelText("章节段落")]
    [InlineEditor]
    [CreateAssetMenu(fileName = "NovelsSceneData", menuName = "Game/NovelsSceneData")]
    public class NovelsSectionData : SerializedScriptableObject
    {
 
        [LabelText("事件列表")]
        public INovelsNode[] EventNodes=new INovelsNode[0];
 
 
        public IEnumerator Run()
        {
            for (int i = 0; i < EventNodes.Length; i++)
            {
                yield return EventNodes[i].Run();
            }
        }
 
    }
}