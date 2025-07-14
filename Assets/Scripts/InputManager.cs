using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameManager gameManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameManager.CanRollByInput())
            {
                gameManager.OnSpaceKey();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.OnRetryKey();
        }
    }

}
