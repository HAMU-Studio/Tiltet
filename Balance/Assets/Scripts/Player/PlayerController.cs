using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    private Vector3 m_Velocity;
    private float m_moveSpeed;
    
     private Transform m_player;
    private Ray m_ray;
    private RaycastHit m_hit;
    private Quaternion m_rot;
    //地面の上なら歩きモーション、違うなら落下モーション 
    
    [Header("通常時移動速度")]
    [SerializeField] private float walkSpeed = 4f;
    [Header("ダッシュ時の速度")]
    [SerializeField] private float dashSpeed = 8f;
    [Header("落下速度の調整　-つける")]
    [SerializeField] float gravityPower = default!;
    [Header("トリガーの反応タイミング")]
    [SerializeField]　private float triggerTiming = 0.5f;         //トリガーがどこまで押し込まれたら反応するか 要調整 
    [Header("回転時間")]
    [SerializeField] private　float smoothTime = 0.3f;                //進行方向への回転にかかる時間
    [Header("ジャンプの強さ")]
    [SerializeField] private float jumpPower = 5f; 
    
    [FormerlySerializedAs("dashMaterial")]
    [Header("ダッシュ時のマテリアル")]
    [SerializeField] Material m_dashMaterial = default!;
    
    [SerializeField] private Material m_material_2P = default!;
    
    private Material m_defaultMaterial;
    [Header("ノックバックの強さ")]
    [SerializeField] private float knockBackP = 5f;              
    [Header("ノックバック時上方向の力")]
    [SerializeField] float knockBackUpP = 3f;            //ノックバック時少し上に浮かす

  
    
    //入力値
    private Vector2 m_inputMove;
    /*private float inputHorizontal;      //水平方向の入力値
    private float inputVertical;        //垂直方向の入力値*/
    private float m_inputTrigger_L;
    private float m_inputTrigger_R;
    
    //flag アニメーション実装したら減らしたい
    private bool isEnteredAttack;
    private bool isResetTrigger_R;
    private bool isResetTrigger_L;
    private bool isJumping = false;         
    private bool isFalling = false;
    private bool isAttacking = false;
    private bool isDashing = false;
    private bool canMove = true;
    // private bool onlyFirst = false;
    //private bool isKnockBack = false;
    
    
    private float targetRotation;   //回転に使う
    private float yVelocity = 0.0f;
    void Start()
    {
        m_player = GetComponent<Transform>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_defaultMaterial = GetComponent<Renderer>().material;
        m_moveSpeed = walkSpeed;
    }

    void Update()
    {
        //Input();
      //  Jump();
        //Attack(); //プロトタイプは現状攻撃なし
       
    }
    private void FixedUpdate()
    {
        Gravity();
        if (canMove) //攻撃中は移動もジャンプもできない->returnじゃなくてその場で固定させたい
        {
            Move(); 
            Dash();
        }
    }

    public void  PlayerMoveInput(InputAction.CallbackContext context)
    {
        //入力値の格納
        if (context.phase == InputActionPhase.Performed)
        {
            m_inputMove = context.ReadValue<Vector2>();
            Debug.Log("value = " + m_inputMove);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            m_inputMove = Vector2.zero;
        }
    }

    public void PlayerDashInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            m_inputTrigger_L = context.ReadValue<float>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            m_inputTrigger_L = 0;
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        //落下中と攻撃中はジャンプをさせない
        if (isJumping == true || isFalling == true) return;  

        
        if (context.phase == InputActionPhase.Started)
        {
            //移動中またはその場でジャンプした時の遷移
            
            //ジャンプする直前の加速度加えて慣性を表現
            m_Rigidbody.AddForce(m_Rigidbody.velocity.normalized, ForceMode.Impulse);
            
            //ジャンプ
            m_Rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
           // canMove = false;
            isJumping = true;
        }
    }

    private void Gravity()
    {
        //落下速度の調整用
        if (isJumping == true)
        {
            m_Rigidbody.AddForce(new Vector3(0, gravityPower, 0));
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //難しい方法はできないからTriggerで判定したい
        if (isJumping == true || isFalling == true)
        {
            if (collision.gameObject.CompareTag("Ground"))  //着地した時
            {
                isJumping = false;
                isFalling = false;
                canMove = true;
                //Debug.Log("toLanding" );
            }
        }
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            KnockBack(collision);
        }
    }

    private void Dash()
    {
        if (m_inputTrigger_L == 0 && isResetTrigger_L == false)
        {
            GetComponent<Renderer>().material = m_defaultMaterial;
            m_moveSpeed = walkSpeed;
            isResetTrigger_L = true;
            isDashing = false;
        }
        if (isAttacking == true || isDashing == true)
            return;

        if (m_inputTrigger_L  > triggerTiming)  
        {
            GetComponent<Renderer>().material = m_dashMaterial;
            m_moveSpeed = dashSpeed;
            isResetTrigger_L = false;
            isDashing = true;
        }
    }

    /// <summary>
    /// Rayを元に法線の計算をして滑らかに坂を上り下りできるように
    /// </summary>
    private Vector3 GetNormal(Vector3　moveForward)
    {
        //プレイヤーの真下方向にRayを飛ばす
        m_ray = new Ray(m_player.position, -transform.up);
        Physics.Raycast(m_ray, out m_hit, 2);
        //平面に投影したいベクトルmoveForwardとrayを飛ばして取得した平面の法線ベクトルから
        //平面に沿ったベクトルを計算
        return Vector3.ProjectOnPlane(moveForward, m_hit.normal);
    }

    void KnockBack(Collision collision)
    {
        isJumping = true;
        Debug.Log("isKnockBack");
        
        //加速度を反転して力を加える
        Vector3 direction = collision.rigidbody.velocity.normalized;
        direction.y = 0;
        m_Rigidbody.AddForce(direction * knockBackP, ForceMode.Impulse);      
        m_Rigidbody.AddForce(transform.up * knockBackUpP, ForceMode.Impulse);   //若干上方向にも飛ばす

    }

    
    /*public void fall()  //落下判定エリアで使う
    {
        isFall = true;
        canMove = false;

        //ここで操作不能にすればすれすれから復帰した時にジャンプができなくなることを防げそう
        //落下モーションへの遷移
    }*/

    private const float controlPower = 0.1f;
    void Move()
    {

        //プレイヤーの正面を基準に移動方向を決めるとぐるぐる回り続ける
        
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * m_inputMove.y + Camera.main.transform.right * m_inputMove.x;
     
        moveForward =  GetNormal(moveForward);

        //移動速度の計算
        //clampは値の範囲制限
        //GetAxisは0から1で入力値を管理する、斜め移動でWとAを同時押しすると
        //1以上の値が入ってくるからVector3.ClampMagnitudeメソッドを使って入力値を１に制限する(多分)
        var clampedInput = Vector3.ClampMagnitude(moveForward, 1f);  

        m_Velocity = clampedInput * m_moveSpeed;
        // transform.LookAt(m_Rigidbody.position + input); //キャラクターの向きを現在地＋入力値の方に向ける

        //Rigidbodyに一度力を加えると抵抗する力がない限りずっと力が加わる
        //AddForceに加える力をwalkSpeedで設定した速さ以上にはならないように
        //今入力から計算した速度から現在のRigidbodyの速度を引く
        m_Velocity = m_Velocity - m_Rigidbody.velocity;

        //　速度のXZを-walkSpeedとwalkSpeed内に収めて再設定
        m_Velocity = new Vector3(Mathf.Clamp(m_Velocity.x, -m_moveSpeed, m_moveSpeed), 0f, Mathf.Clamp(m_Velocity.z, -m_moveSpeed, m_moveSpeed));

      
        if (moveForward != Vector3.zero)
        {
            //SmoothDampAngleで滑らかな回転をするためには引数（moveForwardとvelocityだけ）をVector3からfloatに変換しなければいけない

            targetRotation = Mathf.Atan2(moveForward.x, moveForward.z) * Mathf.Rad2Deg;     //Atan2, ベクトルを角度(ラジアン)に変換する Rad2Deg(radian to degrees?)ラジアンから度に変換する

            //SmoothDampAngle(現在の値, 目的の値, ref 現在の速度, 遷移時間, 最高速度); 現在の速度はnullで良いっぽい？
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref yVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        
        // F・・・力  
        // m・・・質量  
        // a・・・加速度
        // Δt・・・力を加えた時間 (Time.fixedDeltatime) 
        //F = ｍ * a / Δt    Forceは力を加えた時間を使って計算
        if (MoveDuaringJump() == true)
        {
            
            //ジャンプ中スティックの入力値が基準以下なら力加えずに慣性を働かす。
            //入力値が大きいと力を十分の一にして加える->若干空中移動ができるように。
            m_Velocity = Vector3.Scale( m_Velocity, new Vector3(controlPower, controlPower, controlPower));
            m_Rigidbody.AddForce(m_Rigidbody.mass * m_Velocity / Time.fixedDeltaTime, ForceMode.Force);
        }
        else if (isJumping == false)
        {
            m_Rigidbody.AddForce(m_Rigidbody.mass * m_Velocity / Time.fixedDeltaTime, ForceMode.Force);
        }
       
    }

    private const float truncate = 0.5f;
    private bool MoveDuaringJump()
    {
        if (isJumping == true)
        {
            if (m_inputMove.y < -truncate || m_inputMove.y > truncate)
            {
                return true;
            }

            if (m_inputMove.x < -truncate || m_inputMove.x > truncate)
            {
                return true;
            }
        }
        return false;
    }
    
    

    public void Change2PColor(int index)
    {
        //inputManager側で2Pのカラー変更が上手くいかないのでプレイヤーのスクリプトで試みる
      
        if (index == 0)
        {
            m_defaultMaterial = m_material_2P;
            GetComponent<Renderer>().material = m_material_2P;
        }
    }
}