using UnityEngine;

namespace Game.GoldRush
{
    public class DetectionFilon : MicroMonoBehaviour
    {
        public GameManager gm;
        public int placeTab;

        public void DestroyFilon()
        {
            gm.etatFilonOr[placeTab] = false;
            Destroy(gameObject);
        }

    }
}