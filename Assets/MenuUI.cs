using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField]private Button resume, options, menu;
    private Canvas canvas;
    private GameManager manager;
    private Image panel;
    // Start is called before the first frame update

    private void Start()
    {
        manager = GameManager.Instance;
        canvas = FindObjectOfType<Canvas>();
        panel = canvas.GetComponentInChildren<Image>();
        if(!manager.paused)
        {
            panel.enabled = false;
        }    
    }
    public void Resume()
    {
        manager.paused = false;
        Paused(manager.paused);
    }
    public void Options()
    {

    }
    public void QuitToMenu()
    {
        GameManager.Instance.MainMenu();
    }

    public void Paused(bool state)
    {
        if (!state)
        {
            panel.enabled = true;
        }
        if (state)
        {
            panel.enabled = false;
        }
    }
}
