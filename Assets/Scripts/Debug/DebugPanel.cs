using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class DebugPanel : MonoBehaviour {

    protected static DebugPanel _instance;
	public static bool HasInstance { get { return _instance; }}

    [SerializeField]
    private DebugDataSetElement m_DataSetElement;
    [SerializeField]
    private RectTransform m_Content;

    public void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }



    protected void CreatDataSetElement(string name, string value, DebugDataSetElement.InputType type ,UnityAction<string> callback)
    {
        var obj = GameObject.Instantiate(m_DataSetElement.gameObject);
        var script=  obj.GetComponent<DebugDataSetElement>();
        script.Install(name,value, type, callback);
        obj.transform.RestTransform(m_Content);
        obj.name = name;
        obj.SetActive(true);
    }


    //Custom Set
    public void Start()
    {
        Init();
    }

    public virtual void Init()
    {

    }


}


 
