using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorDisplayer : MonoBehaviour
{
    //Public
    public static CursorDisplayer _instance;
    public static CursorDisplayer Instance { get { return _instance; } }

    public SpriteRenderer sprRenderer;

    public Sprite sprIdle;
    public Sprite sprScraping;
    public Sprite sprLose;
    public Sprite sprWin;


    //Private


    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        sprRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnGUI()
    {
        transform.position =  new Vector3 (GameManager.Instance.selectedLabel.transform.position.x,
             (float)(0.03 * GameManager.Instance.selectedLabel.hp + -1.5),
            GameManager.Instance.selectedLabel.transform.position.z);

        if (GameManager.Instance.gameState == GameState.Victory)
        {
            sprRenderer.sprite = sprWin;
        }
        else if (GameManager.Instance.gameState == GameState.Defeat)
        {
            sprRenderer.sprite = sprLose;
        }
        else
        {
            if (GameManager.Instance.playerIsScraping || GameManager.Instance.playerTrapped)
            {
                sprRenderer.sprite = sprScraping;
            }
            else
            {
                sprRenderer.sprite = sprIdle;
            }
        }

    }
}
