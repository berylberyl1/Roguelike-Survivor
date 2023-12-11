using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frequency; // Speed of sine movement
    public float magnitude; // Size of sine movement
    public Vector3 direction; // Direction of sine movement
    Vector3 initialPosition; // Starting position of the object

    void Start()
    {
        // Store the starting position & rotation of the object
        initialPosition = transform.position;
    }

    private void Update()
    {
        transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude;
    }
}
