using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Entities;


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

            Vector2 playerLocalPos = this.playerController.transform.localPosition;

            foreach (Vector2 localPos in this.playerController.ColliderTestPositionsLocal)
            {
                Vector3 worldPos = this.playerController.transform.TransformPoint(localPos);

                if (this.CheckPlayerIsLitAtPoint(worldPos))
                {
                    this.playerController.OnDetection();
                    break;
                }
            }
        }

        bool CheckPlayerIsLitAtPoint(Vector3 point)
        {
            Vector2 deltaVector = point - this.transform.position;
            float angleToPoint = Vector2.Angle(this.transform.up, deltaVector);

            if (deltaVector.magnitude <= this.Light.pointLightInnerRadius && angleToPoint <= this.Light.pointLightInnerAngle / 2) {
                int hitCount = Physics2D.LinecastNonAlloc(this.transform.position, point, this.linecastHits, this.linecastLayerFilter);

                if (hitCount > 0 && this.linecastHits[0].transform.tag == "Player") return true;
            }

            return false;
        }
    }
}
