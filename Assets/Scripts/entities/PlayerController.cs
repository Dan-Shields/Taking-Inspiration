using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Entities {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : BaseEntity
    {
        private MoveState moveState = MoveState.Idle;
        private MoveDirection moveDirection = MoveDirection.Down;

        private Vector3 startPosition;

        private List<Vector2> colliderTestPositionsLocal = new List<Vector2>();
        public List<Vector2> ColliderTestPositionsLocal
        {
            get => colliderTestPositionsLocal;
        }

        private bool sliding = false;
        private bool justStartedSliding = false;

        public Grid Grid;
        private Tilemap tilemap;

        void Awake()
        {
            this.startPosition = this.transform.position;

            BoxCollider2D collider = this.GetComponent<BoxCollider2D>();

            if (collider) {
                this.colliderTestPositionsLocal.Add(collider.offset);
                this.colliderTestPositionsLocal.Add(collider.offset + new Vector2( collider.size.x,  collider.size.y) / 2);
                this.colliderTestPositionsLocal.Add(collider.offset + new Vector2(-collider.size.x,  collider.size.y) / 2);
                this.colliderTestPositionsLocal.Add(collider.offset + new Vector2( collider.size.x, -collider.size.y) / 2);
                this.colliderTestPositionsLocal.Add(collider.offset + new Vector2(-collider.size.x, -collider.size.y) / 2);
            }

            if (this.Grid)
            {
                this.tilemap = this.Grid.transform.GetComponentInChildren<Tilemap>();
            }
        }

        protected override void FixedUpdate()
        {
            
            if (this.tilemap)
            {
                Tile tile = this.tilemap.GetTile<Tile>(this.Grid.WorldToCell(this.transform.position));

                if (tile.gameObject)
                {
                    this.justStartedSliding = tile.gameObject.tag == "SlippyFloor" && !this.sliding;
                    this.sliding = tile.gameObject.tag == "SlippyFloor";
                } else {
                    this.sliding = this.justStartedSliding = false;
                }
            }

            if (this.sliding) {
                if (this.justStartedSliding)
                {
                    this.rigidbody.velocity = this.moveVector;
                } else {
                    this.rigidbody.velocity += this.moveVector * Time.fixedDeltaTime;
                }
            } else {
                Vector2 newPosition = this.rigidbody.position + (this.moveVector * Time.fixedDeltaTime);
                this.rigidbody.MovePosition(newPosition);
            }
        }

        protected override string GetNextSpriteName() {
            string direction = this.moveDirection.ToString().ToLower();
            string state = this.moveState.ToString().ToLower();

            return $"{state} {direction}{this.nextSpriteIndex}".ToString();
        }

        public void Move(InputAction.CallbackContext context) {
            if (context.canceled) {
                this.moveVector = new Vector2();
                this.moveState = MoveState.Idle;
            } else {
                this.moveVector = context.ReadValue<Vector2>() * this.Speed;

                if (this.moveVector.x < 0) {
                    this.moveDirection = MoveDirection.Left;
                } else if (this.moveVector.x > 0) {
                    this.moveDirection = MoveDirection.Right;
                } else if (this.moveVector.y < 0) {
                    this.moveDirection = MoveDirection.Down;
                } else if (this.moveVector.y > 0) {
                    this.moveDirection = MoveDirection.Up;
                }

                this.moveState = MoveState.Walk;
            }
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag == "Detector")
            {
                this.OnDetection();
            }
            else if (collider.tag == "Finish")
            {
                Debug.Log("Sorry Mario - the princess is in another castle!");
            }
        }

        public void OnDetection()
        {
            this.transform.position = this.startPosition;
        }
    }

    enum MoveDirection {
        Up, Down, Left, Right
    }

    enum MoveState {
        Idle,Roll,Walk,Attack
    }
}
