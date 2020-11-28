﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PhotonGame : MonoBehaviourPunCallbacks
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject[] planePrefabs;
    [SerializeField] private Transform[] groundRunwayPosition;
    [SerializeField] private GameObject explosionParticleSystem;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpLerpImage;
    [SerializeField] private Image sightImage;
    [SerializeField] private Sprite[] sightSprites = new Sprite[3];
    [SerializeField] private GameObject timeBar;
    [SerializeField] private Text timeText;
    [SerializeField] private Image timeImage;
    private bool _invincible = false;
    private bool _reconnected = false;
    private float _time = 20.0f;
    private int _maxTime = 20;

    public Camera MainCamera => mainCamera;

    public GameObject ExplosionParticleSystem => explosionParticleSystem;

    public Image SightImage => sightImage;

    public Sprite[] SightSprites => sightSprites;

    public bool Reborn { get; private set; } = false;

    public GameObject LocalPlane { get; private set; }

    public Transform[] GroundRunwayPosition => groundRunwayPosition;

    // Start is called before the first frame update
    private void Start()
    {
        LocalPlane = PhotonNetwork.Instantiate(planePrefabs[Global.totalPlaneInt].name,
            groundRunwayPosition[Global.totalPlayerInt].position + new Vector3(0, 15, 0), Quaternion.identity);

        SightImage.rectTransform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        FindObjectOfType<PhotonGameAI>().InitializeAI();

        StartCoroutine(InvincibleStart());
    }

    // Update is called once per frame
    private void Update()
    {
        hpLerpImage.fillAmount = Mathf.Lerp(
            (float) ((int) PhotonNetwork.LocalPlayer.GetProperties("HP", 100) / 100.0),
            hpLerpImage.fillAmount, 0.95f);

        if (_time > 0)
        {
            timeImage.fillAmount = _time / _maxTime;
            _time -= Time.deltaTime;
        }
        else
        {
            if (Reborn)
            {
                RebornEndOrReconnect();
            }

            if (_invincible)
            {
                InvincibleEnd();
            }

            if (_reconnected)
            {
                SceneManager.LoadScene("StartScene");
            }
        }
    }

    public void OnExitButtonClick()
    {
        Global.returnState = Global.ReturnState.ExitGame;
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void GameOver()
    {
        Global.returnState = Global.ReturnState.GameOver;
        PhotonNetwork.Destroy(LocalPlane);
        if (PhotonNetwork.IsMasterClient)
        {
            SceneManager.LoadScene("StartScene");
        }
    }

    [PunRPC]
    public void Dead()
    {
        GameObject explosion = PhotonNetwork.Instantiate(explosionParticleSystem.name, LocalPlane.transform.position,
            Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        GetComponent<AudioPlayer>().PlayAudio(3);

        LocalPlane.GetComponent<MeshRenderer>().enabled = false;

        ParticleSystem[] particleSystems = LocalPlane.GetComponentsInChildren<ParticleSystem>();
        foreach (var items in particleSystems)
        {
            items.Stop();
        }

        _invincible = false;

        StartCoroutine(RebornStart());

        int death = (int) PhotonNetwork.LocalPlayer.GetProperties("death", 0);
        death++;
        PhotonNetwork.LocalPlayer.SetProperties("death", death);

        PhotonNetwork.LocalPlayer.SetProperties("HP", 100);
    }

    private IEnumerator RebornStart()
    {
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(LocalPlane);

        mainCamera.enabled = true;
        timeBar.SetActive(true);
        sightImage.gameObject.SetActive(false);

        _time = 10.0f;
        _maxTime = 10;
        timeText.text = "重生";

        yield return new WaitForSeconds(1.0f);
        Reborn = true;
    }

    public void RebornEndOrReconnect()
    {
        mainCamera.enabled = false;
        timeBar.SetActive(false);
        sightImage.gameObject.SetActive(true);

        LocalPlane = PhotonNetwork.Instantiate(planePrefabs[Global.totalPlaneInt].name,
            groundRunwayPosition[Global.totalPlayerInt].position + new Vector3(0, 15, 0), Quaternion.identity);

        Reborn = false;
        _reconnected = false;

        StartCoroutine(InvincibleStart());
    }

    private IEnumerator InvincibleStart()
    {
        PhotonNetwork.LocalPlayer.SetProperties("invincible", true);
        timeBar.SetActive(true);
        _time = 20.0f;
        _maxTime = 20;
        timeText.text = "无敌状态";

        yield return new WaitForSeconds(1.0f);
        _invincible = true;
    }

    private void InvincibleEnd()
    {
        PhotonNetwork.LocalPlayer.SetProperties("invincible", false);
        timeBar.SetActive(false);

        _invincible = false;
    }

    public void Disconnect()
    {
        if (_reconnected)
            return;

        mainCamera.enabled = true;
        timeBar.SetActive(true);
        sightImage.gameObject.SetActive(false);

        _time = 60.0f;
        _maxTime = 60;
        timeText.text = "正在重连";

        _reconnected = true;
    }

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(target, changedProps);

        if (target == PhotonNetwork.LocalPlayer)
        {
            hpImage.fillAmount = (float) ((int) target.GetProperties("HP", 100) / 100.0);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        
        PhotonNetwork.Destroy(LocalPlane);
        SceneManager.LoadScene("StartScene");
    }
}