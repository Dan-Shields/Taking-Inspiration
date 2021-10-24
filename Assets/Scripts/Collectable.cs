using UnityEngine;

namespace Collectables {
    public class Collectable : MonoBehaviour
    {
        public CollectableType type = CollectableType.Smoke;
    }

    public enum CollectableType
    {
        Smoke
    }
}

