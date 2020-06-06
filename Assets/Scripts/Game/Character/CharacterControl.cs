using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Sirenix.OdinInspector;
using System;

public static class CharaterData 
{
    public static int CollectItemCount = 0;
    public static bool IsExplore = false;
}

public class CharacterControl : MonoBehaviour
{
    public static List<CharacterControl> _currentActiveControl = new List<CharacterControl>();

    public static CharacterControl Instance 
    {
        get 
        {
            if (_currentActiveControl.Count > 0) 
            {
                return _currentActiveControl[0];
            }
            return null;
        }
    }

    public PlaySpriteAnimation Anim;
    [LabelText("显示节点")]
    public GameObject Show;
    [LabelText("移动速度")]
    public float MoveSpeed=1;
    [LabelText("是否左朝向")]
    public bool IsLeftDir=false;
    [LabelText("移动动作")]
    public string MoveAnim="move";
    public string MoveIdle = "idle";

    [LabelText("角色状态")]
    public EState State = EState.Stop;
    [LabelText("镜头跟随物体")]
    public Transform FollowObj;
   

    [HideLabel]
    public List<ISceneObject> _touchSceneObjects = new List<ISceneObject>();
    public enum EState
    {
        Stop=0,
        Move=1,
        Confire=2,
    }
    [HideInInspector]
    public float MoveInput=0;

    private SpriteRenderer _spriteRender;

    private Coroutine Cor;

    private void Awake()
    {
        if (Anim != null) 
        {
            _spriteRender = Anim.GetComponent<SpriteRenderer>();
        }
        if (string.IsNullOrEmpty(MoveAnim)) 
        {
            MoveAnim = "move";
        }

        if (string.IsNullOrEmpty(MoveIdle))
        {
            MoveIdle = "idle";
        }
    }

    public void OnEnable()
    {
        if (NovelsManager.Instance == null) 
        {
            return;
        }

        _currentActiveControl.Add(this);
        Cor = NovelsManager.Instance.StartCoroutine(Run());

        if (FollowObj == null) 
        {
            FollowObj = gameObject.transform;
        }

       
    }

    private void Start()
    {
        if (_spriteRender != null) 
        {
            if (IsLeftDir)
            {
                _spriteRender.flipX = false;
            }
            else
            {
                _spriteRender.flipX = true;
            }
        }
        if (Anim != null) 
        {
            Anim.Play(MoveIdle, true);
        }
    }

    private void OnDisable()
    {
        _currentActiveControl.Remove(this);
        NovelsManager.Instance.StopCoroutine(Cor);
    }

    public IEnumerator EnterDoor()
    {
        if (_touchSceneObjects.Count <= 0)
        {
            yield break;
        }
        var lastTouch = _touchSceneObjects[0];
        var link = lastTouch as SceneLink;
        link.ImageSwitch_Link.SetImage(1);
        yield return new WaitForSeconds(0.3f);
        gameObject.transform.position = link.TargeLink.transform.position;
        yield return new WaitForSeconds(0.1f);
        link.ImageSwitch_Link.SetImage(0);
        SceneSwitchManager.Instance.Mask.DOFade(1, 0.3f);
        yield return new WaitForSeconds(0.3f);
        SceneSwitchManager.Instance.Mask.DOFade(0, 0.3f);
        SceneSwitchManager.Instance.SetScene(link.TargeScene);
        SceneSwitchManager.Instance.SceneMap[SceneSwitchManager.Instance.CurrentScene].FollowObj.transform.position = gameObject.transform.position;
        yield return new WaitForSeconds(0.3f);
    }

    public void OnConfire() 
    {
        State = EState.Confire;
    }
    private  ISceneObject _lastTouch = null;
    private IEnumerator Run()
    {
        while (true)
        {
            if (State != EState.Stop)
            {
                if (State == EState.Move)
                {
                    OnMove();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    State = EState.Confire;
                }

                _lastTouch = null;
                if (_touchSceneObjects.Count > 0)
                {
                    _lastTouch = _touchSceneObjects[0];
                }

                if (State == EState.Confire) 
                {
                    if (_lastTouch!=null)
                    {
                        Anim.Play(MoveIdle, true);
                    }
                    else 
                    {
                        State = EState.Move;
                    }
                }

                if (_lastTouch != null) 
                {
                    switch (_lastTouch.GetType())
                    {
                        case ESceneObjectType.Link:
                            {
                                if (State == EState.Confire)
                                {
                                    var link = _lastTouch as SceneLink;
                                    link.ImageSwitch_Link.SetImage(1);
                                    yield return new WaitForSeconds(0.3f);
                                    gameObject.transform.position = link.TargeLink.transform.position;
                                    yield return new WaitForSeconds(0.1f);
                                    link.ImageSwitch_Link.SetImage(0);
                                    SceneSwitchManager.Instance.Mask.DOFade(1, 0.3f);
                                    yield return new WaitForSeconds(0.3f);
                                    SceneSwitchManager.Instance.Mask.DOFade(0, 0.3f);
                                    SceneSwitchManager.Instance.SetScene(link.TargeScene);
                                    SceneSwitchManager.Instance.SceneMap[SceneSwitchManager.Instance.CurrentScene].FollowObj.transform.position = gameObject.transform.position;
                                    yield return new WaitForSeconds(0.3f);
                                }


                            }
                            break;
                        case ESceneObjectType.Item:
                            {
                                var item = _lastTouch as SceneItem;

                                switch (item.TiggerType)
                                {
                                    case ESceneTriggerType.Confire:
                                        {
                                            if (State == EState.Confire)
                                            {
                                                Show.gameObject.SetActive(!item.IsHideModel);
                                                Anim.Play(MoveIdle, true);
                                                yield return item.TriggerPlot.Run();
                                                item.Get();
                                                Show.gameObject.SetActive(true);
                                                CharaterData.CollectItemCount++;
                                            }
                                        }
                                        break;
                                    case ESceneTriggerType.Auto:
                                        {
                                            Show.gameObject.SetActive(!item.IsHideModel);
                                            Anim.Play(MoveIdle, true);
                                            yield return item.TriggerPlot.Run();
                                            // Show.gameObject.SetActive(true);
                                            item.Get();
                                        }
                                        break;
                                }
                            }
                            break;
                    }

                    if (State == EState.Confire)
                    {
                        State = EState.Move;
                    }

                }

            }
            


            yield return null;
        }



    }

    public void OnMove()
    {
#if  UNITY_IPHONE || UNITY_ANDROID
#else
        if (UINovelsPanel.Instance.CurrentButtonState == UINovelsPanel.InputButtonState.None) 
        {
            MoveInput = Input.GetAxisRaw("Horizontal");
        }
#endif

        var hor = MoveInput * MoveSpeed * Time.deltaTime;

   
        if (MoveInput > 0)
        {
            _spriteRender.flipX = true;
            Anim.Play(MoveAnim, true);
        }
        else if (MoveInput < 0)
        {
            _spriteRender.flipX = false;
            Anim.Play(MoveAnim, true);
        }
        else
        {
            Anim.Play(MoveIdle, true);
        }

        if (MoveInput != 0)
        {
            var newPos = gameObject.transform.position.x + hor;
            newPos = Mathf.Clamp(newPos, SceneSwitchManager.Instance.MoveRange.x, SceneSwitchManager.Instance.MoveRange.y);
            gameObject.transform.position = new Vector3(newPos, 0, 0);
        }

      
    }

    private void Update()
    {
        if (SceneSwitchManager.Instance != null&& FollowObj!=null)
        {
            if (SceneSwitchManager.Instance.SceneMap[SceneSwitchManager.Instance.CurrentScene].FollowObj != null)
            {
                SceneSwitchManager.Instance.SceneMap[SceneSwitchManager.Instance.CurrentScene].FollowObj.transform.position = FollowObj.position;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var link = collision.gameObject.GetComponent<ISceneObject>();
       
        if (link != null)
        {
            _touchSceneObjects.Add(link);
            _touchSceneObjects= _touchSceneObjects.OrderBy(o => -(int)o.GetType()).ToList();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var link = collision.gameObject.GetComponent<ISceneObject>();

        if (link != null)
        {
            _touchSceneObjects.Remove(link);
        }
    }

}
