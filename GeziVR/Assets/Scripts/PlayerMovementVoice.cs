using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Oculus.Voice;

public class PlayerMovementVoice : MonoBehaviour
{
    [SerializeField] private AppVoiceExperience appVoiceExperience; 
    private bool appVoiceActive;


    Rigidbody rb;
    Vector3 moveDirection;
    public float airMultiplier;
    public float moveSpeed;
    bool grounded = true;
    public Transform orientation;

    private bool isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        appVoiceExperience.events.OnRequestCreated.AddListener((request) => {
            appVoiceActive = true;
            Debug.Log("Request created");}
        );
        appVoiceExperience.events.OnRequestCompleted.AddListener(() => 
        {
            appVoiceActive = false;
            Debug.Log("Request completed");
        });
        appVoiceExperience.events.OnFullTranscription.AddListener((transcription) => 
        {
            Debug.Log("Full transcription: " + transcription);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame && !appVoiceActive)
        {
            appVoiceExperience.Activate();
        }
       
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        
    }

    public void MovePlayer(string [] values)
    {
        if(values[0] == "forward")
        {
            isMoving = true;
            moveDirection = orientation.forward ;
            Debug.Log("forward");       
        }
        else if(values[0] == "back")
        {
            isMoving = true;
            moveDirection = orientation.forward * -1;
            Debug.Log("back");
        }
    }

    public void StopPlayer(string [] values)
    {
        if(values[0] == "stop")
            isMoving = false;
            moveDirection = Vector3.zero;
            Debug.Log("stop");
    }
}
