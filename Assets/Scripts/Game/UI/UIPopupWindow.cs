using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public enum EPopupType
{
    Item,
    Act,
}

public class PopupWindowData
{
    [LabelText("类型")]
    public EPopupType Type;
    [LabelText("显示图片")]
    public Sprite ShowSprite;
    [ShowIf("@Type==EPopupType.Act")]
    [LabelText("中心点偏移")]
    public Vector3 OffsetPos;
    [LabelText("大小")]
    public Vector2 Size;
}


public class UIPopupWindow : MonoSingleton<UIPopupWindow>
{
    [SerializeField]
    private UIWindowElement _element;

    public Transform Trans_Content;

    private Stack<UIWindowElement> _elementPool=new Stack<UIWindowElement>();

    private List<UIWindowElement> _useElements = new List<UIWindowElement>();

    public UIWindowElement CreateWindow()
    {
        UIWindowElement element = null;
        if (_elementPool.Count > 0)
        {
            element = _elementPool.Pop();
        }
        else
        {
            var obj = GameObject.Instantiate(_element.gameObject);
            obj.transform.RestTransform(Trans_Content);
            
            element = obj.GetComponent<UIWindowElement>();
        }

        _useElements.Add(element);
        return element;
    }

    public IEnumerator Open(PopupWindowData data)
    {
        var element = CreateWindow();
        yield return  element.FreshView(data);
    }

 
    public void CloseAll()
    {
        for (int i = 0; i < _useElements.Count; i++)
        {
            _useElements[i].gameObject.SetActive(false);
            _elementPool.Push(_useElements[i]);
        }
        _useElements.Clear();
    }
}
