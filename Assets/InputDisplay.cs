using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputDisplay : MonoBehaviour
{
    private GameObject Lft, Rght, Fwrd, Bck;
    private Color inputcolour;
    private Image inputImg;

    void Update()
    {
        if (Input.GetButton("Left"))
        { 
        }

    }

    void LRPressed ()
    {
        if(Input.GetButton("Left"))
        {
            //Lft.Image = Color.red;
        }
        else 
        { 
        }
    }

    void FBPressed()
    {
        if (Input.GetButton("Forward"))
        {
        }
        else
        {
        }
    }
}
