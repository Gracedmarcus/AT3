using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float mouseSens, yRotation, xRotation;
    private Rigidbody rBody;
    private GameObject playerBody;
    private Camera pCam;
    public float speed;

    private void Start()
    {
        TryGetComponent<Rigidbody>(out rBody);
        pCam = GetComponentInChildren<Camera>();
        mouseSens = 100f;
        speed = 5f;
        playerBody = gameObject;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            ForwardMove();
        }
        else if(Input.GetKey(KeyCode.S))
        {
            BackMove();
        }
        if (Input.GetKey(KeyCode.D))
        {
            RightMove();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            LeftMove();
        }

    float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
    float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

    //Cursor.lockState = CursorLockMode.Locked; //locks and hides mouse cursor to active game screen

    xRotation -= mouseY;
    yRotation += mouseX;

    xRotation = Mathf.Clamp(xRotation, -45f, 60f);

    pCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    playerBody.transform.Rotate(0, yRotation, 0, Space.World);
    }

    public void ForwardMove()
    {
        transform.position += transform.forward * speed * Time.deltaTime; //denotes forward movement along the Z axis
    }
    public void BackMove()
    {
        transform.position -= transform.forward * speed * Time.deltaTime; //denotes backward movement along the Z axis
    }
    public void RightMove()
    {
        transform.position += transform.right * speed * Time.deltaTime; //denotes right movement along the X axis
    }    
    public void LeftMove()
    {
        transform.position -= transform.right * speed * Time.deltaTime; //denotes left movement along the X axis
    }
}
