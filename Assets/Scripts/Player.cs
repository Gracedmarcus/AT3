using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float mouseSens, yRotation, xRotation;
    private Rigidbody rBody;
    private Camera pCam;
    public bool torchOn;
    [SerializeField]private GameObject torch;
    private int speed, batteries, goalNum;
    private GameManager game;

    void Awake()
    {
        game = GameManager.Instance;
    }

    void Start()
    {
        TryGetComponent<Rigidbody>(out rBody);
        pCam = GetComponentInChildren<Camera>();
        mouseSens = 100f;
        speed = 5;
        torchOn = false;
    }

    private void FixedUpdate()
    {
        if(Input.GetAxisRaw("Vertical") > 0)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else if(Input.GetAxisRaw("Vertical") < 0)
        {
            transform.position -= transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.position -= transform.right * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Interact();
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            if(!torchOn)
            { 
                StunUV();
                torchOn = true;
            }
        }

    float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
    float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

    xRotation -= mouseY;
    yRotation += mouseX;
    xRotation = Mathf.Clamp(xRotation, -45f, 60f);
    pCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    gameObject.transform.Rotate(0, yRotation, 0, Space.World);
    }

    public void Interact() //broken atm, doesnt follow camera past 90deg
    {
        Debug.Log("Grabbing");
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 25f))
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.yellow, 25f);
            Debug.Log("Grabbed?");
            if (hit.collider.gameObject.tag=="ObjTag")
            {
                game.GoalUpdate(hit.collider.gameObject);
            }
        }
        Debug.Log("Nothing grabbed");
    }
    public void StunUV()
    {
        Debug.Log("Stun called");
        if (game.batteries != 0)
        {
            game.batteries -= 1;
            Debug.Log("Stun fired");
        }
    }
}
