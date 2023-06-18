using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField]private Button resume, options, menu;
    private bool panel;
    // Start is called before the first frame update

    private void Start()
    {
    }
    public void Resume()
    {
        GameManager.Instance.Resume();
    }
    public void Options()
    {
        GameManager.Instance.Options();
    }
    public void QuitToMenu()
    {
        GameManager.Instance.MainMenu();
    }
}
