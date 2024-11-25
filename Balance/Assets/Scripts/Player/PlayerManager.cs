using System.Collections;
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
    Fall,
    Wait,
    Move,
    Fly,
    SuperLand
}

public class PlayerManager : MonoBehaviour
{
    private Animator m_animator;
    private AnimatorStateInfo m_animState;
    
    private RescueState rescCurrentState;

    [Header("しっぽの付け根オブジェクト")]
    [SerializeField] private GameObject tailBase;
   // private PlayerAnimState animCurrenState;
    
    public RescueState rescState
    {
        set { rescCurrentState = value; }
        get { return rescCurrentState; }
    }
    
    public AnimatorStateInfo AnimState
    {
      //  set { m_animState = value; }

        get { return m_animState; }
        
    }

    public GameObject TailBase
    {
        get { return tailBase; }
    }

    private void Start()
    {
        m_beforeState = rescCurrentState;
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        m_animState = m_animator.GetCurrentAnimatorStateInfo(0);
    }

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

        if (m_beforeState == RescueState.Fall && rescCurrentState == RescueState.Wait)
        {
           SetJoint();
        }
       
        if (m_beforeState == RescueState.None && rescCurrentState == RescueState.Fall)
        {
            //落ちたら救出開始
            GameManager.instance.Rescue = true;
            m_animator.applyRootMotion = false;
            m_animator.StopRecording();
            m_animator.SetTrigger("toWire");
           // m_animator.applyRootMotion = false;
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
    private void SetJoint()
    {
        JointManager jointManager = GetComponent<JointManager>();
        jointManager.SetJointAndLine();
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
}
