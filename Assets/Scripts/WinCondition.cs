using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WinCondition : MonoBehaviourPun
{
    PauseMenu pause;
    // Start is called before the first frame update
    void Start()
    {
        pause = GameObject.Find("Canvas").GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        // Run only on master client
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            return;
        }

        // Check for players only entering the win zone
        if (collider.gameObject.tag == "Player")
        {
            // Call RPC to all players
            this.photonView.RPC("Win", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void Win()
    {
        pause.Win();
    }
}
