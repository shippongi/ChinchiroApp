using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;

    public void TryRoll()
    {
        gameManager.StartPlayerTurn();
    }
}
