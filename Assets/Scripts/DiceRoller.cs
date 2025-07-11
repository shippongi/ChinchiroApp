using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceRoller : MonoBehaviour
{
    public Transform topFace;
    public Transform bottomFace;
    public Transform frontFace;
    public Transform backFace;
    public Transform leftFace;
    public Transform rightFace;

    public TMP_Text resultText;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void RollDice()
    {
        StartCoroutine(RollAndCheck());
    }

    IEnumerator RollAndCheck()
    {
        Roll(); // 実際に転がす

        yield return new WaitForSeconds(2.0f); // 転がり待ち（調整可能）

        int result = GetDiceValueByY();
        Debug.Log("出目：" + result);

        if (resultText != null)
        {
            resultText.text = "出目：" + result.ToString();
        }

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

    // public int GetDiceValue()
    // {
    //     float maxDot = -1f;
    //     int result = -1;

    //     var faceNormals = new (Transform face, int value)[]
    //     {
    //         (topFace, 2),
    //         (bottomFace, 5),
    //         (frontFace, 6),
    //         (backFace, 1),
    //         (leftFace, 3),
    //         (rightFace, 4)
    //     };

    //     foreach (var (face, value) in faceNormals)
    //     {
    //         float dot = Vector3.Dot(face.up, Vector3.up);
    //         if (dot > maxDot)
    //         {
    //             maxDot = dot;
    //             result = value;
    //         }
    //     }

    //     return result;
    // }

    int GetDiceValueByY()
    {
        Dictionary<Transform, int> faceValues = new Dictionary<Transform, int>()
        {
            { topFace, 2 },
            { bottomFace, 5 },
            { frontFace, 6 },
            { backFace, 1 },
            { leftFace, 3 },
            { rightFace, 4 },
        };

        Transform highestFace = null;
        float highestY = float.MinValue;

        foreach (var pair in faceValues)
        {
            float y = pair.Key.position.y;
            if (y > highestY)
            {
                highestY = y;
                highestFace = pair.Key;
            }
        }

        return faceValues[highestFace];
    }
}
