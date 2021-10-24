using UnityEngine;

namespace Lights
{
    public class LightBase : MonoBehaviour
    {
        private Animation Animation;
        public float AnimSpeed = 1;

        void Awake()
        {
            this.Animation = this.GetComponent<Animation>();
            if (this.Animation)
            {
                foreach (AnimationState state in this.Animation)
                {
                    state.speed = this.AnimSpeed;
                }
            }

            this.OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}
