using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIScrollSlider : MonoBehaviour {

    private ScrollRect _scroll_View;

    public enum SliderDir
    {
        Up,
        Down,
        Left,
        Right
    }

    private UnityAction<SliderDir> _callback;

    private bool isRun=false;

    public void Start()
    {
        _scroll_View = gameObject.GetComponent<ScrollRect>();

        if (_scroll_View != null)
        {
            _scroll_View.onValueChanged.AddListener(OnSliderValueChange);
        }

        OnRelease();
        InputListenerManager.RegisterTouchUpEvent(OnRelease);
    }

    public void OnDestroy()
    {
        if (_scroll_View != null)
        {
            _scroll_View.onValueChanged.RemoveListener(OnSliderValueChange);
           
        }

        InputListenerManager.UnRegisterTouchUpEvent(OnRelease);

        _callback = null;
    }

    public void Install(UnityAction<SliderDir> callback)
    {
        _callback += callback;
    }

    public void UnInstall(UnityAction<SliderDir> callback)
    {
        _callback -= callback;
    }

    private void OnSliderValueChange(Vector2 pos)
    {
        if (!isRun)
        {
            return;
        }

        if (pos.x > 1)
        {
            OnSliderAction( SliderDir.Left);
        }

        if (pos.x < 0)
        {
            OnSliderAction(SliderDir.Right);
        }
    }


    private void OnRelease()
    {
        if (_scroll_View != null)
        {
            _scroll_View.normalizedPosition = new Vector2(0.5f, 0.5f);
        }
        isRun = true;
    }

 

    public void OnSliderAction(SliderDir dir)
    {
        if (_callback != null)
        {
            _callback(dir);
        }

        isRun = false;
    }

}
