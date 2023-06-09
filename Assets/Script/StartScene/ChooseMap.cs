﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ChooseMap : MonoBehaviourPunCallbacks
{
    [SerializeField] private UISprite mapSprite;
    [SerializeField] private UILabel mapLabel;
    [SerializeField] private UIButton[] mapButtons;

    public int Index { get; private set; } = 0;

    private readonly string[] _mapNames = {"山脉", "海岛"};

    public void LastMap()
    {
        List<UISpriteData> list = mapSprite.atlas.spriteList;
        int mapIndex = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].name != mapSprite.spriteName) continue;
            mapIndex = i;
            break;
        }

        if (mapIndex == 0) return;

        mapIndex--;

        photonView.RPC("SetMap", RpcTarget.All, mapIndex);
    }

    public void NextMap()
    {
        List<UISpriteData> list = mapSprite.atlas.spriteList;
        int mapIndex = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].name != mapSprite.spriteName) continue;
            mapIndex = i;
            break;
        }

        if (mapIndex == list.Count - 1) return;

        mapIndex++;

        photonView.RPC("SetMap", RpcTarget.All, mapIndex);
    }

    [PunRPC]
    private void SetMap(int index)
    {
        mapSprite.spriteName = mapSprite.atlas.spriteList[index].name;
        mapLabel.text = _mapNames[index];
        Index = index;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            mapButtons.ToList().ForEach(button => button.gameObject.SetActive(true));
        else
            mapButtons.ToList().ForEach(button => button.gameObject.SetActive(false));
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            mapButtons.ToList().ForEach(button => button.gameObject.SetActive(true));
        else
            mapButtons.ToList().ForEach(button => button.gameObject.SetActive(false));
    }
}