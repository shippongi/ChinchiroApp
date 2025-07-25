using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Transform topFace, bottomFace, frontFace, backFace, leftFace, rightFace;

    private Rigidbody rb;
    private Vector3 initialPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 3f;
        rb.drag = 0.5f;
        rb.angularDrag = 0.8f;
    }

    public void SetInitialPosition(Vector3 pos)
    {
        initialPosition = pos;
        transform.position = initialPosition;
    }

    public void Roll()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = initialPosition + new Vector3(
            Random.Range(-0.2f, 0.2f),
            2f,
            Random.Range(-0.2f, 0.2f)
        );

        Vector3 randomDir = Random.onUnitSphere;
        float force = Random.Range(5f, 10f);
        rb.AddForce(Vector3.up * 5f + randomDir * force, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * force, ForceMode.Impulse);
    }

    public bool IsStopped(float threshold = 0.05f)
    {
        return rb.velocity.magnitude < threshold && rb.angularVelocity.magnitude < threshold;
    }

    public int GetValue()
    {
        var faces = new (Transform t, int value)[]
        {
            (topFace, 2), (bottomFace, 5), (frontFace, 6),
            (backFace, 1), (leftFace, 3), (rightFace, 4)
        };

        return faces.OrderByDescending(f => f.t.position.y).First().value;
    }

    public void ResetPosition()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition + new Vector3(0, 0.01f, 0);
    }
}

