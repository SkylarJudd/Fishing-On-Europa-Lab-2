
using Obvious.Soap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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


    }

    [Serializable]
    public class FOERod
    {
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
        public FOELure _lure;

        [Header("Rod")]
        public FOERod _rod;

        [Header("Reel")]
        public FOEReel _reel;

        [Header("Hybrids")]
        public LayerMask fishMask;
        [SerializeField] LayerMask terrainLayerMask;
        public Collider[] nearbyHybrids;

        [Header("MiniGame")]
        [SerializeField] private PullDirections miniGamePullDirection;
        [SerializeField] private int maxHybridsToFind = 5;
        [SerializeField] private int hybridCurrentFightAttempts = 0;
        [SerializeField] private int hybridMaxFightAttempts = 3;
        [SerializeField] private float minHybridPullingDirectionTime = 5;
        [SerializeField] private float maxHybridPullingDirectionTime = 10;
        [SerializeField] private Vector3 lureRotationAxis;
        [SerializeField] public List<FOEItem_Hybrid> hybridsReachedLure;

        [SerializeField] private float playerIncorrectPullTimeBuffer = 1;
        [SerializeField] private float currentIncorrectPullTimeBuffer = 0;
        [SerializeField] private bool resetLureRotation = false;

        [SerializeField] private float resetLureTimeOut = 2f;
        [SerializeField] private float resetLurecurrentTime = 0;





        [Header("Coroutines")]
        private Coroutine lineCastCorutine;
        private Coroutine lureHitWaterCoroutine;
        private Coroutine startMiniGameCorutine;
        private Coroutine restingPeriodCoroutine;
        private Coroutine fightingPeriodCotoutine;

        private Coroutine setHybridDirection;



        private float time = 0.0f;
        private float interpolationPeriod = 1f;


        private void Awake()
        {
            _lure.lure_CurrentDistance.Variable.OnValueChanged += UpdateLureDistance;
            _lure.lure_CurrentMaxDistance.Variable.OnValueChanged += UpdateLureDistance;
            _lure.lure_EnterWaterEvent.OnRaised += OnLureEnterWater;
            _lure.lure_ExitWaterEvent.OnRaised += OnLureExitWater;

        }

        private void OnDisable()
        {
            _lure.lure_CurrentDistance.Variable.OnValueChanged -= UpdateLureDistance;
            _lure.lure_CurrentMaxDistance.Variable.OnValueChanged -= UpdateLureDistance;
            _lure.lure_EnterWaterEvent.OnRaised -= OnLureEnterWater;
            _lure.lure_ExitWaterEvent.OnRaised -= OnLureExitWater;

        }

        private void Start()
        {
            SetupLure();
        }

        private void SetupLure()
        {
            _lure.lure_RB = _lure.lure_Go.GetComponent<Rigidbody>();
        }

        private void Update()
        {

            if (fishingMiniGameState == MiniGameState.LureWindrawn) UpdateLureWithdrawnPosition(); // Updated the lures position so its attached to the rod
            if (fishingMiniGameState != MiniGameState.LureWindrawn) CaculateDistance();//checks to see if the player has casted the line, and if so this function will be called

        }
        /// <summary>
        /// A function that updates the current location of the lure to the end point of the fishing rod
        /// </summary>
        private void UpdateLureWithdrawnPosition()
        {
            _lure.lure_RB.transform.position = _rod.rod_EndPointTransform.Value;
        }

        /// <summary>
        /// This function is called from update and calculates the current distance the end of the rod is from the lure every frame when the lure is not withdrawn
        /// </summary>
        private void CaculateDistance()
        {
            _lure.lure_CurrentDistance.Value = Vector3.Distance(_rod.rod_EndPointTransform.Value, _lure.lure_EndPointTransform.Value);
        }

        /// <summary>
        /// This function is called from the input system and is called when the left trigger value is changed. 
        /// </summary>
        /// <param name="_context"></param>
        public void OnPlayerTriggerInputLeft(InputAction.CallbackContext _context)
        {

            //TODO Check if the player is holding the rod in their right hand and if not return

            print("Left Trigger Pressed");
            float input = _context.ReadValue<float>(); //Will read the value of the trigger from the input context

            //Dose a bunch of checks to see if the line is able to be casted, and if so calls a function to cast the line
            if (input == 0 && _rod.rod_PlayerCastInput.Value == true && fishingMiniGameState == MiniGameState.LureWindrawn && _rod.rod_EndCurrentSpeed > _rod.rod_CastSpeedRequired)
            {
                OnLineCast();
            }
            //Dose a bunch of checks to check if the line has been cast and the player is able to insta reel in the line, if so the OnLureReturnToRod function will be called. 
            else if (input > 0 && (fishingMiniGameState == MiniGameState.LureCast || fishingMiniGameState == MiniGameState.LureHitWater))
            {
                OnLureReturnToRod();
            }

            _rod.rod_PlayerCastInput.Value = input > 0 ? true : false;

        }
        /// <summary>
        /// This function is called from the input system and is called when the right trigger value is changed
        /// </summary>
        /// <param name="_context"></param>
        public void OnPlayerTriggerInputRight(InputAction.CallbackContext _context)
        {

            //TODO Check if the player is holding the rod in their right hand and if not return

            print("Right Trigger Pressed");
            float input = _context.ReadValue<float>();

            //Dose a bunch of checks to see if the line is able to be casted, and if so calls a function to cast the line
            if (input == 0 && _rod.rod_PlayerCastInput.Value == true && fishingMiniGameState == MiniGameState.LureWindrawn && _rod.rod_EndCurrentSpeed > _rod.rod_CastSpeedRequired)
            {
                OnLineCast();
            }
            //Dose a bunch of checks to check if the line has been cast and the player is able to insta reel in the line, if so the OnLureReturnToRod function will be called. 
            else if (input > 0 && (fishingMiniGameState == MiniGameState.LureCast || fishingMiniGameState == MiniGameState.LureHitWater))
            {
                OnLureReturnToRod();
            }

            _rod.rod_PlayerCastInput.Value = input > 0 ? true : false;

        }


        public void OnLineCast()
        {
            if (fishingMiniGameState == MiniGameState.LureWindrawn)
            {
                updateMiniGameState(MiniGameState.LureCast);
                lineCastCorutine = StartCoroutine(LineCastCoroutine());
                // Start a timer, if the lure dose not enter the water in a set amount of time, it will return. 
            }
        }
        public void OnLureHitWater()
        {
            if (fishingMiniGameState == MiniGameState.LureCast)
            {
                updateMiniGameState(MiniGameState.LureHitWater);
                lureHitWaterCoroutine = StartCoroutine(LureHitWaterCoroutine());
                // Will start a timer to make sure the lure stays in the water if it dose not it will restart the timer. 
            }
        }
        public void OnLureReturnToRod()
        {
            updateMiniGameState(MiniGameState.LureReturn);
            // Starts a coroutine reducing the current max distance until the lure arrives back to the rod. 
            StartCoroutine(LerpLureToRod());

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
            // Once all the hybrids have arrived it Will start a Coroutine that will visually show each of the hybrids taking a bite of the bait in ascending order based on rarity. 
        }
        public void OnHybridAssignedToCatch()
        {

            updateMiniGameState(MiniGameState.NonCaughtsReturnToSwim);
            OnHybridFighting();
            // Once a hybrid has been assigned, It will loop through the rest of the hybrids and return them to swimming, and set the caught hybrid to pulling
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
            // Will need to make the line make a snapping noise
            // lerp the lure back to the rod
            // set the hybrid free 
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
            _lure.lure_RB.AddForce(_rod.rod_EndForceDirection.Value * _rod.rod_EndCurrentSpeed.Value * _lure.lure_CastingForceMultiplier);

            _lure.lure_CurrentCastResetTime.Value = 0;  //sets the value to 0 so this function is self resetting

            _lure.lure_CurrentMaxDistance.Value = _lure.lure_MaxDistanceFromRod.Value; //Sets the current distance of the fishing rod to the max distance

            while (fishingMiniGameState == MiniGameState.LureCast)  // Keeps looping while the lure has been casted and has not hit the water
            {
                _lure.lure_CurrentCastResetTime.Value += Time.deltaTime;  // Adds to CurrentCastResetTime timer every frame

                if (_lure.lure_CurrentCastResetTime.Value >= _lure.lure_CastResetTime) // check to see if the CurrentCastResetTime has exceeded the CastResetTime and if so will return the lure to the rod
                {
                    OnLureReturnToRod();
                    StopCoroutine(lineCastCorutine);
                }


                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// This function is used to check of the lure has settled in the water or has bounced out, if the lure has bounced it it has _lure.lure_CastResetTime.Value amount of time to get back in the water
        /// </summary>
        /// <returns></returns>
        private IEnumerator LureHitWaterCoroutine()
        {
            _lure.lure_InWaterTime.Value = 0;  // sets the value back to 0 so this function is self resetting
            _lure.lure_CurrentMaxDistance = _lure.lure_CurrentDistance;
            _lure.lure_LureStartingDistance = _lure.lure_CurrentDistance;

            while (fishingMiniGameState == MiniGameState.LureHitWater)  // Keeps looping while the lure is in the state of hit water. 
            {
                if (_lure.lure_InWater == true)   // This will be set to true when the lure is in a water source
                {
                    _lure.lure_InWaterTime.Value += Time.deltaTime;
                    if (_lure.lure_InWaterTime.Value >= _lure.lure_CastResetTime.Value)
                    {
                        OnStartMiniGame();
                        StopCoroutine(lureHitWaterCoroutine);
                    }
                }
                else if (_lure.lure_InWater == false)  // This will be set to false when the lure is not in a water source
                {
                    _lure.lure_InWaterTime.Value -= Time.deltaTime;
                    if (_lure.lure_InWaterTime.Value <= -_lure.lure_CastResetTime.Value)
                    {
                        OnLureReturnToRod();
                        StopCoroutine(lureHitWaterCoroutine);
                    }
                }

                yield return new WaitForEndOfFrame();
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
                OnLureReturnToRod();
                StopCoroutine(startMiniGameCorutine);
            }
            else
            {


                List<FOEItem_Hybrid> _foundHybrids = GetClosestHybrids(_convertedHybridList); //gets the 5 closest hybrids in the list

                hybridsReachedLure.Clear();  // sets the hybrid reached nav points back to 0 so its self resetting, this value is iterated each time a hybrid reaches its destination. 
                float HybirdReachedNavPointTimeout = 0;

                foreach (FOEItem_Hybrid _hybrid in _foundHybrids)
                {
                    //remove Hybrid From any lists
                    _FNAVM.removeHybrid(_hybrid, false);

                    //Set Hybrid to nav point
                    _FNAVM.AssignHybridWithTargetAndState(_hybrid, _lure.lure_HybridAttachPoint.transform);

                }

                while (hybridsReachedLure.Count < _convertedHybridList.Count || HybirdReachedNavPointTimeout < _lure.lure_HybridToLureResetTime.Value)
                {

                    HybirdReachedNavPointTimeout += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                if (hybridsReachedLure.Count != 0)
                {
                    OnHybridsArrived();
                    StopCoroutine(startMiniGameCorutine);
                }
                else
                {
                    Debug.LogWarning($"was not able to find any hybrids, if there are hybrid spawned, then something is going very wrong");
                    OnLureReturnToRod();
                    StopCoroutine(startMiniGameCorutine);
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
            _lure.lure_MiniGameDistancePoints.Clear();
            hybridCurrentFightAttempts = 0;
            _rod.rod_fishingRodCurrentHP.Value = _rod.rod_fishingRodStartingHP;

            // Calculate divisions of the starting distance
            float division = _lure.lure_LureStartingDistance / hybridMaxFightAttempts;

            // Assign calculated points to the list dynamically based on HybridMaxFightAttempts
            for (int i = hybridMaxFightAttempts; i > 0; i--)
            {
                _lure.lure_MiniGameDistancePoints.Add(division * i);
            }
            _lure.lure_MiniGameDistancePoints.Add(0);
        }
        /// <summary>
        /// This function will lerp the lures current max distance towards 0 causing the lure to move towards the fishing rod.  
        /// </summary>
        /// <returns></returns>
        private IEnumerator LerpLureToRod()
        {
            while (fishingMiniGameState == MiniGameState.LureReturn)
            {
                _lure.lure_CurrentMaxDistance.Value = Mathf.Lerp(_lure.lure_CurrentMaxDistance.Value, 0, _lure.lure_ReturnSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
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
            if (_Hybrid.Count <= maxHybridsToFind)
                return new List<FOEItem_Hybrid>(_Hybrid);

            // Sort the hybrids based on distance to the lure's end point
            _Hybrid.Sort((a, b) =>
                Vector3.Distance(_lure.lure_EndPointTransform.Value, a.europaItemData.itemTransform.position)
                .CompareTo(Vector3.Distance(_lure.lure_EndPointTransform.Value, b.europaItemData.itemTransform.position))
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

            // work out what hybrid will be caught based off this number
            float total = 0;
            int hybridsToAnimate = 0;
            foreach (FOEItem_Hybrid _hybrid in hybridsReachedLure)
            {
                total += _hybrid.navigationData.scaledCatchChance;
                hybridsToAnimate++;

                if (total > randomNumber)
                {
                    _FNAVM.caughtHybrid = _hybrid;
                    break;
                }
            }
            total = hybridsReachedLure[0].navigationData.scaledCatchChance;
            int index = 0;

            for (int i = 1; i < randomNumber; i++)
            {
                if (i > total)
                {
                    //stop the animation the hybrid sussing out the lure for the current index
                    //return the hybrid to swimming and reset its values
                    ResetHybrid(hybridsReachedLure[index]);

                    index++;
                    total += hybridsReachedLure[index].navigationData.scaledCatchChance;
                }

                // animate the current index hybrid to look like its sussing out the  lure
                yield return new WaitForSeconds(0.1f);
            }

            for (int i = index + 1; i < hybridsReachedLure.Count; i++)
            {
                ResetHybrid(hybridsReachedLure[i]);
            }

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
            _Hybrid.navigationData.itemTarget = null;

            _FNAVM.removeHybrid(_Hybrid, false);
            _FNAVM.AddHybridTolist(_Hybrid, HybridState.HybridFlocking);

        }

        /// <summary>
        /// This sets the catch chances for the hybrids in hybridsReachedLure to be between 0 - 100 and then orders them in a list
        /// </summary>
        private void CaculateCatchChanceList()
        {
            int total = 0;

            foreach (FOEItem_Hybrid _hybrid in hybridsReachedLure)
            {
                total += _hybrid.hybridSO.catchChance;
            }

            foreach (FOEItem_Hybrid _hybrid in hybridsReachedLure)
            {
                _hybrid.navigationData.scaledCatchChance = (_hybrid.hybridSO.catchChance / total) * 100;
            }

            hybridsReachedLure.Sort((x, y) => x.navigationData.scaledCatchChance.CompareTo(y.navigationData.scaledCatchChance));
        }

        /// <summary>
        /// Set rest period time and change fishEncounterState
        /// </summary>
        private IEnumerator RestingPeriodCoroutine()
        {
            Debug.Log("Starting Hybrid Resting");

            // Set the current Stamina to the hybrids max stamina;
            _FNAVM.caughtHybrid.navigationData.currentHybridStamina = _FNAVM.caughtHybrid.hybridSO.stamina;
            // Set the rest time to a value between the hybrids max rest time and their min rest time
            _FNAVM.caughtHybrid.navigationData.hybridRestTime = UnityEngine.Random.Range(_FNAVM.caughtHybrid.hybridSO.restMinTime, _FNAVM.caughtHybrid.hybridSO.restMaxTime);
            //remove from all lists and put in Mini Game List 
            _FNAVM.removeHybrid(_FNAVM.caughtHybrid, false);
            _FNAVM.AddHybridTolist(_FNAVM.caughtHybrid, HybridState.HybridMiniGame_Tired);
            //Sets the MiniGame Active Bool To False
            _rod.fightingMiniGameActive.Value = false;



            while (_FNAVM.caughtHybrid.navigationData.hybridRestTime >= 0 || _lure.lure_CurrentDistance < _lure.lure_MiniGameDistancePoints[hybridCurrentFightAttempts])
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

            if (_lure.lure_MiniGameDistancePoints.Count < hybridMaxFightAttempts)
            {
                hybridCurrentFightAttempts++;
            }

            //remove from all lists and put in Mini Game List 
            _FNAVM.removeHybrid(_FNAVM.caughtHybrid, false);
            _FNAVM.AddHybridTolist(_FNAVM.caughtHybrid, HybridState.HybridMiniGame_Pulling);
            //Sets the MiniGame Active Bool To true
            _rod.fightingMiniGameActive.Value = true;

            var targetDir = _rod.rod_EndPointTransform - _lure.lure_HybridAttachPoint.transform.position;
            _lure.lure_StartingAngle.Value = Vector3.Angle(targetDir, _PLAYER.gameObject.transform.forward);

            setHybridDirection = StartCoroutine(SetHybridDirection());  //Starts of Coroutine to set the rotation of the lure



            while (_FNAVM.caughtHybrid.navigationData.currentHybridStamina > 0)  // Checks to see if the hybrids Stamina is > 0 
            {
                RotateLure(lureRotationAxis); //calls a function to rotate the lure using LureRotationAxis, this value is set by the SetHybridDirection that will update the direction every few seconds


                if (miniGamePullDirection != _rod.rod_RodPullDirectionEnum)
                {
                    //player is not pulling the correct Direction;
                    //Checks to see if the player has been pulling in the incorrect direction for more then the playerIncorrectPullTimeBuffer
                    if (currentIncorrectPullTimeBuffer <= playerIncorrectPullTimeBuffer)
                    {
                        currentIncorrectPullTimeBuffer += Time.deltaTime;
                    }
                    else
                    {
                        _rod.rod_fishingRodCurrentHP.Value -= _FNAVM.caughtHybrid.hybridSO.damageToRod * Time.deltaTime;
                        if (_rod.rod_fishingRodCurrentHP.Value <= 0)
                        {
                            OnHybridEscape();
                            StopCoroutine(fightingPeriodCotoutine);
                            StopCoroutine(setHybridDirection);
                        }
                    }

                }
                else
                {
                    // Player is pulling the correct Direction;
                    currentIncorrectPullTimeBuffer = 0;
                }

                _FNAVM.caughtHybrid.navigationData.currentHybridStamina -= Time.deltaTime;  // Subtracts the hybrids stamina each frame based of time.delta time
                yield return new WaitForEndOfFrame();
            }

            StopCoroutine(setHybridDirection);  // Stops the Coroutine that will randomly change the direction the hybrid is pulling
            resetLureRotation = false;  // Sets the ResetLureRotation To False, this value is set to true once the lure reaches its starting angle

            while (resetLureRotation == false)
            {
                ResetLureRotation(lureRotationAxis); // calls a function to return the lure to the starting location
                yield return new WaitForEndOfFrame();
            }

            OnHybridTried();
        }

        /// <summary>
        /// Sets the Direction the hybrid is pulling
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetHybridDirection()
        {
            while (fishingMiniGameState == MiniGameState.HybridPulling)
            {
                int random = UnityEngine.Random.Range(1, 3);
                lureRotationAxis = random == 1 ? lureRotationAxis = Vector3.up : lureRotationAxis = Vector3.down;
                miniGamePullDirection = random == 1 ? PullDirections.Left : PullDirections.Right;

                yield return new WaitForSeconds(UnityEngine.Random.Range(minHybridPullingDirectionTime, maxHybridPullingDirectionTime));
            }
        }



        private void RotateLure(Vector3 _rotationAxix, float _SpeedMutiplayer = 1f)
        {
            //sets the delta angle from the lure rotation speed and the time
            float deltaAngle = (_lure.lure_LureRotationSpeed.Value * _SpeedMutiplayer) * Time.deltaTime;

            Vector3 targetDir = _rod.rod_EndPointTransform.Value - _lure.lure_HybridAttachPoint.transform.position;

            if (_lure.lure_StartingAngle.Value == 0)
            {
                _lure.lure_StartingAngle.Value = Vector3.Angle(targetDir, _PLAYER.gameObject.transform.forward);
            }

            float currentAngle = Vector3.Angle(targetDir, _PLAYER.playerHead.forward) - _lure.lure_StartingAngle.Value;

            //rotate bobber around player
            if (Mathf.Abs(currentAngle) < _lure.lure_MaxAngle && !Physics.CheckSphere(_lure.lure_Go.transform.position, 0.5f, terrainLayerMask))
            {
                _lure.lure_Go.transform.RotateAround(_rod.rod_EndPointTransform, _rotationAxix, deltaAngle);
            }
        }



        private void ResetLureRotation(Vector3 _rotationAxix, float _SpeedMutiplayer = 1.5f)
        {

            //sets the delta angle from the lure rotation speed and the time
            float deltaAngle = (_lure.lure_LureRotationSpeed.Value * _SpeedMutiplayer) * Time.deltaTime;

            Vector3 targetDir = _rod.rod_EndPointTransform.Value - _lure.lure_HybridAttachPoint.transform.position;


            float currentAngle = Vector3.Angle(targetDir, _PLAYER.playerHead.forward) - _lure.lure_StartingAngle.Value;

            //rotate bobber around player
            if (Mathf.Abs(currentAngle) > 0.5f && !Physics.CheckSphere(_lure.lure_Go.transform.position, 0.5f, terrainLayerMask) && resetLurecurrentTime < resetLureTimeOut)
            {
                _lure.lure_Go.transform.RotateAround(_rod.rod_EndPointTransform, _rotationAxix, deltaAngle);
                resetLurecurrentTime += Time.deltaTime;
            }
            else if (Mathf.Abs(currentAngle) <= 0.5f)
            {
                resetLureRotation = true;
            }
            else
            {
                OnLureReturnToRod();
            }

        }



        public bool CheckForHybridCollision(Vector3 direction)
        {
            if (!Physics.Raycast(_lure.lure_HybridAttachPoint.transform.position, direction, 3f, terrainLayerMask))
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
            if (newValue <= _lure.lure_ResetDistance.Value && fishingMiniGameState == MiniGameState.HybridTied)
            {
                OnHybridCaught();
            }
            else if (newValue <= _lure.lure_ResetDistance.Value)
            {
                OnLureReturnToRod();
            }

            if (fishingMiniGameState == MiniGameState.LureCast || fishingMiniGameState == MiniGameState.LureHitWater || fishingMiniGameState == MiniGameState.HybridTied)
            {
                // Check if the current distance exceeds the allowed max distance
                if (_lure.lure_CurrentDistance.Value > _lure.lure_CurrentMaxDistance.Value)
                {
                    // Calculate how much the current distance exceeds the max allowed distance
                    float excessDistance = _lure.lure_CurrentDistance.Value - _lure.lure_CurrentMaxDistance.Value;

                    // Calculate direction from the lure to the fishing rod
                    Vector3 directionToRod = (_rod.rod_EndPointTransform.Value - _lure.lure_EndPointTransform.Value).normalized;

                    // Calculate the force based on the excess distance and force multiplier
                    Vector3 force = directionToRod * excessDistance * _lure.lure_ReelingForceMultiplier;

                    // Clamp the magnitude of the force to the specified maxForce
                    force = Vector3.ClampMagnitude(force, _lure.lure_MaxReelingForce);

                    // Apply the clamped force to the Rigidbody
                    _lure.lure_RB.AddForce(force, ForceMode.Force);

                }
            }
        }

        private void OnLureEnterWater()
        {
            OnLureHitWater();
            _lure.lure_InWater.Value = true;
        }

        private void OnLureExitWater()
        {
            _lure.lure_InWater.Value = false;
        }
    }
}



