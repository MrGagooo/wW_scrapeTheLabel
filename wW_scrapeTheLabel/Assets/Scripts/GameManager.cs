using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Instance
    public static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    //Public
    public enum difficulty
        {Low = 0,
         Medium = 1,
         Hard = 2}

    public int gameDuration = 16;
    public float beatDuration;

    public Text text;

    public float scrapeForce = 3.0f;
    public List<LabelBitBehavior> labelBits;

    public LabelBitBehavior selectedLabel;
    public bool playerTrapped = false;

    //Private
    private float scrollDeltaY;
    private float damageToInflict;


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

        selectedLabel = labelBits[0];
    }



    void Update()
    {
        scrollDeltaY = Input.mouseScrollDelta.y;
        text.text = damageToInflict.ToString();

        TrapTrigger();          // If necessary

        DamageProcess();
        SwitchSelectedLabel();  // If necessary
        InflictDamage();

    }


    private void TrapTrigger()
    {
        if (selectedLabel.trapped = true && selectedLabel.hp < 50f && !playerTrapped)
        {
            playerTrapped = true;
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
}
