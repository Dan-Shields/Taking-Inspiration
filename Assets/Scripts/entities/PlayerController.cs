using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using TMPro;
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
        public SpriteRenderer overlay;
        public TMP_Text text;
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

        public List<GameObject> page1 = new List<GameObject>();
        public List<GameObject> page2 = new List<GameObject>();

        private int currentPage = 1;
        private bool transitioning = false;

        private bool loaded = false;

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
            if (!loaded) return;
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
                StartCoroutine(Win());
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

        private IEnumerator Win()
        {
            Color col;
            for (int i=0; i<=100; i++)
            {
                col = text.color;
                col.a = i * 0.01f;
                text.color = col;

                col = overlay.color;
                col.a = i * 0.01f;
                overlay.color = col;

                yield return null;
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


    public void Continue(InputAction.CallbackContext context) {
        if (context.started && !this.transitioning) {
            StartCoroutine("Transition");
        }
    }

    IEnumerator Transition()
    {
        transitioning = true;

        List<GameObject> page;
        if (this.currentPage == 1) {
            page = this.page1;
        } else if (this.currentPage == 2) {
            page = this.page2;
        } else {
            yield break;
        }

        for (float ft = 1f; ft >= -0.01f; ft -= 0.1f)
        {
            foreach (GameObject obj in page) {
                TMP_Text textComp = obj.GetComponent<TMP_Text>();

                if (!textComp) continue;

                Color color = textComp.color;
                color.a = ft;

                textComp.color = color;
            }
            
            yield return new WaitForSeconds(.1f);
        }

        this.currentPage++;

        if (this.currentPage == 1) {
            page = this.page1;
        } else if (this.currentPage == 2) {
            page = this.page2;
        } else {
            this.loaded = true;
            yield break;
        }

        for (float ft = 0; ft <= 1.01f; ft += 0.1f)
        {
            foreach (GameObject obj in page) {
                TMP_Text textComp = obj.GetComponent<TMP_Text>();

                if (!textComp) continue;

                Color color = textComp.color;
                color.a = ft;

                textComp.color = color;
            }

            yield return new WaitForSeconds(.1f);
        }

        transitioning = false;
    }
    }

    enum MoveDirection {
        Up, Down, Left, Right
    }

    enum MoveState {
        Idle,Roll,Walk,Attack
    }
}
