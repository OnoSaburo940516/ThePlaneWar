﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraActive : MonoBehaviour
{
    [SerializeField] private new GameObject camera;
    private BinnacleScript _binnacleScript;

    public GameObject Camera => camera;

    // Start is called before the first frame update
    private void Start()
    {
        if (!GetComponent<PhotonView>().IsMine) return;
        
        camera.SetActive(true);

        _binnacleScript.player = transform;
        _binnacleScript.playerCamera = camera.transform;
    }

    private void Awake()
    {
        _binnacleScript = FindObjectOfType<BinnacleScript>();
    }
}
