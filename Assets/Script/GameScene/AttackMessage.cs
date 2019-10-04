﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class AttackMessage : MonoBehaviour
{
    [SerializeField] Text text;
    Queue<string> queue = new Queue<string>();

    [PunRPC]
    public void AddAttackMessage(string message)
    {
        queue.Enqueue(message);

        Show();

        StartCoroutine(RemoveLine());
    }

    IEnumerator RemoveLine()
    {
        yield return new WaitForSeconds(6.0f);
        queue.Dequeue();
        Show();
    }

    void Show()
    {
        string[] array = queue.ToArray();
        string data = "";
        for(int i = 0; i < array.Length; i++)
        {
            data += array[i] + "\n";
        }

        text.text = data;
    }
}