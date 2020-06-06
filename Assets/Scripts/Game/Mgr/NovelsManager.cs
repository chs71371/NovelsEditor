using Novels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class NovelsManager : MonoBehaviour
{
    public static NovelsManager Instance;

    public Coroutine MainCoroutine;

    public bool IsAcceptConfirm=false;

    public bool IsPlayableOver = false;

    public PlayableDirector CurrentPlayable;

    public Coroutine TimeLineCoroutine;

    public string CacheContent;

    void Awake()
    {
        Instance = this;
        var sr = gameObject.AddComponent<SignalReceiver>();
    }

    void OnDestory()
    {
        if(MainCoroutine!=null)
        StopCoroutine(MainCoroutine);
    }


    public void Install(int chapterIndex = -1, int sectionIndex = -1)
    {
        MainCoroutine=StartCoroutine(Run(chapterIndex,sectionIndex));
    }

 
    public IEnumerator Run(int chapterIndex=-1,int sectionIndex=-1)
    {
        if (chapterIndex == -1) 
        {
            chapterIndex = SaveManager.Instance.Cfg.ChapterIndex;
        }

        if (sectionIndex == -1)
        {
            sectionIndex = SaveManager.Instance.Cfg.SectionIndex;
        }

        for (int i = chapterIndex; i < NovelsConfig.Instance.Chapters.Count; i++)
        {
            if (i > SaveManager.Instance.Cfg.ChapterIndex)
            {
                SaveManager.Instance.Cfg.SectionIndex = 0;
            }

            for (int j = sectionIndex; j < NovelsConfig.Instance.Chapters[i].SectionNodes.Count; j++)
            {
                var config = Resources.Load<NovelsSectionData>(NovelsConfig.Instance.Chapters[i].SectionNodes[j]);
                SaveManager.Instance.Cfg.ChapterIndex = i;
                SaveManager.Instance.Cfg.SectionIndex = j;
                if (SaveManager.Instance.Cfg.ChapterIndex != 0 && SaveManager.Instance.Cfg.SectionIndex != 0) 
                {
                    SaveManager.Instance.Save();
                }
                for (int k = 0; k < config.EventNodes.Length; k++)
                {
                    yield return config.EventNodes[k].Run();
                }
            }
        }
    }

    public void PauseTimeLine()
    {
        if (CurrentPlayable != null)
        {
            IsPlayableOver = true;
            CurrentPlayable.Pause();
        }
    }

    public void ContineTimeLine()
    {
        if (CurrentPlayable != null)
        {
            IsPlayableOver = false;
            CurrentPlayable.Play();
        }
    }
}
