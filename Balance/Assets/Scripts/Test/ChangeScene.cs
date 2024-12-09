using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour
{
    public void ToGreenScene()
    {
        GameManager.instance.Restart();
        SoundManager.instance.Play("CursorDecision");
    }

    public void ToSavePoint()
    {
        GameManager.instance.RestartAtSavePoint();
        SoundManager.instance.Play("CursorDecision");
    }
}
