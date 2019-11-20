using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int bpmDuration = 16;

    public float scrapeForce = 3.0f;
    public List<LabelBitBehavior> labelBits;

    public LabelBitBehavior selectedLabel;

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

        DamageProcess();
        SwitchSelectedLabel(); // If necessary
        InflictDamage();

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
        damageToInflict = Mathf.Clamp(scrollDeltaY * scrapeForce, Mathf.NegativeInfinity, 0);
    }

    private void InflictDamage()
    {
        selectedLabel.hp += damageToInflict;
    }
}
