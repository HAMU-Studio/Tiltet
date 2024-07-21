using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StageState
{
    //ステージによって何か変わるかも
    First,
    Second,
    Third
}

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
    [SerializeField] private StageState currentStage;

    [SerializeField] private int initialLife = default!;
    [SerializeField] private int initialWave = default!;
    
    private int m_life;
    private int m_wave;
    
    private void Awake()
    {
        SetCurrentState(GameState.WaitStart);

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

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //このあたりはプロトタイプのみ
    public void StartGame()
    {
        InitGame();
        Time.timeScale = 1;
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

    public void InitGame()
    {
        Time.timeScale = 0;
        m_life = initialLife;
        m_wave = initialWave;
        //今後ScoreUIのUpdate呼び出す
    }
    
    public void SetCurrentState(GameState State)
    {
        currentGamestate = State;
    }
    public GameState ReturnCurrentState()
    {
        return currentGamestate;
    }
}
