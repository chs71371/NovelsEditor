using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class SpriteAnimationData
{
    public string Key;
    public float Speed;
    [PreviewField]
    public List<Sprite> Sprites;
}

[ExecuteInEditMode]
public class PlaySpriteAnimation : SerializedMonoBehaviour
{
    public List<SpriteAnimationData> Animations = new List<SpriteAnimationData>();

    private SpriteAnimationData _currentAnimation;
    public bool IsLoop;

    [OnValueChanged("PlayDefaultAnim")]
    public string DefaultName;


    private SpriteRenderer _render;
 
    public SpriteRenderer Render 
    {
        get 
        {
            if (_render == null) 
            {
                _render = gameObject.GetComponent<SpriteRenderer>();
            }
            return _render;
        }
    }
    private bool _isLoop;
    private int _index;
    private float _time;
    private float _speed = 1;
    private bool _isStop = false;
    public bool IsAwakePlay;

 

    private void OnEnable()
    {
        if (IsAwakePlay)
        {
            _index = 0;
            _currentAnimation = null;
            PlayDefaultAnim();
        }
    }


    public void PlayDefaultAnim() 
    {
        Play(DefaultName, IsLoop);
    }

    public void IsStop(bool isActive)
    {
        _isStop = isActive;
    }


    public void Play(string name, bool isLoop,float speed=1)
    {
        if (_currentAnimation != null && _currentAnimation.Key == name) 
        {
            return;
        }
        _time = 0;
        _speed = speed;
       _currentAnimation = Animations.Find(o => o.Key == name);
        _isLoop = isLoop;
        _index = 0;
       
    }

    public void Update()
    {
        if (_isStop) 
        {
            return;
        }

        if (_currentAnimation != null) 
        {
            _time += Time.deltaTime * _currentAnimation.Speed*5* _speed;
            if (Math.Abs(_time) > 1)
            {
                if (_time > 0)
                {
                    _time -= 1;
                    if (_isLoop)
                    {
                        _index = (_index + 1) % _currentAnimation.Sprites.Count;
                    }
                    else
                    {
                        _index = Mathf.Clamp(_index + 1, 0, _currentAnimation.Sprites.Count - 1);
                    }
                }
                else 
                {
                    _time += 1;
                    if (_isLoop)
                    {
                        _index = (_index - 1 + _currentAnimation.Sprites.Count) % _currentAnimation.Sprites.Count;
                    }
                    else
                    {
                        _index = Mathf.Clamp(_index - 1, 0, _currentAnimation.Sprites.Count - 1);
                    }
                   
                }
               
            }
            Render.sprite = _currentAnimation.Sprites[_index];
        }
    }
}
