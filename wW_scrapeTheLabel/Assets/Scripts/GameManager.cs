using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instance
    public static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    //Public
    public enum difficultyEnum
        {Easy = 0,
         Medium = 1,
         Hard = 2}

    public difficultyEnum difficulty = difficultyEnum.Easy;
    public int gameDuration = 6;

    

    public GameState gameState = GameState.Ongoing;

    public bool debug;
    public Text text;

    public float scrapeForce = 3.0f;
    public List<LabelBitBehavior> labelBits;

    public LabelBitBehavior selectedLabel;
    public bool playerTrapped = false;

    public bool playerIsScraping = false;
    public bool scrapingEnabled = true;

    //Private
    private float scrollDeltaY;
    private float damageToInflict;
    private int actualGameDuration;


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

        SetupGame();
    }

    private void SetupGame()
    {
        selectedLabel = labelBits[0];

        TrapSetup();

        text.text = "";
        StartCoroutine(Timer());
    }

    private void TrapSetup()
    {
        if (difficulty == difficultyEnum.Medium)
        {
            var trap1 = Random.Range(0, 3);

            for(int i = 0; i < labelBits.Count - 1; i++)
            {
                if (i == trap1)
                {
                    labelBits[i].trapped = true;
                }
            }
        }
        else if (difficulty == difficultyEnum.Hard)
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
        if (damageToInflict < 0)
        {
            playerIsScraping = true;
        }
        else
        {
            playerIsScraping = false;
        }
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
                playerTrapped = false;
                selectedLabel.trapped = false;
            }
        }
    }

    private void CheckWin()
    {
        if (labelBits.Count == 1 && selectedLabel.hp <= 0)
        {
            EndGame(true);
        }
    }


    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(gameDuration);

        EndGame(false);
    }

    private void EndGame(bool victory)
    {
        scrapingEnabled = false;
        StopCoroutine(Timer());
        StopAllCoroutines();
        if (victory)
        {
            gameState = GameState.Victory;
            text.color = Color.green;
            text.text = "jéjé";
        }
        else
        {
            gameState = GameState.Defeat;
            text.color = Color.red;
            text.text = "mek t con c fou";
        }

        StartCoroutine(StopGame());
    }

    private IEnumerator StopGame()
    {
        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

public enum GameState
{
    Ongoing = 0,
    Victory = 1,
    Defeat = 2
}
