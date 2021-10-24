using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SmokeThrow : MonoBehaviour
{
    [Header("References")]
    public GameObject activeSmoke;

    private new Rigidbody2D rigidbody;

    void Awake()
    {
        this.rigidbody = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.rigidbody && this.rigidbody.velocity.magnitude < 0.1f && this.activeSmoke)
        {
            this.Invoke("Activate",  0.2f);
        }
    }

    void Activate()
    {
        Instantiate(activeSmoke, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }
}
