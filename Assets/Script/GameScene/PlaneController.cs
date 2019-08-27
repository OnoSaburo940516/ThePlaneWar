﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlaneController : MonoBehaviourPunCallbacks
{
    [SerializeField] Image HP_Image;
    [SerializeField] Image sight_Image;
    [SerializeField] Sprite[] sight_Sprites = new Sprite[2];

    // Start is called before the first frame update
    void Start()
    {
        CustomProperties.SetProperties(PhotonNetwork.LocalPlayer, "HP", 100);
    }

    public void Attack(Collider collider)
    {
        Player player = collider.gameObject.GetComponent<PhotonView>().Controller;

        int randomAttack = Random.Range(5, 15);

        int totalHP = (int)CustomProperties.GetProperties(player, "HP");
        CustomProperties.SetProperties(player, "HP", totalHP - randomAttack);

        StartCoroutine(ShowSight());
    }

    System.Collections.IEnumerator ShowSight()
    {
        sight_Image.sprite = sight_Sprites[1];
        yield return new WaitForSeconds(0.5f);
        sight_Image.sprite = sight_Sprites[0];
    }

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(target, changedProps);

        if (target == PhotonNetwork.LocalPlayer)
        {
            HP_Image.fillAmount = (float)((int)CustomProperties.GetProperties(target, "HP") / 100.0);
        }
    }
}