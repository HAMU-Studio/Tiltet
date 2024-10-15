using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
    [FormerlySerializedAs("currentStage")] [SerializeField] private RescueState currentRescue;

    [SerializeField] private int initialLife = default!;
    [SerializeField] private int initialWave = default!;
    
    private void Awake()
    {
        CurrentState = GameState.WaitStart;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
   
    void Start()
    {
        InitGame();
    }
    
    private int m_life;
    private int m_wave;
    private int m_parts;
  
    public void InitGame()
    {
     //   Time.timeScale = 0;
        m_life = initialLife;
        m_wave = initialWave;
        m_parts = 0;
        StartGame();
        //今後ScoreUIのUpdate呼び出す
    }

    //このあたりはプロトタイプのみ
    public void StartGame()
    {
        InitGame();
        Time.timeScale = 1;
        CurrentState = GameState.Search;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
    public GameState CurrentState
    {
        set
        {
            currentGamestate = value;
        }
        get
        {
            return currentGamestate;
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
        get
        {
            Debug.Log("axis = " + m_axis);
            return m_axis;
        }
        
        set { m_axis = value;}
    }

    public GameObject Pivot
    {
        get { return m_pivot; }

        set { m_pivot = value; }
    }
    
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

    public void AddPartsNum()
    {
        m_parts++;
    }
    public int GetPartsNum()
    {
        return m_parts;
    }
    
    public void ChangeStageModeTo(GameState state)
    {
        //tips GameManager.instance.CurrentState
        //でスクリプトアタッチしなくても現在が探索中なのか戦闘中なのか確認できるよ
        
        if (state == GameState.EnemyBattle || state == GameState.Search)
            GameManager.instance.CurrentState = state;
        
        /*foreach (GameObject wall in walls)    壁はなくなりそう
        {
            wall.SetActive(false);
        }*/

        //stageの移動停止と再開処理とかカメラの切り替え処理呼ぶ　ここは最悪相互参照になってもいいかも
        
    }
}
