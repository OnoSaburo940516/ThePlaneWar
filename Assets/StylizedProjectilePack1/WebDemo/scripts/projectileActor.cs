﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Photon.Pun;

public class projectileActor : MonoBehaviour {

    public Transform spawnLocator; 
    public Transform spawnLocatorMuzzleFlare;
    public Transform shellLocator;
    public Animator recoilAnimator;

    public Transform[] shotgunLocator;

    [System.Serializable]
    public class projectile
    {
        public string name;
        public Rigidbody bombPrefab;
        public GameObject muzzleflare;
        public float min, max;
        public bool rapidFire;
        public float rapidFireCooldown;   

        public bool shotgunBehavior;
        public int shotgunPellets;
        public GameObject shellPrefab;
        public bool hasShells;
    }
    public projectile[] bombList;


    string FauxName;
    public Text UiText;

    public bool UImaster = true;
    public bool CameraShake = true;
    public float rapidFireDelay;
    public CameraShake CameraShakeCaller;

    float firingTimer;
    public bool firing;
    public int bombType = 0;

   // public ParticleSystem muzzleflare;

    public bool swarmMissileLauncher = false;
    int projectileSimFire = 1;

    public bool Torque = false;
    public float Tor_min, Tor_max;

    public bool MinorRotate;
    public bool MajorRotate = false;
    int seq = 0;


	// Use this for initialization
	void Start ()
    {
        if (UImaster)
        {
            UiText.text = bombList[bombType].name.ToString();
        }
        if (swarmMissileLauncher)
        {
            projectileSimFire = 5;
        }

        Switch(Global.totalFireInt);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!GetComponent<PhotonView>().IsMine || tag == "AI") return;

        //Movement
        if(Input.GetButton("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                gameObject.transform.Rotate(Vector3.up, -25 * Time.deltaTime);
            }
            else
            {
                gameObject.transform.Rotate(Vector3.up, 25 * Time.deltaTime);
            }
        }

        //BULLETS
        /*
        if (Input.GetKeyDown(KeyCode.D))
        {
            Switch(-1);
        }
        if (CrossPlatformInputManager.GetButtonDown("Fire2")  || Input.GetKeyDown(KeyCode.A) )
        {
            Switch(1);
        }
        */
        
	    if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            firing = true;
            Fire();
        }
        if (CrossPlatformInputManager.GetButtonUp("Fire1"))
        {
            firing = false;
            firingTimer = 0;
        }

        if (bombList[bombType].rapidFire && firing)
        {
            if(firingTimer > bombList[bombType].rapidFireCooldown+rapidFireDelay)
            {
                Fire();
                firingTimer = 0;
            }
        }

        if(firing)
        {
            firingTimer += Time.deltaTime;
        }
	}

    public void Switch(int value)
    {
        /* 
            bombType += value;
            if (bombType < 0)
            {
              bombType = bombList.Length;
              bombType--;
            }
            else if (bombType >= bombList.Length)
            {
                bombType = 0;
            }
        if (UImaster)
        {
            UiText.text = bombList[bombType].name.ToString();
        }
        */
        bombType = value;
    }

    public void Fire()
    {
        if(CameraShake)
        {
            CameraShakeCaller.ShakeCamera();
        }
        Instantiate(bombList[bombType].muzzleflare, spawnLocatorMuzzleFlare.position, spawnLocatorMuzzleFlare.rotation);
        //   bombList[bombType].muzzleflare.Play();

        if (bombList[bombType].hasShells)
        {
            Instantiate(bombList[bombType].shellPrefab, shellLocator.position, shellLocator.rotation);
        }
        recoilAnimator.SetTrigger("recoil_trigger");

        Rigidbody rocketInstance;
        rocketInstance = PhotonNetwork.Instantiate(bombList[bombType].bombPrefab.name, spawnLocator.position, spawnLocator.rotation).GetComponent<Rigidbody>();
        // Quaternion.Euler(0,90,0)
        rocketInstance.AddForce(spawnLocator.forward * Random.Range(bombList[bombType].min, bombList[bombType].max));

        if (bombList[bombType].shotgunBehavior)
        {
            for(int i = 0; i < bombList[bombType].shotgunPellets ;i++ )
            {
                Rigidbody rocketInstanceShotgun;
                rocketInstanceShotgun = PhotonNetwork.Instantiate(bombList[bombType].bombPrefab.name, shotgunLocator[i].position, shotgunLocator[i].rotation).GetComponent<Rigidbody>();
                // Quaternion.Euler(0,90,0)
                rocketInstanceShotgun.AddForce(shotgunLocator[i].forward * Random.Range(bombList[bombType].min, bombList[bombType].max));
            }
        }

        if (Torque)
        {
            rocketInstance.AddTorque(spawnLocator.up * Random.Range(Tor_min, Tor_max));
        }
        if (MinorRotate)
        {
            RandomizeRotation();
        }
        if (MajorRotate)
        {
            Major_RandomizeRotation();
        }

        FindObjectOfType<AudioPlayer>().PlayAudio(0);
    }

    public void AIShoot()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Rigidbody rocketInstance;
            rocketInstance = PhotonNetwork.Instantiate(bombList[bombType].bombPrefab.name, spawnLocator.position, spawnLocator.rotation).GetComponent<Rigidbody>();
            // Quaternion.Euler(0,90,0)
            rocketInstance.AddForce(spawnLocator.forward * Random.Range(bombList[bombType].min, bombList[bombType].max));

            rocketInstance.gameObject.AddComponent<AIBullet>().AITarget = transform;
        }
    }


    void RandomizeRotation()
    {
        if (seq == 0)
        {
            seq++;
            transform.Rotate(0, 1, 0);
        }
      else if (seq == 1)
        {
            seq++;
            transform.Rotate(1, 1, 0);
        }
      else if (seq == 2)
        {
            seq++;
            transform.Rotate(1, -3, 0);
        }
      else if (seq == 3)
        {
            seq++;
            transform.Rotate(-2, 1, 0);
        }
       else if (seq == 4)
        {
            seq++;
            transform.Rotate(1, 1, 1);
        }
       else if (seq == 5)
        {
            seq = 0;
            transform.Rotate(-1, -1, -1);
        }
    }

    void Major_RandomizeRotation()
    {
        if (seq == 0)
        {
            seq++;
            transform.Rotate(0, 25, 0);
        }
        else if (seq == 1)
        {
            seq++;
            transform.Rotate(0, -50, 0);
        }
        else if (seq == 2)
        {
            seq++;
            transform.Rotate(0, 25, 0);
        }
        else if (seq == 3)
        {
            seq++;
            transform.Rotate(25, 0, 0);
        }
        else if (seq == 4)
        {
            seq++;
            transform.Rotate(-50, 0, 0);
        }
        else if (seq == 5)
        {
            seq = 0;
            transform.Rotate(25, 0, 0);
        }
    }
}
