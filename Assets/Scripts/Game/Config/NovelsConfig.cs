using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Novels;
using System.Linq;

[InlineEditor]
public class NovelsConfig : SerializedScriptableObject
{
    public static NovelsConfig _instance;

    public static NovelsConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                var res = Resources.Load<NovelsConfig>("Config/NovelsConfig");

                if (res != null)
                {
                    _instance = GameObject.Instantiate(res);
                }

            }
            return _instance;
        }
    }


    [LabelText("剧本章节")]
    public class NovelsChapterData
    {
        //public List<NovelsSectionData> Sections = new List<NovelsSectionData>();


        [ResourcePath(typeof(NovelsSectionData))]
        public List<string> SectionNodes = new List<string>();
    }

    [LabelText("章节列表")]
    public List<NovelsChapterData> Chapters = new List<NovelsChapterData>();



    public static void Reload()
    {
        var res = Resources.Load<NovelsConfig>("Config/NovelsConfig");

        if (res != null)
        {
            _instance = res;
        }
    }
}
