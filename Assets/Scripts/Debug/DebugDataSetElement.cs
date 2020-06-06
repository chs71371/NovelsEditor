using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDataSetElement : MonoBehaviour {

    [SerializeField]
    private Text m_KeyName;
    [SerializeField]
    private InputField m_Input;
    [SerializeField]
    private Button m_Button;

    public enum InputType
    {
        Text,
        Button
    }


    public void Install(string name, string value, InputType inputType, UnityEngine.Events.UnityAction<string> callback)
    {
        m_KeyName.text = name;
        m_Input.text = value;
        m_Button.gameObject.SetActive(false);

        switch (inputType)
        {
            case InputType.Text:
                m_Input.onValueChanged.AddListener((o) =>
                {
                    if (string.IsNullOrEmpty(o))
                    {
                        return;
                    }
                    callback(o);
                });
                break;
            case InputType.Button:
                m_Button.gameObject.SetActive(true);
                m_Button.onClick.AddListener(() =>
                {
                    callback(m_Input.text);
                });
                break;
        }
    }

}
