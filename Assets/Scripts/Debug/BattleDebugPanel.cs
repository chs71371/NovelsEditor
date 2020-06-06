using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class BattleDebugPanel : DebugPanel
{

    public static string GetStack()
    {
        return new System.Diagnostics.StackTrace().ToString();
    }
    public static void CreateDebugPanel()
    {
        var res = Resources.Load<GameObject>("Debug/TestDataView");
        if (res != null)
        {
            var obj = GameObject.Instantiate(res);

        }
    }

    public static void DestoryDebugPanel()
    {
        GameObject.Destroy(_instance.gameObject);
    }

    public override void Init()
    {
        CreatDataSetElement("设置章节", "0", DebugDataSetElement.InputType.Button, (o) =>
           {
               SaveManager.Instance.Cfg.ChapterIndex = int.Parse(o);
               SaveManager.Instance.Save();
           });

        CreatDataSetElement("设置段落", "0", DebugDataSetElement.InputType.Button, (o) =>
        {
            SaveManager.Instance.Cfg.SectionIndex = int.Parse(o);
            SaveManager.Instance.Save();
        });


        CreatDataSetElement("加速", "1", DebugDataSetElement.InputType.Button, (o) =>
        {
            Time.timeScale = int.Parse(o);
        });
    }
}