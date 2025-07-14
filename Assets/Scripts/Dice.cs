using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Transform topFace, bottomFace, frontFace, backFace, leftFace, rightFace;
    private Rigidbody rb;
    private Vector3 initialPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        initialPosition = transform.position;
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

    public int GetValueByY()
    {
        var faces = new (Transform t, int value)[]
        {
            (topFace, 2),
            (bottomFace, 5),
            (frontFace, 6),
            (backFace, 1),
            (leftFace, 3),
            (rightFace, 4)
        };

        float maxY = float.MinValue;
        int result = -1;
        foreach (var (t, value) in faces)
        {
            if (t.position.y > maxY)
            {
                maxY = t.position.y;
                result = value;
            }
        }
        return result;
    }

    public void ResetPosition()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.position += new Vector3(0, 0.01f, 0);
    }

}
