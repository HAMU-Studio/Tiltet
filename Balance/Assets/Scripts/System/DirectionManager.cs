using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Foward,
    Back,
    Left,
    Right,
}
public class DirectionManager : MonoBehaviour
{

    /// <summary>
    /// 救出アクションの方向によって符号を変えないとバグが発生する
    /// </summary>
    public Direction direction;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
