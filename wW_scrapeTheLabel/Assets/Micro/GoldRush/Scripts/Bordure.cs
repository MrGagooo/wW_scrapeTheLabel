using UnityEngine;

namespace Game.GoldRush
{
    public class Bordure : MicroMonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.GetComponent<Dynamite>()!=null)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}