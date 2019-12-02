using UnityEngine;

namespace Game.GoldRush
{
    public class RayonExplosion : MicroMonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject, 0.2f);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<DetectionFilon>()!=null)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, collision.gameObject.transform.position - transform.position, Vector2.Distance(transform.position, collision.gameObject.transform.position));
                if (hit.collider.GetComponent<DetectionFilon>() != null)
                {
                    collision.GetComponent<DetectionFilon>().DestroyFilon();
                }
            }
        }
    }
}