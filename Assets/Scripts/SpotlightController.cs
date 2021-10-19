using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class SpotlightController : MonoBehaviour
{
    public float RotateSpeed = 1;
    private Animation Animation;

    void Awake()
    {
        this.Animation = this.GetComponent<Animation>();
        foreach (AnimationState state in this.Animation)
        {
            state.speed = this.RotateSpeed;
        }
    }
}
