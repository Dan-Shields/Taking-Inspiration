using UnityEngine;
using Entities;
using UnityEngine.Experimental.Rendering.Universal;


namespace Lights
{
    [RequireComponent(typeof(Light2D))]
    public class BlockedLight : LightBase
    {
        public PlayerController playerController;

        private bool inited = false;
        private Light2D Light;
        
        // Default this to Player and Level layers
        private int linecastLayerFilter = (1 << 6) | (1 << 8);
        private RaycastHit2D[] linecastHits = new RaycastHit2D[1];

        protected override void OnAwake()
        {
            this.Light = this.GetComponent<Light2D>();

            if (this.playerController && this.Light.lightType == Light2D.LightType.Point)
            {
                this.inited = true;
            }
        }


        void FixedUpdate()
        {
            if (!this.inited) return;

            Vector2 deltaVector = this.playerController.transform.position - this.transform.position;

            float angleToPlayer = Vector2.Angle(this.transform.up, deltaVector);

            //Debug.Log(angleToPlayer);
            //Debug.Log(deltaVector.magnitude);
            Debug.Log(this.Light.pointLightInnerRadius);
            Debug.Log(this.Light.pointLightInnerAngle);

            if (deltaVector.magnitude <= this.Light.pointLightInnerRadius && angleToPlayer <= this.Light.pointLightInnerAngle / 2) {
                int hitCount = Physics2D.LinecastNonAlloc(this.transform.position, this.playerController.transform.position, this.linecastHits, this.linecastLayerFilter);

                if (hitCount > 0 && this.linecastHits[0].transform.tag == "Player")
                {
                    this.playerController.OnDetection();
                }
            }
        }
    }
}
