using UnityEngine;

namespace Lights {
    public class ArcSweep : MonoBehaviour
    {
        [Header("Constants")]
        public float sweepAngle = 120f;
        public float sweepDuration = 5f;

        private float startTime;
        private bool reversed = false;
        private float initialRotation;

        void Awake()
        {
            this.startTime = Time.time;
            this.initialRotation = this.transform.rotation.eulerAngles.z;
        }

        // Update is called once per frame
        void Update()
        {
            float t = !this.reversed ? (Time.time - this.startTime) / this.sweepDuration : 1f - ((Time.time - this.startTime) / this.sweepDuration);

            if (!this.reversed && t >= 1f) {
                this.startTime = Time.time;
                this.reversed = true;
            } else if (this.reversed && t <= 0f) {
                this.startTime = Time.time;
                this.reversed = false;
            }

            transform.rotation = Quaternion.Euler(0, 0, Mathf.SmoothStep(-sweepAngle / 2, sweepAngle / 2, t) + this.initialRotation);
        }
    }
}
