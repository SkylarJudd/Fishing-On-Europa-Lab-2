using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;
using static Crest.Spline.Spline;

namespace Europa
{


    public class FishNavigationManager : Singleton<FishNavigationManager>
    {
        [Header("All Hybrids In Pond")]

        [Tooltip("A list that contains all the tamed hybrids.")]
        [SerializeField] public ScriptableListFOEItem_Hybrid _hybridsTamedList;

        [Tooltip("a list that contains all the hybrids that has been spawned into this pond.")]
        [SerializeField] private ScriptableListFOEItem_Hybrid _hybridsToNavList;

        [SerializeField] private ScriptableListFOEItem_Hybrid _removeFromHybridsToNavList;

        #region Hybrid Nav Lists
        [Header("Hybrids Doing Different Tasks")] // Lists for each of the hybrids doing different things so we don't have to loop over them all. 

        [Tooltip("a list that contains all the hybrids that are Idel.")]
        [SerializeField] private ScriptableListFOEItem_Hybrid hybridsIdle;

        [Tooltip("a list that contains all the hybrids that are Flying.")]
        [SerializeField] private ScriptableListFOEItem_Hybrid hybridsFlying;

        [Tooltip("a list that contains all the hybrids that are Hitting the water.")]
        [SerializeField] private ScriptableListFOEItem_Hybrid hybridsHitWater;

        [Tooltip("a list that contains all the hybrids that are Swimming.")]
        [SerializeField] private ScriptableListFOEItem_Hybrid hybridsSwimming;

        [Tooltip("a list that contains all the hybrids that are Swimming To the Lure or an item in the players hand.")]
        [SerializeField] private ScriptableListFOEItem_Hybrid hybridSwimToPoint;

        [Tooltip("a list that contains all the hybrids that are reacting to the player.")]
        [SerializeField] private ScriptableListFOEItem_Hybrid hybridReactToPlayer;

        [Tooltip("Holds the info about he Hybrid that has Been Caught")]
        public FOEItem_Hybrid caughtHybrid;
        #endregion

        #region RemoveHybridLists

        [SerializeField] private ScriptableListFOEItem_Hybrid removeHybridsIdle;
        [SerializeField] private ScriptableListFOEItem_Hybrid removeHybridsFlying;
        [SerializeField] private ScriptableListFOEItem_Hybrid removeHybridsHitWater;
        [SerializeField] private ScriptableListFOEItem_Hybrid removeHybridsSwimming;
        [SerializeField] private ScriptableListFOEItem_Hybrid removeHybridSwimToLure;
        [SerializeField] private ScriptableListFOEItem_Hybrid removeHybridReact;
        #endregion

        #region Boids Settings
        [Header("Boids Nav Settings")]
        [SerializeField] int applyBoidsChance;
        [SerializeField] int rayCastCheckChance;
        [SerializeField] bool debug = true;

        // boid behavior range
        [SerializeField][Range(0.0f, 3.0f)] float separationRange;
        [SerializeField][Range(0.0f, 3.0f)] float alignmentRange;
        [SerializeField][Range(0.0f, 3.0f)] float cohesionRange;

        // boid behavior weights
        [SerializeField][Range(0.0f, 3.0f)] float separationFactor;
        [SerializeField][Range(0.0f, 3.0f)] float alignmentFactor;
        [SerializeField][Range(0.0f, 3.0f)] float cohesionFactor;
        #endregion

        [SerializeField] float correctRotationSpeed = 0.1f; //being used by UpdateHitWater()

        #region Hybrid Nav Coroutines
        private Coroutine updateHybridFlyingCoroutine;
        private Coroutine updateHitWaterCoroutine;
        private Coroutine updateSwimmingCoroutine;
        private Coroutine updateSwimToLure;
        private Coroutine UpdateHybridStatesCoroutine;
        private Coroutine UpdateHybridReactCoroutine;
        #endregion

        private GameObject lureLocation;

        private float waterDrag = 5.0f;
        private float airDrag = 0.0f;

        [Header("Player Interaction Settings")]
        [SerializeField] float playerReactionDistance;
        [SerializeField] float playerReactionDistanceReset;

        [SerializeField] float playerHandReactionDistance;
        [SerializeField] float playerHandReactionDistanceReset;

        [SerializeField] float moveToPlayerStoppingDistance;

        [SerializeField] float hybridDistanceToEat;


        private void Start()
        {


            StartCoroutines();
        }

        private void StartCoroutines()
        {
            updateHybridFlyingCoroutine = StartCoroutine(UpdateHybridFlying());
            updateHitWaterCoroutine = StartCoroutine(UpdateHitWater());
            updateSwimmingCoroutine = StartCoroutine(UpdateSwimming());
            updateSwimToLure = StartCoroutine(UpdateHybridToLure());
            UpdateHybridStatesCoroutine = StartCoroutine(UpdateHybridStates());
            UpdateHybridReactCoroutine = StartCoroutine(UpdateHybridReact());
        }


        /// <summary>
        /// Continuously updates the state of each hybrid in the pond, reacting to the player's proximity and actions.
        /// This coroutine runs in a loop and processes interactions between the player and hybrids based on distance and held items.
        /// </summary>
        private IEnumerator UpdateHybridStates()
        {
            // Continuously update hybrid states runs in a while loop so its not creating garbage creating new Corutines each time
            while (true)
            {
                // check to see if there are any hybrids in the pond and only if there are hybrids in the pond will continue
                if (_hybridsToNavList.Count > 0)
                {
                    // Iterate through each hybrid in the pond
                    foreach (FOEItem_Hybrid _hybrid in _hybridsToNavList)
                    {

                        // Check if the hybrid is within the player's reaction distance using trigger on hybrid
                        if (_hybrid.hybridSO.hybridInteractionTrigger.playerInRange)
                        {
                            // Add the hybrid to the react list if it's not already included
                            if (!hybridReactToPlayer.Contains(_hybrid))
                            {
                                removeHybrid(_hybrid.europaItemData.itemGO, false);
                                hybridReactToPlayer.Add(_hybrid);
                            }

                            // Check if player is holding an item
                            if (_PLAYER.hasItem)
                            {
                                ProcessPlayerWithItem(_hybrid);
                            }
                            else
                            {
                                ProcessPlayerWithoutItem(_hybrid);
                            }

                            //Check if player is patting hybrid
                            if (_hybrid.ReturnPatTrigger() != null)
                            {
                                HybridPat(_hybrid.ReturnPatTrigger(), _hybrid);
                            }
                        }
                        else if (_hybrid.navigationData.distanceToPlayer > playerReactionDistanceReset && _hybrid.navigationData.hybridState == HybridState.HybridWatchPlayer)
                        {
                            // Reset hybrid state to flocking when the player moves away beyond reset distance
                            _hybrid.navigationData.hybridState = HybridState.HybridFlocking;
                            removeHybridReact.Add(_hybrid);
                            hybridsSwimming.Add(_hybrid);
                        }
                    }

                    // Clean up hybrids that are no longer reacting
                    foreach (var _hybrid in removeHybridReact)
                    {
                        hybridReactToPlayer.Remove(_hybrid);
                    }
                    removeHybridReact.Clear();
                }

                // Wait until the next physics update
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Handles hybrid interactions when the player is holding an item, checking for specific reactions based on the type of item held.
        /// </summary>
        private void ProcessPlayerWithItem(FOEItem_Hybrid _hybrid)
        {
            // Assume player could hold food in either hand
            FOEItem_Food _rightFood = _PLAYER.rightHandFood;
            FOEItem_Food _leftFood = _PLAYER.leftHandFood;

            // Check for food interactions
            if (_PLAYER.rightHandFood != null || _PLAYER.leftHandFood != null)
            {
                CheckFoodInteraction(_hybrid, _rightFood, _leftFood);
            }
            // Check for plushie interactions
            else if (_PLAYER.rightHandPlushie != null || _PLAYER.leftHandPlushie != null)
            {
                CheckPlushieInteraction(_hybrid);
            }
        }

        /// <summary>
        /// Processes hybrid interactions when the player is not holding any item, determining reactions based on proximity to the player's hands.
        /// </summary>
        private void ProcessPlayerWithoutItem(FOEItem_Hybrid _hybrid)
        {
            // Calculate distances to player's hands
            float distanceToRightHand = Vector3.Distance(_hybrid.europaItemData.itemGO.transform.position, _PLAYER.rightHand.transform.position);
            float distanceToLeftHand = Vector3.Distance(_hybrid.europaItemData.itemGO.transform.position, _PLAYER.leftHand.transform.position);

            // Determine which hand is closer, or default to watching the player
            if (distanceToRightHand < playerHandReactionDistance || distanceToLeftHand < playerHandReactionDistance)
            {
                _hybrid.navigationData.itemTarget = (distanceToRightHand < distanceToLeftHand) ? _PLAYER.rightHand : _PLAYER.leftHand;
                _hybrid.navigationData.hybridState = HybridState.HybridLookAtHand;
            }
            else
            {
                _hybrid.navigationData.hybridState = HybridState.HybridWatchPlayer;
            }
        }

        /// <summary>
        /// Checks for food interactions between the player's held items and the hybrid's preferences, setting the target and state accordingly.
        /// </summary>
        private void CheckFoodInteraction(FOEItem_Hybrid _hybrid, FOEItem_Food _rightFood, FOEItem_Food _leftFood)
        {
            // Interact with the type of food the hybrid eats
            foreach (FoodType _food in _hybrid.hybridSO.foodEaten)
            {
                if (_food == _leftFood.foodType)
                {
                    SetHybridTargetState(_hybrid, _PLAYER.leftHandFood.europaItemData.itemGO.transform);
                    HybridEat(_hybrid, _leftFood, _PLAYER.leftHandFood.europaItemData.itemGO.transform);
                }
                else if (_food == _rightFood.foodType)
                {
                    SetHybridTargetState(_hybrid, _PLAYER.rightHandFood.europaItemData.itemGO.transform);
                    HybridEat(_hybrid, _rightFood, _PLAYER.rightHandFood.europaItemData.itemGO.transform);
                }
            }
        }

        /// <summary>
        /// Checks plushie interactions based on the hybrid's favorite toy and the player's held plushies.
        /// </summary>
        private void CheckPlushieInteraction(FOEItem_Hybrid _hybrid)
        {
            // Interact with the favorite toy
            if (_hybrid.hybridSO.favToy == _PLAYER.leftHandPlushie.ToyItem)
            {
                SetHybridTargetState(_hybrid, _PLAYER.leftHandPlushie.europaItemData.itemGO.transform);

            }
            else if (_hybrid.hybridSO.favToy == _PLAYER.rightHandPlushie.ToyItem)
            {
                SetHybridTargetState(_hybrid, _PLAYER.rightHandPlushie.europaItemData.itemGO.transform);

            }
        }

        /// <summary>
        /// Processes player patting hybrid
        /// </summary>
        void HybridPat(Transform handState, FOEItem_Hybrid _hybridInfo)
        {

            //check if hand that is in trigger is holding an item
            if (handState == _PLAYER.leftHand && _PLAYER.leftHandFood == null && _PLAYER.leftHandPlushie == null)
            {

                //check if hand is moving
                if (_PLAYER.leftHand.GetComponent<Rigidbody>().velocity.magnitude >= 1)
                {
                    //play animation here

                    //Increase Trust Here
                    _TM.UpdateHybridPatTrust(_hybridInfo);

                }
            }
            else if (handState == _PLAYER.rightHand && _PLAYER.rightHandFood == null && _PLAYER.rightHandPlushie == null)
            {

                //check if hand is moving
                if (_PLAYER.rightHand.GetComponent<Rigidbody>().velocity.magnitude >= 1)
                {

                    //play animation here


                    //Increase Trust Here
                    _TM.UpdateHybridPatTrust(_hybridInfo);

                }
            }
        }

        /// <summary>
        /// Check if hybrid is close enough to object to eat, then processes eat
        /// </summary>
        /// <param name="_hybrid"></param>
        /// <param name="_food"></param>
        /// <param name="_foodTransform"></param>
        void HybridEat(FOEItem_Hybrid _hybrid, FOEItem_Food _food, Transform _foodTransform)
        {
            if (Vector3.Distance(_hybrid.transform.position, _foodTransform.position) < hybridDistanceToEat)
            {
                //destroy/remove food item
                _OPM.ReturnObjectToPool(_food);

                //check if favourite food
                bool isFav = false;
                if (_hybrid.hybridSO.favFood == _food.foodItem)
                    isFav = true;
                //Increase Trust
                _TM.UpdateHybridFeedTrust(_hybrid, isFav);

                SetHybridTargetState(_hybrid, _PLAYER.leftHandPlushie.europaItemData.itemGO.transform);
            }

        }

        /// <summary>
        /// Sets the hybrid's target and updates its state based on its proximity to the target and current behavior parameters.
        /// </summary>
        private void SetHybridTargetState(FOEItem_Hybrid _hybrid, Transform targetTransform)
        {
            // Set target and update state based on proximity to the player
            _hybrid.navigationData.itemTarget = targetTransform;
            _hybrid.navigationData.hybridState = HybridState.HybridSwimToItem;
            if (_hybrid.navigationData.distanceToPlayer < moveToPlayerStoppingDistance)
            {
                _hybrid.navigationData.hybridState = HybridState.HybridLookAtHand;

            }
        }


        private IEnumerator UpdateIdle()
        {
            while (true)
            {
                if (hybridsIdle.Count > 0)
                {
                    foreach (FOEItem_Hybrid _hybrid in hybridsIdle)
                    {
                        //Play Idle animation
                    }
                    // Remove hybrids from hybridsHitWater
                    foreach (var _hybrid in removeHybridsIdle)
                    {
                        hybridsIdle.Remove(_hybrid);
                    }
                    removeHybridsIdle.Clear();
                }
                yield return new WaitForFixedUpdate();
            }
        }


        /// <summary>
        /// loops though the list of Flying Hybrids and updates their rigid body values and check when they hit the water. 
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateHybridFlying()
        {
            while (true)
            {

                if (hybridsFlying.Count > 0)
                {
                    //print("Hybrids flying count is grater then 0");
                    for (int i = hybridsFlying.Count - 1; i >= 0; i--)
                    {
                        FOEItem_Hybrid _hybrid = hybridsFlying[i];

                        // Animate the Hybrids

                        // Check if Hybrid has left the water
                        if (_hybrid.navigationData.hybridState != HybridState.HybridFlying)
                        {
                            UpdateRB(_hybrid.europaItemData.itemRB, true, airDrag, airDrag);
                            _hybrid.navigationData.hybridState = HybridState.HybridFlying;
                        }

                        //print($"{_hybridd} has a y value of {_hybridd.hybridGameObject.transform.position.y} and the water hight is {waterHight}.y ");
                        // Check if the Hybrids have hit the water
                        if (_hybrid.europaItemData.itemGO.transform.position.y <= _hybrid.navigationData.waterHeight)
                        {
                            hybridsHitWater.Add(_hybrid);
                            //hybridsFlying.RemoveAt(i);
                            removeHybridsFlying.Add(_hybrid);
                        }
                    }

                    // Remove hybrids from hybridsHitWater
                    foreach (var hybrid in removeHybridsFlying)
                    {
                        hybridsFlying.Remove(hybrid);
                    }
                    removeHybridsFlying.Clear();
                }

                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Loops through the list of hybrids that have hit the water and updates their rigid bodies and corrects their rotation
        /// to ensure they align properly with the water surface. Hybrids are moved to the swimming state once their rotation is corrected.
        /// </summary>
        private IEnumerator UpdateHitWater()
        {
            while (true)
            {
                if (hybridsHitWater.Count > 0)
                {
                    for (int i = hybridsHitWater.Count - 1; i >= 0; i--)
                    {
                        FOEItem_Hybrid _hybrid = hybridsHitWater[i];

                        // Initialize hybrid's physics when they first hit the water
                        if (_hybrid.navigationData.hybridState != HybridState.HybridHitWater)
                        {
                            // Disable gravity and adjust drag to simulate water resistance
                            UpdateRB(_hybrid.europaItemData.itemRB, false, waterDrag, waterDrag);
                            _hybrid.navigationData.hybridState = HybridState.HybridHitWater;
                        }

                        // Define and interpolate towards the target rotation to align with the water surface
                        Quaternion targetRotation = Quaternion.Euler(0, _hybrid.europaItemData.itemGO.transform.rotation.eulerAngles.y, 0);
                        _hybrid.europaItemData.itemGO.transform.rotation = Quaternion.Lerp(_hybrid.europaItemData.itemGO.transform.rotation, targetRotation, Time.deltaTime * correctRotationSpeed);

                        // Check if the hybrid is aligned within acceptable thresholds to transition to swimming
                        Vector3 currentRotation = _hybrid.europaItemData.itemGO.transform.rotation.eulerAngles;
                        if (IsRotationWithinRange(currentRotation))
                        {
                            // Mark for transition to swimming state
                            hybridsSwimming.Add(_hybrid);
                            removeHybridsHitWater.Add(_hybrid);
                            _hybrid.navigationData.velocity = _hybrid.europaItemData.itemGO.transform.position;
                        }
                    }

                    // Remove processed hybrids from the hit water list
                    foreach (var hybrid in removeHybridsHitWater)
                    {
                        hybridsHitWater.Remove(hybrid);
                    }
                    removeHybridsHitWater.Clear();
                }
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Checks if the given rotation is within the acceptable range to be considered aligned with the water.
        /// </summary>
        /// <param name="rotation">The current rotation as Euler angles.</param>
        /// <returns>True if within range, otherwise false.</returns>
        private bool IsRotationWithinRange(Vector3 rotation)
        {
            return (rotation.x < 0.5f || rotation.x > 359.5f) && (rotation.z < 0.5f || rotation.z > 359.5f);
        }

        /// <summary>
        /// A helper fuction to updates the properties of a inputed Rigid Body. 
        /// </summary>
        /// <param name="_rb">The Rigid Body you want to update the values of</param>
        /// <param name="_gravity">To use Gravity or not</param>
        /// <param name="_drag">The drag of the Rigid body</param>
        /// <param name="_angularDrag">The agular drag of the Rigid body</param>
        private void UpdateRB(Rigidbody _rb, bool _gravity, float _drag, float _angularDrag)
        {
            _rb.useGravity = _gravity;
            _rb.drag = _drag;
            _rb.angularDrag = _angularDrag;
        }

        /// <summary>
        /// loops though the list of hybrids that are swimming and calls the fuctions that are needed for each hybrid to move. 
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateSwimming()
        {
            while (true)
            {
                if (hybridsSwimming.Count > 0)
                {
                    foreach (FOEItem_Hybrid _hybrid in hybridsSwimming)
                    {
                        bool outofWater = false;

                        if (_hybrid.europaItemData.itemGO.transform.position.y > _hybrid.navigationData.waterHeight)
                        {
                            outofWater = true;
                            //print("Help im Out of water");
                        }

                        if (UnityEngine.Random.Range(0, rayCastCheckChance) < 1 && hybridsSwimming.Count > 1)
                        {
                            Ray hybridRay = new Ray(_hybrid.europaItemData.itemGO.transform.position, _hybrid.europaItemData.itemGO.transform.forward);
                            float raycastDistance = 0.5f;
                            int layerMask = ~LayerMask.GetMask("Fish");

                            if (debug)
                                Debug.DrawRay(hybridRay.origin, hybridRay.direction * raycastDistance, Color.red);

                            if (Physics.Raycast(hybridRay, out RaycastHit hit, raycastDistance, layerMask))
                            {
                                if (hit.collider.gameObject.CompareTag("Wall"))
                                {
                                    _hybrid.navigationData.aboutToHitWall = true;
                                }
                                else
                                {
                                    _hybrid.navigationData.aboutToHitWall = false;
                                    _hybrid.navigationData.hybridState = HybridState.HybridFlocking;
                                }
                            }
                            else
                            {
                                _hybrid.navigationData.aboutToHitWall = false;
                                _hybrid.navigationData.hybridState = HybridState.HybridFlocking;
                            }
                        }

                        if (UnityEngine.Random.Range(0, applyBoidsChance) < 1 && hybridsSwimming.Count > 1 || _hybrid.navigationData.firstNav == true)
                        {
                            _hybrid.navigationData.firstNav = false;
                            Vector3 separationVelocity = Vector3.zero;
                            Vector3 alignmentVelocity = Vector3.zero;
                            Vector3 cohesionVelocity = Vector3.zero;

                            int numOfBoidsToAvoid = 0;
                            int numOfBoidsToAlignWith = 0;
                            int numOfBoidsInFlock = 0;
                            Vector3 currBoidPosition = _hybrid.europaItemData.itemGO.transform.position;
                            Vector3 positionToMoveTowards = Vector3.zero;

                            foreach (FOEItem_Hybrid _otherBoid in hybridsSwimming)
                            {
                                if (ReferenceEquals(_otherBoid, _hybrid))
                                {
                                    continue;
                                }

                                Vector3 otherBoidsPosition = _otherBoid.europaItemData.itemGO.transform.position;
                                float dist = Vector3.Distance(currBoidPosition, otherBoidsPosition);

                                // Separation Check
                                if (dist < separationRange)
                                {
                                    Vector3 otherBoidToCurrentBoid = currBoidPosition - otherBoidsPosition;
                                    Vector3 dirToTravel = otherBoidToCurrentBoid.normalized;
                                    separationVelocity += dirToTravel / dist;
                                    numOfBoidsToAvoid++;
                                }

                                // Alignment Check
                                if (dist < alignmentRange)
                                {
                                    alignmentVelocity += _otherBoid.navigationData.velocity;
                                    numOfBoidsToAlignWith++;
                                }

                                // Cohesion Check
                                if (dist < cohesionRange)
                                {
                                    positionToMoveTowards += otherBoidsPosition;
                                    numOfBoidsInFlock++;
                                }
                            }

                            if (numOfBoidsToAvoid != 0)
                            {
                                separationVelocity /= numOfBoidsToAvoid;
                                separationVelocity.Normalize();
                                separationVelocity *= separationFactor;
                            }

                            if (numOfBoidsToAlignWith != 0)
                            {
                                alignmentVelocity /= numOfBoidsToAlignWith;
                                alignmentVelocity.Normalize();
                                alignmentVelocity *= alignmentFactor;
                            }

                            if (numOfBoidsInFlock != 0)
                            {
                                positionToMoveTowards /= numOfBoidsInFlock;
                                Vector3 cohesionDirection = positionToMoveTowards - currBoidPosition;
                                cohesionDirection.Normalize();
                                cohesionVelocity = cohesionDirection * cohesionFactor;
                            }

                            _hybrid.navigationData.velocity += separationVelocity;
                            _hybrid.navigationData.velocity += alignmentVelocity;
                            _hybrid.navigationData.velocity += cohesionVelocity;

                            _hybrid.navigationData.velocity = Vector3.ClampMagnitude(_hybrid.navigationData.velocity, _hybrid.navigationData.maxSpeed);

                            Vector3 direction = _hybrid.navigationData.velocity.normalized;
                            float speed = _hybrid.navigationData.velocity.magnitude;
                            speed = Mathf.Clamp(speed, _hybrid.navigationData.minSpeed, _hybrid.navigationData.maxSpeed);
                            _hybrid.navigationData.velocity = direction * speed;
                        }

                        if (outofWater)
                        {
                            _hybrid.navigationData.velocity.y = -Mathf.Abs(_hybrid.navigationData.velocity.y);
                            _hybrid.navigationData.velocity.z = -Mathf.Abs(_hybrid.navigationData.velocity.z);

                        }

                        if (_hybrid.navigationData.aboutToHitWall == true && _hybrid.navigationData.hybridState != HybridState.HybridAvoidingWall)
                        {
                            Vector3 oppositeDirection = -_hybrid.europaItemData.itemGO.transform.forward;
                            _hybrid.navigationData.velocity = oppositeDirection * _hybrid.navigationData.maxSpeed;
                            _hybrid.navigationData.hybridState = HybridState.HybridAvoidingWall;
                        }

                        // Move the Hybrid in the direction of Velocity
                        _hybrid.europaItemData.itemGO.transform.position += _hybrid.navigationData.velocity * Time.deltaTime;

                        // Rotate the Hybrid toward the direction it is moving
                        Quaternion targetRotation = Quaternion.LookRotation(_hybrid.navigationData.velocity);
                        //Debug.Log($"Updating Rotation: {_Hybrid.hybridGameObject.name} Current Rotation: {_Hybrid.hybridGameObject.transform.rotation} Target Rotation: {targetRotation}");
                        _hybrid.europaItemData.itemGO.transform.rotation = Quaternion.Lerp(_hybrid.europaItemData.itemGO.transform.rotation, targetRotation, _hybrid.hybridSO.rotationSpeed * Time.deltaTime);

                    }

                    // Remove hybrids from hybridsHitWater
                    foreach (var hybrid in removeHybridsSwimming)
                    {
                        hybridsSwimming.Remove(hybrid);
                    }
                    removeHybridsSwimming.Clear();
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator UpdateHybridToLure()
        {
            while (true)
            {
                if (hybridSwimToPoint.Count > 0)
                {
                    foreach (FOEItem_Hybrid _hybrid in hybridSwimToPoint)
                    {
                        print("swim part 2" + _FMGM.bobberTipGO.transform.position);
                        Vector3 directionToTarget = _FMGM.bobberGameObject.transform.position - _hybrid.europaItemData.itemGO.transform.position;
                        float yOffset = 0.5f;
                        directionToTarget = new Vector3(directionToTarget.x, directionToTarget.y - yOffset, directionToTarget.z);

                        // Rotate towards the target
                        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _hybrid.hybridSO.rotationSpeed);

                        // Move towards the target
                        Vector3 targetPosition = _FMGM.bobberTipGO.transform.position;

                        if (targetPosition.y > _hybrid.navigationData.waterHeight)
                        {
                            targetPosition.y = _hybrid.navigationData.waterHeight;
                        }
                        _hybrid.europaItemData.itemGO.transform.position = Vector3.Lerp(_hybrid.europaItemData.itemGO.transform.position, new Vector3(targetPosition.x, targetPosition.y - yOffset, targetPosition.z), Time.deltaTime * (_hybrid.navigationData.maxSpeed * 2));

                        // Check if the object has reached the target position
                        float distanceToTarget = Vector3.Distance(_hybrid.europaItemData.itemGO.transform.position, targetPosition);
                        float threshold = 1f; // Adjust the threshold as needed

                        //print(distanceToTarget);

                        if (distanceToTarget < threshold && _hybrid.navigationData.arrivedAtLure == false)
                        {
                            Debug.Log("Object has reached the target!");
                            _hybrid.navigationData.hybridState = HybridState.HybridIdle;
                            _hybrid.navigationData.arrivedAtLure = true;
                            hybridsIdle.Add(_hybrid);
                            removeHybridSwimToLure.Add(_hybrid);

                            caughtHybrid = _hybrid;

                            //send an update to minigame Manager that the Hybrid has arrived
                            _FMGM.fishingMiniGameState = FishingMiniGameManager.BobberState.AttachedFish;
                            _FMGM.fishEncounterState = FishingMiniGameManager.FishEncounterState.Fighting;

                            StartCoroutine(UpdateMiniGame()); //start mini game coroutine

                        }

                    }
                    foreach (var hybrid in removeHybridSwimToLure)
                    {
                        hybridSwimToPoint.Remove(hybrid);
                    }
                    removeHybridSwimToLure.Clear();
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator UpdateMiniGame()
        {
            while (true)
            {
                if (caughtHybrid != null)
                {

                    switch (caughtHybrid.navigationData.hybridState)
                    {
                        case HybridState.HybridMiniGame_Pulling:

                            print("Fighintg");

                            //look at target

                            ////Move towards the target
                            Vector3 targetPosition = _FMGM.bobberTipGO.transform.position;

                            if (targetPosition.y > caughtHybrid.navigationData.waterHeight)
                            {
                                targetPosition.y = caughtHybrid.navigationData.waterHeight;
                            }
                            caughtHybrid.europaItemData.itemGO.transform.position = Vector3.Lerp(caughtHybrid.europaItemData.itemGO.transform.position, new Vector3(targetPosition.x, targetPosition.y - 0, targetPosition.z), Time.deltaTime * (caughtHybrid.navigationData.maxSpeed * 3));

                            //get pull direction
                            if (_FMGM.currentPullDirection == FishingMiniGameManager.PullDirections.NotSet) _FMGM.currentPullDirection = _FMGM.GetDirection();
                            Vector3 rotationAxis = new();
                            Vector3 direction = new();

                            float deltaAngle = _FMGM.bobberRotationSpeedFighting * Time.deltaTime;
                            var targetDir = _FMGM.fishingRod.transform.position - _FMGM.bobberTipGO.transform.position;
                            if (_FMGM.startingBobberAngle == 0) _FMGM.startingBobberAngle = Vector3.Angle(targetDir, _PLAYER.gameObject.transform.forward);

                            float currentAngle = Vector3.Angle(targetDir, _PLAYER.gameObject.transform.forward) - _FMGM.startingBobberAngle;


                            switch (_FMGM.currentPullDirection)
                            {
                                case FishingMiniGameManager.PullDirections.Left:
                                    //set bobber rotation
                                    rotationAxis = Vector3.down;
                                    direction = Vector3.right;
                                    //reset temp

                                    break;
                                case FishingMiniGameManager.PullDirections.Right:
                                    rotationAxis = Vector3.up;
                                    direction = Vector3.left;
                                    break;

                            }
                            //print(currentAngle + " " + _FMGM.maxAngle);

                            //rotate bobber around player
                            if (Mathf.Abs(currentAngle) < _FMGM.maxAngle && !Physics.CheckSphere(_FMGM.bobberGameObject.transform.position, 0.5f, _FMGM.fishCollisionLayerMask))
                            {
                                _FMGM.bobberGameObject.transform.RotateAround(_FMGM.fishingRod.transform.position, rotationAxis, deltaAngle);
                            }

                            break;

                        case HybridState.HybridMiniGame_Tired:

                            rotationAxis = new();
                            //reset bobber to center
                            switch (_FMGM.currentPullDirection)
                            {
                                case FishingMiniGameManager.PullDirections.Left:
                                    //set bobber rotation
                                    rotationAxis = Vector3.up;
                                    direction = Vector3.left;
                                    //reset temp

                                    break;
                                case FishingMiniGameManager.PullDirections.Right:
                                    rotationAxis = Vector3.down;
                                    direction = Vector3.right;
                                    break;

                            }

                            deltaAngle = (_FMGM.bobberRotationSpeedFighting / 2) * Time.deltaTime;
                            targetDir = _FMGM.fishingRod.transform.position - _FMGM.bobberTipGO.transform.position;
                            currentAngle = Vector3.Angle(targetDir, _PLAYER.gameObject.transform.forward) - _FMGM.startingBobberAngle;
                            //print(currentAngle + " " + _FMGM.startingBobberAngle);
                            //rotate bobber around player
                            if (Mathf.Abs(currentAngle) > 0.5f && !Physics.CheckSphere(_FMGM.bobberGameObject.transform.position, 0.5f, _FMGM.fishCollisionLayerMask))
                            {
                                _FMGM.bobberGameObject.transform.RotateAround(_FMGM.fishingRod.transform.position, rotationAxis, deltaAngle);
                            }


                            //print("Attached to line");
                            Vector3 directionToTarget = _FMGM.bobberTipGO.transform.position - transform.position;
                            float yOffset = 0.5f;
                            directionToTarget = new Vector3(directionToTarget.x, directionToTarget.y - yOffset, directionToTarget.z);
                            // Rotate towards the target

                            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * caughtHybrid.hybridSO.rotationSpeed);

                            ////Move towards the target
                            targetPosition = _FMGM.bobberTipGO.transform.position;

                            if (targetPosition.y > caughtHybrid.navigationData.waterHeight)
                            {
                                targetPosition.y = caughtHybrid.navigationData.waterHeight;
                            }
                            caughtHybrid.europaItemData.itemGO.transform.position = Vector3.Lerp(caughtHybrid.europaItemData.itemGO.transform.position, new Vector3(targetPosition.x, targetPosition.y - 0, targetPosition.z), Time.deltaTime * (caughtHybrid.navigationData.maxSpeed * 3));

                            break;

                        case HybridState.HybridMiniGame_Caught:

                            ////Move towards the target
                            targetPosition = _FMGM.bobberTipGO.transform.position;

                            if (targetPosition.y > caughtHybrid.navigationData.waterHeight)
                            {
                                targetPosition.y = caughtHybrid.navigationData.waterHeight;
                            }
                            caughtHybrid.europaItemData.itemGO.transform.position = Vector3.Lerp(caughtHybrid.europaItemData.itemGO.transform.position, new Vector3(targetPosition.x, targetPosition.y - 0, targetPosition.z), Time.deltaTime * (caughtHybrid.navigationData.maxSpeed * 3));


                            //end courtunie
                            yield return null;
                            break;
                        case HybridState.HybridMiniGame_Escaped:

                            //not tested

                            //swim away from player
                            RotateAndMoveHybridTowards(caughtHybrid, _PLAYER.transform.TransformPoint(_PLAYER.transform.forward * 3));

                            //remove hybrid
                            removeHybrid(caughtHybrid.europaItemData.itemGO, true);

                            break;
                    }


                }
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Continuously updates the behavior of hybrids that are reacting to the player, handling state transitions and movements based on the hybrid's current state.
        /// </summary>
        private IEnumerator UpdateHybridReact()
        {
            while (true)
            {
                // Only process if there are hybrids currently reacting to the player
                if (hybridReactToPlayer.Count > 0)
                {
                    // Iterate backwards through the list to safely remove elements if needed
                    for (int i = hybridReactToPlayer.Count - 1; i >= 0; i--)
                    {
                        FOEItem_Hybrid _hybrid = hybridReactToPlayer[i];

                        // Reset hybrids avoiding wall back to watching player ( This is a bug, Needs to be fixed, The Hybrid should not have a state of avoiding walls while in here I have no idea why it dose.) 
                        if (_hybrid.navigationData.hybridState == HybridState.HybridAvoidingWall)
                        {
                            _hybrid.navigationData.hybridState = HybridState.HybridWatchPlayer;
                        }

                        // Handle behavior based on current state
                        switch (_hybrid.navigationData.hybridState)
                        {
                            case HybridState.HybridWatchPlayer:
                                RotateHybridTowards(_hybrid, _PLAYER.playerHead.transform.position);
                                break;
                            case HybridState.HybridSwimToItem:
                                RotateAndMoveHybridTowards(_hybrid, _hybrid.navigationData.itemTarget.transform.position);
                                break;
                            case HybridState.HybridLookAtHand:
                                RotateHybridTowards(_hybrid, _hybrid.navigationData.itemTarget.transform.position);
                                break;
                        }
                    }

                    // Clear the list of hybrids to remove after processing
                    foreach (var hybrid in removeHybridReact)
                    {
                        hybridReactToPlayer.Remove(hybrid);
                    }
                    removeHybridReact.Clear();
                }

                // Wait until the next physics update
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Rotates the hybrid towards a target position.
        /// </summary>
        /// <param name="_hybrid">Hybrid to rotate.</param>
        /// <param name="_targetPos">Position to face.</param>
        private void RotateHybridTowards(FOEItem_Hybrid _hybrid, Vector3 _targetPos)
        {
            Vector3 direction = (_targetPos - _hybrid.europaItemData.itemGO.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _hybrid.europaItemData.itemGO.transform.rotation = Quaternion.Lerp(_hybrid.europaItemData.itemGO.transform.rotation, targetRotation, _hybrid.hybridSO.rotationSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Rotates and moves the hybrid towards a target position.
        /// </summary>
        /// <param name="_hybrid">Hybrid to move.</param>
        /// <param name="_targetPos">Position to move to and face.</param>
        private void RotateAndMoveHybridTowards(FOEItem_Hybrid _hybrid, Vector3 _targetPos)
        {
            RotateHybridTowards(_hybrid, _targetPos);

            // Clamp target position to water height
            if (_targetPos.y > _hybrid.navigationData.waterHeight)
            {
                _targetPos.y = _hybrid.navigationData.waterHeight;
            }

            // Move towards the target
            _hybrid.europaItemData.itemGO.transform.position = Vector3.Lerp(_hybrid.europaItemData.itemGO.transform.position, _targetPos, Time.deltaTime * _hybrid.navigationData.maxSpeed);
        }

        public void removeHybrid(GameObject go, bool _RemoveFromPond)
        {
            foreach (FOEItem_Hybrid _hybrid in _hybridsToNavList)
            {
                if (_hybrid.europaItemData.itemGO == go)
                {
                    switch (_hybrid.navigationData.hybridState)
                    {
                        case HybridState.HybridIdle:
                            removeHybridsIdle.Add(_hybrid);
                            break;
                        case HybridState.HybridFlying:
                            removeHybridsFlying.Add(_hybrid);
                            break;
                        case HybridState.HybridHitWater:
                            removeHybridsHitWater.Add(_hybrid);
                            break;
                        case HybridState.HybridFlocking or HybridState.HybridAvoidingWall:
                            removeHybridsSwimming.Add(_hybrid);
                            break;
                        case HybridState.HybridMiniGame_SwimToLure:
                            removeHybridSwimToLure.Add(_hybrid);
                            break;
                    }
                    if (_RemoveFromPond)
                        _removeFromHybridsToNavList.Add(_hybrid);
                }
            }
        }

        public void removeHybrid(FOEItem_Hybrid _hybridInfo, bool _RemoveFromPond)
        {

            switch (_hybridInfo.navigationData.hybridState)
            {
                case HybridState.HybridIdle:
                    removeHybridsIdle.Add(_hybridInfo);
                    break;
                case HybridState.HybridFlying:
                    removeHybridsFlying.Add(_hybridInfo);
                    break;
                case HybridState.HybridHitWater:
                    removeHybridsHitWater.Add(_hybridInfo);
                    break;
                case HybridState.HybridFlocking or HybridState.HybridAvoidingWall:
                    removeHybridsSwimming.Add(_hybridInfo);
                    break;
                case HybridState.HybridMiniGame_SwimToLure:
                    removeHybridSwimToLure.Add(_hybridInfo);
                    break;
            }
            if (_RemoveFromPond)
            {
                _removeFromHybridsToNavList.Add(_hybridInfo);
            }
        }

        public void AddHybridTolist(GameObject go)
        {
            var _hybrid = GetHybridFromGO(go);

            if (_hybrid.europaItemData.itemGO == go)
            {
                print(_hybrid.navigationData.hybridState);
                switch (_hybrid.navigationData.hybridState)
                {
                    case HybridState.HybridIdle:
                        hybridsIdle.Add(_hybrid);
                        break;
                    case HybridState.HybridFlying:
                        hybridsFlying.Add(_hybrid);
                        break;
                    case HybridState.HybridHitWater:
                        hybridsHitWater.Add(_hybrid);
                        break;
                    case HybridState.HybridFlocking or HybridState.HybridAvoidingWall:
                        hybridsSwimming.Add(_hybrid);
                        break;
                    case HybridState.HybridMiniGame_SwimToLure:
                        print("SWIM");
                        hybridSwimToPoint.Add(_hybrid);
                        break;
                }
            }
        }

        public void AddHybridTolist(FOEItem_Hybrid _hybrid)
        {

            print(_hybrid.navigationData.hybridState);
            switch (_hybrid.navigationData.hybridState)
            {
                case HybridState.HybridIdle:
                    hybridsIdle.Add(_hybrid);
                    break;
                case HybridState.HybridFlying:
                    hybridsFlying.Add(_hybrid);
                    break;
                case HybridState.HybridHitWater:
                    hybridsHitWater.Add(_hybrid);
                    break;
                case HybridState.HybridFlocking or HybridState.HybridAvoidingWall:
                    hybridsSwimming.Add(_hybrid);
                    break;
                case HybridState.HybridMiniGame_SwimToLure:
                    print("SWIM");
                    hybridSwimToPoint.Add(_hybrid);
                    break;

            }
        }

            public void AddHybridTolist(FOEItem_Hybrid _hybrid, HybridState _state)
        {
            switch (_state)
            {
                case HybridState.HybridIdle:
                    hybridsIdle.Add(_hybrid);
                    break;
                case HybridState.HybridFlying:
                    hybridsFlying.Add(_hybrid);
                    break;
                case HybridState.HybridHitWater:
                    hybridsHitWater.Add(_hybrid);
                    break;
                case HybridState.HybridFlocking or HybridState.HybridAvoidingWall:
                    hybridsSwimming.Add(_hybrid);
                    break;
                case HybridState.HybridMiniGame_SwimToLure:
                    print("SWIM");
                    hybridSwimToPoint.Add(_hybrid);
                    break;
            }
        }

        public void OnPickUp(FOEItem_Hybrid _hybridInfo)
        {
            _hybridInfo.SetVisuals(HybridVisualsState.Bubble);
            removeHybrid(_hybridInfo.europaItemData.itemGO, true);
        }

        public void OnDrop(FOEItem_Hybrid _hybridInfo)
        {
            _hybridInfo.SetVisuals(HybridVisualsState.World);

            _hybridsToNavList.Add(_hybridInfo);
        }

        public void RemoveAllHybrids()
        {

        }

        /// <summary>
        /// update the hybrid state Using a gameObject
        /// </summary>
        /// <param name="_hybridGO"></param>
        /// <param name="_HybridState"></param>
        public void UpdateHybridState(GameObject _hybridGO, HybridState _HybridState)
        {
            FOEItem_Hybrid _hybrid = GetHybridFromGO(_hybridGO);
            _hybrid.navigationData.hybridState = _HybridState;
            print(_hybrid.navigationData.hybridState);
        }

        /// <summary>
        /// Update the Hybrid State Using the HybridNavData
        /// </summary>
        /// <param name="_hybridGO"></param>
        /// <param name="_HybridState"></param>
        public void UpdateHybridState(FOEItem_Hybrid _hybridGO, HybridState _HybridState)
        {
            _hybridGO.navigationData.hybridState = _HybridState;
        }

        //dont call, called updateHybridState
        /// <summary>
        /// HelperFuction to find a hybrid NavData Using a GameObject
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public FOEItem_Hybrid GetHybridFromGO(GameObject go)
        {
            foreach (FOEItem_Hybrid _hybrid in _hybridsToNavList)
            {
                if (_hybrid.europaItemData.itemGO == go)
                {
                    return _hybrid;
                }
            }
            return null;
        }

        /// <summary>
        /// Return true if hybrid is currently in passed state
        /// </summary>
        /// <param name="go"></param>
        /// <param name="hybridState"></param>
        /// <returns></returns>
        public bool CheckHybridState(GameObject _hybridGO, HybridState _HybridState)
        {
            FOEItem_Hybrid _hybrid = GetHybridFromGO(_hybridGO);


            if (_hybrid.navigationData.hybridState == _HybridState)
                return true;
            else return false;

        }

        public bool CheckHybridState(FOEItem_Hybrid _hybrid, HybridState _HybridState)
        {
            if (_hybrid.navigationData.hybridState == _HybridState)
                return true;
            else return false;
        }


    }
}



