using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : BaseEntity
    {
        private MoveState moveState = MoveState.Idle;
        private MoveDirection moveDirection = MoveDirection.Down;

        private Vector3 startPosition;

        void Awake()
        {
            this.startPosition = this.transform.position;
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
