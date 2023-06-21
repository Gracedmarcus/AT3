using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBox : MonoBehaviour
{
    private Collider player;
    private GameManager game;
    private bool exited;

    private void OnCollisionEnter(Collision collision)
    {
        if(game.currentObj && game.goalInt == 2)
        {
            if(collision.gameObject == player.gameObject)
            {
                game.GameWin();
            }
        }
    }
}
