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
    
    WaitStart,  //今後消す
    None,
    //SelectionScreen,
    //ConnectionScreen,
    //Countdown,
    Search,
    EnemyBattle,
    //Pose,
    Result
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private GameState currentGamestate;
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
            Destroy(this.gameObject);
        }
        isPlayerSpawn = new bool [2];
        playerInstances = new GameObject[2];

        InitGame();
    }
    private void InitGame()
    {
       // Time.timeScale = 0;
        m_life = initialLife;
       // m_wave = initialWave;
        m_mainParts = 0;
        m_subParts = 0;
        isPlayerSpawn = new bool [2];
        int i = 0;
      //  StartGame();
        //今後ScoreUIのUpdate呼び出す
    }

    //このあたりはプロトタイプのみ
    public void StartGame()
    {
     //   InitGame();
        Time.timeScale = 1;
    //    CurrentState = GameState.Search;
    }

    public void Restart()
    {
        _sceneManager.FadeStart();
        currentGamestate = GameState.Search;
        PlayerDestroy();
    }

    public void PlayerDestroy()
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

    private void Update()
    {
        if (m_beforeState != CurrentState)
        {
            OnStateChange();
        }
        
    }

   
    public void GameOver()
    {
        _sceneManager.nextSceneName = "GameOver";
        _sceneManager.FadeStart();
        CurrentState = GameState.Result;
    }

    private FadeAndSceneTransition _sceneManager;
    public void SetSceneManager(FadeAndSceneTransition sceneManager)
    {
        _sceneManager = sceneManager;
    }

    public GameState CurrentState
    {
        set { currentGamestate = value; }
       
        get { return currentGamestate; }
    }

    public bool isConnected
    {
        get { return connectFlag; }

        set { connectFlag = value; }
    }
    
    
    private GameState m_beforeState;
    private void OnStateChange()
    {
        //Debug.Log("stateChange " + m_beforeState + " to " + CurrentState);
        if (m_beforeState == GameState.EnemyBattle &&
            currentGamestate == GameState.Search)   // 戦闘->探索
        {
            StartCoroutine(BetaDelayRun(1.7f));
        }

        if (m_beforeState == GameState.Search &&
            currentGamestate == GameState.EnemyBattle) // 探索->戦闘
        {
            SaveAircraftPos(m_aircraftInstance.transform.position);
            AircraftMoveSwitch(false);
            StartCoroutine(DelayResetPlayer());
        }
        
        m_beforeState = currentGamestate;
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

    /// <summary>
    /// 自機が移動するかどうかの切り替え 戦闘と探索の切り替えで使用
    /// </summary>
    /// <param name="activate"></param>
    public void AircraftMoveSwitch(bool activate)
    {
        StageMovement stageMovement = m_aircraftInstance.GetComponent<StageMovement>();

        if (activate)
        {
            stageMovement.enabled = true;
        }
        else
        {
            stageMovement.enabled = false;
            ResetRBVelocity(m_aircraftInstance);
        }
    }


    private Vector3 m_axis;
    private GameObject m_pivot;
    private bool m_rescue;
    /// <summary>
    /// 振り子の方向制御用
    /// </summary>
    public Vector3 Axis
    {
        get { return m_axis; }
        
        set { m_axis = value;}
    }

    private bool temp;
    int i = 0;
    public void SavePlayerInstance(GameObject playerInstance)
    {
        if (temp)
            return;
        
        playerInstances[i] = playerInstance;
       // _playerManagers[i] =  playerInstances[i].GetComponent<PlayerManager>();
        if (i == 0)
        {
            playerInstances[i].GetComponent<PlayerController>().SetSoundName("PlayerMove", "PlayerHit");
           
        }
        else if (i == 1)
        {
            playerInstances[i].GetComponent<PlayerController>().SetSoundName("Player2Move", "Player2Hit");
        }
       
        i++;
        
        //プレイヤー二人とも保存されたら自動で呼び出す シーン読み込んだら呼び出すように改善したい
        if (i == 2)
        {
         //   instance.SetPlayerPos();
            temp = true;
        }
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

    public void ResetPlayer()
    {
        foreach (GameObject player in playerInstances)
        {
            if (player == null)
            {
                return;
            }
            StartCoroutine(player.GetComponent<PlayerManager>().ResetPlayerState());
        }
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

    public bool Rescue
    {
       get { return m_rescue; }
       
       set { m_rescue = value;}
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

    private IEnumerator BetaDelayRun(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SetAircraftPos();
        SetPlayerPos();
        AircraftMoveSwitch(true);
    }
    
    /// <summary>
    /// シーン移動が終わってからプレイヤーセット
    /// プレイヤーセットしたら自動で要改善
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayResetPlayer()
    {
        yield return new WaitForSeconds(1.8f);
        //急にうごかなくなったから消した
      //  SetPlayerPos();
    }
    
}
