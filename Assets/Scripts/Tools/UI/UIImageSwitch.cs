using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageSwitch : MonoBehaviour {

    private RectTransform _RectTrans;

    public RectTransform RectTrans
    {
        get
        {
            if (_RectTrans == null)
            {
                _RectTrans = gameObject.GetComponent<RectTransform>();
 
            }
            return _RectTrans;
        }
    }

    private Image _targetImage;

    public Image TargetImage
    {
        get
        {
            if (_targetImage == null)
            {
                _targetImage = gameObject.GetComponent<Image>();

                if (_targetImage == null)
                {
                    var button = gameObject.GetComponent<Button>();
                    if (button != null)
                    {
                        _targetImage = button.targetGraphic.GetComponent<Image>();
                    }
                }
            }

            return _targetImage;
        }
    }

    public bool IsSetNativeSize = false;

    public List<Sprite> Sprites = new List<Sprite>();

    public List<GameObject> ImageObjs = new List<GameObject>();

    private int _curIndex=0;

    private void Awake()
    {
        for (int i = 0; i < ImageObjs.Count; i++)
        {
            if (i == _curIndex)
            {
                ImageObjs[_curIndex].gameObject.SetActive(true);
            }
            else
            {
                ImageObjs[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetImage(int index)
    {
        if (_curIndex == index)
        {
            return;
        }

        _curIndex = index;

        SetSprite();
        ActiveObj();
    }

    private void SetSprite()
    {
        if (Sprites.Count ==0|| _curIndex>= Sprites.Count)
        {
            return;
        }
     
        TargetImage.sprite = Sprites[_curIndex];
        if (IsSetNativeSize)
        {
            TargetImage.SetNativeSize();
        }
    }

    private void ActiveObj()
    {
        if (ImageObjs.Count == 0)
        {
            return;
        }

        for (int i = 0; i < ImageObjs.Count; i++)
        {
            if (i == _curIndex)
            {
                ImageObjs[_curIndex].gameObject.SetActive(true);
            }
            else
            {
                ImageObjs[i].gameObject.SetActive(false);
            }
        }
       
    }
}
