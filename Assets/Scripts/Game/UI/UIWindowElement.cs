using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowElement : MonoBehaviour
{
    public Image Image_Content;

    public IEnumerator FreshView(PopupWindowData data)
    {
        Image_Content.sprite = data.ShowSprite;
        var rect = gameObject.GetComponent<RectTransform>();
        switch (data.Type)
        {
            case EPopupType.Act:
               
                rect.anchoredPosition = data.OffsetPos;
                break;
            case EPopupType.Item:
                break;
        }
        gameObject.SetActive(true);
        rect.sizeDelta = data.Size;

         yield break;
    }
}
