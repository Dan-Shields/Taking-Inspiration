using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Collectables;

namespace Entities {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : BaseEntity
    {
        [Header("References")]
        public Grid Grid;
        public GameObject SmokeThrowablePrefab;
        public Camera Camera;
        private Tilemap tilemap;

        [Header("Constants")]
        public float ThrowSpeedMultiplier = 1.0f;
        public float MaxThrowSpeed = 2.0f;
        public float IceGrip = 1.0f;

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

        private int smokeCount = 0;

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

                if (tile && tile.gameObject)
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
                    this.rigidbody.velocity += this.moveVector * Time.fixedDeltaTime * this.IceGrip;
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
            else if (collider.tag == "Checkpoint")
            {
                this.startPosition = new Vector3(collider.transform.position.x, collider.transform.position.y, this.transform.position.z);
            }
            else if (collider.tag == "Collectable")
            {
                Collectable collectable = collider.gameObject.GetComponent<Collectable>();

                if (collectable.type == CollectableType.Smoke)
                {
                    this.smokeCount++;
                    Destroy(collider.gameObject);
                }
            }
        }

        public void OnDetection()
        {
            this.transform.position = this.startPosition;
        }

        public void ThrowSmoke(InputAction.CallbackContext context) {
            if (context.started && this.SmokeThrowablePrefab && this.smokeCount > 0)
            {
                GameObject thrownSmoke = Instantiate(SmokeThrowablePrefab, this.transform.position, Quaternion.identity);
                Rigidbody2D smokeRb = thrownSmoke.GetComponent<Rigidbody2D>();

                if (smokeRb)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    Vector2 playerToMouseVector = this.Camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, this.transform.position.z)) - this.transform.position;
                    Vector2 initialVelocity = Vector2.ClampMagnitude(playerToMouseVector * this.ThrowSpeedMultiplier, this.MaxThrowSpeed);
                    smokeRb.velocity = initialVelocity;
                    smokeRb.angularVelocity = Random.Range(-360f, 360f);
                    this.smokeCount--;
                } else {
                    Destroy(thrownSmoke);
                }
            }
        }

        void OnGUI()
        {
            GUI.Label(new Rect(5, 5, 150, 50), $"Smokes: {this.smokeCount}");
        }
    }

    enum MoveDirection {
        Up, Down, Left, Right
    }

    enum MoveState {
        Idle,Roll,Walk,Attack
    }
}
