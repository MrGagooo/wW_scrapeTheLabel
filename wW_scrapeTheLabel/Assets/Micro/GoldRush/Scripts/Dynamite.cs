using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GoldRush
{
    public class Dynamite : MicroMonoBehaviour
    {
        public float speed = 0.1f;
        public Vector2 target;

        public GameManager gm;

        Rigidbody2D rigiBoy;
        public Vector2 velocity;
        public Vector3 mousePos;

        public GameObject rayonExplosion;
        bool isDestroying;

        private void Start()
        {
            rigiBoy = GetComponent<Rigidbody2D>();
            velocity = (target - rigiBoy.position).normalized;
        }

        private void Update()
        {
            if (Vector2.Distance(rigiBoy.position, target) < 0.03f)
            {
                gm.ExplodeDynamite(mousePos);
                rayonExplosion.SetActive(true);
                //Son et FX d'explosion ??
                Destroy(gameObject,Time.deltaTime);
                isDestroying = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<DetectionObjetIndestructible>() != null && !isDestroying)
            {
                Ray2D collisionRay = new Ray2D(transform.position, transform.position - collision.gameObject.transform.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, collision.gameObject.transform.position - transform.position, Vector2.Distance(transform.position, collision.gameObject.transform.position));
                
                Vector3 myNormal = hit.normal;

                float angle = Vector3.Angle(myNormal, Vector2.right);

                velocity = Vector2.Reflect(velocity, myNormal);

                gm.ReajustRay(this);
            }
            else if (collision.GetComponent<DetectionFilon>() != null && !isDestroying)
            {
                gm.ExplodeDynamite(mousePos);
                rayonExplosion.SetActive(true);
                Destroy(gameObject, Time.deltaTime);
                isDestroying = true;
            }
        }
        
        private void FixedUpdate()
        {
            rigiBoy.MovePosition(rigiBoy.position + velocity * speed * Time.fixedDeltaTime);
        }
    }
}
