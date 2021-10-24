using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Lights
{
    [RequireComponent(typeof(Light2D))]
    public class BlockedLight : MonoBehaviour
    {
        private Entities.PlayerController playerController;
        private Light2D Light;

        private bool inited = false;

        // Default this to Player, Level and LightBlocker layers
        private int linecastLayerFilter = (1 << 6) | (1 << 8) | (1 << 9);
        private RaycastHit2D[] linecastHits = new RaycastHit2D[1];

        void Awake()
        {
            this.Light = this.GetComponent<Light2D>();

            this.playerController = GameObject.FindObjectOfType<Entities.PlayerController>(false);

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
