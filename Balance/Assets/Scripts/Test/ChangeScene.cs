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
        GameManager.instance.SceneManager.FadeStart("MainStage");
        GameManager.instance.RestartAtSavePoint(true);
        GameManager.instance.CurrentState = GameState.Restart;
        SoundManager.instance.Play("CursorDecision");
        Debug.Log("State = " + GameManager.instance.CurrentState);
    }
}
