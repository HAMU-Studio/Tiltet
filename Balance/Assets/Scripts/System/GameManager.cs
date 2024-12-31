using FadeSystem;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Collections;
using UnityEngine.Rendering;

public enum GameState
{
    //制作の進捗具合によって逐次追加
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

    [SerializeField] private GameState m_currentState;
    [SerializeField] private RescueState currentRescue;

    [SerializeField] private int initialLife = default!;
    [SerializeField] private int initialWave = default!;

    private bool connectFlag = false;
  
    private int m_life;
   // private int m_wave;
    private int m_mainParts;
    private int m_subParts;

    private GameObject m_aircraftInstance;
    private Vector3 m_aircraftPos; //探索に復帰した時用 自機の座標

    private GameObject[] playerInstances;
    private void Awake()
    {
       // CurrentState = GameState.WaitStart;

        if (instance == null)
        {
            gameObject.transform.parent = null;
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 他のシーン遷移した時の二重生成防ぐ
            Destroy(this.gameObject);
        }
        InitGame();
    }
    private void InitGame()
    {
        m_life = initialLife;
        m_mainParts = 0;
        m_subParts = 0;
        isPlayerSpawn = new bool [2];
        method = new ThrowawayMethod();
        count = 0;
        isConnected = false;
        playerInstances = new GameObject[2];
        // SavePointの初期化はどうせ上書きされるから必要
    }

    public void StartGame()
    {
        _sceneManager.FadeStart("GreenStage");
        CurrentState = GameState.Search;
    }

    public void Restart()
    {
        _sceneManager.FadeStart("GreenStage");
        m_currentState = GameState.Search;
        PlayerDestroy();
        InitGame();
    }

    /// <summary>
    /// セーブポイントから再スタート GameManagerは基本ボタンから呼び出せなさそう
    /// </summary>
    public void RestartAtSavePoint(bool beforeLoading)
    {
        if (beforeLoading)
        {
            PlayerDestroy();
            InitGame();
        }
        else
        {
            SetAircraftPos();
            AircraftMoveSwitch(true);
        }
    }
    
    private void PlayerDestroy()
    {
        foreach (var player in playerInstances)
        {
            if (player == null)
                return;
            
            Destroy(player);
        }
    }
    
    public void EndGame()
    {
        //ゲームプレイ終了
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
#else
       Application.Quit();
#endif
    }

    private ThrowawayMethod method = new ThrowawayMethod();
  
    private void Update()
    {
        if (m_beforeState != m_currentState)
        {
            OnStateChange();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            method.RunOnce(GameClear); 
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            method.RunOnce(Restart);
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            _sceneManager.FadeStart("GreenStage");
            instance.CurrentState = GameState.Search;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame();
        }
        
        if (m_mainParts >= 1)
        {
            if (CurrentState != GameState.Clear)
            {
                GameClear();
            }
        }
    }
   
    public void GameOver()
    {
        _sceneManager.FadeStart("GameOver");
        CurrentState = GameState.GameOver;
    }

    public void GameClear()
    {
        _sceneManager.FadeStart("Clear");
        CurrentState = GameState.Clear;
        PlayerDestroy();
    }

    public void Back2StartMenu()
    {
        _sceneManager.FadeStart("Start");
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
        set { m_currentState = value; }
        get { return m_currentState; }
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
    
    
    private GameState m_beforeState;
    // ここで呼んでるコルーチンを
    private void OnStateChange()
    {
      //  Debug.Log("stateChange " + m_cuurentState + " to " + m_nextState);
        if (m_beforeState == GameState.EnemyBattle &&
            m_currentState == GameState.Search)   // 戦闘->探索
        {
            //     RespawnPlayer_Unloaded();
            //StartCoroutine(Back2Search(1.7f));
        }
        else if (m_beforeState == GameState.Search &&
                 m_currentState == GameState.EnemyBattle) // 探索->戦闘
        {
            SaveAircraftPos(m_aircraftInstance.transform.position);
            AircraftMoveSwitch(false);
        }
        else if (m_beforeState == GameState.Search &&
                 m_currentState == GameState.GameOver) // 探索->ゲームオーバー
        {
            SaveAircraftPos(m_aircraftInstance.transform.position);
            AircraftMoveSwitch(false);
        }
        /*
        else if (m_beforeState != GameState.Restart ||
                 m_currentState == GameState.Restart)
        {
            SaveAircraftPos(m_aircraftInstance.transform.position);
            AircraftMoveSwitch(false);
            
        }
        */
        
        m_beforeState = m_currentState;
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
    private void SaveAircraftPos(Vector3 position)
    {
        m_aircraftPos = position;
    }

    /// <summary>
    /// 戦闘->探索に切り替わったとき呼びたい
    /// </summary>
    private void SetAircraftPos()
    {
        if (m_aircraftInstance == null)
        {
            Debug.LogAssertion("m_aircraftInstance is null!");
            return;
        }
        m_aircraftInstance.transform.position = m_aircraftPos;
    }


    private StageMovement _stageStageMovement;
    /// <summary>
    /// 自機が移動するかどうかの切り替え 戦闘と探索の切り替えで使用
    /// </summary>
    /// <param name="activate"></param>
    public void AircraftMoveSwitch(bool activate)
    {
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
        //プレイヤー二人とも保存されたら自動で呼び出す シーン読み込んだら呼び出すように改善したい
        if (count == 2)
            return;
        
        playerInstances[count] = playerInstance;
       // _playerManagers[i] =  playerInstances[i].GetComponent<PlayerManager>();
        if (count == 0)
        {
            playerInstances[count].GetComponent<PlayerController>().SetSoundAndParticleName("PlayerMove", "PlayerHit", "RunDust1");
        }
        else if (count == 1)
        {
            playerInstances[count].GetComponent<PlayerController>().SetSoundAndParticleName("Player2Move", "Player2Hit", "RunDust2");
        }
        
        count++;
    }

    private void ResetPlayer()
    {
        playerInstances = null;
    }

    /// <summary>
    /// ポジション0にすればスポーンセンサーが検知して再セットしてくれる
    /// </summary>
    public void SetPlayerPos()
    {
        foreach (GameObject player in playerInstances)
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
        foreach (GameObject player in playerInstances)
        {
            if (player == null)
            {
                Debug.Log("PlayerInstance is null");
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
    /// <summary>
    /// 戦闘->探索に戻った時の処理
    /// </summary>
    public void Back2Search()
    {
        SetAircraftPos();
        SetPlayerPos();
        AircraftMoveSwitch(true);
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
    }
    public int GetMainPartsNum()
    {
        return m_mainParts;
    }

    public void AddSubPartsNum()
    {
        m_subParts++;
    }
    public int GetSubPartsNum()
    {
        return m_subParts;
    }
    
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
}