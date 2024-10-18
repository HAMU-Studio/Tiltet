using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private PlayerManager m_PM;
    
    private Rigidbody m_RB;
    private Vector3 m_Velocity;
    private float m_moveSpeed;
    
    private Transform m_player;
    private Ray m_ray;
    private RaycastHit m_hit;
    private Quaternion m_rot;
   
    //地面の上なら歩きモーション、違うなら落下モーション 
    
    [Header("通常時移動速度")]
    public float walkSpeed = 4f;
    [Header("ダッシュ時の速度")]
    public float dashSpeed = 8f;
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
    private float m_inputTrigger_L;
    private float m_inputTrigger_R;
    
    //flag アニメーション実装したら減らしたい、enum使えばもっといろいろ楽そう
    private bool isEnteredAttack;
    private bool isResetTrigger_R;
    private bool isResetTrigger_L;
    private bool isJumping = false;         
    private bool isKnockBack = false;
    private bool isAttacking = false;
    private bool isDashing = false;
    private bool canMove = true;
    
    private float targetRotation;   //回転に使う
    private float yVelocity = 0.0f;
    void Start()
    {
        m_player = GetComponent<Transform>();
        m_RB = GetComponent<Rigidbody>();
        m_defaultMaterial = GetComponent<Renderer>().material;
        m_moveSpeed = walkSpeed;
        canRescueAct = false;
        isChanged = false;

        m_PM = GetComponent<PlayerManager>();
    }

    private float elapsedTime;
    [Header("ノックバックされてから動けるようになるまでの時間")]
    [SerializeField] private float canMoveTime = 0.5f; 
    void Update()
    {
        if (isKnockBack && canMove == false)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= canMoveTime)
            {
                //移動不能だけ解除、低減した重力は着地までそのまま
                canMove = true;
                elapsedTime = 0;
            }
        }

        if (isFleezing)
        {
            //PlayerFreeze();
        }

        if (m_PM.State == RescueState.Fly && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            //スーパー着地
            //   Debug.Log("Call 1");
            SuperLanding();
        }

    }
    private void FixedUpdate()
    {
        Gravity();
        if (canMove) 
        {
            MoveCalc(); 
            if (isJumping || isKnockBack)
            { 
                AirMovement();
            }
            else
            {
                NormalMovement();
            }
           
            DashSwitch();
        }
    }

    public void  PlayerMoveInput(InputAction.CallbackContext context)
    {
        //入力値の格納
        if (context.phase == InputActionPhase.Performed)
        {
            m_inputMove = context.ReadValue<Vector2>();
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
        if (isJumping|| canMove == false || isKnockBack) return;  

        
        if (context.phase == InputActionPhase.Started)
        {
            //移動中またはその場でジャンプした時の遷移
            
            //ジャンプする直前の加速度加えて慣性を表現
            m_RB.AddForce(m_RB.velocity.normalized, ForceMode.Impulse);
            
            //ジャンプ
            m_RB.AddForce(transform.up * jumpPower, ForceMode.Impulse);
           // canMove = false;
            isJumping = true;
        }
    }

    private GameObject m_rescueCube;
    private bool canRescueAct;
    public void RescueActionInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {

            if (m_PM.State == RescueState.Fly)
            {
                //スーパー着地
             //   Debug.Log("Call 1");
                SuperLanding();
                m_PM.State = RescueState.SuperLand;
            }
            if (canRescueAct)
            {
                m_rescueCube.GetComponent<Rescue>().StartRescue();
                canRescueAct = false;
            }
            // CMSwitchを探し、プレイヤーが接触しているか確認する処理
            CMSwitch[] switches = FindObjectsOfType<CMSwitch>();
            foreach (var cmswitch in switches)
            {
                if (cmswitch.IsPlayerInContact())
                {
                    cmswitch.SetSwitchPressed(true); // スイッチを押す
                }
            }
        }
    }
    [SerializeField] private Vector3 scalePow;
    private void SuperLanding()
    {
        m_RB.velocity = Vector3.zero;
        m_RB.angularVelocity = Vector3.zero;
        
        m_RB.AddForce(Vector3.Scale(Vector3.down, scalePow), ForceMode.Impulse);
       // Debug.Log("call 2");
    }

    private void Gravity()
    {   //落下速度の調整用
       
        //ジャンプ中のみ重力 -> 常に重力でノックバック時のみ低減 ->救出アクション中は重力なし
        if (canMove == false || m_PM.State != RescueState.None)
            return;
        
        if (isKnockBack == false)
        {
            m_RB.AddForce(new Vector3(0, gravityPower, 0));
        }
        else
        {
            //ノックバック時はふんわり落下
            m_RB.AddForce(new Vector3(0, gravityPower * 0.5f, 0));
        }
    }

    //その場で固定するかどうか。-> 振り子。救出アクション待機でつかう。
    private bool isFleezing = false;
    public void ChangePlayerState(bool isFleezing)
    {
        if (isFleezing)
        {
            canMove = false;
            this.isFleezing = true;
        }
        else
        {
            this.isFleezing = false;
        }
    }
        
    private bool isChanged;
    private void OnCollisionEnter(Collision collision)
    {
        if (isJumping|| isKnockBack || canMove == false)
        {
            if (collision.gameObject.CompareTag("Ground"))  
            {
                isJumping = false;
                isKnockBack = false;
                canMove = true;
                //Debug.Log("toLanding" );
                
                if (m_PM.State == RescueState.Fly ||
                    m_PM.State == RescueState.SuperLand)
                {
                    m_PM.State = RescueState.None;
                 //   Debug.Log("pm = " + m_PM.State);
                }
            }
        }
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            KnockBack(collision);
        }
        
        if (isChanged)
            return;
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            //collision.gameObject.GetComponent<StageManager>().SetToStageChild(gameObject);
            isChanged = true;
        }
    }

    /// <summary>
    /// 救出可能エリアにいるか
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RescueArea"))
        {
            canRescueAct = true;
            m_rescueCube = other.gameObject;
        }
    }

    private void DashSwitch()
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
        isKnockBack = true;
        canMove = false;
        
        //プレイヤーの場所 - 敵の場所をして得た進行方向を正規化
        Vector3 direction = (transform.position - collision.gameObject.transform.position).normalized;
        direction.y = 0;
        m_RB.AddForce(direction * knockBackP, ForceMode.Impulse);      
        m_RB.AddForce(transform.up * knockBackUpP, ForceMode.Impulse);   //若干上方向にも飛ばす

    }

    private const float controlPower = 0.1f;
    void MoveCalc()
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
        m_Velocity = m_Velocity - m_RB.velocity;

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
    }
    
    //AddForceの部分を通常移動と空中移動で分けた
    private void NormalMovement()
    {
        // F・・・力  
        // m・・・質量  
        // a・・・加速度
        // Δt・・・力を加えた時間 (Time.fixedDeltatime) 
        //F = ｍ * a / Δt    Forceは力を加えた時間を使って計算
      
        if (isJumping == false && isKnockBack == false)
        {
            m_RB.AddForce(m_RB.mass * m_Velocity / Time.fixedDeltaTime, ForceMode.Force);
        }
    }

    private void AirMovement()
    {
        if (MoveDuaringAir())
        {
            //ジャンプ中スティックの入力値が基準以下なら力加えずに慣性を働かす。
            //入力値が大きいと力を十分の一にして加える->若干空中移動ができるように。
            m_Velocity = Vector3.Scale( m_Velocity, new Vector3(controlPower, controlPower, controlPower));
            m_RB.AddForce(m_RB.mass * m_Velocity / Time.fixedDeltaTime, ForceMode.Force);
        }
    }

    private const float reference = 0.2f;
    private bool MoveDuaringAir()
    {
        //入力が小さい時は切り捨てて空中移動を制限
        if (isJumping || isKnockBack && canMove)
        {
            if (m_inputMove.y < -reference || m_inputMove.y > reference)
            {
                return true;
            }

            if (m_inputMove.x < -reference || m_inputMove.x > reference)
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

    public void ChangePlayerCanMove(bool canMove)
    {
        if (canMove)
        {
            this.canMove = true;
        }
        else
        {
            this.canMove = false;
        }
    }
}