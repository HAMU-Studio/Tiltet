using Dialogue;
using System.FadeSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameState
{
   None,
   StartMenu,
   Search,
   EnemyBattle,
   //Pose,
   Clear,
   Restart,
   GameOver,
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    public event Action OnInitGame;
    
    [SerializeField] private GameState currentState;
    [SerializeField] private RescueState currentRescue;

    [SerializeField] private int initialLife = default!;

    private bool connectFlag = false;
  
    private int m_life;
    private int m_mainParts;
    private int m_subParts;

    private GameObject m_aircraftInstance;
    private Vector3    m_aircraftPos;       //探索に復帰した時用 自機の座標

    private GameObject[] m_playerInstances;
    private void Awake()
    {
        if (instance == null)
        {
            transform.parent = null;
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            m_beforeState = currentState;
        }
        else
        {
            // 他のシーン遷移した時の二重生成防ぐ
            Destroy(this.gameObject);
        }

        // ここで体力初期化しないとMainStageから開始した時UIバグる
        InitLifeAndTime();
        
        // デバッグ用 戦闘からでもプレイヤーが動く
        if (CurrentState == GameState.EnemyBattle)
            InitGame(true);
    }
    /// <summary>
    /// ゲームの初期化 セーブポイントからスタートなのか、最初からなのかによって処理を切り替え
    /// </summary>
    /// <param name="isContinue"> true : セーブポイントからスタート </param>
    public void InitGame(bool isContinue)
    {
        InitLifeAndTime();
        isPlayerSpawn = new bool [2];
        isConnected = false;
        m_playerInstances = new GameObject[2];
        count = 0;

        if (isContinue == false)
        {
            m_mainParts = 0;
            m_subParts = 0;
            OnInitGame?.Invoke();   // フィールドアイテムの取得状況を初期化する関数を呼び出す
            isSkip = false;
        }
        // SavePointの初期化はどうせ上書きされるから必要ない
    }

    /// <summary>
    /// いつ初期化してもエラーが起きない変数のみ
    /// </summary>
    private void InitLifeAndTime()
    {
        m_timeArray = new int[4] { 0, 0, 0, 0 };
        m_life = initialLife;
    }
    
    public IEnumerator Restart()
    {
        _sceneManager.StartTransition("MainStage");
        currentState = GameState.Search;
        
        if (m_beforeState != GameState.StartMenu)
         PlayerDestroy();
        
        yield return new WaitForSeconds(1.7f);
        InitGame(false);
    }

    /// <summary>
    /// セーブポイントから再スタート GameManagerは基本ボタンから呼び出せなさそう
    /// </summary>
    public void RestartAtSavePoint(bool beforeLoading)
    {
        if (beforeLoading)
        {
            PlayerDestroy();
            InitLifeAndTime();
        }
        else
        {
            InitGame(true);
            SetAircraftPos();
            AircraftMoveSwitch(true);
        }
    }

    private bool skipStartDialogue;
    public bool isSkip
    {
        get { return skipStartDialogue; }
        set { skipStartDialogue = value; }
    }
    
    private void PlayerDestroy()
    {
        foreach (var player in m_playerInstances)
        {
            if (player == null)
                return;
            
            Destroy(player);
        }
    }
    
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
#else
       Application.Quit();
#endif
    }
  
    private void Update()
    {
        if (m_beforeState != currentState)
        {
            OnStateChange();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(GameClear());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Restart());
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(instance.FightClear());
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            _sceneManager.StartTransition("MainStage");
            instance.CurrentState = GameState.Restart;
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            _sceneManager.StartTransition("Fight");
            instance.CurrentState = GameState.EnemyBattle;
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            _sceneManager.StartTransition("SnowFight");
            instance.CurrentState = GameState.EnemyBattle;
        }
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            _sceneManager.StartTransition("VolcanoFight");
            instance.CurrentState = GameState.EnemyBattle;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame();
        }
        
        if (m_mainParts >= 3)
        {
            if (CurrentState != GameState.Clear)
            {
                StartCoroutine(GameClear());
            }
        }
    }
   
    public IEnumerator GameOver()
    {
        DisplayDialogue.system.Enqueue("Finish");
        SoundManager.instance.Play("Finish");
        
        yield return new WaitForSeconds(1.5f);
        
        _sceneManager.StartTransition("GameOver");
        CurrentState = GameState.GameOver;
        yield return null;
    }

    public IEnumerator GameClear()
    {
        CurrentState = GameState.Clear;
        DisplayDialogue.system.Enqueue("Clear");
        SoundManager.instance.Play("Finish");
        
        yield return new WaitForSeconds(1.5f);
        
        _sceneManager.StartTransition("Clear");
       
        yield return new WaitForSeconds(1.5f);
        
        PlayerDestroy();
        InitGame(false);
        yield return null;
    }

    public IEnumerator FightClear()
    {
        DisplayDialogue.system.Enqueue("Clear");
        SoundManager.instance.Play("Finish");
        
        yield return new WaitForSeconds(1.5f);
        
        instance.SceneManager.StartTransition("MainStage");
        instance.CurrentState = GameState.Search;
    }

    public void Back2StartMenu()
    {
        _sceneManager.StartTransition("Start");
        CurrentState = GameState.StartMenu;
        
        //ゲーム中から戻った時のためにプレイヤーいたら消す
        PlayerDestroy();
    }

    private FadeAndSceneTransition _sceneManager;
    public FadeAndSceneTransition SceneManager
    {
        set { _sceneManager = value; }
        get { return _sceneManager; }
    }

    public GameState CurrentState
    {
        set { currentState = value; }
        get { return currentState; }
    }

    public GameState BeforeState
    {
        set { m_beforeState = value; }
        get { return m_beforeState; }
    }
    

    public bool isConnected
    {
        get { return connectFlag; }

        set { connectFlag = value; }
    }
    
    
    /// <summary>
    /// ステートが切り替わると呼ばれる シーンのロード前
    /// </summary>
    private GameState m_beforeState;
    private void OnStateChange()
    {
       if (m_beforeState != GameState.Search && m_beforeState != GameState.EnemyBattle)
       {
           m_beforeState = currentState;
           return;
       }
       
       instance.PlayerLock();
       
        if (m_beforeState == GameState.Search &&
                 CurrentState == GameState.EnemyBattle) // 探索->戦闘
        {
            AircraftMoveSwitch(false);
        }
        else if (m_beforeState == GameState.Search &&
                 CurrentState == GameState.GameOver)   // 探索->ゲームオーバー
        {
            SaveAircraftPos(m_aircraftInstance.transform.position);
            AircraftMoveSwitch(false);
        }
        
        if (m_beforeState == GameState.Search && CurrentState == GameState.Restart)
            SaveAircraftPos(m_aircraftInstance.transform.position);
        
        m_beforeState = CurrentState;
    }

    public GameObject Aircraft
    {
        get { return m_aircraftInstance; }
    }

    /// <summary>
    /// 自機のインスタンス保存 シーン読み込んだら呼びたい
    /// </summary>
    public void SaveAircraftInstance(GameObject obj)
    {
        m_aircraftInstance = obj;
    }

    /// <summary>
    /// 自機の場所保存 探索->戦闘に切り替わったとき呼びたい
    /// </summary>
    public void SaveAircraftPos(Vector3 position)
    {
        m_aircraftPos = position;
    }

    /// <summary>
    /// 戦闘->探索に切り替わったとき呼びたい
    /// </summary>
    private void SetAircraftPos()
    {
        StartCoroutine(DelaySetAirCraft());
    }


    private StageMovement _stageStageMovement;
    /// <summary>
    /// 自機が移動するかどうかの切り替え 戦闘と探索の切り替えで使用
    /// </summary>
    /// <param name="activate"></param>
    public void AircraftMoveSwitch(bool activate)
    {
        if (m_aircraftInstance == null)
            return;
        
        StageMovement = m_aircraftInstance.GetComponent<StageMovement>();

        if (activate)
        {
            _stageStageMovement.enabled = true;
        }
        else
        {
            _stageStageMovement.enabled = false;
            ResetRBVelocity(m_aircraftInstance);
        }
    }

    int[] m_timeArray;

    public int[] SetTimer(int[] timeArray)
    {
        for (int i = 0; i < timeArray.Length; i++)
        {
            timeArray[i] = m_timeArray[i];
        }
        return timeArray;
    }
    public void SaveCurrentTime(int[] timeArray)
    {
        for (int i = 0; i < timeArray.Length; i++)
        {
            m_timeArray[i] = timeArray[i];
        }
    }
    
    public StageMovement StageMovement
    {
        get { return _stageStageMovement; }
        set { _stageStageMovement = value; }
    }

    private Vector3 m_axis;
    private GameObject m_pivot;
    private bool m_isRescue;
    /// <summary>
    /// 振り子の方向制御用
    /// </summary>
    public Vector3 Axis
    {
        get { return m_axis; }
        
        set { m_axis = value;}
    }
    
    int count = 0;
    public void SavePlayerInstance(GameObject playerInstance)
    {
        // プレイヤー二人とも保存されたら自動で呼び出す シーン読み込んだら呼び出すように改善したい
        if (count == 2)
            return;
        
        m_playerInstances[count] = playerInstance;
        if (count == 0)
        {
            m_playerInstances[count].GetComponent<PlayerController>().SetSoundAndParticleName("PlayerMove", "PlayerHit", "RunDust1");
        }
        else if (count == 1)
        {
            m_playerInstances[count].GetComponent<PlayerController>().SetSoundAndParticleName("Player2Move", "Player2Hit", "RunDust2");
        }
        
        count++;
    }

    private void ResetPlayer()
    {
        m_playerInstances = null;
    }

    /// <summary>
    /// ポジション0にすればスポーンセンサーが検知して再セットしてくれる
    /// </summary>
    public void SetPlayerPos()
    {
        if (BeforeState != GameState.Search && BeforeState != GameState.EnemyBattle)
        {
            return;
        }
        foreach (GameObject player in m_playerInstances)
        {
            if (player == null)
            {
                Debug.LogError("playerInstance is null!");
                return;
            }
            player.transform.position = Vector3.zero;
        }
    }

    /// <summary>
    /// プレイヤーを自機に再スポーンさせる処理 
    /// </summary>
    /// <param name="beforeLoading">シーンロード前とロード後で処理を呼び分け</param>
    public void RespawnPlayer(bool beforeLoading)
    {
        if (BeforeState != GameState.Search && BeforeState != GameState.EnemyBattle)
        {
            return;
        }
        foreach (GameObject player in m_playerInstances)
        {
            if (player == null)
            {
                Debug.LogError("PlayerInstance is null");
                return;
            }

            if (beforeLoading)
            {
                player.GetComponent<PlayerManager>().ResetPlayer_Unloaded();
            }
            else
            {
                player.GetComponent<PlayerManager>().ResetPlayer_Loaded();
            }
        }
    }

    public void PlayerLock()
    {
        if (BeforeState != GameState.Search && BeforeState != GameState.EnemyBattle)
        {
            return;
        }
        foreach (GameObject player in m_playerInstances)
        {
            if (player == null)
            {
                Debug.LogAssertion("PlayerInstance is null");
                return;
            }

            player.GetComponent<PlayerManager>().LockPos();
        }
    }
    
    public void PlayerUnLock()
    {
        foreach (GameObject player in m_playerInstances)
        {
            if (player == null)
            {
                Debug.LogError("PlayerInstance is null");
                return;
            }

            player.GetComponent<PlayerManager>().UnLockPos();

        }
    }
    /// <summary>
    /// 戦闘->探索に戻った時の処理
    /// </summary>
    public void Back2Search()
    {
        SetAircraftPos();
    }
    
    public GameObject Pivot
    {
        get { return m_pivot; }

        set { m_pivot = value; }
    }
    //ステージ耐久値
    public int Life
    {
        get { return m_life;}
        
        set { m_life = value;}
    }

    public bool IsRescue
    {
       get { return m_isRescue; }
       
       set { m_isRescue = value;}
    }
    
    private bool[] isPlayerSpawn;
    public bool P1Spawn
    {
        get { return isPlayerSpawn[0]; }

        set { isPlayerSpawn[0] = value; }
    }

    public bool P2Spawn
    {
        get { return isPlayerSpawn[1]; }

        set { isPlayerSpawn[1] = value; }
    }

    public void AddMainPartsNum()
    {
        m_mainParts++;
        if (m_mainParts == 1)
        {
            DisplayDialogue.dialogue.Enqueue("GetMainPart");
            DisplayDialogue.dialogue.Enqueue("TwoLeft");
        }
        else if (m_mainParts == 2)
        {
            DisplayDialogue.dialogue.Enqueue("GetMainPart");
            DisplayDialogue.dialogue.Enqueue("OneLeft");
        }
    }
    public int GetMainPartsNum()
    {
        return m_mainParts;
    }

    public void AddSubPartsNum()
    {
        m_subParts++;
        if (m_subParts == 1)
        {
            for (int i = 1; i < 4; i++)
            {
                DisplayDialogue.dialogue.Enqueue($"GetSubPart0{i}");
            }
        }
    }
    public int GetSubPartsNum()
    {
        return m_subParts;
    }
    
    // RBからとGameObjectからどちらでも速度の初期化ができるように
    public void ResetRBVelocity(Rigidbody RB)
    {
        RB.velocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero;
    }
    
    public void ResetRBVelocity(GameObject obj)
    {
        Rigidbody RB = obj.GetComponent<Rigidbody>();
        
        RB.velocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero;
    }

    private IEnumerator DelaySetAirCraft()
    {
        yield return new WaitForSeconds(0.1f);
        if (m_aircraftInstance == null)
        {
            Debug.LogError("m_aircraftInstance is null!");
            yield break;
        } 
        
        m_aircraftInstance.transform.position = m_aircraftPos;
        
        SetPlayerPos();
        AircraftMoveSwitch(true);
        
    }
}