using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SpriteAdjustRectTransform : MonoBehaviour {

    public RectTransform Rect_Target;

    public float ScaleFactor;

    public void LateUpdate()
    {
        if (Rect_Target == null)
        {
            return;
        }


        gameObject.transform.localScale = new Vector3(Rect_Target.rect.width, Rect_Target.rect.height) * ScaleFactor;
    }
}
