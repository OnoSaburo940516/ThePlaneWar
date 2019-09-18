﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PhotonGame : MonoBehaviour {
    [SerializeField] GameObject[] planePrefabs;
    [SerializeField] Transform[] groundRunwayPosotion;
    [SerializeField] GameObject explosionParticleSystem;
    [SerializeField] Image rebornImage;
    [SerializeField] GameObject timeBar;
    [SerializeField] Text timeText;
    [SerializeField] Image timeImage;
    GameObject localPlane;
    bool reborn = false;
    bool invincible = false;
    float time = 0.0f;
    int maxTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        localPlane = PhotonNetwork.Instantiate(planePrefabs[Global.totalPlaneInt].name, groundRunwayPosotion[Global.totalPlayerInt].position + new Vector3(0, 10, 0), Quaternion.identity);

        Global.inGame = true;

        InvincibleState();
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0)
        {
            timeImage.fillAmount = time / maxTime;
            time -= Time.deltaTime;
        }
        else
        {
            if (reborn)
            {
                rebornImage.enabled = false;
                timeBar.SetActive(false);

                localPlane = PhotonNetwork.Instantiate(planePrefabs[Global.totalPlaneInt].name, groundRunwayPosotion[Global.totalPlayerInt].position + new Vector3(0, 10, 0), Quaternion.identity);

                reborn = false;

                InvincibleState();
            }

            if (invincible)
            {
                CustomProperties.SetProperties(PhotonNetwork.LocalPlayer, "invincible", false);
                timeBar.SetActive(false);

                invincible = false;
            }
        }
    }

    public void OnExitButtonClick()
    {
        PhotonNetwork.Destroy(localPlane);
        SceneManager.LoadScene("StartScene");
    }

    [PunRPC]
    public void Dead()
    {
        GameObject explosion = PhotonNetwork.Instantiate(explosionParticleSystem.name, localPlane.transform.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();

        PhotonNetwork.Destroy(localPlane);
        Reborn();

        int death = (int)CustomProperties.GetProperties(PhotonNetwork.LocalPlayer, "death", 0);
        death++;
        CustomProperties.SetProperties(PhotonNetwork.LocalPlayer, "death", death);

        CustomProperties.SetProperties(PhotonNetwork.LocalPlayer, "HP", 100);
    }

    void Reborn()
    {
        rebornImage.enabled = true;
        timeBar.SetActive(true);
        reborn = true;
        time = 10.0f;
        maxTime = 10;
        timeText.text = "重生";
    }

    void InvincibleState()
    {
        CustomProperties.SetProperties(PhotonNetwork.LocalPlayer, "invincible", true);
        timeBar.SetActive(true);
        invincible = true;
        time = 20.0f;
        maxTime = 20;
        timeText.text = "无敌状态";
    }
}
