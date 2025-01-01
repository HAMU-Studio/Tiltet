using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace System
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager instance = null;
        
        private Dictionary<string, ParticleList> _lists = new();

        [SerializeField] private ParticleList[] particleDatas;
        [SerializeField] private ParticleInstance[] particleInstances;
        
        [Header("一度生成してから、次生成出来るまでの間隔(秒)")]
        [SerializeField] private　float playableDistance = 0.2f;
        
        [Serializable]
        public class ParticleList
        {
            public string     Name;
            [Header("ParticleのPrefab")]
            public  GameObject Prefab;
            [Header("Position")]
            public Transform  Transform;
            public Quaternion Quaternion;
            [HideInInspector]
            public int        UseCount = 0;  
            [Header("停止までの時間 (0で停止なし)")]
            public  float     StopTime;
            [Header("停止時に破棄するか")]
            public bool　　　  IsDiscardOnStop;
            [HideInInspector]
            public float      playedTime;  // 前回再生した時間
        }
       
        [Serializable]
        public class ParticleInstance
        {
            public GameObject     Instance;
            public ParticleSystem Particle;  
            public float          PlayTime;
            public ParticleList   List;
            public bool           IsPlay;
        }

        private void Awake()
        {
            if (instance == null)
            {
                transform.parent = null;
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            particleInstances = new ParticleInstance[20];
            
            for (int i = 0; i < particleDatas.Length; i++)
            {
                _lists.Add(particleDatas[i].Name, particleDatas[i]);
            }

            for (int i = 0; i < particleInstances.Length; i++)
            {
                particleInstances[i] = new ParticleInstance();
            }
        }

        private ParticleInstance Generate(string name)
        {
            ParticleList part =  GetParticleData(name);
            
            if (part == null) return null;

            if (Time.realtimeSinceStartup - part.playedTime < playableDistance)  // 連続生成を防ぐ 
                return null;
            
            // 未使用の配列を取得しインスタンスの登録
            ParticleInstance partInstance = GetUnusedParticleInstance();
            if (partInstance == null) return null;
            
            partInstance.Instance = Instantiate(part.Prefab, part.Transform.position, part.Quaternion);
            partInstance.List = part;
            partInstance.Particle = partInstance.Instance.GetComponent<ParticleSystem>();
            
            // StopEmitting         : 新たな生成のみ停止(徐々にパーティクルが消える)
            // StopEmittingAndClear : 生成を停止し、画面上の既存のパーティクルも全消去
            partInstance.Particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            partInstance.IsPlay = false;
            part.playedTime = Time.realtimeSinceStartup;    //前回再生した時間を登録
            
            return partInstance;
        }

        /// <summary>
        /// 場所と回転だけ追加で登録する  ※ Quaternion.Euler(x, y, z)でRotationと同じように登録可能
        /// </summary>
        public void Register(string name, Transform transform, Quaternion quaternion)
        {
            if (instance._lists.TryGetValue(name, out ParticleList particle))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                particle.Transform =  transform;
                particle.Quaternion = quaternion;
            }
        }
        
        // 場所のみ
        public void Register(string name, Transform transform)
        {
            if (instance._lists.TryGetValue(name, out ParticleList particle))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                particle.Transform =  transform;
            }
        }
        
        private void OnDestroy() => RemoveAll();

        public void ForceRemove(ParticleInstance part)
        { 
            if (part.Instance != null)
            {
                Destroy(part.Instance);
                part.Instance = null;
            }
         
            part.IsPlay = false;
            part.PlayTime = 0f;
            part.Particle = null;
            part.List = null;
        }

        public void RemoveAll()
        {
            foreach (var particle in particleInstances)
            {
                ForceRemove(particle);
            }
        }

        // 遅延付き削除用のコルーチン
        private IEnumerator DestroyParticleWithDelay(GameObject instance)
        {
            yield return new WaitForSeconds(0.1f); // 停止処理が完了するまで待機
            Destroy(instance);
        }

        private void Update() => TimeManage();

        private void FixedUpdate() =>  FollowAircraftMovement();
       
        private void TimeManage()
        {
            foreach (var particle in particleInstances)
            {
                if (particle.Instance == null || !particle.IsPlay) 
                    continue;   // 先頭から
                
                particle.PlayTime += Time.deltaTime;
                    
                // 停止時間を超えた場合の処理
                if (particle.PlayTime >= particle.List.StopTime && particle.List.StopTime != 0f)   // 0なら停止なし
                {
                    particle.Particle.Stop();
                    
                    if (particle.List.IsDiscardOnStop)
                    {
                        StartCoroutine(DestroyParticleWithDelay(particle.Instance));
                        ForceRemove(particle);
                    }
                    else
                    {
                        particle.IsPlay = false; // 再利用のため停止フラグを更新
                        particle.PlayTime = 0f;
                    }
                }
            }
        }

        /// <summary>
        /// 自機の移動に合わせてパーティクルを移動 　※ simulation spaceをLocalにしないと動かない
        /// </summary>
        private Vector3 movementAmount;
        private void FollowAircraftMovement()
        {
            foreach (var particle in particleInstances)
            {
                if (particle.Instance == null || !particle.IsPlay)
                    continue;
               
                movementAmount = particle.Instance.transform.position + GameManager.instance.StageMovement.MovementAmount;
                particle.Instance.transform.position = movementAmount;
            }
        }

        // 動作未確認
        public void GenerateAndPlay(string name, Transform transform,  Quaternion quaternion)
        {
            Register(name, transform, quaternion);
            ParticleInstance part = Generate(name);
            Play(part);
        }

        public void GenerateAndPlay(string name, Transform transform)
        {
            Register(name, transform);
            ParticleInstance part = Generate(name);
            Play(part);
        }

        public void GenerateAndPlay(string name)
        {
            ParticleInstance part = Generate(name);
            Play(part);
        }

        private void Play(ParticleInstance part)
        {
            if (part == null)
            {
                Debug.Log("generate failed!");
                return;
            }
            
            part.Particle.Play();
            part.IsPlay = true;
        }

        private ParticleInstance GetUnusedParticleInstance()
        {
            foreach (var particle in particleInstances)
            {
                if (particle.Instance == null) return particle;
            }
            Debug.LogError("There is no room in the array");
            return null;
        }
        
        private ParticleList GetParticleData(string name)
        {
            if (instance._lists.TryGetValue(name, out ParticleList particle))
                return particle;
            
            Debug.LogError("The particle does not exist");
            return null;
        }
    }
}