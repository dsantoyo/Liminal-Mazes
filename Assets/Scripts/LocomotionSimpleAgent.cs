// LocomotionSimpleAgent.cs
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class LocomotionSimpleAgent : MonoBehaviourPun
{
    Animator anim;
    NavMeshAgent agent;
    //Vector2 smoothDeltaPosition = Vector2.zero;
    //Vector2 velocity = Vector2.zero;
    Vector3 velocity = Vector3.zero;

    void Start()
    {
        //if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        //{
        //    return;
        //}
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Don’t update position automatically
        //agent.updatePosition = false;
    }

    void FixedUpdate()
    {

        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            return;
        }

        //Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        //float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        //float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        //Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        //float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        //smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        //if (Time.deltaTime > 1e-5f)
        //    velocity = smoothDeltaPosition / Time.deltaTime;

        //NavMeshAgent.velocity.x

        velocity = agent.velocity;

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

        // Update animation parameters
        anim.SetBool("move", shouldMove);
        anim.SetFloat("velx", velocity.x);
        anim.SetFloat("vely", velocity.y);
        anim.SetFloat("Speed", velocity.magnitude);

        //GetComponent<LookAt>().lookAtTargetPosition = agent.steeringTarget + transform.forward;
    }

    //void OnAnimatorMove()
    //{
    //    // Update position to agent position
    //    transform.position = agent.nextPosition;
    //}

    void OnTriggerEnter(Collider other)
    {
        // we dont' do anything if we are not the master client.
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            return;
        }
        // We are only interested in players
        if (other.gameObject.tag == "Player")
        {
            anim.SetTrigger("touchingPlayer");
        }
    }

    void OnTriggerStay(Collider other)
    {
        // we dont' do anything if we are not the master client.
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            return;
        }
        // We are only interested in players
        if (other.gameObject.tag == "Player")
        {
            anim.SetTrigger("touchingPlayer");
        }
    }
}