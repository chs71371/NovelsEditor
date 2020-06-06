using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragMovePos : MonoBehaviour,IPointerUpHandler,IDragHandler {

    private RectTransform _rect;

    public GameObject Obj_Active;

    private bool _isDrag = false;

    private void Start()
    {
        _rect = gameObject.GetComponent<RectTransform>();
    }


    public void OnDrag(PointerEventData eventData)
    {
        _isDrag = true;
        _rect.position = eventData.position;
    }

 

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDrag)
        {
            Obj_Active.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        _isDrag = false;
    }
}
