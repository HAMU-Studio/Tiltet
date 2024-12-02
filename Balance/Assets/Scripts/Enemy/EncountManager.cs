using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FadeSystem;

public class EncountManager : MonoBehaviour
{
    public bool isEncount {  get; set; }
    [SerializeField] private FadeAndSceneTransition _transition;

    // Start is called before the first frame update
    void Start()
    {
        isEncount = false;
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log(isEncount);
       if (isEncount)
       {
           _transition.FadeStart();
           GameManager.instance.CurrentState = GameState.EnemyBattle;
       }
    }
}
