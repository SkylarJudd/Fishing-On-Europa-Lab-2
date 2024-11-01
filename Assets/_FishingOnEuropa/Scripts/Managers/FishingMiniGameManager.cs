
using Obvious.Soap;
using Obvious.Soap.Example;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.VolumeComponent;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace Europa
{

    public enum MiniGameState { LureWindrawn, LureCast, LureHitWater, LureReturn, StartMiniGame, RollForHybridCatch,NonCaughtsReturnToSwim, HybridPulling, HybridTied, HybridCaught , HybridEscape }
    public enum FishEncounterState { None, Resting, Fighting, Caught }
    public enum PullDirections { NotSet, Left, Right, Middle }

    [Serializable]
    public class FOELure
    {
        [Header("GameObjects")]
        public GameObject lure_Go;
        public GameObject lure_HybridAttachPoint;
        public Rigidbody lure_RB;

        [Header("Position")]
        public Vector3Reference lure_EndPointTransform; // The current Position of the LureEndPoint

        [Header("Distance")]
        public FloatReference lure_CurrentDistance; // This is the current distance the lure is from the fishing rod end point
        public FloatReference lure_CurrentMaxDistance; // This is the current Max distance the lure is aloud to be from the rods end point, if the line exceeds this point it will add a force to the lure
        public FloatReference lure_MaxDistanceFromRod; // This is the Max distance the lure will ever be, if the lure exceeds this value it will add a force in the direction of the fishing rod

        [Header("Casting Forces")]
        public FloatReference lure_CastingForceMultiplier; // The force multilayer that will be applied to the lure when the lure is casted
        public FloatReference lure_MaxCastingForce; // The Max amount of force that can be applied to the lure when casting

        [Header("Reeling Forces")]
        public FloatReference lure_ReelingForceMultiplier; // The force multilayer that will be applied to the lure when the lure is reeled
        public FloatReference lure_MaxReelingForce; // The Max amount of force that can be applied to the lure when reeling

        [Header("Reset")]
        public FloatReference lure_ResetDistance;  // The distance the lure needs to be from the end of the rod to reset
        public FloatReference lure_CurrentCastResetTime; // The current time elapsed that the reset time has been running for
        public FloatReference lure_CastResetTime; // The max amount of time before the line will reset
        public FloatReference lure_HybridToLureResetTime; // The max amount of time before the line will reset
        public FloatReference lure_ReturnSpeed; // The speed the lure will return to the fishing rod


        [Header("Lure States")]
        public BoolReference lure_InWater;  // A bool that will be toggled when the lure is in the water, True for in water, and false for not in the water
        public FloatReference lure_InWaterTime; // A counter to track how long the lure has been in the water
        public FloatReference lure_InWaterTimeOut; // the max amount of time the lure can be out of the water after being in the water before it will reset itself
        public ScriptableEventNoParam lure_EnterWaterEvent; // An event that will trigger when the lure enters the water
        public ScriptableEventNoParam lure_ExitWaterEvent; // An event that will trigger when the lure exits the water

        [Header("Mini Game")]
        public FloatReference lure_StartingAngle;  // The starting angle of the fishing rod mini game
        public FloatReference lure_MaxAngle;  // The angle the lure can rotate
        public FloatReference lure_LureRotationSpeed;  // The rotation speed of the lure during the mini game
        public FloatReference lure_LureStartingDistance;  // The rotation speed of the lure during the mini game
        public List<float> lure_MiniGameDistancePoints;

        [Header("RB Drag")]
        public FloatReference lure_AirDrag;
        public FloatReference lure_AirRotationalDrag;
        public FloatReference lure_WaterDrag;
        public FloatReference lure_WaterRotationalDrag;


    }

    [Serializable]
    public class FOERod
    {
        [Header("GameObjects")]
        public GameObject rod_Go;
        public GameObject rod_EndPoint;
        public Rigidbody rod_RB;
        public HandEnum rod_Hand;
        public bool rod_Held = false;

        [Header("Position")]
        public Vector3Reference rod_EndPointTransform; // The current Position of the LureEndPoint

        [Header("Rod End Data")]
        public Vector3Reference rod_EndForceDirection; // this keeps track of the direction the rod is traveling in to be used in adding the direction to the lure on cast
        public FloatReference rod_EndCurrentSpeed; // this keeps track of the current speed of the end point of the rod, to be added to the lure on cast. 
        public FloatReference rod_CastSpeedRequired; // this is the min speed the rod needs to be traveling to cast 

        [Header("Player Input")]
        public BoolReference rod_PlayerCastInput; // this keeps track of if the player is holding down the trigger to cast the fishing rod. 

        [Header("MiniGame")]

        public PullDirections rod_RodPullDirectionEnum;
        public FloatReference rod_fishingRodCurrentHP; //how much hp the rod has 
        public FloatReference rod_fishingRodStartingHP; //the starting HP of the fishing Rod 
        public FloatReference rod_fishingEfficiency; //the rate at which hybrid stamina is drained per second

        public BoolReference fightingMiniGameActive; //this Bool will be true when the mini game is active
    }

    [Serializable]
    public class FOEMiniGame
    {

        public PullDirections miniGamePullDirection;
        public int maxHybridsToFind = 5;
        public int hybridCurrentFightAttempts = 0;
        public int hybridMaxFightAttempts = 3;
        public float minHybridPullingDirectionTime = 5;
        public float maxHybridPullingDirectionTime = 10;
        public Vector3 lureRotationAxis;
        public List<FOEItem_Hybrid> hybridsReachedLure;

        public float playerIncorrectPullTimeBuffer = 1;
        public float currentIncorrectPullTimeBuffer = 0;
        public bool resetLureRotation = false;

        public float resetLureTimeOut = 2f;
        public float resetLurecurrentTime = 0;

        public Vector3 playerStartLocation;
        public Vector3 lureStartLocation;

        public float startingAngle;
        public float currentAngleOffset;
        public Transform lureTargetPosition;

        public FloatReference currentMiniGameRNGCount;
        public ScriptableEventBool miniGameLureUIActive;

        public FloatReference currentStaminaOnHybrid;
        public FloatReference startingStaminaOnHybrid;
        public ScriptableEventBool miniGameLureHybridStaminaUIActive;

    }

    [Serializable]
    public class FOEReel
    {
        [Header("Rotation")]
        public FloatReference reel_CurrentReelRotation; // the current rotation of the reel in world space
        public FloatReference reel_SmoothedReelRotation; // the current smoothed rotation of the reel in world space
        public FloatReference reel_LastReelRotation; // the last frames rotation of the reel in world space

        [Header("Smoothing")]
        public IntReference smoothingWindow; // how many frames to smooth the rotation over, used to iron out spikes and maintain an average consistent movement 

        [Header("Player Input")]
        public BoolReference reel_PlayersHandOnReel; // keeps track of when the player has grabbed the reel on the fishing rod, true if grabbed false if not. 

    }

    public class FishingMiniGameManager : Singleton<FishingMiniGameManager>
    {
        public MiniGameState fishingMiniGameState;

        [Header("Lure")]
        public FOELure lure;

        [Header("Rod")]
        public FOERod rod;

        [Header("Reel")]
        public FOEReel reel;

        [Header("Hybrids")]
        public LayerMask fishMask;
        [SerializeField] LayerMask terrainLayerMask;


        [Header("MiniGame")]
        public FOEMiniGame miniGame;


        



        [Header("Coroutines")]
        private Coroutine lineCastCorutine;
        private Coroutine lureHitWaterCoroutine;
        private Coroutine startMiniGameCorutine;
        private Coroutine restingPeriodCoroutine;
        private Coroutine fightingPeriodCotoutine;
        private Coroutine lerpLureToRodCotoutine;
        private Coroutine hyrbidCatchRollCorutine;
        private Coroutine setHybridDirection;



        private float time = 0.0f;
        private float interpolationPeriod = 1f;
        private ParticleSystem _rippleParticleSystem;


        private void Awake()
        {
            lure.lure_CurrentDistance.Variable.OnValueChanged += UpdateLureDistance;
            lure.lure_CurrentMaxDistance.Variable.OnValueChanged += UpdateLureDistance;
            lure.lure_EnterWaterEvent.OnRaised += OnLureEnterWater;
            lure.lure_ExitWaterEvent.OnRaised += OnLureExitWater;

        }

        private void OnDisable()
        {
            lure.lure_CurrentDistance.Variable.OnValueChanged -= UpdateLureDistance;
            lure.lure_CurrentMaxDistance.Variable.OnValueChanged -= UpdateLureDistance;
            lure.lure_EnterWaterEvent.OnRaised -= OnLureEnterWater;
            lure.lure_ExitWaterEvent.OnRaised -= OnLureExitWater;

        }

        private void Start()
        {
            SetupLure();
            SetupRod();
        }

        private void SetupLure()
        {
            lure.lure_RB = lure.lure_Go.GetComponent<Rigidbody>();
            SetRBDrag(lure.lure_AirDrag.Value, lure.lure_AirRotationalDrag.Value);
        }

        private void SetupRod()
        {
            rod.rod_RB = rod.rod_Go.GetComponent<Rigidbody>();
        }

        private void Update()
        {

            if (fishingMiniGameState == MiniGameState.LureWindrawn) UpdateLureWithdrawnPosition(); // Updated the lures position so its attached to the rod


        }

        private void FixedUpdate()
        {
            if (fishingMiniGameState != MiniGameState.LureWindrawn) CaculateDistance();//checks to see if the player has casted the line, and if so this function will be called
        }

        /// <summary>
        /// A function that updates the current location of the lure to the end point of the fishing rod
        /// </summary>
        private void UpdateLureWithdrawnPosition()
        {
            lure.lure_RB.transform.position = rod.rod_EndPointTransform.Value;
        }

        /// <summary>
        /// This function is called from update and calculates the current distance the end of the rod is from the lure every frame when the lure is not withdrawn
        /// </summary>
        private void CaculateDistance()
        {
            lure.lure_CurrentDistance.Value = Vector3.Distance(rod.rod_EndPointTransform.Value, lure.lure_EndPointTransform.Value);
        }

        /// <summary>
        /// This function is called from the input system and is called when the left trigger value is changed. 
        /// </summary>
        /// <param name="_context"></param>
        public void OnPlayerTriggerInputLeft(InputAction.CallbackContext _context)
        {


            if (rod.rod_Hand != HandEnum.LeftHand) return;


            float input = _context.ReadValue<float>(); //Will read the value of the trigger from the input context

            //Dose a bunch of checks to see if the line is able to be casted, and if so calls a function to cast the line
            if (input == 0 && rod.rod_PlayerCastInput.Value == true && fishingMiniGameState == MiniGameState.LureWindrawn && rod.rod_EndCurrentSpeed > rod.rod_CastSpeedRequired)
            {
                OnLineCast();
            }
            //Dose a bunch of checks to check if the line has been cast and the player is able to insta reel in the line, if so the OnLureReturnToRod function will be called. 
            else if (input > 0 && (fishingMiniGameState == MiniGameState.LureCast || fishingMiniGameState == MiniGameState.LureHitWater))
            {
                print("I am calling Lure Return");
                OnLureReturnToRod();
            }

            rod.rod_PlayerCastInput.Value = input > 0 ? true : false;

        }
        /// <summary>
        /// This function is called from the input system and is called when the right trigger value is changed
        /// </summary>
        /// <param name="_context"></param>
        public void OnPlayerTriggerInputRight(InputAction.CallbackContext _context)
        {

            if (rod.rod_Hand != HandEnum.RightHand) return;


            float input = _context.ReadValue<float>();

            //Dose a bunch of checks to see if the line is able to be casted, and if so calls a function to cast the line
            if (input == 0 && rod.rod_PlayerCastInput.Value == true && fishingMiniGameState == MiniGameState.LureWindrawn && rod.rod_EndCurrentSpeed > rod.rod_CastSpeedRequired)
            {
                OnLineCast();
            }
            //Dose a bunch of checks to check if the line has been cast and the player is able to insta reel in the line, if so the OnLureReturnToRod function will be called. 
            else if (input > 0 && (fishingMiniGameState == MiniGameState.LureCast || fishingMiniGameState == MiniGameState.LureHitWater))
            {
                print("I am calling Lure Return");
                OnLureReturnToRod();
            }

            rod.rod_PlayerCastInput.Value = input > 0 ? true : false;

        }


        public void OnLineCast()
        {
            if (fishingMiniGameState == MiniGameState.LureWindrawn)
            {
                updateMiniGameState(MiniGameState.LureCast);

                if (lineCastCorutine != null)
                    StopCoroutine(lineCastCorutine);

                lineCastCorutine = StartCoroutine(LineCastCoroutine());
                // Start a timer, if the lure dose not enter the water in a set amount of time, it will return. 
            }
        }
        public void OnLureHitWater()
        {
            if (fishingMiniGameState == MiniGameState.LureCast)
            {

                updateMiniGameState(MiniGameState.LureHitWater);

                if (lureHitWaterCoroutine != null)
                    StopCoroutine(lureHitWaterCoroutine);

                lureHitWaterCoroutine = StartCoroutine(LureHitWaterCoroutine());
                // Will start a timer to make sure the lure stays in the water if it dose not it will restart the timer. 
            }
        }
        public void OnLureReturnToRod()
        {
            updateMiniGameState(MiniGameState.LureReturn);
            // Starts a coroutine reducing the current max distance until the lure arrives back to the rod. 

            if (lerpLureToRodCotoutine != null)
                StopCoroutine(lerpLureToRodCotoutine);

            lerpLureToRodCotoutine = StartCoroutine(LerpLureToRod());

        }
        public void OnLureArriveAtRod()
        {
            updateMiniGameState(MiniGameState.LureWindrawn);
            // Will Reset the mini Game and make sure everything is set back to its starting values. 
        }
        public void OnStartMiniGame()
        {
            updateMiniGameState(MiniGameState.StartMiniGame);
            startMiniGameCorutine = StartCoroutine(StartMiniGameCoroutine());
            // Will cause the 5 closest hybrids to swim towards the lure and wait for them to send a message saying they have arrived
        }
        public void OnHybridsArrived()
        {
            updateMiniGameState(MiniGameState.RollForHybridCatch);

            hyrbidCatchRollCorutine = StartCoroutine(HyrbidCatchRoll());
            // Once all the hybrids have arrived it Will start a Coroutine that will visually show each of the hybrids taking a bite of the bait in ascending order based on rarity. 
        }
        public void OnHybridAssignedToCatch()
        {

            updateMiniGameState(MiniGameState.NonCaughtsReturnToSwim);
            OnHybridFighting();
            //Once a hybrid has been assigned, It will loop through the rest of the hybrids and return them to swimming, and set the caught hybrid to pulling
        }
        public void OnHybridTried()
        {
            updateMiniGameState(MiniGameState.HybridTied);
            restingPeriodCoroutine = StartCoroutine(RestingPeriodCoroutine());
        }
        public void OnHybridFighting()
        {
            updateMiniGameState(MiniGameState.HybridPulling);
            // will make the fish pull left and right and compare the players movements left and right, if the player is pulling in the incorrect direction then it will add
            fightingPeriodCotoutine = StartCoroutine(FightingPeriodCotoutine());

        }
        public void OnHybridCaught()
        {
            updateMiniGameState(MiniGameState.LureWindrawn);
            // Will need to set the hybrid state to be an item and remove it from the navigation list. 
        }

        public void OnHybridEscape()
        {
            updateMiniGameState(MiniGameState.HybridEscape);

            StopCoroutine(fightingPeriodCotoutine);

            ResetHybrid(_FNAVM.caughtHybrid);


            _FNAVM.caughtHybrid = null;
            print("I am calling Lure Return");
            OnLureReturnToRod();
        }

        /// <summary>
        /// A function that updates the state of the mini game to the inputted state
        /// </summary>
        /// <param name="_State"></param>
        private void updateMiniGameState(MiniGameState _State)
        {
            fishingMiniGameState = _State;
        }

        /// <summary>
        /// a Function that is used to check how long the lure has been in its casting state, aka not hit the water, if the lure dose not come into contact with water it will return to the fishing rod
        /// </summary>
        /// <returns></returns>
        private IEnumerator LineCastCoroutine()
        {
            lure.lure_RB.AddForce(rod.rod_EndForceDirection.Value * rod.rod_EndCurrentSpeed.Value * lure.lure_CastingForceMultiplier.Value);

            lure.lure_CurrentCastResetTime.Value = 0;  //sets the value to 0 so this function is self resetting

            lure.lure_CurrentMaxDistance.Value = lure.lure_MaxDistanceFromRod.Value; //Sets the current distance of the fishing rod to the max distance

            while (fishingMiniGameState == MiniGameState.LureCast)  // Keeps looping while the lure has been casted and has not hit the water
            {
                lure.lure_CurrentCastResetTime.Value += Time.fixedDeltaTime;  // Adds to CurrentCastResetTime timer every frame

                if (lure.lure_CurrentCastResetTime.Value >= lure.lure_CastResetTime.Value) // check to see if the CurrentCastResetTime has exceeded the CastResetTime and if so will return the lure to the rod
                {
                    Debug.LogWarning($"Lure Has Been Cast for more then {lure.lure_CastResetTime.Value.ToString()} seconds without hitting the water, Returning Lure To rod");
                    print("I am calling Lure Return");
                    OnLureReturnToRod();
                    yield break;
                }


                yield return new WaitForFixedUpdate();
            }

        }

        /// <summary>
        /// This function is used to check of the lure has settled in the water or has bounced out, if the lure has bounced it it has _lure.lure_CastResetTime.Value amount of time to get back in the water
        /// </summary>
        /// <returns></returns>
        private IEnumerator LureHitWaterCoroutine()
        {
            lure.lure_InWaterTime.Value = 0;  // sets the value back to 0 so this function is self resetting
            lure.lure_CurrentMaxDistance.Value = lure.lure_CurrentDistance.Value;
            lure.lure_LureStartingDistance.Value = lure.lure_CurrentDistance.Value;

            while (fishingMiniGameState == MiniGameState.LureHitWater)  // Keeps looping while the lure is in the state of hit water. 
            {
                if (lure.lure_InWater == true)   // This will be set to true when the lure is in a water source
                {
                    lure.lure_InWaterTime.Value += Time.fixedDeltaTime;
                    if (lure.lure_InWaterTime.Value >= lure.lure_CastResetTime.Value)
                    {
                        OnStartMiniGame();
                        yield break;
                    }
                }
                else if (lure.lure_InWater == false)  // This will be set to false when the lure is not in a water source
                {
                    lure.lure_InWaterTime.Value -= Time.fixedDeltaTime;
                    if (lure.lure_InWaterTime.Value <= -lure.lure_CastResetTime.Value)
                    {
                        print("I am calling Lure Return");
                        OnLureReturnToRod();
                        yield break;
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// This Function Will start the mini game set all the values that are needed and then find the 5 closest hybrids to the lure then wait till they have all arrived at the lure
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartMiniGameCoroutine()
        {
            SetUpMiniGame();

            List<FOEItem_Hybrid> _convertedHybridList = ConvertToNormalList(_FNAVM._hybridsToNavList); // converts the hybrid nav list to a normal list so we can use sorting functions

            if (_convertedHybridList == null)
            {
                Debug.LogError($"No hybrids where found in the list, make sure hybrids have spawned before starting the mini game, if hybrids are spawned something has gone wrong");
                print("I am calling Lure Return");
                OnLureReturnToRod();
                yield break;
            }
            else
            {


                List<FOEItem_Hybrid> _foundHybrids = GetClosestHybrids(_convertedHybridList); //gets the 5 closest hybrids in the list

                miniGame.hybridsReachedLure.Clear();  // sets the hybrid reached nav points back to 0 so its self resetting, this value is iterated each time a hybrid reaches its destination. 
                float HybirdReachedNavPointTimeout = 0;

                foreach (FOEItem_Hybrid _hybrid in _foundHybrids)
                {
                    //remove Hybrid From any lists
                    _FNAVM.removeHybrid(_hybrid, false);

                    //Set Hybrid to nav point
                    _FNAVM.AssignHybridWithTargetAndState(_hybrid, lure.lure_HybridAttachPoint.transform);

                }

                while (miniGame.hybridsReachedLure.Count <= _convertedHybridList.Count && HybirdReachedNavPointTimeout < lure.lure_HybridToLureResetTime.Value)
                {
                    HybirdReachedNavPointTimeout += Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }

                if (miniGame.hybridsReachedLure.Count != 0)
                {
                    OnHybridsArrived();
                    yield break;
                }
                else
                {
                    Debug.LogWarning($"was not able to find any hybrids, if there are hybrid spawned, then something is going very wrong");
                    print("I am calling Lure Return");
                    OnLureReturnToRod();
                    yield break;
                }
            }
        }

        /// <summary>
        /// Converts the ScriptableListFOEItem_Hybrid to a normal list so we can use sorting functions bc soap lists done have sorting for some reason reeeee
        /// </summary>
        /// <param name="_HybridList"></param>
        /// <returns></returns>
        private List<FOEItem_Hybrid> ConvertToNormalList(ScriptableListFOEItem_Hybrid _HybridList)
        {
            List<FOEItem_Hybrid> _hybridList = new List<FOEItem_Hybrid>();

            foreach (FOEItem_Hybrid _hybrid in _HybridList)
            {
                _hybridList.Add(_hybrid);
            }

            return _hybridList;
        }

        /// <summary>
        /// A function that is called at the start of the mini game for setting any values that need to be reset before the game starts
        /// </summary>
        private void SetUpMiniGame()
        {
            lure.lure_MiniGameDistancePoints.Clear();
            miniGame.hybridCurrentFightAttempts = 0;
            rod.rod_fishingRodCurrentHP.Value = rod.rod_fishingRodStartingHP;

            // Calculate divisions of the starting distance
            float division = lure.lure_LureStartingDistance / miniGame.hybridMaxFightAttempts;

            // Assign calculated points to the list dynamically based on HybridMaxFightAttempts
            for (int i = miniGame.hybridMaxFightAttempts; i > 0; i--)
            {
                lure.lure_MiniGameDistancePoints.Add(division * i);
            }
            lure.lure_MiniGameDistancePoints.Add(0);
        }
        /// <summary>
        /// This function will lerp the lures current max distance towards 0 causing the lure to move towards the fishing rod.  
        /// </summary>
        /// <returns></returns>
        private IEnumerator LerpLureToRod()
        {
            while (fishingMiniGameState == MiniGameState.LureReturn)
            {
                // Debug.Log($"Returning to Lure {_lure.lure_CurrentDistance.Value}");
                if (lure.lure_CurrentMaxDistance.Value < 0.1f)
                {
                    lure.lure_CurrentMaxDistance.Value = 0;
                    UpdateLureDistance(0);
                    yield break;
                }
                else
                {
                    lure.lure_CurrentMaxDistance.Value = Mathf.Lerp(lure.lure_CurrentMaxDistance.Value, 0, lure.lure_ReturnSpeed.Value * Time.fixedDeltaTime);
                }

                yield return new WaitForFixedUpdate();
            }

        }

        /// <summary>
        /// Finds the 5 closest FOEItem_Hybrids and returns them as a list.
        /// </summary>
        /// <param name="_hybrids">List of FOEItem_Hybrid objects to check distance from.</param>
        /// <returns>List of 5 closest FOEItem_Hybrid objects.</returns>
        private List<FOEItem_Hybrid> GetClosestHybrids(List<FOEItem_Hybrid> _Hybrid)
        {
            // Check if there are fewer than 5 hybrids to avoid unnecessary processing
            if (_Hybrid.Count <= miniGame.maxHybridsToFind)
                return new List<FOEItem_Hybrid>(_Hybrid);

            // Sort the hybrids based on distance to the lure's end point
            _Hybrid.Sort((a, b) =>
                Vector3.Distance(lure.lure_EndPointTransform.Value, a.europaItemData.itemTransform.position)
                .CompareTo(Vector3.Distance(lure.lure_EndPointTransform.Value, b.europaItemData.itemTransform.position))
            );

            // Return the top 5 closest hybrids
            return _Hybrid.GetRange(0, 5);
        }

        /// <summary>
        /// This function is used calculate the caught hybrid, by scaling all their catch chances between 1 - 100, it then orders them and begins counting up an int. Hybrids will return to swimming,
        /// their number is passed, but if it stops on their number then that will be the caught hybrid. 
        /// </summary>
        /// <returns></returns>
        private IEnumerator HyrbidCatchRoll()
        {
            // use the list of the hybrids attached to the lure to calculate the catch change of each of them. 
            CaculateCatchChanceList();
            // get a random number between 0 - 100
            int randomNumber = UnityEngine.Random.Range(1, 100);

            randomNumber = 99;

            // work out what hybrid will be caught based off this number
            float total = 0;
            foreach (FOEItem_Hybrid _hybrid in miniGame.hybridsReachedLure)
            {
                total += _hybrid.navigationData.scaledCatchChance;

                if (total > randomNumber)
                {
                    _FNAVM.caughtHybrid = _hybrid;
                    Debug.Log($"Caught Hybrid = {_hybrid.name}");
                    break;
                }
            }

            int index = 0;
            total = miniGame.hybridsReachedLure[index].navigationData.scaledCatchChance;
            miniGame.hybridsReachedLure[index].navigationData.orbitDistance = 0.3f;
            miniGame.miniGameLureUIActive.Raise(true);

            for (miniGame.currentMiniGameRNGCount.Value = 1; miniGame.currentMiniGameRNGCount.Value < randomNumber; miniGame.currentMiniGameRNGCount.Value++)
            {

                if (index >= miniGame.hybridsReachedLure.Count - 1)
                {
                    Debug.LogWarning("Index reached the end of the list. Breaking early.");
                    break;  // Prevent out-of-bounds access.
                }

                if (miniGame.currentMiniGameRNGCount.Value > total)
                {
                    if (_FNAVM.caughtHybrid == miniGame.hybridsReachedLure[index])
                    {
                        // Play Bite Animate for Hybrid
                        print(" Hybrid Bites");

                    }
                    else
                    {
                        //stop the animation the hybrid sussing out the lure for the current index
                        //return the hybrid to swimming and reset its values
                        ResetHybrid(miniGame.hybridsReachedLure[index]);
                        lure.lure_RB.AddForce(Vector3.down * 10f, ForceMode.Impulse);


                        if (rod.rod_Held)
                        {
                            //adds a force to the rod that makes it rotate downwards towards the _lure.lure_tranform

                            // Calculate the direction from the rod to the lure.
                            Vector3 directionToLure = (lure.lure_EndPointTransform.Value - rod.rod_EndPointTransform.Value).normalized;

                            // Calculate the desired rotation as a quaternion to point the rod toward the lure.
                            Quaternion targetRotation = Quaternion.LookRotation(directionToLure, Vector3.up);

                            // Calculate the difference between the current rotation and target rotation.
                            Quaternion rotationDifference = targetRotation * Quaternion.Inverse(rod.rod_Go.transform.rotation);

                            // Convert the quaternion difference into an angular velocity.
                            Vector3 angularVelocity = new Vector3(
                                rotationDifference.x,
                                rotationDifference.y,
                                rotationDifference.z
                            ) * 10f;  // Adjust 10f to control rotation speed

                            // Apply torque to the rod's Rigidbody to rotate it toward the lure.
                            rod.rod_RB.AddTorque(angularVelocity * 20f, ForceMode.Impulse);
                            rod.rod_RB.AddForce(directionToLure * 3f, ForceMode.Impulse);


                            _HM.PlayHaptic(rod.rod_Hand);
                        }


                        _VFXM.PlayVFX(FOEVFXClass.Water, FOEVFX.W_Splash3, lure.lure_Go.transform, false);

                    }



                    index++;
                    miniGame.hybridsReachedLure[index].navigationData.orbitDistance = 0.2f;
                    total += miniGame.hybridsReachedLure[index].navigationData.scaledCatchChance;
                }

                // animate the current index hybrid to look like its sussing out the  lure
                yield return new WaitForSeconds(0.1f);
            }

            lure.lure_RB.AddForce(Vector3.down * 20f, ForceMode.Impulse);

            //TODO Play test this, if the game need a snagging mechanics to catch they hybrid add this here. Start a coroutine and check if the player pulls back on the fishing rod, 

            miniGame.miniGameLureUIActive.Raise(false);

            foreach (FOEItem_Hybrid _hybrid in miniGame.hybridsReachedLure)
            {
                if (_hybrid != _FNAVM.caughtHybrid)
                {
                    ResetHybrid(_hybrid);
                }
            }

            //remove from all lists and put in Mini Game List 
            _FNAVM.removeHybrid(_FNAVM.caughtHybrid, false);
            _FNAVM.caughtHybrid.navigationData.hybridState = HybridState.HybridMiniGame_Pulling;
            _FNAVM.StartMiniGame(); //Starts the MiniGame Coroutine in the Fish Navigation Manager

            OnHybridAssignedToCatch();
        }

        /// <summary>
        /// a function that resets the hybrids before the return to swimming
        /// </summary>
        /// <param name="_Hybrid"></param>
        private void ResetHybrid(FOEItem_Hybrid _Hybrid)
        {

            _Hybrid.navigationData.scaledCatchChance = 0;
            _Hybrid.navigationData.arrivedAtLure = false;
            _Hybrid.navigationData.firstNav = true;
            //_Hybrid.navigationData.itemTarget = null;

            _FNAVM.removeHybrid(_Hybrid, false);
            _FNAVM.AddHybridTolist(_Hybrid, HybridState.HybridFlocking);

        }

        /// <summary>
        /// This sets the catch chances for the hybrids in hybridsReachedLure to be between 0 - 100 and then orders them in a list
        /// </summary>
        private void CaculateCatchChanceList()
        {
            float total = 0;

            foreach (FOEItem_Hybrid _hybrid in miniGame.hybridsReachedLure)
            {
                total += _hybrid.hybridSO.catchChance;
                _hybrid.navigationData.hybridState = HybridState.HybirdSwimAroundLure;
                _hybrid.navigationData.orbitDirection = UnityEngine.Random.Range(0, 2) == 0;
                _hybrid.navigationData.orbitDistance = 1f;
            }

            foreach (FOEItem_Hybrid _hybrid in miniGame.hybridsReachedLure)
            {
                _hybrid.navigationData.scaledCatchChance = (_hybrid.hybridSO.catchChance / total) * 100f;

            }

            miniGame.hybridsReachedLure.Sort((y, x) => x.navigationData.scaledCatchChance.CompareTo(y.navigationData.scaledCatchChance));
        }

        /// <summary>
        /// Set rest period time and change fishEncounterState
        /// </summary>
        private IEnumerator RestingPeriodCoroutine()
        {
            Debug.Log("Starting Hybrid Resting");


            // Set the rest time to a value between the hybrids max rest time and their min rest time
            _FNAVM.caughtHybrid.navigationData.hybridRestTime = UnityEngine.Random.Range(_FNAVM.caughtHybrid.hybridSO.restMinTime, _FNAVM.caughtHybrid.hybridSO.restMaxTime);
            //remove from all lists and put in Mini Game List 
            _FNAVM.removeHybrid(_FNAVM.caughtHybrid, false);
            _FNAVM.AddHybridTolist(_FNAVM.caughtHybrid, HybridState.HybridMiniGame_Tired);
            //Sets the MiniGame Active Bool To False
            rod.fightingMiniGameActive.Value = false;



            while (_FNAVM.caughtHybrid.navigationData.hybridRestTime >= 0 || lure.lure_CurrentDistance < lure.lure_MiniGameDistancePoints[miniGame.hybridCurrentFightAttempts])
            {
                _FNAVM.caughtHybrid.navigationData.hybridRestTime -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            OnHybridFighting();
        }

        /// <summary>
        /// A function that deals with the fighting stage  of the hybrid Mini game
        /// </summary>
        /// <returns></returns>
        private IEnumerator FightingPeriodCotoutine()
        {
            Debug.Log("Starting Hybrid Fighting");

            miniGame.hybridCurrentFightAttempts++;
            if (lure.lure_MiniGameDistancePoints.Count < miniGame.hybridMaxFightAttempts) miniGame.hybridCurrentFightAttempts = miniGame.hybridMaxFightAttempts;


            SetUpMiniGamePulling();


            while (_FNAVM.caughtHybrid.navigationData.currentHybridStamina > 0)  // Checks to see if the hybrids Stamina is > 0 
            {

                RotateLure();
                CaculateRodHP();




                _FNAVM.caughtHybrid.navigationData.currentHybridStamina -= Time.deltaTime;  // Subtracts the hybrids stamina each frame based of time.delta time
                miniGame.currentStaminaOnHybrid.Value = _FNAVM.caughtHybrid.navigationData.currentHybridStamina;
                yield return new WaitForEndOfFrame();
            }

            StopCoroutine(setHybridDirection);  // Stops the Coroutine that will randomly change the direction the hybrid is pulling
            miniGame.resetLureRotation = false;  // Sets the ResetLureRotation To False, this value is set to true once the lure reaches its starting angle
            FlipRotationAxis();

            while (miniGame.resetLureRotation == false)
            {
                //(miniGame.lureRotationAxis, forward, targetDirection); // calls a function to return the lure to the starting location
                yield return new WaitForEndOfFrame();
            }

            OnHybridTried();
        }

        private void RotateLure()
        {
            float deltaAngle = lure.lure_LureRotationSpeed.Value * Time.deltaTime;
            miniGame.currentAngleOffset = Vector3.Angle(_PLAYER.transform.forward, miniGame.lureTargetPosition.transform.position - _PLAYER.transform.position) - miniGame.startingAngle;

            // Calculate the shortest path to rotate back within the valid angle range
            if (Mathf.Abs(miniGame.currentAngleOffset) < lure.lure_MaxAngle.Value)
            {
                miniGame.lureTargetPosition.RotateAround(_PLAYER.transform.position, miniGame.lureRotationAxis, deltaAngle);
            }
            else
            {
                // Gradually rotate back towards the maximum allowed angle
                float correctionAngle = Mathf.Sign(miniGame.currentAngleOffset) * deltaAngle;

                if (miniGame.lureRotationAxis == Vector3.up)
                {
                    miniGame.lureTargetPosition.RotateAround(_PLAYER.transform.position, miniGame.lureRotationAxis, -correctionAngle);
                }
                else
                {
                    miniGame.lureTargetPosition.RotateAround(_PLAYER.transform.position, miniGame.lureRotationAxis, correctionAngle);
                }

            }

        }

        private void CaculateRodHP()
        {
            if (miniGame.miniGamePullDirection != rod.rod_RodPullDirectionEnum)
            {
                //player is not pulling the correct Direction;
                //Checks to see if the player has been pulling in the incorrect direction for more then the playerIncorrectPullTimeBuffer
                if (miniGame.currentIncorrectPullTimeBuffer <= miniGame.playerIncorrectPullTimeBuffer)
                {
                    miniGame.currentIncorrectPullTimeBuffer += Time.deltaTime;
                }
                else
                {
                    rod.rod_fishingRodCurrentHP.Value -= _FNAVM.caughtHybrid.hybridSO.damageToRod * Time.deltaTime;
                    if (rod.rod_fishingRodCurrentHP.Value <= 0)
                    {
                        print("I called Hybrid Escaped");
                        OnHybridEscape();
                    }
                }

            }
            else
            {
                // Player is pulling the correct Direction;
                miniGame.currentIncorrectPullTimeBuffer = 0;
            }
        }

        private void SetUpMiniGamePulling()
        {
            setHybridDirection = StartCoroutine(SetHybridDirection());

            SetupHybridPulling();
            SetUpMiniGamePreCaculations();


        }

        private void SetupHybridPulling()
        {
             // Set the current Stamina to the hybrids max stamina;
            _FNAVM.caughtHybrid.navigationData.currentHybridStamina = _FNAVM.caughtHybrid.hybridSO.stamina;
            miniGame.currentStaminaOnHybrid.Value = _FNAVM.caughtHybrid.hybridSO.stamina;
            miniGame.startingStaminaOnHybrid.Value = _FNAVM.caughtHybrid.hybridSO.stamina;
            miniGame.miniGameLureHybridStaminaUIActive.Raise(true);
            Debug.Log($" Hybrids Current stamina = {_FNAVM.caughtHybrid.navigationData.currentHybridStamina}");
        
        }

        private void SetUpMiniGamePreCaculations()
        {
            miniGame.playerStartLocation = _PLAYER.transform.position;
            miniGame.lureStartLocation = lure.lure_Go.transform.position;

            miniGame.startingAngle = Vector3.Angle(_PLAYER.transform.forward, lure.lure_Go.transform.position - _PLAYER.transform.position);
            miniGame.lureTargetPosition.position = lure.lure_Go.transform.position;

            rod.rod_fishingRodCurrentHP.Value = rod.rod_fishingRodStartingHP.Value;
        }

      

      

        /// <summary>
        /// Sets the Direction the hybrid is pulling
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetHybridDirection()
        {
            int random = UnityEngine.Random.Range(1, 3);
            miniGame.lureRotationAxis = random == 1 ? miniGame.lureRotationAxis = Vector3.up : miniGame.lureRotationAxis = Vector3.down;
            miniGame.miniGamePullDirection = random == 1 ? PullDirections.Left : PullDirections.Right;

            while (fishingMiniGameState == MiniGameState.HybridPulling)
            {
                FlipRotationAxis();
                yield return new WaitForSeconds(UnityEngine.Random.Range(miniGame.minHybridPullingDirectionTime, miniGame.maxHybridPullingDirectionTime));
            }
        }

        private void FlipRotationAxis()
        {
            Debug.Log($"Rotation was {miniGame.lureRotationAxis}");
            miniGame.lureRotationAxis = miniGame.lureRotationAxis == Vector3.down ? Vector3.up : Vector3.down;
            miniGame.miniGamePullDirection = miniGame.lureRotationAxis == Vector3.down ? PullDirections.Right : PullDirections.Left;
            Debug.Log($"Rotation is now {miniGame.lureRotationAxis}");
        }

        private void RotateLure(Vector3 _rotationAxis,  Vector3 _forward, Vector3 _targetDir, float _SpeedMutiplayer = 1f)
        {
            // Rotates the lure around the Player based, the _RotationAxis determines the direction of the rotation

            float deltaAngle;
            float currentAngle;

            (deltaAngle, currentAngle) = CaculateAngle(_SpeedMutiplayer, _forward, _targetDir );

            //rotate bobber around player
            if (Mathf.Abs(currentAngle) < lure.lure_MaxAngle.Value && !Physics.CheckSphere(lure.lure_Go.transform.position, 0.5f, terrainLayerMask))
            {
                RotateLureAroundPlayer(_rotationAxis, deltaAngle);
            }
            else if (Physics.CheckSphere(lure.lure_Go.transform.position, 0.5f, terrainLayerMask))
            {
                Debug.LogWarning("Hybrid Hit Land and got away");
                print("I called Hybrid Escaped");
                OnHybridEscape();
            }
        }



        private void ResetLureRotation(Vector3 _rotationAxis, Vector3 _forward, Vector3 _targetDir,  float _SpeedMutiplayer = 1.5f)
        {
            float deltaAngle;
            float currentAngle;

            (deltaAngle, currentAngle) = CaculateAngle(_SpeedMutiplayer, _forward, _targetDir);

            //rotate bobber around player
            if (Mathf.Abs(currentAngle) > 0.5f && !Physics.CheckSphere(lure.lure_Go.transform.position, 0.5f, terrainLayerMask) && miniGame.resetLurecurrentTime < miniGame.resetLureTimeOut)
            {
                RotateLureAroundPlayer(_rotationAxis, deltaAngle);
            }
            else if (Mathf.Abs(currentAngle) <= 0.5f)
            {
                miniGame.resetLureRotation = true;
            }
            else if (Physics.CheckSphere(lure.lure_Go.transform.position, 0.5f, terrainLayerMask))
            {
                Debug.LogWarning("Hybrid Hit Land and got away");
                print("I called Hybrid Escaped");
                OnHybridEscape();
            }
            else if (miniGame.resetLurecurrentTime > miniGame.resetLureTimeOut)
            {
                miniGame.resetLureRotation = true;
            }

            miniGame.resetLurecurrentTime += Time.deltaTime;
        }

        private (float, float) CaculateAngle(float _SpeedMutiplayer, Vector3 _forward , Vector3 _targetDir)
        {
            float deltaAngle = (lure.lure_LureRotationSpeed.Value * _SpeedMutiplayer) * Time.deltaTime;
            float currentAngle = Vector3.Angle(_targetDir, _forward) - lure.lure_StartingAngle.Value;

            return ( deltaAngle, currentAngle );
        }

        private void RotateLureAroundPlayer(Vector3 _rotationAxis , float _deltaAngle) => lure.lure_Go.transform.RotateAround(rod.rod_EndPointTransform.Value, _rotationAxis, _deltaAngle);
       


        public bool CheckForHybridCollision(Vector3 direction)
        {
            if (!Physics.Raycast(lure.lure_HybridAttachPoint.transform.position, direction, 3f, terrainLayerMask))
            {
                return true; //no collision
            }
            else return false; //there is a collision
        }

        /// <summary>
        /// This function updates the distance of the lure when the current distance changes or when the max distance changes
        /// </summary>
        /// <param name="newValue"></param>
        private void UpdateLureDistance(float newValue)
        {
            if (fishingMiniGameState == MiniGameState.LureCast)
                return;

            
            if (newValue <= lure.lure_ResetDistance.Value && fishingMiniGameState == MiniGameState.HybridTied)
            {
                OnHybridCaught();
            }
            else if (newValue <= lure.lure_ResetDistance.Value * 4 && fishingMiniGameState == MiniGameState.LureReturn)
            {
                OnLureArriveAtRod();
            }
            else if (newValue <= lure.lure_ResetDistance.Value)
            {
                OnLureArriveAtRod();
            }

            if (fishingMiniGameState == MiniGameState.LureCast || fishingMiniGameState == MiniGameState.LureHitWater || fishingMiniGameState == MiniGameState.HybridTied || fishingMiniGameState == MiniGameState.LureReturn)
            {
                // Check if the current distance exceeds the allowed max distance
                if (lure.lure_CurrentDistance.Value > lure.lure_CurrentMaxDistance.Value)
                {
                    // Calculate how much the current distance exceeds the max allowed distance
                    float excessDistance = lure.lure_CurrentDistance.Value - lure.lure_CurrentMaxDistance.Value;

                    // Calculate direction from the lure to the fishing rod
                    Vector3 directionToRod = (rod.rod_EndPointTransform.Value - lure.lure_EndPointTransform.Value).normalized;

                    // Calculate the force based on the excess distance and force multiplier
                    Vector3 force = directionToRod * excessDistance * lure.lure_ReelingForceMultiplier;

                    // Clamp the magnitude of the force to the specified maxForce
                    force = Vector3.ClampMagnitude(force, lure.lure_MaxReelingForce);

                    // Apply the clamped force to the Rigidbody
                    lure.lure_RB.AddForce(force, ForceMode.Force);

                }
            }
        }

        private void OnLureEnterWater()
        {
            OnLureHitWater();
            lure.lure_InWater.Value = true;

            SetRBDrag(lure.lure_WaterDrag.Value, lure.lure_WaterRotationalDrag.Value);
            _VFXM.PlayVFX(FOEVFXClass.Water, FOEVFX.W_Splash2, lure.lure_Go.transform, false);
            _VFXM.PlayVFX(FOEVFXClass.Water, FOEVFX.W_Ripples1, lure.lure_Go.transform, Quaternion.identity, true, out _rippleParticleSystem);

        }

        private void OnLureExitWater()
        {
            lure.lure_InWater.Value = false;
            SetRBDrag(lure.lure_AirDrag.Value, lure.lure_AirRotationalDrag.Value);
            _VFXM.StopVFX(_rippleParticleSystem);
        }

        private void SetRBDrag(float _drag, float _rotDrag)
        {
            lure.lure_RB.linearDamping = _drag;
            lure.lure_RB.angularDamping = _rotDrag;

        }
    }
}



