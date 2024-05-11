//古澤
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    private Vector3 m_Velocity;

    //地面の上なら歩きモーション、違うなら落下モーション              

    private bool isJump = false;         
    private bool isFall = false;
    public bool isAttack = false;
    private bool canMove = true;
    // private bool onlyFirst = false;
    //private bool isKnockBack = false;

    [Header("移動スピード")]
    [SerializeField] private float walkSpeed = 4f;
    [Header("落下速度の調整　-つける")]
    [SerializeField] float gravityPower = default!;
 

    [Header("トリガーの反応タイミング")]
    [SerializeField] float triggerTiming = 0.5f;         //トリガーがどこまで押し込まれたら反応するか 要調整 
    [Header("回転時間")]
    [SerializeField] float smoothTime = 0.3f;                //進行方向への回転にかかる時間
    [Header("ジャンプの強さ")]
    [SerializeField] private float jumpPower = 5f;            
    // [Header("ノックバックの強さ")]
    // [SerializeField] private float knockBackP = 5f;              
    // [Header("ノックバック時上方向の力")]
    // [SerializeField] float knockBackUpP = 3f;            //ノックバック時少し上に浮かす


    float inputHorizontal;      //水平方向の入力値
    float inputVertical;        //垂直方向の入力値
    //float L_inputTrigger;
    float R_inputTrigger;
    bool inputAttack;

    bool R_isReset;
    //  bool L_isReset;
    float targetRotation;   //回転に使う
    float yVelocity = 0.0f;
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Input();
        Jump();
       //Attack(); //プロトタイプは現状攻撃なし
    }
    private void FixedUpdate()
    {
        Gravity();
        Move();
    }

    private void Input()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.X))    //デバッグ用無敵モードon
        {
           // _MainGameManager.isInvincible = true;
            Debug.Log("無敵");
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.M))    //デバッグ用無敵モードoff
        {
            //_MainGameManager.isInvincible = false;
            Debug.Log("無敵解除");
        }
        
        //入力値の格納
        inputHorizontal = UnityEngine.Input.GetAxisRaw("Horizontal");   
        inputVertical = UnityEngine.Input.GetAxisRaw("Vertical");
        R_inputTrigger = UnityEngine.Input.GetAxis("R_Trigger");
        inputAttack = UnityEngine.Input.GetButtonDown("Attack");    
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        //難しい方法はできないからTriggerで判定したい
        if (isJump == true || isFall == true)
        {
            if (collision.gameObject.CompareTag("Ground"))  //着地した時
            {
                isJump = false;
                isFall = false;
                canMove = true;
                //Debug.Log("toLanding" );
            }
        }

        /*if (_MainGameManager.isInvincible == false)
        {
            if (collision.gameObject.CompareTag("Enemy") )     //無敵時間中はダメージを食らわない
            {
                canMove = false;
                KnockBack(collision);
                _MainGameManager.Miss();
            }
        }*/
    }

    /*void Attack()   //ジャンプ中は攻撃できない
    {
        if (R_inputTrigger == 0 && inputAttack == false)
        {
            R_isReset = true;
            return;
        }
        if (isAttack == true || isJump == true || R_isReset == false)
            return;

        if (R_inputTrigger > triggerTiming || inputAttack)  //AボタンかRTで攻撃
        {
            isAttack = true;
            R_isReset = false;
        }
    }*/
    
    /*void KnockBack(Collision collision)
    {
        isJump = true;
        Debug.Log("isKnockBack");
        Vector3 direction = collision.gameObject.transform.forward;

        m_Rigidbody.AddForce(-direction * knockBackP, ForceMode.Impulse);      
        m_Rigidbody.AddForce(transform.up * knockBackUpP, ForceMode.Impulse);   //若干上方向にも飛ばす

    }*/

    /*public void fall()  //落下判定エリアで使う
    {
        isFall = true;
        canMove = false;

        //ここで操作不能にすればすれすれから復帰した時にジャンプができなくなることを防げそう
        //落下モーションへの遷移
    }*/

    void Move()
    {
        if (!canMove) //攻撃中は移動もジャンプもできない->returnじゃなくてその場で固定させたい
            return;

        if (isAttack == true)
        {
            inputHorizontal = 0;
            inputVertical = 0;
        }

        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;


        //移動速度の計算
        //clampは値の範囲制限
        //GetAxisは0から1で入力値を管理する、斜め移動でWとAを同時押しすると
        //1以上の値が入ってくるからVector3.ClampMagnitudeメソッドを使って入力値を１に制限する(多分)
        var clampedInput = Vector3.ClampMagnitude(moveForward, 1f);   
                                                                    

        m_Velocity = clampedInput * walkSpeed;
        // transform.LookAt(m_Rigidbody.position + input); //キャラクターの向きを現在地＋入力値の方に向ける

        //Rigidbodyに一度力を加えると抵抗する力がない限りずっと力が加わる
        //AddForceに加える力をwalkSpeedで設定した速さ以上にはならないように
        //今入力から計算した速度から現在のRigidbodyの速度を引く
        m_Velocity = m_Velocity - m_Rigidbody.velocity;

        //　速度のXZを-walkSpeedとwalkSpeed内に収めて再設定
        m_Velocity = new Vector3(Mathf.Clamp(m_Velocity.x, -walkSpeed, walkSpeed), 0f, Mathf.Clamp(m_Velocity.z, -walkSpeed, walkSpeed));

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
        m_Rigidbody.AddForce(m_Rigidbody.mass * m_Velocity / Time.fixedDeltaTime, ForceMode.Force);
       
    }

    void Jump()
    {
        //落下中と攻撃中はジャンプをさせない
        if (isJump == true || isFall == true || isAttack == true) return;  

        if (UnityEngine.Input.GetButtonDown("Jump"))
        {
            //移動中またはその場でジャンプした時の遷移
            m_Rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
            isJump = true;
        }
    }

    void Gravity()
    {
        //落下速度の調整用
        if (isJump == true)
        {
            m_Rigidbody.AddForce(new Vector3(0, gravityPower, 0));
        }
    }
}