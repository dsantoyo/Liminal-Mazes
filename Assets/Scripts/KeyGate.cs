using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KeyGate : MonoBehaviourPun
{
    public AudioSource doorSound;
    bool play = false;
    void Update()
    {
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player" && GameVariables.keyCount < 1)
        {
            
            if (!play)
            {
                doorSound.Play();
                play = true;
            }
            Destroy(gameObject);
        }
    }
}
