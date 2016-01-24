using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SteeringAgent : MonoBehaviour
{
    public float MaxVelocity;
    public float MaxAngularVelocity;

    public Vector3 Velocity { get; private set; }
    public float AngularVelocity { get; private set; }
    
    void Start()
    {
        throw new NotImplementedException();
    }
    
    void FixedUpdate()
    {
        UpdateVelocities(Time.deltaTime);

        UpdatePosition(Time.deltaTime);
        UpdateRotation(Time.deltaTime);
    }

    public void ResetVelocities()
    {
        Velocity = Vector3.zero;
        AngularVelocity = 0f;
    }

    private void UpdateVelocities(float deltaTime)
    {
        throw new NotImplementedException();
    }

    private void UpdatePosition(float deltaTime)
    {
        throw new NotImplementedException();
    }

    private void UpdateRotation(float deltaTime)
    {
        if(Velocity.sqrMagnitude > 0f)
            transform.rotation = Quaternion.LookRotation(Velocity.normalized, Vector3.up);
    }
}
