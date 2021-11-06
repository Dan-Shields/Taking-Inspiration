using UnityEngine;

namespace Lights {
    public class FullRotate : MonoBehaviour
    {
        [Header("Constants")]
        public float rotationPeriod = 5f;
        public bool rotateClockwise = false;

        private float startTime;
        private float initialRotation;

        void Awake()
        {
            this.startTime = Time.time;
            this.initialRotation = this.transform.rotation.eulerAngles.z;
        }

        // Update is called once per frame
        void Update()
        {
            float t = (Time.time - this.startTime) / this.rotationPeriod;
            t = this.rotateClockwise ? 1f - t : t;

            if (t >= 1f) {
                this.startTime = Time.time;
            }
            
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0f, 360f, t) + this.initialRotation);
        }
    }
}
