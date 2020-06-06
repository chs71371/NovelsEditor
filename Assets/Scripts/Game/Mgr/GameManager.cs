using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : SerializedMonoBehaviour
{
    public static GameManager Instance;

    [LabelText("标题BGM")]
    public AudioClipData TitleBGM;

    public StoryScenePrefab CurrentScene;

    public bool IsTimelineMode;


    public void Awake()
    {
        Instance = this;

        SaveManager.CreateInstance();
        GameHelper.Alloc<GameObject>("Prefabs/Base/UIRoot");
        GameHelper.Alloc<GameObject>("Prefabs/Base/AudioManager");

        GameHelper.Alloc<GameObject>("Prefabs/UI/Novels/UINovelsPanel").transform.RestTransform(UIRoot.Instance.Trans_NovelsPoint);
        GameHelper.Alloc<GameObject>("Prefabs/UI/Novels/UITitlePanel").transform.RestTransform(UIRoot.Instance.Trans_NovelsPoint);

        UINovelsPanel.Instance.gameObject.SetActive(false);
        UITitlePanel.Instance.gameObject.SetActive(false);
        UITitlePanel.Instance.Button_Continue.gameObject.SetActive(SaveManager.Instance.IsHasSave);
 
    }

    public void Start()
    {
        EnterTitle();
    }

    public void EnterTitle()
    {
        AudioManager.Instance.SetBgmFadeIn(TitleBGM,0.5f);
        UITitlePanel.Instance.gameObject.SetActive(true);
    }
 
    public void LeaveTitle()
    {
        UITitlePanel.Instance.gameObject.SetActive(false);
        UINovelsPanel.Instance.gameObject.SetActive(true);
    }


    public void BackTitle()
    {
        PlayerPrefs.DeleteKey("Save");
        SaveManager.Instance.Cfg.ChapterIndex = 0;
        SaveManager.Instance.Cfg.SectionIndex = 0;
        UINovelsPanel.Instance.gameObject.SetActive(false);
        AudioManager.Instance.SetBgmFadeIn(TitleBGM, 0.5f);
        _CreateNovelsMgr();
        UITitlePanel.Instance.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        LeaveTitle();
        _ClearNovelsMgr();
        _CreateNovelsMgr();
    }

    public void ContinueGame()
    {

        SaveManager.Instance.Load();
        LeaveTitle();
        _ClearNovelsMgr();
        _CreateNovelsMgr();
    }

    private void _CreateNovelsMgr()
    {
        var novelsMgr = new GameObject("NovelsManager").AddComponent<NovelsManager>();
        novelsMgr.Install();

    }

    private void _ClearNovelsMgr() 
    {
        if (GameManager.Instance.CurrentScene != null)
            GameHelper.Recycle(GameManager.Instance.CurrentScene.gameObject);
        if(NovelsManager.Instance!=null)
        GameHelper.Recycle(NovelsManager.Instance.gameObject);
    }

    public void EndGame()
    {
        Application.Quit();
    }
 
}
