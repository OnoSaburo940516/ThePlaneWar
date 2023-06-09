﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class ExplodingMissile : MonoBehaviourPun
{
    [SerializeField] private GameObject muzzlePrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float speed = 800;
    [SerializeField] private float explosionTimer = 6;
    private Rigidbody _thisRigidbody;
    private Collider _thisCollider;
    private Transform _missileTarget;
    private float _timer;

    // Start is called before the first frame update
    private void Start()
    {
        Instantiate(muzzlePrefab);
        _missileTarget = Global.GetNearTargetTransform(transform);

        if (_missileTarget != null && _missileTarget.CompareTag("Plane"))
            FindObjectOfType<PhotonGame>().photonView
                .RPC("PlayAudio", _missileTarget.GetComponent<PhotonView>().Controller, 4);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!photonView.IsMine) return;

        if (GetComponent<AIBullet>() && GetComponent<AIBullet>().aiTarget.CompareTag("AI") &&
            !PhotonNetwork.IsMasterClient) return;

        _timer += Time.deltaTime;
        if (!(_timer >= explosionTimer)) return;

        if (explosionPrefab)
            Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.Euler(0, 0, 0));

        PhotonNetwork.Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        if (GetComponent<AIBullet>() && GetComponent<AIBullet>().aiTarget.CompareTag("AI") &&
            !PhotonNetwork.IsMasterClient) return;

        if (_missileTarget != null)
        {
            float distance = Global.GetDistance(transform, _missileTarget);
            if (distance != 0)
                transform.LookAt(_missileTarget);
            else
                _missileTarget = null;
        }

        _thisRigidbody.AddForce(transform.forward * speed);

        if (_timer >= 0.05f)
            transform.rotation = Quaternion.LookRotation(_thisRigidbody.velocity);
    }

    private void Awake()
    {
        _thisRigidbody = GetComponent<Rigidbody>();
        _thisCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FX") || collision.gameObject.CompareTag("Plane") ||
            collision.gameObject.CompareTag("AI")) return;

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
        Vector3 pos = contact.point;
        Instantiate(explosionPrefab, pos, rot);
        _thisCollider.enabled = false;
        _thisRigidbody.velocity = Vector3.zero;

        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        AIBullet aiBullet = GetComponent<AIBullet>();

        if (aiBullet == null)
        {
            switch (other.gameObject.tag)
            {
                case "Plane" when !other.GetComponent<PhotonView>().IsMine:
                    FindObjectOfType<PhotonGame>().LocalPlane.GetComponent<PlaneAttack>()
                        .Attack(other.GetComponent<PhotonView>().Controller, other.transform);

                    PhotonNetwork.Destroy(gameObject);
                    break;
                case "AI":
                    FindObjectOfType<PhotonGame>().LocalPlane.GetComponent<PlaneAttack>()
                        .AttackAI(other.transform);

                    PhotonNetwork.Destroy(gameObject);
                    break;
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Plane") && !other.GetComponent<PhotonView>().IsMine)
            {
                aiBullet.aiTarget.GetComponent<AIAttack>()
                    .Attack(other.GetComponent<PhotonView>().Controller, other.transform);

                PhotonNetwork.Destroy(gameObject);
            }
            else if (other.gameObject.CompareTag("AI") && other.transform != aiBullet.aiTarget)
            {
                aiBullet.aiTarget.GetComponent<AIAttack>().AttackAI(other.transform);

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}