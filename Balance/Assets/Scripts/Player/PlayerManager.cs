using System;
using System.Collections;
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
    OutsideMove,   
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
    private void Awake()
    {
        m_beforeState = rescCurrentState;
        animator = GetComponent<Animator>();
        if (GameManager.instance == null)
        {
            Debug.Log("GameManager is null");
            return;
        }
        GameManager.instance.SavePlayerInstance(gameObject);

        m_RB = GetComponent<Rigidbody>();

        if (GameManager.instance.isConnected == false)
        {
            LockPos();
        }
    }

    private void Update()
    {
        /*if (GameManager.instance.P1Spawn == false || GameManager.instance.P2Spawn == false)
        {
            GameManager.instance.SavePlayerInstance(gameObject);
            GameManager.instance.P1Spawn = true;
            GameManager.instance.P2Spawn = true;
        }*/
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
            if (GameManager.instance.IsRescue == false)
            {
                GameManager.instance.IsRescue = true;
                PlayStruggle();
            }
        }

        // Wait to Move or Wait to Fly
        if (m_beforeState == RescueState.Wait && rescCurrentState == RescueState.OutsideMove
            || m_beforeState == RescueState.Wait && rescCurrentState == RescueState.Fly)
        {
            SoundManager.instance.StopPlay("Struggle");
            animator.Play("Walk_01");

            if (rescCurrentState == RescueState.OutsideMove)
            {
                // 外側に飛ばす音
            }
            else
            {
                SoundManager.instance.Play("Fly");
            }
        }
        
        // Move to Fly
        if (m_beforeState == RescueState.OutsideMove && rescCurrentState == RescueState.Fly)
        {
            SoundManager.instance.Play("Fly");
            animator.Play("Walk_01");
        }

        if (m_beforeState == RescueState.Fly || m_beforeState == RescueState.SuperLand)
        {
            //着地したら救出終了
            if (rescCurrentState == RescueState.None)
            {
                GameManager.instance.IsRescue = false;
                
                if (m_beforeState == RescueState.Fly)   //通常着地
                {
                   ParticleManager.instance.GenerateAndPlay("Landing", this.transform);
                   SoundManager.instance.Play("NormalLanding");
                   animator.SetTrigger("toLand");
                   Debug.Log("NormalLanding");
                }
                else
                {
                    // スーパー着地
                    animator.SetTrigger("toLand");
                    ParticleManager.instance.GenerateAndPlay("Landing", this.transform);
                    SoundManager.instance.Play("SuperLanding");
                    Debug.Log("SuperLanding"); 
                    
                }
            }
        }

        if (m_beforeState == RescueState.Fly && rescCurrentState == RescueState.SuperLand)
        {
            animator.ResetTrigger(animator.name);
            animator.SetTrigger("toSuperLand"); 
        }

        if (m_beforeState == RescueState.OutsideMove && rescCurrentState == RescueState.Fly)
        {
            //飛んだらレイヤーですり抜けon
            SlipThroughOn();
         
        }
        m_beforeState = rescCurrentState;
    }

    public void PlayStruggle()
    {
        SoundManager.instance.Play("Struggle");
        animator.SetTrigger("toStruggle");
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
    private PlayerController _playerController;
    public void ResetPlayer_Unloaded()
    {
              
        if (gameObject == null)
        {
            Debug.LogAssertion("this gameObject is null!");
            return;
        }

        _playerController = GetComponent<PlayerController>();
        _playerController.enabled = false;
        GameManager.instance.IsRescue = false;
        rescState = RescueState.None;
    }

    public void ResetPlayer_Loaded()
    {
        animator.Play("Wait_01");
        // GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<JointManager>().Reset();
        GameManager.instance.SetPlayerPos();
        _playerController.Initialize();
        _playerController.enabled = true;
        transform.rotation = quaternion.identity;
    }

    private Rigidbody m_RB;

    public void LockPos()
    {
        GameManager.instance.ResetRBVelocity(m_RB);
        m_RB.isKinematic = true;
      //  Debug.Log("call Lock");
        //GameManager.instance.ResetRBVelocity(m_RB);
    }
 
    public void UnLockPos() => m_RB.isKinematic = false;
    
    public bool IsLockPos() => m_RB.isKinematic;
}
