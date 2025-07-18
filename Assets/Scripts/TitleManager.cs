using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void OnStartGameButtonPressed()
    {
        SceneManager.LoadScene("ChinchiroGameScene");
    }
    public void OnStartExtraGameButtonPressed()
    {
        SceneManager.LoadScene("ExtraGameScene");
    }
}
