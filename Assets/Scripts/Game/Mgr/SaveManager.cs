using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using LitJson;


public class SaveConfig
{
    [LabelText("章节坐标")]
    public int ChapterIndex;
    [LabelText("段落坐标")]
    public int SectionIndex;
    //[LabelText("事件坐标")]
    //public int EventIndex;

    public int BgmVolume;

    public int CharSpeed;

    public int ForceTextWait;

    public bool IsSkip = false;

  
}

[AutoCreateSingleton]
public class SaveManager:Singleton<SaveManager>
{
    public SaveConfig Cfg=new SaveConfig();

    public bool IsHasSave 
    {
        get 
        {
            return PlayerPrefs.HasKey("Save");
        }
    }

    protected override void Initialize()
    {
        if (IsHasSave)
        {
            Load();
        }
        else 
        {
            Cfg = new SaveConfig();
            Cfg.IsSkip = false;
            Cfg.BgmVolume = 100;
            Cfg.CharSpeed = (int)(GlobalConfig.Instance.CharSpeed*1000);
            Cfg.ForceTextWait = (int)(GlobalConfig.Instance.ForceTextWait*1000);
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("Save", JsonMapper.ToJson(Cfg));
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("Save"))
        {
            var str = PlayerPrefs.GetString("Save");
            Cfg = JsonMapper.ToObject<SaveConfig>(str);
        }
    }
}
