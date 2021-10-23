using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkPath : MonoBehaviour
{
    public List<Vector2> points;

    private Vector2 direction;
    private int index = 0;
    private float segmentLength;

    void Start()
    {
        this.StartSegment();
    }

    Vector2 Point(int i)
    {
        return this.points[i % this.points.Count];

    }

    void StartSegment()
    {
        this.direction = (this.Point(this.index + 1) - this.Point(this.index)).normalized;
        this.segmentLength = (this.Point(this.index + 1) - this.Point(this.index)).magnitude;
        this.transform.position = (Vector3) this.Point(this.index);
    }

    void FixedUpdate()
    {
        float distance = (this.Point(this.index) - (Vector2) this.transform.position).magnitude;
        if (distance >= this.segmentLength)
        {
            this.index += 1;
            this.StartSegment();
        }
        this.transform.position += (Vector3) this.direction * 0.2f * Time.fixedDeltaTime;
    }
}
