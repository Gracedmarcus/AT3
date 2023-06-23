using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBox : MonoBehaviour
{
    public GameManager game;
    public Player player;

    private void Start()
    {
        game = GameManager.Instance;
        player = game.player;
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject == player)
        {
            game.GameWin();
        }
    }
}
