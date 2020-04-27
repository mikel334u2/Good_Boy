using UnityEngine;
using System.Collections;

// Makes objects float up & down while gently spinning.
public class Floater : MonoBehaviour
{
    // User Inputs
    public float amplitude = 0.5f;
    public float frequency = 1f;

    private float phaseShift;
    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        // Store the starting position & rotation of the object
        offset = transform.position;
        phaseShift = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Float up/down with a Sin()
        float newY = Mathf.Sin((Time.time - phaseShift) * Mathf.PI * frequency) * amplitude + offset.y;
        transform.Translate(Vector3.up * (newY - offset.y), Space.World);
    }
}