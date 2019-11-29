using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScrapeTheLabel
{
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
            sprMask.transform.localScale = new Vector3(sprMask.transform.localScale.x, Mathf.Clamp((float)(0.0041 * hp), 0, .41f), sprMask.transform.localScale.z);
            sprMask.transform.position = new Vector3(sprMask.transform.position.x, (float)(0.0275 * hp + -2.75), sprMask.transform.position.z);
        }

    }
}
