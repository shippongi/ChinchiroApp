using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public Rigidbody diceRb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }
    }

    void RollDice()
    {
        diceRb.velocity = Vector3.zero;
        diceRb.angularVelocity = Vector3.zero;

        diceRb.transform.position = new Vector3(0, 2, 0);
        diceRb.AddForce(Random.onUnitSphere * 5f, ForceMode.Impulse);
        diceRb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
    }
}
