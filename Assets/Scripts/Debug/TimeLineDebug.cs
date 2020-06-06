using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineDebug : MonoBehaviour
{
    public void Awake()
    {

        GameHelper.Alloc<GameObject>("Prefabs/Base/UIRoot");
        GameHelper.Alloc<GameObject>("Prefabs/Base/AudioManager");

        GameHelper.Alloc<GameObject>("Prefabs/UI/Novels/UINovelsPanel").transform.RestTransform(UIRoot.Instance.Trans_NovelsPoint);
        var novelsMgr = new GameObject("NovelsManager").AddComponent<NovelsManager>();
        UINovelsPanel.Instance.gameObject.SetActive(true);

    }
}
