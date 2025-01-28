using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FadeSystem;

public class EncountManager : MonoBehaviour
{
    public bool isEncount {  get; set; }
    [SerializeField] private FadeAndSceneTransition _transition;

    void Start()
    {
        isEncount = false;
    }
 
    void Update()
    {
       /*if (isEncount)
       {
           _transition.FadeStart();
           GameManager.instance.CurrentState = GameState.EnemyBattle;
       }*/

       if (Input.GetKeyDown(KeyCode.E))
       {
          // Encount();
       }
    }

    public void Encount(string sceneName)
    {
        _transition.FadeStart(sceneName);
        GameManager.instance.CurrentState = GameState.EnemyBattle;
        isEncount = true;
    }
}
