using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float mouseSens, yRotation, xRotation;
    private Rigidbody rBody;
    [SerializeField]private GameObject goalObj, torchObj;
    private Camera pCam;
    public bool torchOn;
    private int speed, batteries;

    void Start()
    {
        TryGetComponent<Rigidbody>(out rBody);
        pCam = GetComponentInChildren<Camera>();
        mouseSens = 100f;
        speed = 5;
        batteries = 6;
        torchOn = false;
        torchObj.SetActive(false);
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
        else if (Input.GetKey(KeyCode.E))
        {
            Interact();
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            if(!torchOn)
            { 
                StunUV();
            }
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
    float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

    //Cursor.lockState = CursorLockMode.Locked; //locks and hides mouse cursor to active game screen

    xRotation -= mouseY;
    yRotation += mouseX;

    xRotation = Mathf.Clamp(xRotation, -45f, 60f);

    pCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    gameObject.transform.Rotate(0, yRotation, 0, Space.World);
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
    public void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(pCam.transform.position, transform.forward, out hit)) 
        {
           Debug.DrawRay(pCam.transform.position, transform.forward, Color.yellow);
           if (hit.collider == goalObj)
           {
                //pickup event
           }
        }
    }
    public void StunUV()
    {
        Debug.Log("Stun called");
        if (batteries != 0)
        {
            torchOn = true;
            batteries -= 1;
            torchObj.SetActive(true);
            Debug.Log("Stun successful");
            GameManager.Instance.PlayerUI(batteries);
            torchObj.SetActive(false);
            torchOn = false;
        }
    }
}
