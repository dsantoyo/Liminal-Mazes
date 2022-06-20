using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    public CharacterController controller;
    Animator anim;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 8f;
    public AudioSource walkSound;
    public AudioSource hurtSound;
    public AudioSource attackSound;
    public AudioSource music;
    public AudioSource keySound;

    public Transform groundCheck;
    public float groundDistance = 4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    public GameObject playerCamera;
    public GameObject playerMesh;

    PauseMenu pause;

    [Tooltip("The current Health of our player")]
    public float Health = 1f;

    public float minHealth = 0f;
    public float maxHealth = 1f;
    //bool paused = false;

    //[SerializeField] CanvasGroup holder = null;
    [SerializeField] Slider lifeSlider = null;
    [SerializeField] Image lifeSliderFill = null;

    [SerializeField] TextMeshProUGUI keyInfo = null;

    [SerializeField] Color fullLifeColor = Color.green;
    [SerializeField] Color deadLifeColor = Color.red;

    float GetLifePercentage
    {
        get
        {
            return (Health - minHealth) / (maxHealth - minHealth);
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        pause = GameObject.Find("Canvas").GetComponent<PauseMenu>();
        music.Play();
    }

    public override void OnEnable()
    {
        if(!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            playerCamera.SetActive(false);
            playerMesh.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause.Paused();
        }

        updateKeyCount();
        updateHealthSlider();

        if (Health <= 0f)
        {
            pause.Lose();
        }

        if (pause.isPaused)
        {
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight + -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        bool shouldMove = (x > 0.1 || x < -0.1 || z > 0.1 || z < -0.1);
        if (shouldMove)
        {
            if (!walkSound.isPlaying)
            {
                walkSound.Play();
            }
        }

        anim.SetBool("isMoving", shouldMove);
        anim.SetFloat("velx", x);
        anim.SetFloat("vely", z);
        //anim.SetFloat("Speed", velocity.magnitude);

    }

    private void updateHealthSlider()
    {
        //Check if life slider is referenced and if so update the life slider's value to battery life value.
        if (lifeSlider) { lifeSlider.value = Health; }
        //Check if life slider fill is referenced.
        if (lifeSliderFill)
        {
            //Update the life slider fill UI image color.
            lifeSliderFill.color = Color.Lerp(deadLifeColor, fullLifeColor, GetLifePercentage);
        }
    }

    private void updateKeyCount()
    {
        if (keyInfo) {
            if (GameVariables.keyCount != 0)
                keyInfo.text = "There are " + GameVariables.keyCount + " keys left.";
            else
                keyInfo.text = "The door will now open.";
        }
    }

    /// <summary>
    /// MonoBehaviour method called when the Collider 'other' enters the trigger.
    /// Affect Health of the Player if the collider is an Monster
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // we dont' do anything if we are not the local player.
        if (!photonView.IsMine)
        {
            return;
        }
        // We are only interested in Monsters
        if (other.gameObject.tag == "Monster")
        {
            Health -= 0.1f;
            if (!hurtSound.isPlaying)
            {
                hurtSound.Play();
            }
        }
        if(other.gameObject.tag == "key"){
            //keySound.Play();
        }
    }
    /// <summary>
    /// MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
    /// We're going to affect health while the Monsters are touching the player
    /// </summary>
    /// <param name="other">Other.</param>
    void OnTriggerStay(Collider other)
    {
        // we dont' do anything if we are not the local player.
        if (!photonView.IsMine)
        {
            return;
        }
        // We are only interested in Monsters
        if (other.gameObject.tag == "Monster")
        {
            // we slowly affect health when monster is constantly hitting us, so player has to move to prevent death.
            Health -= 0.1f * Time.deltaTime;
            if (!attackSound.isPlaying)
            {
                attackSound.Play();
            }
        }
    }

    #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(Health);
        }
        else
        {
            // Network player, receive data
            this.Health = (float)stream.ReceiveNext();
        }
    }


    #endregion


}
