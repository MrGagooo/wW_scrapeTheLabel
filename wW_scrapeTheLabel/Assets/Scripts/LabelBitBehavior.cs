using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelBitBehavior : MonoBehaviour
{
    //Public
    public float hp = 100;
    public bool trapped = false;

    public Text text;

    //Private


    private void OnGUI()
    {
        text.text = hp.ToString();
    }

}
