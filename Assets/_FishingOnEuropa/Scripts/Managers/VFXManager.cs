// Ignore Spelling: Dict FOEVFX VFX IFOEVFX

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;


namespace Europa
{
    public enum FOEVFX
    {
        //Water
        W_Splash1,

        //Player


        //MiniGame
        MG_Catch,

        //Hybrid


        //Other
        O_Confetti1, O_Confetti2,
        O_Area1, O_Area2, O_Area3, O_Area4, O_Area5,
        O_Dust1,
        O_Flash1, O_Flash2, O_Flash3, O_Flash4, O_Flash5, O_Flash6, O_Flash7, O_Flash8,
        O_Shine1, O_Shine2, O_Shine3,
        O_Sparkle1,
        O_Water1, O_Water2

    }

   

    [Serializable]
    public class FOE_Particle
    {
        public ParticleSystem ParticleSystem;
        public FOEVFX FOEVFX;
    }

    public enum FOEVFXClass
    {
        Water,
        Player,
        MiniGame,
        Hybrid,
        Other,
    }

    public interface IFOEVFX
    {
        public ParticleSystem GetVFX(FOEVFX _VFX)
        {
            return null;
        }

        public void InitDictionary()
        {

        }
    }
    public class VFXManager : Singleton<VFXManager>
    {
        public Dictionary<FOEVFXClass, IFOEVFX> vFXDictionary = new();

        [SerializeField] private FOEVFX_Water fOEVFX_Water;
        [SerializeField] private FOEVFX_Player fOEVFX_Player;
        [SerializeField] private FOEVFX_MiniGame fOEVFX_MiniGame;
        [SerializeField] private FOEVFX_Hybrid fOEVFX_Hybrid;
        [SerializeField] private FOEVFX_Other fOEVFX_Other;

        public List<ParticleSystem> playedParticles;




        private void Start()
        {
            InitVFXDictionary();
            InitializeParticles();
        }
        /// <summary>
        /// a function for Initializing the Dictionary 
        /// </summary>
        private void InitVFXDictionary()
        {
            if (vFXDictionary == null)
                Debug.Log($"The Dictionary dose not exist, IDK why ; - ; ");

            if (fOEVFX_Water == null)
                print("NotGood");
            else
                vFXDictionary.Add(FOEVFXClass.Water, fOEVFX_Water);
            if (fOEVFX_Player == null)
                print("NotGood");
            else
                vFXDictionary.Add(FOEVFXClass.Player, fOEVFX_Player);
            if (fOEVFX_MiniGame == null)
                print("NotGood");
            else
                vFXDictionary.Add(FOEVFXClass.MiniGame, fOEVFX_MiniGame);
            if (fOEVFX_Hybrid == null)
                print("NotGood");
            else
                vFXDictionary.Add(FOEVFXClass.Hybrid, fOEVFX_Hybrid);
            if (fOEVFX_Other == null)
                print("NotGood");
            else
                vFXDictionary.Add(FOEVFXClass.Other, fOEVFX_Other);

        }

        private void InitializeParticles()
        {
            fOEVFX_Water?.Init();
            fOEVFX_Player?.Init();
            fOEVFX_MiniGame?.Init();
            fOEVFX_Hybrid?.Init();
            fOEVFX_Other?.Init();
        }

        /// <summary>
        /// Spawns A particle Effect using the inputted Enums and the inputted Location 
        /// </summary>
        /// <param name="_VFXGroup"></param>
        /// <param name="_VFX"></param>
        /// <param name="_position"></param>
        /// <param name="_parent"></param>
        public void PlayVFX(FOEVFXClass _VFXGroup, FOEVFX _VFX, Transform _position, bool _parent)
        {
            bool check;
            ParticleSystem _particle;

            (_particle, check) = SpawnVFXErrorCheck(_VFXGroup, _VFX);

            if (!check)
                return;

            _particle = GetParticleSystem(_particle, _position, _parent);

            PlayVFX(_particle);
        }

        // Overload with exit time to handle looping animations.
        public void PlayVFX(FOEVFXClass _VFXGroup, FOEVFX _VFX, Transform _position, bool _parent, float _exitTime)
        {

            bool check;
            ParticleSystem _particle;

            (_particle, check) = SpawnVFXErrorCheck(_VFXGroup, _VFX);

            if (!check)
                return;

            _particle = GetParticleSystem(_particle, _position, _parent);

            PlayVFX(_particle);

            // Start the coroutine to stop the particle system after _exitTime.
            StartCoroutine(StopParticleAfterTime(_particle, _exitTime));
        }

        // Overload with exit time to handle looping animations.
        public void PlayVFX(FOEVFXClass _VFXGroup, FOEVFX _VFX, Transform _position, Quaternion _customRotation, bool _parent)
        {

            bool check;
            ParticleSystem _particle;

            (_particle, check) = SpawnVFXErrorCheck(_VFXGroup, _VFX);

            if (!check)
                return;

            _particle = GetParticleSystem(_particle, _position, _customRotation, _parent);

            PlayVFX(_particle);

            
        }

        private void PlayVFX(ParticleSystem _PS)
        {
            _PS.gameObject.AddComponent<ParticleAutoReturn>().Init(_PS, ReturnToPool);

            // Play the particle system.
            _PS.Play();

            playedParticles.Add(_PS);
        }

        private ParticleSystem GetParticleSystem(ParticleSystem _PS, Transform _position, bool _parent)
        {
            GameObject particleGO = _OPM.SpawnObject(_PS.gameObject, _position.position, _position.rotation, PoolType.Particales);
            ParticleSystem particleSpawened = particleGO.GetComponent<ParticleSystem>();

            if (_parent)
                particleGO.transform.parent = _position;

            return particleSpawened;
        }

        private ParticleSystem GetParticleSystem(ParticleSystem _PS, Transform _position , Quaternion _rotation, bool _parent)
        {
            GameObject particleGO = _OPM.SpawnObject(_PS.gameObject, _position.position, _rotation, PoolType.Particales);
            ParticleSystem particleSpawened = particleGO.GetComponent<ParticleSystem>();

            if (_parent)
                particleGO.transform.parent = _position;

            return particleSpawened;
        }


        private (ParticleSystem, bool) SpawnVFXErrorCheck(FOEVFXClass _VFXGroup, FOEVFX _VFX)
        {
            if (!vFXDictionary.TryGetValue(_VFXGroup, out IFOEVFX VFXFound))
            {
                Debug.LogError($"Item not found: {_VFXGroup} is not in the dictionary.");
                return (null, false);
            }

            ParticleSystem particleFound = VFXFound.GetVFX(_VFX);

            if (particleFound == null)
            { 
                Debug.LogError($"VFX {_VFX} could not be found in {_VFXGroup}");
                return (null, false);
            }

            return (particleFound, true);
        }

        // Coroutine to stop the particle system after the given exit time.
        private IEnumerator StopParticleAfterTime(ParticleSystem particle, float exitTime)
        {
            yield return new WaitForSeconds(exitTime);

            if (particle != null && particle.isPlaying)
            {
                particle.Stop(); // Stop the particle system.
                ReturnToPool(particle); // Return it to the pool.
            }
        }

        // Function to return the particle to the object pool.
        private void ReturnToPool(ParticleSystem particle)
        {
            playedParticles.Remove(particle);
            _OPM.ReturnObjectToPool(particle.gameObject);
        }
    }


    [Serializable]
    public class FOEVFX_Water : FOEVFXBase, IFOEVFX
    {
        public FOE_Particle[] fOE_WaterParticles;
        public Dictionary<FOEVFX, ParticleSystem> fOE_WaterParticlesDict;

        public FOEVFX_Water()
        {
            fOE_WaterParticles = fOE_WaterParticles ?? Array.Empty<FOE_Particle>();
            fOE_WaterParticlesDict = new Dictionary<FOEVFX, ParticleSystem>();
            InitDictionary(ref fOE_WaterParticlesDict, fOE_WaterParticles);
        }

        public void Init()
        {
            InitDictionary(ref fOE_WaterParticlesDict, fOE_WaterParticles);
        }

        public ParticleSystem GetVFX(FOEVFX _VFX)
        {
            return GetVFX(fOE_WaterParticlesDict, _VFX);
        }

    }

    [Serializable]
    public class FOEVFX_Player : FOEVFXBase, IFOEVFX
    {
        public FOE_Particle[] fOE_PlayerParticles;
        public Dictionary<FOEVFX, ParticleSystem> fOE_PlayerParticlesDict;

        public FOEVFX_Player()
        {
            fOE_PlayerParticles = fOE_PlayerParticles ?? Array.Empty<FOE_Particle>();
            fOE_PlayerParticlesDict = new Dictionary<FOEVFX, ParticleSystem>();
            InitDictionary(ref fOE_PlayerParticlesDict, fOE_PlayerParticles);
        }

        public void Init()
        {
            InitDictionary(ref fOE_PlayerParticlesDict, fOE_PlayerParticles);
        }

        public ParticleSystem GetVFX(FOEVFX _VFX)
        {
            return GetVFX(fOE_PlayerParticlesDict, _VFX);
        }

        
    }

    [Serializable]
    public class FOEVFX_MiniGame : FOEVFXBase, IFOEVFX
    {
        public FOE_Particle[] fOE_MiniGameParticles;
        public Dictionary<FOEVFX, ParticleSystem> fOE_MiniGameParticlesDict;

        public FOEVFX_MiniGame()
        {
            fOE_MiniGameParticles = fOE_MiniGameParticles ?? Array.Empty<FOE_Particle>();
            fOE_MiniGameParticlesDict = new Dictionary<FOEVFX, ParticleSystem>();
            InitDictionary(ref fOE_MiniGameParticlesDict, fOE_MiniGameParticles);
        }

        public void Init()
        {
            InitDictionary(ref fOE_MiniGameParticlesDict, fOE_MiniGameParticles);
        }

        public ParticleSystem GetVFX(FOEVFX _VFX)
        {
            return GetVFX(fOE_MiniGameParticlesDict, _VFX);
        }


    }

    [Serializable]
    public class FOEVFX_Hybrid : FOEVFXBase, IFOEVFX
    {
        public FOE_Particle[] fOE_HybridParticles;
        public Dictionary<FOEVFX, ParticleSystem> fOE_HybridParticlesDict;

        public FOEVFX_Hybrid()
        {
            fOE_HybridParticles = fOE_HybridParticles ?? Array.Empty<FOE_Particle>();
            fOE_HybridParticlesDict = new Dictionary<FOEVFX, ParticleSystem>();
            InitDictionary(ref fOE_HybridParticlesDict, fOE_HybridParticles);
        }

        public void Init()
        {
            InitDictionary(ref fOE_HybridParticlesDict, fOE_HybridParticles);
        }

        public ParticleSystem GetVFX(FOEVFX _VFX)
        {
            return GetVFX(fOE_HybridParticlesDict, _VFX);
        }
    }

    [Serializable]
    public class FOEVFX_Other : FOEVFXBase, IFOEVFX
    {
        public FOE_Particle[] fOE_OtherParticles;
        public Dictionary<FOEVFX, ParticleSystem> fOE_OtherParticlesDict;

        public FOEVFX_Other()
        {
            fOE_OtherParticles = fOE_OtherParticles ?? Array.Empty<FOE_Particle>();
            fOE_OtherParticlesDict = new Dictionary<FOEVFX, ParticleSystem>();
            InitDictionary(ref fOE_OtherParticlesDict, fOE_OtherParticles);
        }

        public void Init()
        {
            InitDictionary(ref fOE_OtherParticlesDict, fOE_OtherParticles);
        }

        public ParticleSystem GetVFX(FOEVFX _VFX)
        {
            return GetVFX(fOE_OtherParticlesDict, _VFX);
        }
    }

    public class FOEVFXBase
    {
        public void InitDictionary(ref Dictionary<FOEVFX, ParticleSystem> _dict, FOE_Particle[] _particles)
        {
            foreach (FOE_Particle _particle in _particles)
            {
                Debug.Log($"Particle {_particle.FOEVFX.ToString()} has been added to {_dict}");
                _dict.Add(_particle.FOEVFX, _particle.ParticleSystem);
            }
        }

        public ParticleSystem GetVFX(Dictionary<FOEVFX, ParticleSystem> _dict, FOEVFX _VFX)
        {
            if (_dict.TryGetValue(_VFX, out ParticleSystem particleSystem))
            {
                return particleSystem;
            }

            Debug.LogError($"VFX {_VFX} not found in the dictionary.");
            return null;
        }
    }
}
