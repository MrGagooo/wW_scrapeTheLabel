using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ScrapeTheLabel
{
    public class TutorialArrow : MonoBehaviour
    {
        public bool tutEnabled;

        //Private
        [SerializeField]
        private Transform cursorTransform;
        [SerializeField]
        private SpriteRenderer sprRenderer;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }


        public void Update()
        {
            transform.position = cursorTransform.position;

            if (tutEnabled)
            {
                sprRenderer.color = new Color(sprRenderer.color.r, sprRenderer.color.g, sprRenderer.color.b, 1f);
                animator.SetBool("Active", true);
            }
            else
            {
                sprRenderer.color = new Color(sprRenderer.color.r, sprRenderer.color.g, sprRenderer.color.b, 0f);
                animator.SetBool("Active", false);
            }
        }
    }
}
