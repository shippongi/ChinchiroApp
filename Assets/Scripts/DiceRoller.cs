using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public Transform topFace, bottomFace, frontFace, backFace, leftFace, rightFace;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Roll()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = new Vector3(0, 2, 0);

        Vector3 randomDir = Random.onUnitSphere;
        float force = Random.Range(5f, 10f);
        rb.AddForce(Vector3.up * 5f + randomDir * force, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * force, ForceMode.Impulse);
    }

    public int GetTopFaceValue()
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

        Transform maxT = null;
        float maxY = float.MinValue;
        int result = -1;

        foreach (var (t, value) in faces)
        {
            if (t.position.y > maxY)
            {
                maxY = t.position.y;
                result = value;
                maxT = t;
            }
        }

        return result;
    } 
}
