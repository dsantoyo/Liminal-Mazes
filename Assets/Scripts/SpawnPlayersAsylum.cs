using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayersAsylum : MonoBehaviour
{
    public PhotonView playerPrefab;

    private void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0,0.5f,35) , Quaternion.identity);
    }
}
