using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KeyItem : MonoBehaviourPun
{
    public AudioSource keySound;
    public AudioClip key;
    void OnEnable()
    {
        GameVariables.keyCount += 1;
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            return;
        }
        if (collider.gameObject.tag == "Player")
        {
            this.photonView.RPC("PickUpKey", RpcTarget.AllBuffered);
            keySound.Play();

        }
    }

    [PunRPC]
    void PickUpKey()
    {
        GameVariables.keyCount -= 1;
        
        Destroy(gameObject);
        
    }
}
