using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float mouseSens, yRotation, xRotation;
    private Rigidbody rBody;
    [SerializeField] private GameObject[] goalObj;
    private Camera pCam;
    public bool torchOn;
    private int speed, batteries, goalNum;

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

    public void Interact()
    {
        if (goalNum != 3)
        {
            RaycastHit hit;
            if (Physics.Raycast(pCam.transform.position, transform.forward, out hit))
            {

                Debug.DrawRay(pCam.transform.position, transform.forward, Color.yellow);
                if (goalObj[goalNum].gameObject == hit.collider)
                {
                    goalNum++;
                    GameManager.Instance.GoalUpdate(goalNum);
                }
            }
        }
    }
    public void StunUV()
    {
        Debug.Log("Stun called");
        if (batteries != 0)
        {
            batteries -= 1;
            Debug.Log("Stun successful");
            //GameManager.Instance.PlayerUI(batteries);
        }
    }
}
