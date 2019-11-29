using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ScrapeTheLabel
{
    public class CursorDisplayer : MonoBehaviour
    {
        //Public
        public static CursorDisplayer _instance;
        public static CursorDisplayer Instance { get { return _instance; } }

        public SpriteRenderer sprRenderer;
        public Animator animator;

        public Sprite sprIdle;
        public Sprite sprScraping;
        public Sprite sprLose;
        public Sprite sprWin;


        //Private


        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

            sprRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();
        }

        private void OnGUI()
        {
            transform.position = new Vector3(GameManager.Instance.selectedLabel.transform.position.x,
                 (float)(0.055 * GameManager.Instance.selectedLabel.hp + -2.75),
                GameManager.Instance.selectedLabel.transform.position.z);

            if (GameManager.Instance.gameState == GameState.Victory)
            {
                sprRenderer.sprite = sprWin;
                animator.SetTrigger("Win");
            }
            else if (GameManager.Instance.gameState == GameState.Defeat)
            {
                sprRenderer.sprite = sprLose;
                animator.SetTrigger("Lose");
            }
            else
            {
                if (GameManager.Instance.playerIsScraping)
                {
                    sprRenderer.sprite = sprScraping;
                    animator.SetBool("IsTrapped", false);
                }
                else if (GameManager.Instance.playerTrapped)
                {
                    animator.SetBool("IsTrapped", true);
                }
                else
                {
                    sprRenderer.sprite = sprIdle;
                    animator.SetBool("IsTrapped", false);
                }
            }

        }
    }
}
