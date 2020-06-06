using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class UITitlePanel : MonoBehaviour
{
    public static UITitlePanel Instance;

    public Button Button_Start;

    public Button Button_Continue;

    public Button Button_Over;

    public CanvasGroup Group;


    void Awake()
    {
        Instance = this;

        Button_Start.onClick.RemoveAllListeners();
        Button_Start.onClick.AddListener(() =>
        {
            GameManager.Instance.StartGame();
        });

        Button_Continue.onClick.RemoveAllListeners();
        Button_Continue.onClick.AddListener(() =>
        {
            GameManager.Instance.ContinueGame();
        });

        Button_Over.onClick.RemoveAllListeners();
        Button_Over.onClick.AddListener(() =>
        {
            GameManager.Instance.EndGame();
        });
    }
 

}
