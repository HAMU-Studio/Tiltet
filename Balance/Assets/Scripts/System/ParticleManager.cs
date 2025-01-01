using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
            /*if (instance == null)
            {
                transform.parent = null;
              
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }*/
            instance = this;
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

            if (Time.realtimeSinceStartup - part.playedTime < playableDistance)
                return null;
            
            ParticleInstance partInstance = GetUnusedParticleInstance();
            if (partInstance == null) return null;
            
            partInstance.Instance = Instantiate(part.Prefab, part.Transform.position, part.Quaternion);
            partInstance.List = part;
            partInstance.Particle = partInstance.Instance.GetComponent<ParticleSystem>();
            
            // StopEmitting         : 新たな生成のみ停止(徐々にパーティクルが消える)
            // StopEmittingAndClear : 生成を停止し、画面上の既存のパーティクルも全消去
            partInstance.Particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            partInstance.IsPlay = false;
            part.playedTime = Time.realtimeSinceStartup;
            return partInstance;
        }

        /// <summary>
        /// 場所と回転だけ追加で登録する
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

        private void Update()
        {
            foreach (var particle in particleInstances)
            {
                if (particle.Instance == null || !particle.IsPlay) 
                    continue;
                
                particle.PlayTime += Time.deltaTime;
                
            
                //  particle.Instance.transform.position += GameManager.instance.StageMovement.MovementAmount;
                    
                // 停止時間を超えた場合の処理
                if (particle.PlayTime >= particle.List.StopTime)
                {
                    particle.Particle.Stop();
                    
                    if (particle.List.IsDiscardOnStop)
                    {
                        //Destroy(particle.Instance);
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

        private void FixedUpdate()
        {
            foreach (var particle in particleInstances)
            {
                if (particle.Instance == null || !particle.IsPlay)
                    continue;
                
                // パーティクルの移動処理 simulation spaceをLocalにしないと動かない
                Vector3 movementAmount = particle.Instance.transform.position + GameManager.instance.StageMovement.MovementAmount;
                particle.Instance.transform.position = movementAmount;
            }
        }

        public void GenerateAndPlay(string name, Transform transform,  Quaternion? quaternion = null)
        {
            ParticleInstance part = Generate(name);
            quaternion ??= Quaternion.identity;   // nullならidentity入れる
            // instantiate()
            ParticleSystem particle = part.Particle;
            particle.transform.SetPositionAndRotation(transform.position, quaternion.Value);
            particle.Play();
            part.IsPlay = true;
        }

        public void GenerateAndPlay(string name)
        {
            ParticleInstance part = Generate(name);
            if (part == null)
            {
                //Debug.Log("generate failed!");
                return;
            }
            part.Particle.Play();
            part.IsPlay = true;
        }

        /*public void Play(string name)
        {
            ParticleInstance part = GetUnusedParticleInstance(name);
            
            if (part == null) return;
            
            part.Particle.Play();
            
            if (!part.IsPlay)
                part.IsPlay = true;
        }*/

        /*public void Restart(string name)
        {
            ParticleInstance part = GetUnusedParticleInstance(name);
            if (part == null)
                return;
            part.Particle.Stop();
            part.Particle.Clear();
            part.Particle.Play();
        }*/

        /*public void Stop(string name)
        {
            ParticleInstance part = GetUnusedParticleInstance(name);
            if (part == null)
                return;
            part.Particle.Stop();
        }*/

        /*public void Pause(string name)
        {
            ParticleInstance part = GetUnusedParticleInstance(name);
            if (part == null)
                return;
            part.Particle.Pause();
        }*/

        private ParticleInstance GetUnusedParticleInstance()
        {
            foreach (var particle in particleInstances)
            {
                if (particle.Instance == null)
                    return particle;
            }
            Debug.LogError("There is no room in the array");
            return null;
        }
        
        private ParticleList GetParticleData(string name)
        {
            if (instance._lists.TryGetValue(name, out ParticleList particle))
            {
                return particle;
            }
            Debug.LogError("The particle does not exist");
            return null;
        }
    }
}