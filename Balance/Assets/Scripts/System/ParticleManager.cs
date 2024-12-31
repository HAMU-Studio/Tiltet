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
        private ParticleInstance[] particleInstances;
        
        [Header("一度生成してから、次生成出来るまでの間隔(秒)")]
        [SerializeField] private　float playableDistance = 0.2f;
        
        [Serializable]
        public class ParticleList
        {
            public string     Name;
            [Header("ParticleのPrefab")]
            public  GameObject Prefab;
            //  public ParticleSystem Particle; //= Prefab.GetComponent<ParticleSystem>();
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
       
        public class ParticleInstance
        {
            public GameObject     Instance;
            public ParticleSystem Particle;  //= Instance.GetComponent<ParticleSystem>();
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
            //  AddOrIncrementAsync参考
            ParticleList part =  GetParticleData(name);
            ParticleInstance partInstance = GetUnusedParticleInstance();
            
            if (part == null)
                return null;

            if (Time.realtimeSinceStartup - part.playedTime < playableDistance)
                return null;
            
            partInstance.Instance = Instantiate(part.Prefab, part.Transform.position, part.Quaternion);
            partInstance.List = part;
            partInstance.Instance.GetComponent<ParticleSystem>().Stop();
            partInstance.Particle = partInstance.Instance.GetComponent<ParticleSystem>();
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
            /*if (!instance._instances.TryGetValue(name, out ParticleInstance particle))
            {
                Debug.LogError("The particle does not exist");
                return;
            }
            instance._instances.Remove(name);*/
           // Destroy(particle.Particle.gameObject);
           part.Instance = null;
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

        private void Update()
        {
            foreach (var particle in particleInstances)
            {
                if (particle.Instance == null)
                    return;
                
                if (particle.IsPlay)
                {
                    particle.PlayTime += Time.deltaTime;
                    if (particle.PlayTime >= particle.List.StopTime)
                    {
                        particle.Particle.Stop();
                        if (particle.List.IsDiscardOnStop)
                        {
                            // instance._instances.Remove(particle.Name);
                          //  Debug.Log("call remove");
                            Destroy(particle.Instance);
                            ForceRemove(particle);
                           
                        }
                    }
                }
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