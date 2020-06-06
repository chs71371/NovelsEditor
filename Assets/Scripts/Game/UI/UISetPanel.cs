using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetPanel : MonoBehaviour
{
    public Text Bgm_Value;
    public Slider Slider_BGM;

    public Text Text_TextAppear;
    public Slider Slider_TextAppear;

    public Text Text_TextDisable;
    public Slider Slider_TextDisable;

    public Button Button_SkipOn;
    public Button Button_SkipOff;

    public Button Button_Close;

    public GameObject Content;

    public Toggle Toggle_IsSkip;

    private bool _isChange=false;

    public void Start()
    {
        Content.gameObject.SetActive(false);
        Slider_BGM.value = SaveManager.Instance.Cfg.BgmVolume;
        AudioManager.Instance.BgmVolume = (float)SaveManager.Instance.Cfg.BgmVolume / 100;
        Slider_TextAppear.value = (float)SaveManager.Instance.Cfg.CharSpeed / 1000;
        Slider_TextDisable.value = (float)SaveManager.Instance.Cfg.ForceTextWait / 1000;

        Bgm_Value.text = Slider_BGM.value.ToString();
        Text_TextAppear.text = Slider_TextAppear.value.ToString("0.00") + "s";
        Text_TextDisable.text = Slider_TextDisable.value.ToString("0.0") + "s";

        //Button_SkipOn.gameObject.SetActive(!SaveManager.Instance.Cfg.IsSkip);
        //Button_SkipOff.gameObject.SetActive(SaveManager.Instance.Cfg.IsSkip);



        Toggle_IsSkip.isOn = SaveManager.Instance.Cfg.IsSkip;
        Toggle_IsSkip.onValueChanged.AddListener((o) =>
        {
            SaveManager.Instance.Cfg.IsSkip = o;
            _isChange = true;
        });

 

        //Button_SkipOn.onClick.AddListener(() =>
        //{
        //    Button_SkipOff.gameObject.SetActive(true);
        //    Button_SkipOn.gameObject.SetActive(false);
        //    SaveManager.Instance.Cfg.IsSkip = true;
        //    SaveManager.Instance.Save();
        //});

        //Button_SkipOff.onClick.AddListener(() =>
        //{
        //    Button_SkipOff.gameObject.SetActive(false);
        //    Button_SkipOn.gameObject.SetActive(true);
        //    SaveManager.Instance.Cfg.IsSkip = false;
        //    SaveManager.Instance.Save();
        //});


        Slider_BGM.onValueChanged.AddListener((o) =>
        {
            Bgm_Value.text = o.ToString();
            SaveManager.Instance.Cfg.BgmVolume = (int)o;
            AudioManager.Instance.BgmVolume = o/100;
            _isChange = true;
        });

        Slider_TextAppear.onValueChanged.AddListener((o) =>
        {
            Text_TextAppear.text = o.ToString("0.00") + "s";
            SaveManager.Instance.Cfg.CharSpeed = (int)(o*1000);
            _isChange = true;
        });

        Slider_TextDisable.onValueChanged.AddListener((o) =>
        {
            Text_TextDisable.text = o.ToString("0.0") + "s";
            SaveManager.Instance.Cfg.ForceTextWait = (int)(o * 1000);
            _isChange = true;
        });
    }

    private void Update()
    {
        if (_isChange) 
        {
            _isChange = false;
            SaveManager.Instance.Save();
        }
    }
}
