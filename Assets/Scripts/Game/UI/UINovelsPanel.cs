using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;


public class CharaShowData
{
    public enum EEffType
    {
        [LabelText("关闭")]
        Close = 0,
        [LabelText("显示")]
        Show = 1,
        [LabelText("压暗")]
        Dark = 2,
    }

    public enum EPosType
    {
        None=-1,
        [LabelText("左侧")]
        Left=0,
        [LabelText("右侧")]
        Right=1,
        [LabelText("居中")]
        Center=2,
    }

    public bool IsFlipX = false;

    public string CharaKey="";

    public int Expression;

    public string Name;

    public EEffType State;
}


public enum ETextEffect
{
    None,
    [LabelText("逐步")]
    Step,
    [LabelText("淡入")]
    Fade
}

public enum EDialogType
{
    [LabelText("对话文本")]
    DialogBox=0,
    [LabelText("黑底提示")]
    Black=1,
    
}

public enum EBlackType
{
    [LabelText("背景黑幕")]
    Black = 0,
    [LabelText("背景透明")]
    Alpha = 1,
}


public class UINovelsPanel : SerializedMonoBehaviour
{
    public static UINovelsPanel Instance; 

    [TabGroup("界面元素-对话框")]
    [LabelText("对话框动画")]
    public Animator Anim_Dialog;

    [TabGroup("界面元素-对话框")]
    [LabelText("左侧")]
    public Animator Anim_Left;

    [TabGroup("界面元素-对话框")]
    [LabelText("右侧")]
    public Animator Anim_Right;

    [TabGroup("界面元素-对话框")]
    [LabelText("中间")]
    public Animator Anim_Center;

    [TabGroup("界面元素-对话框")]
    public UIImageSwitch ImageSwitch_DialogBg;
    [TabGroup("界面元素-对话框")]
    public Image Image_CharaCenter;
    [TabGroup("界面元素-对话框")]
    public Image Image_CharaLeft;
    [TabGroup("界面元素-对话框")]
    public Image Image_CharaRight;
    [TabGroup("界面元素-对话框")]
    public Text Text_Dialog;
    [TabGroup("界面元素-对话框")]
    public Image Image_NextStepTip;
    [TabGroup("界面元素-对话框")]
    public Transform Trans_CharaNameLeft;
    [TabGroup("界面元素-对话框")]
    public Transform Trans_CharaNameRight;
    [TabGroup("界面元素-对话框")]
    public Text Text_CharaNameLeft;
    [TabGroup("界面元素-对话框")]
    public Text Text_CharaNameRight;


    [TabGroup("界面元素-黑屏")]
    public UIImageSwitch ImageSwitch_BlackBG;
    [TabGroup("界面元素-黑屏")]
    public Text Text_Content;
    [TabGroup("界面元素-黑屏")]
    public CanvasGroup CanvasGroup_Black;

    [TabGroup("事件层")]
    public GameObject Mask;
    [TabGroup("事件层")]
    public GameObject PresenterPanel;
    [TabGroup("事件层")]
    public Button Button_LeftMove;
    [TabGroup("事件层")]
    public Button Button_RightMove;
    [TabGroup("事件层")]
    public Button Button_Confire;



    [TabGroup("数据层")]
    public float FadeTime = 0.3f;
    [TabGroup("数据层")]
    public bool IsShow
    {
        get
        {
            return IsBlackShow || IsDialogShow;
        }
    }
    [TabGroup("数据层")]
    public bool IsBlackShow = false;
    [TabGroup("数据层")]
    public bool IsDialogShow = false;

    [TabGroup("数据层")]
    public EShowType CurrentShowType = EShowType.None;
    [TabGroup("数据层")]
    public Dictionary<CharaShowData.EPosType, CharaShowData> CurrentShowDict = new Dictionary<CharaShowData.EPosType, CharaShowData>()
    {
        { CharaShowData.EPosType.Left, new CharaShowData() },
          { CharaShowData.EPosType.Right, new CharaShowData() },
            { CharaShowData.EPosType.Center, new CharaShowData() },
    };
    [TabGroup("数据层")]
    public CharaShowData.EPosType LastShowPos = CharaShowData.EPosType.None;
    [TabGroup("数据层")]
    public Dictionary<EDialogType, Color> DialogTextColorMap = new Dictionary<EDialogType, Color>();

    public enum EShowType
    {
        None,
        [LabelText("对话框")]
        Dialog=1,
        [LabelText("黑屏")]
        BlackScreen=2,
    }


    public EDialogType CurrentDialogType = EDialogType.DialogBox;

    public enum InputButtonState 
    {
      None,
      Left,
     Right,
    }

    public InputButtonState CurrentButtonState= InputButtonState.None;

    private void Awake()
    {
        Instance = this;

        Clear();

        InputListenerManager.RegisterInputEvent(Button_LeftMove.gameObject, new InputCallback()
        { 
             PressCallBack= () => 
             {
                 CurrentButtonState = InputButtonState.Left;
             },
             CancelCallBack = (o) => 
            {
                if(CurrentButtonState== InputButtonState.Left)
                CurrentButtonState = InputButtonState.None;
            }
        },InputListenerManager.PriorityType.UITigger);

        InputListenerManager.RegisterInputEvent(Button_RightMove.gameObject, new InputCallback()
        {
            PressCallBack = () =>
            {
                CurrentButtonState = InputButtonState.Right;
            },
            CancelCallBack = (o) =>
            {
                if (CurrentButtonState == InputButtonState.Right)
                    CurrentButtonState = InputButtonState.None;
            }
        }, InputListenerManager.PriorityType.UITigger);


        InputListenerManager.RegisterInputEvent(Button_Confire.gameObject, new InputCallback()
        {
            ClickCallBack = ()=>
            {
                if (CharacterControl.Instance != null)
                {
                    CharacterControl.Instance.OnConfire();
                }
            }
        }, InputListenerManager.PriorityType.UITigger);

 
        var input = new InputCallback()
        {
            ClickCallBack = () =>
            {
                if (IsShow)
                {
                    NovelsManager.Instance.IsAcceptConfirm = true;
                }
            }
        };


        InputListenerManager.RegisterInputEvent(typeof(UIConfirmBlock), input, InputListenerManager.PriorityType.UI);
    }

    private void OnDestory()
    {
        Instance = null;
        InputListenerManager.UnInputRegister(typeof(UIConfirmBlock));
        InputListenerManager.UnInputRegister(Button_LeftMove.gameObject);
        InputListenerManager.UnInputRegister(Button_RightMove.gameObject);
        InputListenerManager.UnInputRegister(Button_Confire.gameObject);
    }



    public void Clear()
    {
        Text_Dialog.text = "";
        Text_Content.text = "";
      
 
        CurrentShowDict = new Dictionary<CharaShowData.EPosType, CharaShowData>() {
        { CharaShowData.EPosType.Left, new CharaShowData() },
          { CharaShowData.EPosType.Right, new CharaShowData() },
            { CharaShowData.EPosType.Center, new CharaShowData() }, };

        Anim_Right.SetInteger("State", (int)CharaShowData.EEffType.Close);
        Anim_Left.SetInteger("State", (int)CharaShowData.EEffType.Close);
        Anim_Center.SetInteger("State", (int)CharaShowData.EEffType.Close);

        Trans_CharaNameRight.gameObject.SetActive(false);
        Trans_CharaNameLeft.gameObject.SetActive(false);
    }
 
    public void SetContent(EShowType tp, string str)
    {
        switch (tp)
        {
            case EShowType.Dialog:
                Text_Dialog.text = str;
                break;
            case EShowType.BlackScreen:
                Text_Content.text = str;
                break;
        }
    }

    public IEnumerator TextFadeIn(EShowType tp,float fadeTime)
    {
        switch (tp)
        {
            case EShowType.Dialog:
                {
                    var col = Text_Dialog.color;
                    col.a = 0;
                    Text_Dialog.color = col;
                    Text_Dialog.DOFade(1, fadeTime);
                }
                break;
            case EShowType.BlackScreen:
                {
                    var col = Text_Content.color;
                    col.a = 0;
                    Text_Content.color = col;
                    Text_Content.DOFade(1, fadeTime);
                }
                break;
        }

        yield return new WaitForSeconds(fadeTime);
    }

    public void SetCharaClose(CharaShowData.EPosType posType)
    {
        CurrentShowDict[posType].CharaKey = "";
        CurrentShowDict[posType].State = CharaShowData.EEffType.Close;
        switch (posType)
        {
            case CharaShowData.EPosType.Left:
                Anim_Left.SetInteger("State", (int)CharaShowData.EEffType.Close);
                break;
            case CharaShowData.EPosType.Right:
                Anim_Right.SetInteger("State", (int)CharaShowData.EEffType.Close);
                break;
            case CharaShowData.EPosType.Center:
                Anim_Center.SetInteger("State", (int)CharaShowData.EEffType.Close);
                break;
        }
    }

    public IEnumerator SetCharaState(ShowCharaSet data, int expression,string charaName)
    {
        LastShowPos = data.PosType;
        var charaData = GlobalConfig.Instance.NovelsCharas.ToList().Find(o => o.Key == data.CharaKey);

        if (CurrentShowDict.ContainsKey(data.PosType))
        {
            CurrentShowDict[data.PosType].CharaKey = data.CharaKey;
            CurrentShowDict[data.PosType].State = data.State;
            CurrentShowDict[data.PosType].Expression = expression;
            CurrentShowDict[data.PosType].IsFlipX = data.IsFlipX;
            if (string.IsNullOrEmpty(charaName))
            {
                if (charaData != null)
                {
                    CurrentShowDict[data.PosType].Name = charaData.Name;
                }
            }
            else
            {
                CurrentShowDict[data.PosType].Name = charaName;
            }
     
        }
      

        switch (data.PosType)
        {
            case CharaShowData.EPosType.Left:
                if (CurrentShowDict[CharaShowData.EPosType.Right].State == CharaShowData.EEffType.Show)
                {
                    CurrentShowDict[CharaShowData.EPosType.Right].State = CharaShowData.EEffType.Dark;
                    Anim_Right.SetInteger("State", (int)CharaShowData.EEffType.Dark);
                    
                }
                Anim_Left.SetInteger("State", (int)data.State);

                break;
            case CharaShowData.EPosType.Right:
                if (CurrentShowDict[CharaShowData.EPosType.Left].State == CharaShowData.EEffType.Show)
                {
                    CurrentShowDict[CharaShowData.EPosType.Left].State = CharaShowData.EEffType.Dark;
                    Anim_Left.SetInteger("State", (int)CharaShowData.EEffType.Dark);
                }
                Anim_Right.SetInteger("State", (int)data.State);
                break;
            case CharaShowData.EPosType.Center:
                SetCharaClose( CharaShowData.EPosType.Left);
                SetCharaClose(CharaShowData.EPosType.Right);
                Anim_Center.SetInteger("State", (int)data.State);
                break;
        }

        //设置对话框位置
        if (CurrentDialogType == EDialogType.DialogBox)
        {
            if (data.PosType == CharaShowData.EPosType.Left)
            {
                ImageSwitch_DialogBg.SetImage(0);
            }
            else
            {
                ImageSwitch_DialogBg.SetImage(1);
            }
        }

        FreshSprite();
        yield return new WaitForSeconds(0.3f);
    }

    public void FreshSprite()
    {

        foreach (var info in CurrentShowDict)
        {
 
            if (string.IsNullOrEmpty(info.Value.CharaKey))
            {
                
                continue;
            }

           

            var find = GlobalConfig.Instance.NovelsCharas.ToList().Find(o => o.Key == info.Value.CharaKey);

            if (find == null)
            {
                continue;
            }


            Image setImg = null;
            switch (info.Key)
            {
                case CharaShowData.EPosType.Left:
                    {
                        setImg = Image_CharaLeft;
                        if (info.Value.IsFlipX)
                        {
                            setImg.transform.localScale = new Vector3(-1, 1, 1);
                        }
                        else
                        {
                            setImg.transform.localScale = new Vector3(1, 1, 1);
                        }

                        var isOpenName = !string.IsNullOrEmpty(info.Value.Name);
                        Trans_CharaNameLeft.gameObject.SetActive(isOpenName);
                        if (isOpenName)
                        {
                            Text_CharaNameLeft.text = info.Value.Name;
                        }
                    }
                   

                    break;
                case CharaShowData.EPosType.Right:
                    {
                        setImg = Image_CharaRight;
                        if (info.Value.IsFlipX)
                        {
                            setImg.transform.localScale = new Vector3(1, 1, 1);
                        }
                        else
                        {
                            setImg.transform.localScale = new Vector3(-1, 1, 1);
                        }

                        var isOpenName = !string.IsNullOrEmpty(info.Value.Name);
                        Trans_CharaNameRight.gameObject.SetActive(isOpenName);
                        if (isOpenName)
                        {
                            Text_CharaNameRight.text = info.Value.Name;
                        }
                    }
                    break;
                case CharaShowData.EPosType.Center:
                    setImg = Image_CharaCenter;
                    break;
            }


 
            
 
            if (setImg != null)
            {
                var texData = find.Sprites.Find(o => (int)o.Express == info.Value.Expression);
                if (texData == null) 
                {
                    Debug.LogError(info.Value.CharaKey + "找不到表情："+ info.Value.Expression);
                    texData = find.Sprites[0];
                }
                setImg.sprite = texData.Sprite;
                setImg.rectTransform.sizeDelta = texData.TextureSize;
            }
        }
    }


    private void _SetBlack(bool isShow,float fadeTime)
    {
        CanvasGroup_Black.DOFade(isShow?1:0, fadeTime);
    }
 
    public IEnumerator BlackEnter(EBlackType tp= EBlackType.Black,float fadeTime =-1)
    {
        
        if (fadeTime == -1)
        {
            fadeTime = FadeTime;
        }
        ImageSwitch_BlackBG.SetImage((int)tp);
        _SetBlack(true, fadeTime);
        IsBlackShow = true;
        yield return new WaitForSeconds(fadeTime);
    }

    public IEnumerator BlackLeave(float fadeTime = -1)
    {
        if (fadeTime == -1)
        {
            fadeTime = FadeTime;
        }

        if (!string.IsNullOrEmpty(Text_Content.text))
        {
            Text_Content.DOFade(0, fadeTime);
            yield return new WaitForSeconds(fadeTime);
        }

        _SetBlack(false, fadeTime);
        IsBlackShow = false;
        yield return new WaitForSeconds(fadeTime);
        Text_Content.text = "";
        Text_Content.color = Color.white;
    }

   

    public IEnumerator DialogEnter(EDialogType tp= EDialogType.DialogBox)
    {
        Image_NextStepTip.gameObject.SetActive(false);
        CurrentDialogType = tp;
        Text_Dialog.color = DialogTextColorMap[tp];
        if (tp == EDialogType.Black)
        {
            ImageSwitch_DialogBg.SetImage(2);
        }
        IsDialogShow = true;
        Anim_Dialog.SetBool("IsShow", true);
        yield break;
    }

    public IEnumerator DialogLeave()
    {
        IsDialogShow = false;
        Anim_Dialog.SetBool("IsShow", false);
        yield return new WaitForSeconds(FadeTime);
        NovelsManager.Instance.CacheContent = "";
        Clear();

       
    }


    public void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsShow)
            {
                NovelsManager.Instance.IsAcceptConfirm = true;
            }
        }


#if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
        if (!IsDialogShow&& !IsBlackShow && CharacterControl.Instance != null && CharacterControl.Instance.State == CharacterControl.EState.Move)
        {
            PresenterPanel.gameObject.SetActive(true);
            switch (CurrentButtonState)
            {
                case InputButtonState.None:
                    CharacterControl.Instance.MoveInput = 0;
                    break;
                case InputButtonState.Left:
                    CharacterControl.Instance.MoveInput = -1;
                    break;
                case InputButtonState.Right:
                    CharacterControl.Instance.MoveInput = 1;
                    break;
            }
        }
        else
#endif
        {
            PresenterPanel.gameObject.SetActive(false);
            CurrentButtonState = InputButtonState.None;
        }
    }
}