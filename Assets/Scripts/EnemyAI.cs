using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class EnemyAI : MonoBehaviourPunCallbacks
{

    [SerializeField] private Transform movePositionTransform;

    private NavMeshAgent navMeshAgent;
    public AudioSource zombieMoan;
    public AudioClip clip1;
    public int speed;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        speed = 4;
        zombieMoan = GetComponent<AudioSource>();
        
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            return;
        }

        movePositionTransform = FindClosestPlayer().transform;

        navMeshAgent.destination = movePositionTransform.position;

        navMeshAgent.speed = speed + (speed / (GameVariables.keyCount + 1));

    }


    public GameObject FindClosestPlayer()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
                
            }
        }
        return closest;
    }
}