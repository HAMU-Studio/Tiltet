using Dialogue;
using JetBrains.Annotations;
using Player;
using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerCondition condition;
    private void Awake()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager is null");
            return;
        }
        GameManager.instance.SavePlayerInstance(gameObject);

        m_RB = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerController>();

        if (GameManager.instance.isConnected == false && GameManager.instance.CurrentState == GameState.Search)
        {
            LockPos();
        }
    }
    
    /// <summary>
    /// 救出アクション中進行不可能にならないようにLayerを変更して念のため貫通するように
    /// </summary>
    public void SlipThroughOn()
    {
        //NameToLayerは名前から数値への変換。本来Layerは数字
        StartCoroutine("AutoSlipThroughOff");
        gameObject.layer = LayerMask.NameToLayer("Fly");
    }

    public void SlipThroughOff()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
    
    [Header("飛び始めてから〇秒ですり抜け機能はoffに")] 
    [SerializeField] private float waitTime = 3f;
    public IEnumerator AutoSlipThroughOff()
    {
        yield return new WaitForSeconds(waitTime);
        //NameToLayerは名前から数値への変換。本来Layerは数字

        if (gameObject.layer == LayerMask.NameToLayer("Fly"))
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }
    
    /// <summary>
    /// シーン切り替え時に呼ぶプレイヤーのリセット シーンのロード前とロード後に行う処理がある
    /// </summary>
    /// <returns></returns>
    private PlayerController _playerController;
    public void ResetPlayer_Unloaded()
    {
        if (gameObject == null)
        {
            Debug.LogAssertion("this gameObject is null!");
            return;
        }
        
        _playerController.enabled = false;
        GameManager.instance.IsRescue = false;
        condition.RescueState = State.None;
    }

    public void ResetPlayer_Loaded()
    {
        animator.Play("Wait_01");
       
        GetComponent<JointManager>().Reset();
        GameManager.instance.SetPlayerPos();
        _playerController.Initialize();
        _playerController.enabled = true;
        transform.rotation = quaternion.identity;
    }

    private Rigidbody m_RB;

    public void LockPos()
    {
        _playerController.ForceStop();
        GameManager.instance.ResetRBVelocity(m_RB);
        m_RB.isKinematic = true;
    }
 
    public void UnLockPos()
    {
        m_RB.isKinematic = false; 
    } 
    
    public bool IsLockPos() => m_RB.isKinematic;
}
