using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIButtonCD : MonoBehaviour {

    public Image Image_CD;

    public float IntervalTime;

    private Button _curButton;
 
    private List<Graphic> _rayImages = new List<Graphic>();

    private bool _isRun = false;
    private float _curWaitTime = 0;
 
    private void Start()
    {
        _curButton = gameObject.GetComponent<Button>();
        
        _rayImages.Clear();
        var rayImages = new List<Graphic>();

        rayImages.Add(gameObject.GetComponent<Graphic>());
        rayImages.AddRange(gameObject.GetComponentsInChildren<Graphic>());

        for (int i = 0; i < rayImages.Count; i++)
        {
            if (rayImages[i] != null && rayImages[i].raycastTarget)
            {
                _rayImages.Add(rayImages[i]);
            }
        }

        Restore();
    }

    private void OnDisable()
    {
        Restore();
    }

    private void Restore()
    {
        _isRun = false;
        _curWaitTime = 0;
        Image_CD.fillAmount = 0;

        for (int i = 0; i < _rayImages.Count; i++)
        {
            if (_rayImages[i] != null)
            {
                _rayImages[i].raycastTarget = true;
            }
        }
    }

    public void Enter()
    {
        if (Image_CD != null)
        {
            for (int i = 0; i < _rayImages.Count; i++)
            {
                if (_rayImages[i] != null)
                {
                    _rayImages[i].raycastTarget = false;
                }
            }

            _isRun = true;
            _curWaitTime = 0;
            Image_CD.fillAmount = 1;
        }
    }

    private void Update()
    {
        if (_isRun)
        {
            _curWaitTime += Time.deltaTime;

            Image_CD.fillAmount = Mathf.Lerp(1,0, _curWaitTime/ IntervalTime);

            if (_curWaitTime > IntervalTime)
            {
                _curWaitTime = 0;
                Image_CD.fillAmount = 0;
                _isRun = false;
                for (int i = 0; i < _rayImages.Count; i++)
                {
                    if (_rayImages[i] != null)
                    {
                        _rayImages[i].raycastTarget = true;
                    }
                }
            }
        }   
    }
}
