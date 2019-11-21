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

    public GameObject sprMask;

    //Private


    private void OnGUI()
    {
        if (GameManager.Instance.debug)
        {
            text.text = hp.ToString();
        }
        else
        {
            text.text = "";
        }

        //100 hp = .2 Yscale && 0 Ypos
        //0 hp = 0 Yscale && -1.5Ypos
        sprMask.transform.localScale = new Vector3(sprMask.transform.localScale.x, Mathf.Clamp(hp / 500f, 0, .2f), sprMask.transform.localScale.z);
        sprMask.transform.position = new Vector3(sprMask.transform.position.x, (float)(0.015 * hp + -1.5), sprMask.transform.position.z);
    }

}
