using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LauncherScript : MonoBehaviourPunCallbacks
{
    public PhotonView playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //try to connect
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //connected
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room successfully");
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
    }
}
