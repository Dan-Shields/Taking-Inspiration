using UnityEngine;
using UnityEngine.U2D;

namespace Entities {
    public class BaseEntity : MonoBehaviour
    {
        [Header("References")]
        public SpriteAtlas Atlas;
        protected SpriteRenderer spriteRenderer;
        protected new Rigidbody2D rigidbody;


        [Header("Constants")]
        public string SpritePrefix = "";

        [Range(1, 20)]
        public float Speed = 1;

        protected int nextSpriteIndex = 0;
        protected Vector2 moveVector = new Vector2();

        void Start()
        {
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            this.rigidbody = GetComponent<Rigidbody2D>();
            InvokeRepeating("CycleSprite", 0, 0.2f);
        }

        // Update is called once per frame
        void Update()
        {
            Sprite sprite = this.GetNextSprite();
            this.spriteRenderer.sprite = sprite;
        }

        protected virtual void FixedUpdate()
        {
            Vector2 newPosition = this.rigidbody.position + (this.moveVector * Time.fixedDeltaTime);
            this.rigidbody.MovePosition(newPosition);
        }

        void CycleSprite() {
            this.nextSpriteIndex = this.nextSpriteIndex + 1;
        }

        protected virtual string GetNextSpriteName()
        {
            return this.nextSpriteIndex.ToString();
        }

        protected Sprite GetNextSprite(bool recurse = true) {
            string prefix = this.SpritePrefix == "" ? "" : $"{this.SpritePrefix}_";
            string spriteName = $"{prefix}{this.GetNextSpriteName()}";

            Sprite sprite = this.Atlas.GetSprite(spriteName);

            if (!sprite) {
                if (!recurse) return null;

                this.nextSpriteIndex = 1;
                return this.GetNextSprite(false);
            } else {
                return sprite;
            }
        }
    }
}
