using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameManager gameManager;

    void Update()
    {
        // if (!gameManager.isInputEnabled) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameManager.CanRollByInput())
            {
                gameManager.OnSpaceKey();  // ← ここで処理
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.OnRetryKey();
        }
    }

}
