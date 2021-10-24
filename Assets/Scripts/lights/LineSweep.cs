using UnityEngine;

namespace Lights {
    public class LineSweep : MonoBehaviour
    {
        [Header("Constants")]
        public float sweepDuration = 5f;
        public Vector2 endpoint;

        private float startTime;
        private bool reversed = false;
        private Vector2 initialPos;

        void Awake()
        {
            this.startTime = Time.time;
            this.initialPos = this.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            float t = !this.reversed
                ? (Time.time - this.startTime) / this.sweepDuration
                : 1f - ((Time.time - this.startTime) / this.sweepDuration);

            if (!this.reversed && t >= 1f) {
                this.startTime = Time.time;
                this.reversed = true;
            } else if (this.reversed && t <= 0f) {
                this.startTime = Time.time;
                this.reversed = false;
            }

            transform.position = this.initialPos + this.endpoint * Mathf.SmoothStep(0.0f, 1.0f, t);
        }
    }
}
