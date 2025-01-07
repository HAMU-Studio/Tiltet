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
       if (isEncount)
       {
           _transition.FadeStart();
           GameManager.instance.CurrentState = GameState.EnemyBattle;
       }

       if (Input.GetKeyDown(KeyCode.E))
       {
           Encount();
       }
    }

    void Encount()
    {
        _transition.FadeStart("Fight");
        GameManager.instance.CurrentState = GameState.EnemyBattle;
    }
}
