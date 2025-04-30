using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour
{
    public void ToGreenScene()
    {
        StartCoroutine(GameManager.instance.Restart());
        SoundManager.instance.Play("CursorDecision");
    }

    public void ToOpening()
    {
        GameManager.instance.SceneManager.StartTransition();
        GameManager.instance.CurrentState = GameState.None;
        SoundManager.instance.Play("CursorDecision");
    }
    

    public void ToSavePoint()
    {
        GameManager.instance.SceneManager.StartTransition("MainStage");
        GameManager.instance.RestartAtSavePoint(true);
        GameManager.instance.CurrentState = GameState.Restart;
        SoundManager.instance.Play("CursorDecision");
        Debug.Log("State = " + GameManager.instance.CurrentState);
    }
}
