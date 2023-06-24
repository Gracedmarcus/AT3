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
    private int speed;
    private GameManager game;

    void Awake()
    {
        game = GameManager.Instance;
        
    }
    void Start()
    {
        TryGetComponent(out rBody);
        pCam = GetComponentInChildren<Camera>();
        mouseSens = 100f;
        speed = 5;
        torch.gameObject.SetActive(false);
        torchOn = false;
    }

    private void FixedUpdate()
    {        
        if (!torchOn)
        {
            if(Input.GetButton("Torch"))
            { 
                StartCoroutine(StunUV());
                torchOn = true;
                torch.gameObject.SetActive(true);
            }
        }        
        if (Input.GetButtonDown("Interact"))
        {
            Interact();
        }
        if(Input.GetAxisRaw("Vertical") != 0)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            } 
            else if(Input.GetAxisRaw("Vertical") < 0)
            {
            transform.position -= transform.forward * speed * Time.deltaTime;
            }
        }
       
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                transform.position += transform.right * speed * Time.deltaTime;
            }        
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
            transform.position -= transform.right * speed * Time.deltaTime;
            }
        }

    float mouseX = Input.GetAxis("MouseX") * mouseSens * Time.deltaTime;
    float mouseY = Input.GetAxis("MouseY") * mouseSens * Time.deltaTime;

    xRotation -= mouseY;
    yRotation += mouseX;
    xRotation = Mathf.Clamp(xRotation, -45f, 60f);
    pCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    gameObject.transform.Rotate(0, yRotation, 0, Space.World);
    }

    public void Interact() 
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 10f))
        {
            if (hit.collider.gameObject.CompareTag("ObjTag"))
            {
                Debug.Log("Grabbed " + hit.collider.gameObject);
                game.GoalUpdate(hit.collider.gameObject);
            }
            else 
            {  
                Debug.Log("Nothing grabbed");
            }
        }
    }
    public IEnumerator StunUV()
    {
        if (game.batteries != 0)
        {
            game.batteries -= 1;
            if (game.batteries != game.BatteriesList.Length)
            {
                Image target = game.BatteriesList[game.batteries];
                while (target.fillAmount != 0)
                {
                    target.fillAmount -= 0.1f;
                    yield return new WaitForSeconds(0.20f);
                }
            }
            torchOn = false;
            torch.gameObject.SetActive(false);
            Debug.Log(game.batteries + " Batts remaining");
        }
    }
}
