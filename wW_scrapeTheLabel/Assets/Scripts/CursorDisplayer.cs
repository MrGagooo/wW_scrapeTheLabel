using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorDisplayer : MonoBehaviour
{
    //Public
    public SpriteRenderer sprRenderer;

    public Sprite sprIdle;
    public Sprite sprScraping;
    public Sprite sprLose;
    public Sprite sprWin;


    //Private


    private void Awake()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnGUI()
    {
        if(GameManager.Instance.playerIsScraping || GameManager.Instance.playerTrapped)
        {
            sprRenderer.sprite = sprScraping;
        }
        else
        {
            sprRenderer.sprite = sprIdle;
        }
    }
}
