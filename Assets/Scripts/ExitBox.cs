using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBox : MonoBehaviour
{
    public GameManager game = GameManager.Instance;
    public GameObject player;

    private void Start()
    {
        player = game.player.gameObject;
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject == player && game.goalInt == 2)
        {
            game.GameWin();
        }
    }
}
