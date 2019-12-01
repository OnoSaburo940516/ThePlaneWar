﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissleActorButton : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Button button;
    [SerializeField] Text text;
    float time = 10.0f;
    int totalCount = 3;
    const float maxTime = 10.0f;
    const int maxCount = 3;

    public bool Missle
    {
        get
        {
            return totalCount > 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0 && totalCount < maxCount)
        {
            image.fillAmount = time / maxTime;
            time -= Time.deltaTime;
        }
        else if (totalCount == maxCount)
        {
            image.fillAmount = 0.0f;
        }
        else
        {
            ShootEnd();
        }
    }

    public void ShootStart()
    {
        totalCount--;
        button.enabled = Missle;
        text.text = totalCount.ToString();
    }

    void ShootEnd()
    {
        if (totalCount < maxCount)
            totalCount++;

        time = maxTime;
        image.fillAmount = 1.0f;
        button.enabled = Missle;
        text.text = totalCount.ToString();
    }
}
