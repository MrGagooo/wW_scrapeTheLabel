using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Game.ScrapeTheLabel
{
    public class GameManager : MicroMonoBehaviour
    {
        //Instance
        public static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

        //Public
        public enum difficultyEnum
        {
            Easy = 0,
            Medium = 1,
            Hard = 2
        }

        public difficultyEnum difficulty = difficultyEnum.Easy;
        public int gameDuration = 6;



        public GameState gameState = GameState.Ongoing;

        public Text text;

        public float scrapeForce = 3.0f;
        public List<LabelBitBehavior> labelBits;

        public LabelBitBehavior selectedLabel;
        public bool playerTrapped = false;

        public bool playerIsScraping = false;
        public bool scrapingEnabled = true;

        public GameObject trap;
        public TutorialArrow tutorialArrow;

        //Private
        private float scrollDeltaY;
        private float damageToInflict;
        private int actualGameDuration;

        private float trappedTutoTimer = 0.5f;
        private float timer;

        private float scrapingTimer;
        private bool wasScrapingLastFrame;


        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

            SetupGame();

            Macro.StartGame();
        }

        private void SetupGame()
        {
            selectedLabel = labelBits[0];

            TrapSetup();
        }

        private void TrapSetup()
        {
            if (Macro.Difficulty == 2)
            {
                var trap1 = Random.Range(0, 3);

                for (int i = 0; i < labelBits.Count - 1; i++)
                {
                    if (i == trap1)
                    {
                        labelBits[i].trapped = true;
                    }
                }
            }
            else if (Macro.Difficulty == 3)
            {
                var trap1 = Random.Range(0, 3);
                var trap2 = Random.Range(0, 3);
                while (trap1 == trap2)
                {
                    trap2 = Random.Range(0, 3);
                }

                for (int i = 0; i < labelBits.Count - 1; i++)
                {
                    if (i == trap1 || i == trap2)
                    {
                        labelBits[i].trapped = true;
                    }
                }
            }
        }

        protected override void OnGameStart()
        {
            if (Macro.BPM == 96)
            {
                Macro.StartTimer(5.0f, false);
            }
            else if (Macro.BPM == 120)
            {
                Macro.StartTimer(4.0f, false);
            }
            else if (Macro.BPM == 160)
            {
                Macro.StartTimer(3.0f, false);
            }
            else if (Macro.BPM == 180)
            {
                Macro.StartTimer(2.0f, false);
            }
            else
            {
                Macro.StartTimer(5.0f, false);
            }

            SoundManager.Instance.StartMusic();
            Macro.DisplayActionVerb("Scrape!", 1);
        }

        void Update()
        {
            if (scrapingEnabled)
            {
                scrollDeltaY = Input.mouseScrollDelta.y;
            }
            else
            {
                scrollDeltaY = 0;
            }

            TutorialSpawn();        // If necessary

            TrapTrigger();          // If necessary

            DamageProcess();
            SwitchSelectedLabel();  // If necessary
            InflictDamage();

            CheckWin();

        }


        private void TrapTrigger()
        {
            if (selectedLabel.trapped && selectedLabel.hp <= 50f && !playerTrapped)
            {
                playerTrapped = true;
                selectedLabel.hp = 50;
                Instantiate(trap, selectedLabel.transform.position, Quaternion.identity);
            }
        }

        private void SwitchSelectedLabel()
        {
            if (selectedLabel.hp <= 0 && damageToInflict == 0)
            {
                if (labelBits.Count != 1)
                {
                    labelBits.RemoveAt(0);
                    selectedLabel = labelBits[0];
                }
            }
        }

        private void DamageProcess()
        {
            if (!playerTrapped)
            {
                damageToInflict = Mathf.Clamp(scrollDeltaY * scrapeForce, Mathf.NegativeInfinity, 0);
            }
            else
            {
                damageToInflict = Mathf.Clamp(scrollDeltaY * scrapeForce, 0, Mathf.Infinity);
            }

            scrapingTimer += Time.deltaTime;

            if (damageToInflict < 0)
            {
                playerIsScraping = true;
                scrapingTimer = 0;
            }
            else
            {
                if (scrapingTimer >= 0.1f)
                {
                    playerIsScraping = false;
                    SoundManager.Instance.StopScrapingSound();
                }
            }

            if (playerIsScraping && !wasScrapingLastFrame)
            {
                SoundManager.Instance.PlayScrapingSound();
            }

            wasScrapingLastFrame = playerIsScraping;
        }

        private void InflictDamage()
        {
            if (!playerTrapped)
            {
                selectedLabel.hp += damageToInflict;
            }
            else
            {
                if (damageToInflict > 0)
                {
                    Untrap();
                }
            }
        }

        private void CheckWin()
        {
            if (labelBits.Count == 1 && selectedLabel.hp <= 0 && gameState == GameState.Ongoing)
            {
                EndGame(true);
            }
        }

        protected override void OnTimerEnd()
        {
            EndGame(false);
        }

        private void EndGame(bool victory)
        {
            Untrap();
            scrapingEnabled = false;
            if (victory)
            {
                gameState = GameState.Victory;
                Macro.Win();
            }
            else
            {
                gameState = GameState.Defeat;
                Macro.Lose();
            }

            StartCoroutine(StopGame());
        }

        private IEnumerator StopGame()
        {
            yield return new WaitForSeconds(2);

            Macro.EndGame();
        }

        private void Untrap()
        {
            playerTrapped = false;
            selectedLabel.trapped = false;
            Destroy(GameObject.Find("paperTrap(Clone)"));
            SoundManager.Instance.PlayUntrapSound();
        }

        private void TutorialSpawn()
        {
            if (playerTrapped)
            {
                timer += Time.deltaTime;

                if (timer >= trappedTutoTimer)
                {
                    tutorialArrow.tutEnabled = true;
                }
            }
            else
            {
                timer = 0;
                tutorialArrow.tutEnabled = false;
            }    
        }
    }

    public enum GameState
    {
        Ongoing = 0,
        Victory = 1,
        Defeat = 2
    }
}
