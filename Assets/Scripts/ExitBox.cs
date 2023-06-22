using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBox : MonoBehaviour
{
    public GameManager game = GameManager.Instance;

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag=="Player")
        {
            game.GameWin();
        }
    }
}
