using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 元はGamaManagerにstateを置いていたが、インスタンスが一つなのでエラーが多発したので移動。
/// ついでにアニメーション実装を楽にするためにPlayerControllerのフラグを減らしてこっちに構造体として移植したい。
/// HingeManagerの移植もありかも
/// </summary>

public enum RescueState
{
    //紐を無くすタイミングのためThrowingとFlyに分ける
    //飛ばす直前の位置に移動させるためMove追加
    None,
    Wait,
    Move,
    Fly,
    SuperLand
}

public enum PlayerAnimState
{
    None,
    Idle,
    Walk,
}

public class PlayerManager : MonoBehaviour
{
    private RescueState rescCurrentState;

   // private PlayerAnimState animCurrenState;
    
    public RescueState rescState
    {
        set { rescCurrentState = value; }
        get { return rescCurrentState; }
    }

    /*public PlayerAnimState AnimState
    {
        set { animCurrenState = value; }

        get { return animCurrenState; }
    }*/
    Animator animator;
    private void Start()
    {
        m_beforeState = rescCurrentState;
        animator = GetComponent<Animator>();
        if (GameManager.instance == null)
        {
            Debug.Log("GameManager is null");
            return;
        }
        GameManager.instance.SavePlayerInstance(gameObject);
       
    }

    private ThrowawayMethod method;
    private bool temp;
    private void FixedUpdate()
    {
        if (rescCurrentState != m_beforeState)
        {
            OnStateChange();
        }
    }
    
    private RescueState m_beforeState;
    private void OnStateChange()
    {
        // Debug.Log("state change " + m_beforeState + "->" + currentState);
        
        if (m_beforeState == RescueState.None && rescCurrentState == RescueState.Wait)
        {
            //落ちたら救出開始
            if (GameManager.instance.Rescue == false)
            {
                GameManager.instance.Rescue = true;
                SoundManager.instance.Play("Struggle");
                animator.SetTrigger("toStruggle");
            }
        }

        if (m_beforeState == RescueState.Wait && rescCurrentState == RescueState.Move
            || m_beforeState == RescueState.Wait && rescCurrentState == RescueState.Fly)
        {
            SoundManager.instance.StopPlay("Struggle");
            animator.Play("Wait_01");
        }

        if (m_beforeState == RescueState.Fly || m_beforeState == RescueState.SuperLand)
        {
            //着地したら救出終了
            if (rescCurrentState == RescueState.None)
            {
                GameManager.instance.Rescue = false;
            }
        }

        /*if (m_beforeState == RescueState.Fly && rescCurrentState == RescueState.SuperLand)
        {
            if (gameObject.layer ==  LayerMask.NameToLayer("Fly"))
            {
                SlipThroughOff();
            }
        }*/

        if (m_beforeState == RescueState.Move && rescCurrentState == RescueState.Fly)
        {
            //飛んだらレイヤーですり抜けon
            SlipThroughOn();
         
        }
        m_beforeState = rescCurrentState;
    }
    
    /// <summary>
    /// 救出アクション中進行不可能にならないようにLayerを変更して念のため貫通するように
    /// </summary>
    private void SlipThroughOn()
    {
        //NameToLayerは名前から数値への変換。本来Layerは数字
        StartCoroutine("AutoSlipThroughOff");
        gameObject.layer = LayerMask.NameToLayer("Fly");
        Debug.Log("Layer Change to " + LayerMask.LayerToName(gameObject.layer));
    }

    public void SlipThroughOff()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        Debug.Log("Layer Change to " + LayerMask.LayerToName(gameObject.layer));
    }
    
    [Header("飛び始めてから〇秒ですり抜け機能はoffに")] 
    [SerializeField] private float waitTime = 3f;
    private IEnumerator AutoSlipThroughOff()
    {
        yield return new WaitForSeconds(waitTime);
        //NameToLayerは名前から数値への変換。本来Layerは数字

        if (gameObject.layer == LayerMask.NameToLayer("Fly"))
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            Debug.Log("Layer Change to " + LayerMask.LayerToName(gameObject.layer));
        }
    }
    
    /// <summary>
    /// シーン切り替え時に呼ぶプレイヤーのリセット シーンのロード前とロード後に行う処理がある
    /// </summary>
    /// <returns></returns>
    public IEnumerator ResetPlayerState()
    {
       
        if (gameObject == null)
        {
            Debug.LogAssertion("this gameObject is null!");
            yield break;
        }

       // GetComponent<Rigidbody>().isKinematic = true;
        PlayerController _playerController = GetComponent<PlayerController>();
        _playerController.enabled = false;
        GameManager.instance.Rescue = false;
        rescState = RescueState.None;
        
        yield return new WaitForSeconds(1.7f);
        animator.Play("Wait_01");
       // GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<JointManager>().Reset();
        GameManager.instance.SetPlayerPos();
        _playerController.Initialize();
        _playerController.enabled = true;
        transform.rotation = quaternion.identity;
    }
  
}
