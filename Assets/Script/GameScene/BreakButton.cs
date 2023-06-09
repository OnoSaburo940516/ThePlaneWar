using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class BreakButton : MonoBehaviour
{ 
    [SerializeField] private Image image;
    [SerializeField] private GameObject button;
    private ButtonType _buttonType = ButtonType.Normal;
    private float _time = 0.0f;
    private const int CoolingMaxTime = 10;

    private enum ButtonType
    {
        Normal,
        Cooling
    }
    
    // Start is called before the first frame update
    void Start()
    {
        CoolingStart();
    }

    // Update is called once per frame
    void Update()
    {
        int maxTime = _buttonType switch
        {
            ButtonType.Cooling => CoolingMaxTime,
            _ => 0
        };
        
        if (_time > 0)
        {
            image.fillAmount = _time / maxTime;
            _time -= Time.deltaTime;
        }
        else
        {
            switch (_buttonType)
            {
                case ButtonType.Cooling:
                    CoolingEnd();
                    break;
                case ButtonType.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void CoolingStart()
    {
        _buttonType = ButtonType.Cooling;
        _time = CoolingMaxTime;
        image.fillAmount = 1.0f;
        button.GetComponent<Button>().enabled = false;
        button.GetComponent<EventTrigger>().enabled = false;
        button.GetComponent<ButtonHandler>().SetUpState();
    }
    
    private void CoolingEnd()
    {
        _buttonType = ButtonType.Normal;
        button.GetComponent<Button>().enabled = true;
        button.GetComponent<EventTrigger>().enabled = true;
    }
}