using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand.Demo;
using System;
using NaughtyAttributes;
using UnityEngine.Serialization;
using Europa;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Autohand {
    

    

    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider)), DefaultExecutionOrder(1)]
    [HelpURL("https://app.gitbook.com/s/5zKO0EvOjzUDeT2aiFk3/auto-hand-3.1/auto-hand-player")]
    public class AutoHandSkybox : MonoBehaviour {


        static bool notFound = false;
        public static AutoHandSkybox _Instance;
        public static AutoHandSkybox Instance {
            get {
                if(_Instance == null && !notFound)
                    _Instance = AutoHandExtensions.CanFindObjectOfType<AutoHandSkybox>();

                if(_Instance == null)
                    notFound = true;

                return _Instance;
            }
        }



        [AutoHeader("Auto Hand Player")]
        public bool ignoreMe;



        [Tooltip("The tracked headCamera object")]
        public Camera headCamera;
        [Tooltip("The object that represents the forward direction movement, usually should be set as the camera or a tracked controller")]
        public Transform forwardFollow;
        [Tooltip("This should NOT be a child of this body. This should be a GameObject that contains all the tracked objects (head/controllers)")]
        public Transform trackingContainer;
        


        [Tooltip("Player settings stored in a ScriptableObject")]
        public SettingsSO playerSettings;

       
            [AutoToggleHeader("Movement")]
        public bool useMovement = true;
        [EnableIf("useMovement"), FormerlySerializedAs("moveSpeed")]
        [Tooltip("Movement speed when isGrounded")]
        public float maxMoveSpeed = 2.3f;
        [EnableIf("useMovement")]
        [Tooltip("Movement acceleration when isGrounded")]
        public float moveAcceleration = 100000f;
        [EnableIf("useMovement")]
        [Tooltip("Whether or not to use snap turning or smooth turning"), Min(0)]
        public RotationType rotationType =  RotationType.snap;
        [Tooltip("turn speed when not using snap turning - if snap turning, represents angle per snap")]
        public float snapTurnAngle = 30f;
        public float smoothTurnSpeed = 180f;
        public bool bodyFollowsHead = true;
        public float maxHeadDistance = 0.5f;



        [AutoToggleHeader("Height")]
        public bool showHeight = true;
        [ShowIf("showHeight"), Tooltip("Smooths camera upward movement when stepping up")]
        public float heightSmoothSpeed = 10f;
        [ShowIf("showHeight")]
        public float heightOffset = 0f;
        [ShowIf("showHeight")]
        public bool crouching = false;
        [ShowIf("showHeight")]
        public float crouchHeight = 0.6f;
        [ShowIf("showHeight")]
        [Tooltip("Whether or not the capsule height should be adjusted to match the headCamera height")]
        public bool autoAdjustColliderHeight = true;
        [ShowIf("showHeight")]
        [Tooltip("Minimum and maximum auto adjusted height, to adjust height without auto adjustment change capsule collider height instead")]
        public Vector2 minMaxHeight = new Vector2(0.5f, 2.5f);
        [ShowIf("showHeight")]
        public bool useHeadCollision = true;
        [ShowIf("showHeight")]
        public float headRadius = 0.15f;




        [AutoToggleHeader("Use Grounding")]
        public bool useGrounding = true;
        [EnableIf("useGrounding"), Tooltip("Maximum height that the body can step up onto"), Min(0)]
        public float maxStepHeight = 0.3f;
        [Tooltip("The space between the bottom of the body and the spherecast the checks for the ground and steps")]
        public float groundingPenetrationOffset = 0.1f;
        [EnableIf("useGrounding"), Tooltip("Maximum angle the player can walk on"), Min(0)]
        public float maxStepAngle = 45f;
        [EnableIf("useGrounding"), Tooltip("The layers that count as ground")]
        public LayerMask groundLayerMask;
        [EnableIf("useGrounding"), Tooltip("Movement acceleration when isGrounded")]
        public float groundedDrag = 10000f;
        [Tooltip("Movement acceleration when grounding is disabled")]
        public float flyingDrag = 4f;



        [AutoToggleHeader("Enable Climbing")]
        [Tooltip("Whether or not the player can use Climbable objects  (Objects with the Climbable component)")]
        public bool allowClimbing = true;
        [Tooltip("Whether or not the player move while climbing")]
        [ShowIf("allowClimbing")]
        public bool allowClimbingMovement = true;
        [Tooltip("How quickly the player can climb")]
        [ShowIf("allowClimbing")]
        public Vector3 climbingStrength = new Vector3(20f, 20f, 20f);
        public float climbingAcceleration = 30f;
        public float climbingDrag = 5f;
        [Tooltip("Inscreases the step height while climbing up to make it easier to step up onto a surface")]
        public float climbUpStepHeightMultiplier = 3f;



        [AutoToggleHeader("Enable Pushing")]
        [Tooltip("Whether or not the player can use Pushable objects (Objects with the Pushable component)")]
        public bool allowBodyPushing = true;
        [Tooltip("How quickly the player can climb")]
        [EnableIf("allowBodyPushing")]
        public Vector3 pushingStrength = new Vector3(10f, 10f, 10f);
        public float pushingAcceleration = 10f;
        public float pushingDrag = 1f;
        [Tooltip("Inscreases the step height while pushing up to make it easier to step up onto a surface")]
        public float pushUpStepHeightMultiplier = 3f;



        [AutoToggleHeader("Enable Platforming")]
        [Tooltip("Platforms will move the player with them. A platform is an object with the Transform component on it")]
        public bool allowPlatforms = true;
        [EnableIf("useGrounding"), Tooltip("The layers that platforming will be enabled on, will not work with layers that the HandPlayer can't collide with")]
        public LayerMask platformingLayerMask = ~0;


        public AutoHandPlayerEvent OnSnapTurn;
        public AutoHandPlayerEvent OnSmoothTurn;
        public AutoHandPlayerEvent OnTeleported;


        [HideInInspector]
        /// <summary>Amount of input required to trigger movement, some vr controllers will return > 0.1 input when not touching the thumbsticks so using a deadzone to prevent movement drift</summary>
        public float movementDeadzone = 0.2f;
        [HideInInspector]
        /// <summary>Amount of input required to trigger a turn, some vr controllers will return > 0.1 input when not touching the thumbsticks so using a deadzone of 0.35 or higher is important</summary>
        public float turnDeadzone = 0.4f;
        [HideInInspector]
        /// <summary>Amount of input required to reset the turn state for snap turning</summary>
        public float turnResetzone = 0.3f;


        //How many times the head will attempt to move the body to the head and depenetrate it from colliders in a single frame,
        //this value uses nested loop so the actual iterations will be 2^bodySyncMaxIterations. Recommended value of 2-4
        const int bodySyncMaxIterations = 2;
        //How many times the head will attempt to depenetrate from colliders in a single frame. Recommended value of 3-8
        const int headCollisionMaxIterations = 4;

        public const string HandPlayerLayer = "HandPlayer";

        public CapsuleCollider bodyCollider { get { return bodyCapsule; } }

        public Rigidbody body { get; protected set; }

        public RaycastHit lastGroundHit { get; protected set; }









        protected HeadPhysicsFollower headPhysicsFollower;
        protected Vector3 moveDirection;
        protected float turningAxis;

        protected Vector3 climbAxis;
        protected Dictionary<Hand, Climbable> climbing = new Dictionary<Hand, Climbable>();

        protected Vector3 pushAxis;
        protected Dictionary<Pushable, Hand> pushRight = new Dictionary<Pushable, Hand>();
        protected Dictionary<Pushable, int> pushRightCount = new Dictionary<Pushable, int>();
        protected Dictionary<Pushable, Hand> pushLeft = new Dictionary<Pushable, Hand>();
        protected Dictionary<Pushable, int> pushLeftCount = new Dictionary<Pushable, int>();

        protected CapsuleCollider bodyCapsule;
        protected Hand lastRightHand;
        protected Hand lastLeftHand;
        protected Collider[] colliderNonAlloc = new Collider[128];

        protected bool trackingStarted = false;
        protected bool isGrounded = false;
        protected bool axisReset = true;
        protected bool tempDisableGrounding = false;
        protected bool lastCrouching;
        protected float lastCrouchingHeight;
        protected float playerHeight;
        protected Vector3 lastUpdatePosition;
        protected Vector3 lastHeadPos;

        Vector3 targetTrackedPos;
        Vector3 targetPosOffset;
        Vector3 lastPlatformPosition;
        Quaternion lastPlatformRotation;
        public RaycastHit lastPlatformingHit { get; protected set; }

        float headHeightOffset;
        float highestPoint;
        int handPlayerMask;

        public virtual void Awake() {
            if(_Instance == null) {
                _Instance = this;
                notFound = false;
            }

            lastUpdatePosition = transform.position;

            gameObject.layer = LayerMask.NameToLayer(HandPlayerLayer);

            bodyCapsule = GetComponent<CapsuleCollider>();
            bodyCapsule.material = Resources.Load<PhysicsMaterial>("NoFriction");

            body = GetComponent<Rigidbody>();
            body.interpolation = RigidbodyInterpolation.None;
            body.freezeRotation = true;
            if(body.collisionDetectionMode == CollisionDetectionMode.Discrete)
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if(forwardFollow == null)
                forwardFollow = headCamera.transform;

            targetTrackedPos = trackingContainer.position;
            if(useHeadCollision)
                CreateHeadFollower();
        }


        public virtual void Start() {
            StartCoroutine(WaitFlagForTrackingStart());

            handPlayerMask = AutoHandExtensions.GetPhysicsLayerMask(gameObject.layer);
#if UNITY_EDITOR
                if (Selection.activeGameObject == gameObject)
                {
                    Selection.activeGameObject = null;
                    Debug.Log("Auto Hand: highlighting hand component in the inspector can cause lag and quality reduction at runtime in VR. (Automatically deselecting at runtime) Remove this code at any time.", this);
                    Application.quitting += () => { if (Selection.activeGameObject == null) Selection.activeGameObject = gameObject; };
                }

#endif
        }


        IEnumerator WaitFlagForTrackingStart() {
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            lastHeadPos = headCamera.transform.position;
            while(!trackingStarted) {
                if(headCamera.transform.position != lastHeadPos)
                    trackingStarted = true;
                lastHeadPos = headCamera.transform.position;
                yield return new WaitForEndOfFrame();
            }
        }

        void CreateHeadFollower() {
            if(headPhysicsFollower == null) {
                var headFollower = new GameObject().transform;
                headFollower.transform.position = headCamera.transform.position;
                headFollower.name = "Head Follower";
                headFollower.parent = transform.parent;

                var col = headFollower.gameObject.AddComponent<SphereCollider>();
                col.material = bodyCapsule.material;
                col.radius = headRadius;

                var headBody = headFollower.gameObject.AddComponent<Rigidbody>();
                headBody.linearDamping = 5;
                headBody.angularDamping = 5;
                headBody.freezeRotation = false;
                headBody.useGravity = false;
                headBody.mass = body.mass / 3f;

                headPhysicsFollower = headFollower.gameObject.AddComponent<HeadPhysicsFollower>();
                headPhysicsFollower.headCamera = headCamera;
                headPhysicsFollower.followBody = transform;
                headPhysicsFollower.trackingContainer = trackingContainer;
                headPhysicsFollower.maxBodyDistance = maxHeadDistance;
                headPhysicsFollower.Init();
            }
        }


        public void IgnoreCollider(Collider col, bool ignore) {
            Physics.IgnoreCollision(bodyCapsule, col, ignore);
            Physics.IgnoreCollision(headPhysicsFollower.headCollider, col, ignore);
        }


        /// <summary>Sets move direction for this fixedupdate</summary>
        public virtual void Move(Vector2 axis, bool useDeadzone = true, bool useRelativeDirection = false) {
            moveDirection.x = (!useDeadzone || Mathf.Abs(axis.x) > movementDeadzone) ? axis.x : 0;
            moveDirection.z = (!useDeadzone || Mathf.Abs(axis.y) > movementDeadzone) ? axis.y : 0;
            if(useRelativeDirection)
                moveDirection = transform.rotation * moveDirection;
        }

        public virtual void Turn(float turnAxis) {
            turnAxis = (Mathf.Abs(turnAxis) > turnDeadzone) ? turnAxis : 0;
            turningAxis = turnAxis;
        }

        protected virtual void LateUpdate() {
            if(useMovement) {
                UpdateTurn(Time.deltaTime);
            }
        }

        protected virtual void FixedUpdate() {
            UpdatePlayerHeight();

            if(useMovement) {
                UpdateRigidbody();
                UpdatePlatform();
                Ground();
            }
        }


        protected virtual void UpdateRigidbody() {
            var move = AlterDirection(moveDirection);
            var yVel = body.linearVelocity.y;

            //1. Moves velocity towards desired push direction
            if (pushAxis != Vector3.zero) {
                body.linearVelocity = Vector3.MoveTowards(body.linearVelocity, pushAxis, pushingAcceleration * Time.fixedDeltaTime);
                body.linearVelocity *= Mathf.Clamp01(1 - pushingDrag * Time.fixedDeltaTime);
            }

            //2. Moves velocity towards desired climb direction
            if(climbAxis != Vector3.zero) {
                body.linearVelocity = Vector3.MoveTowards(body.linearVelocity, climbAxis, climbingAcceleration * Time.fixedDeltaTime);
                body.linearVelocity *= Mathf.Clamp01(1 - climbingDrag * Time.fixedDeltaTime);
            }

            //3. Moves velocity towards desired movement direction
            if(move != Vector3.zero && CanInputMove()) {

                var newVel = Vector3.MoveTowards(body.linearVelocity, move * maxMoveSpeed, moveAcceleration * Time.fixedDeltaTime);
                if(newVel.magnitude > maxMoveSpeed)
                    newVel = newVel.normalized * maxMoveSpeed;
                body.linearVelocity = newVel;
            }

            //5. Checks if gravity should be turned off
            if (IsClimbing() || pushAxis.y > 0)
                body.useGravity = false;


            //4. This creates extra drag when grounded to simulate foot strength, or if flying greats drag in every direction when not moving
            if (move.magnitude <= movementDeadzone && isGrounded)
                body.linearVelocity *= (Mathf.Clamp01(1 - groundedDrag * Time.fixedDeltaTime));
            else if(!useGrounding)
                body.linearVelocity *= (Mathf.Clamp01(1 - flyingDrag * Time.fixedDeltaTime));

            //6. This will keep velocity if consistent when moving while falling
            if(body.useGravity)
                body.linearVelocity = new Vector3(body.linearVelocity.x, yVel, body.linearVelocity.z);

            //7. This will move the body to track the head in tracking space without overlapping colliders
            if(bodyFollowsHead) {
                PreventHeadOverlap();
            }
        }



        /// <summary>This function is responsible for keeping the body matching the head position when moving the head within the tracking space</summary>
        protected virtual void PreventHeadOverlap() {
            //By using moveTowards with a radius * 0.95f, we can prevent the spheres from ever clipping through objects
            //because it will always depenetrate to the correct direction from the last depenetrated position
            Vector3 currentHeadPosition = headCamera.transform.position;
            Vector3 cameraHeadPosition = Vector3.MoveTowards(lastHeadPos, currentHeadPosition, headRadius*0.95f);

            int overlapCount = Physics.OverlapSphereNonAlloc(
                cameraHeadPosition,
                headRadius,
                colliderNonAlloc,
                handPlayerMask,
                QueryTriggerInteraction.Ignore
            );

            

            //If the head is overlapping with something, move the head/body away from the overlapped objects
            if(overlapCount > 0) {
                int attempts = 0;
                while(overlapCount > 0 && attempts < headCollisionMaxIterations) {
                    Vector3 averageDepentration = Vector3.zero;
                    for(int j = 0; j < overlapCount; j++) {
                        Collider otherCollider = colliderNonAlloc[j];
                        if(Physics.ComputePenetration(otherCollider, otherCollider.transform.position, otherCollider.transform.rotation, headPhysicsFollower.headCollider, cameraHeadPosition, transform.rotation, out var closestDepentrationDirection, out var closestDepenetrationDistance)) {
                            averageDepentration += (closestDepentrationDirection * closestDepenetrationDistance);
                        }
                    }

                    //Offsets the head on the Y-axis, but push the body away on the X/Z-axis
                    headHeightOffset -= averageDepentration.y;
                    targetTrackedPos -= Vector3.up * averageDepentration.y;
                    cameraHeadPosition.y -= averageDepentration.y;
                    currentHeadPosition -= averageDepentration;
                    averageDepentration.y = 0f;

                    transform.position -= averageDepentration;
                    cameraHeadPosition -= averageDepentration;
                    body.position = transform.position; 
                    
                    cameraHeadPosition = Vector3.MoveTowards(cameraHeadPosition, currentHeadPosition, headRadius*0.95f);

                    overlapCount = Physics.OverlapSphereNonAlloc(
                        cameraHeadPosition,
                        headRadius,
                        colliderNonAlloc,
                        handPlayerMask,
                        QueryTriggerInteraction.Ignore
                    );


                    attempts++;
                }
            }
            //If there is height debt to pay, pay it
            else if(headHeightOffset != 0) {
                Vector3 castDirection = (headHeightOffset > 0) ? -Vector3.up : Vector3.up;
                float castDistance = Mathf.Abs(headHeightOffset);
                Vector3 startPosition = cameraHeadPosition + Vector3.up * Mathf.Sign(headHeightOffset) * 0.001f;

                //Check if there is space to move the head to pay of the height debt
                //Using a 0.001f buffer to prevent the sphere cast from ignoring the collider if perfectly flush
                if(Physics.SphereCast(startPosition, headRadius, castDirection, out var hit, castDistance, handPlayerMask, QueryTriggerInteraction.Ignore) && hit.distance > 0.001f) {
                    var adjustment = headHeightOffset - Mathf.MoveTowards(headHeightOffset, 0, hit.distance - 0.001f);
                    headHeightOffset -= adjustment;
                    cameraHeadPosition.y -= adjustment;
                    targetTrackedPos -= Vector3.up * adjustment;
                }
                //If there is no space to move the head to pay of the height debt, just move the body
                else {
                    targetTrackedPos -= Vector3.up * headHeightOffset;
                    cameraHeadPosition.y -= headHeightOffset;
                    headHeightOffset = 0;
                }
            }

            if(trackingStarted)
                lastHeadPos = cameraHeadPosition;
        }
        
        protected virtual bool CanInputMove() {
            return (allowClimbingMovement || !IsClimbing());
        }




        protected virtual void UpdateTurn(float deltaTime) {

            //Snap turning
            if(rotationType == RotationType.snap) {
                if(Mathf.Abs(turningAxis) > turnDeadzone && axisReset) {
                    var angle = turningAxis > turnDeadzone ? playerSettings.turnAngle : -playerSettings.turnAngle;

                    var targetPos = transform.position - headCamera.transform.position; targetPos.y = 0;

                    trackingContainer.position += targetPos;
                    if(headPhysicsFollower != null) {
                        headPhysicsFollower.transform.position += targetPos;
                        headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
                    }

                    lastUpdatePosition = new Vector3(transform.position.x, lastUpdatePosition.y, transform.position.z);
                    

                    trackingContainer.RotateAround(transform.position, Vector3.up, angle);
                    targetPosOffset = Vector3.zero;
                    targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y, trackingContainer.position.z);

                   
                    Physics.SyncTransforms();

                    
                    axisReset = false;
                }
            }
            else if(Mathf.Abs(turningAxis) > turnDeadzone) {
                
                lastUpdatePosition = new Vector3(transform.position.x, lastUpdatePosition.y, transform.position.z);
                trackingContainer.RotateAround(transform.position, Vector3.up, playerSettings.turnSpeed * (Mathf.MoveTowards(turningAxis, 0, turnDeadzone)) * deltaTime);

                targetPosOffset = Vector3.zero;
                targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y, trackingContainer.position.z);

                
                Physics.SyncTransforms();

                
                axisReset = false;
            }

            if(Mathf.Abs(turningAxis) < turnResetzone)
                axisReset = true;
        }


        RaycastHit[] hitsNonAlloc = new RaycastHit[128];
        protected virtual void Ground() {
            isGrounded = false;
            lastGroundHit = new RaycastHit();

            if(!tempDisableGrounding && useGrounding && !IsClimbing() && !(pushAxis.y > 0)) {
                highestPoint = -1;

                float stepAngle;
                float dist;
                float scale = transform.lossyScale.x > transform.lossyScale.z ? transform.lossyScale.x : transform.lossyScale.z;

                var maxStepHeight = this.maxStepHeight;
                maxStepHeight *= climbAxis.y > 0 ? climbUpStepHeightMultiplier : 1;
                maxStepHeight *= pushAxis.y > 0 ? pushUpStepHeightMultiplier : 1;
                maxStepHeight *= scale;

                var point1 = scale * bodyCapsule.center + transform.position + scale * bodyCapsule.height / 2f * -Vector3.up + (maxStepHeight + scale * bodyCapsule.radius * 2) * Vector3.up;
                var point2 = scale * bodyCapsule.center + transform.position + (scale * bodyCapsule.height / 2f + groundingPenetrationOffset) * -Vector3.up;

                var radius = scale*bodyCapsule.radius*2 + Physics.defaultContactOffset*2;
                int hitCount = Physics.SphereCastNonAlloc(point1, radius, -Vector3.up, hitsNonAlloc, Vector3.Distance(point1, point2) + scale * bodyCapsule.radius*4, groundLayerMask, QueryTriggerInteraction.Ignore);

                //Inital Grounding Check includes the body and the area right around it
                CheckGroundHits();

                if(!isGrounded && hitCount > 0) {
                    //If its hitting something but not valid ground, check the smaller area just below the feet.
                    //This specifically fixes a bug where a mesh collider wont return a valid grounding hit when standing against a verticle step just above the max step height
                    //This is because spherecast will only return the first highest valid hit per collider
                    radius = scale*bodyCapsule.radius;
                    hitCount = Physics.SphereCastNonAlloc(point1, radius, -Vector3.up, hitsNonAlloc, Vector3.Distance(point1, point2) + scale * bodyCapsule.radius*4, groundLayerMask, QueryTriggerInteraction.Ignore);

                    CheckGroundHits();
                }


                void CheckGroundHits() {
                    for(int i = 0; i < hitCount; i++) {
                        var hit = hitsNonAlloc[i];

                        if(hit.collider != bodyCapsule) {
                            if(hit.point.y >= point2.y && hit.point.y <= point2.y + maxStepHeight + groundingPenetrationOffset) {
                                stepAngle = Vector3.Angle(hit.normal, Vector3.up);
                                dist = hit.point.y - transform.position.y;

                                if(stepAngle < maxStepAngle && dist > highestPoint) {
                                    isGrounded = true;
                                    highestPoint = dist;
                                    lastGroundHit = hit;
                                }
                            }
                        }
                    }
                }


                if(isGrounded) {
                    body.linearVelocity = new Vector3(body.linearVelocity.x, 0, body.linearVelocity.z);
                    body.position = new Vector3(body.position.x, lastGroundHit.point.y, body.position.z);
                    transform.position = body.position;
                }

                body.useGravity = !isGrounded;
            }
        }

        public bool IsGrounded() {
            return isGrounded;
        }

        public void ToggleFlying() {
            useGrounding = !useGrounding;
            body.useGravity = useGrounding;
        }

        protected virtual void UpdatePlayerHeight() {
            if(crouching != lastCrouching) {
                if(lastCrouching)
                    heightOffset += lastCrouchingHeight;
                if(!lastCrouching)
                    heightOffset -= crouchHeight;

                lastCrouching = crouching;
                lastCrouchingHeight = crouchHeight;
            }

            if(autoAdjustColliderHeight) {
                playerHeight = Mathf.Clamp(headCamera.transform.position.y - transform.position.y, minMaxHeight.x, minMaxHeight.y);
                bodyCapsule.height = playerHeight;
                var centerHeight = playerHeight / 2f > bodyCapsule.radius ? playerHeight / 2f : bodyCapsule.radius;
                bodyCapsule.center = new Vector3(0, centerHeight, 0);
            }
        }


        protected void UpdatePlatform(){
            if (isGrounded && lastGroundHit.transform != null && (platformingLayerMask == (platformingLayerMask | (1 << lastGroundHit.collider.gameObject.layer)))) {
                if (!lastGroundHit.transform.Equals(lastPlatformingHit.transform)) {
                    lastPlatformingHit = lastGroundHit;
                    lastPlatformPosition = lastPlatformingHit.transform.position;
                    lastPlatformRotation = lastPlatformingHit.transform.rotation;
                }
                else if(lastGroundHit.transform.Equals(lastPlatformingHit.transform))
                {
                    if (lastPlatformingHit.transform.position != lastPlatformPosition || lastPlatformingHit.transform.rotation.eulerAngles != lastPlatformRotation.eulerAngles) {
                        lastPlatformingHit = lastGroundHit;
                        Transform ruler = AutoHandExtensions.transformRuler;
                        ruler.position = transform.position;
                        ruler.rotation = transform.rotation;
                        ruler.position += lastPlatformingHit.transform.position - lastPlatformPosition;

                        var deltaPos = ruler.transform.position - transform.position;
                        var deltaRot = (lastPlatformingHit.transform.rotation * Quaternion.Inverse(lastPlatformRotation));

                        ruler.transform.RotateAround(lastPlatformingHit.transform.position, Vector3.up, deltaRot.eulerAngles.y);
                        trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, deltaRot.eulerAngles.y);

                        transform.position += deltaPos;
                        body.position = transform.position;

                        trackingContainer.position += deltaPos;

                        lastUpdatePosition = transform.position;

                        targetPosOffset = Vector3.zero;

                        targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y + deltaPos.y, trackingContainer.position.z);
                        lastPlatformPosition = lastPlatformingHit.transform.position;
                        lastPlatformRotation = lastPlatformingHit.transform.rotation;
                    }
                }
            }
        }


        /// <summary>Legacy function, use body.addfoce instead</summary>
        public void AddVelocity(Vector3 force, ForceMode mode = ForceMode.Acceleration) {
            body.AddForce(force, mode);
        }

       

        protected virtual void StopPush(Hand hand, GameObject other) {
            if(!allowBodyPushing)
                return;

            if(other.CanGetComponent(out Pushable push)) {
                if(hand.left && pushLeft.ContainsKey(push)) {
                    var count = --pushLeftCount[push];
                    if(count == 0) {
                        pushLeft.Remove(push);
                        pushLeftCount.Remove(push);
                    }
                }
                if(!hand.left && pushRight.ContainsKey(push)) {
                    var count = --pushRightCount[push];
                    if(count == 0) {
                        pushRight.Remove(push);
                        pushRightCount.Remove(push);
                    }
                }
            }
        }

       

        

        public bool IsClimbing() {
            foreach(var climb in climbing)
                if(climb.Value.enabled)
                    return true;
            return false;
        }



        public virtual void SetPosition(Vector3 position) {
            SetPosition(position, headCamera.transform.rotation);
        }

        public virtual void SetPosition(Vector3 position, Quaternion rotation) {
            Vector3 deltaPos = position - transform.position;
            transform.position += deltaPos; 
            //This code will move the tracking objects to match the body collider position when moving
            var targetPos = transform.position - headCamera.transform.position; targetPos.y = deltaPos.y;
            trackingContainer.position += targetPos;
            lastUpdatePosition = transform.position;
            targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y + deltaPos.y, trackingContainer.position.z);
            targetPosOffset = Vector3.zero;
            body.position = transform.position;
            if(headPhysicsFollower != null) {
                headPhysicsFollower.transform.position += targetPos;
                headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
            }

            

            var deltaRot = rotation * Quaternion.Inverse(headCamera.transform.rotation);
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, deltaRot.eulerAngles.y);

            if(deltaRot.eulerAngles.magnitude > 10f || deltaPos.magnitude > 0.5f)
                

            lastHeadPos = headCamera.transform.position;
        }

        public virtual void SetRotation(Quaternion rotation) {
            var targetPos = transform.position - headCamera.transform.position; targetPos.y = 0;

            trackingContainer.position += targetPos;
            if(headPhysicsFollower != null) {
                headPhysicsFollower.transform.position += targetPos;
                headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
            }
            lastUpdatePosition = transform.position;

            var deltaRot = rotation * Quaternion.Inverse(headCamera.transform.rotation);
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, deltaRot.eulerAngles.y);

            targetPosOffset = Vector3.zero;
            targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y, trackingContainer.position.z);

            
        }

        public virtual void AddRotation(Quaternion addRotation) {
            var targetPos = transform.position - headCamera.transform.position; targetPos.y = 0;

            trackingContainer.position += targetPos;
            if(headPhysicsFollower != null) {
                headPhysicsFollower.transform.position += targetPos;
                headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
            }
            lastUpdatePosition = transform.position;

            trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, addRotation.eulerAngles.y);

            targetPosOffset = Vector3.zero;
            targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y, trackingContainer.position.z);

            
        }

        public virtual void Recenter() {
            var targetPos = transform.position - headCamera.transform.position; targetPos.y = 0;

            trackingContainer.position += targetPos;
            if(headPhysicsFollower != null) {
                headPhysicsFollower.transform.position += targetPos;
                headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
            }
            lastUpdatePosition = transform.position;

            targetPosOffset = Vector3.zero;
            targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y, trackingContainer.position.z);
        }

      

        protected virtual Vector3 AlterDirection(Vector3 moveAxis) {
            if(useGrounding)
                return Quaternion.AngleAxis(forwardFollow.eulerAngles.y, Vector3.up) * (new Vector3(moveAxis.x, moveAxis.y, moveAxis.z));
            else
                return forwardFollow.rotation * (new Vector3(moveAxis.x, moveAxis.y, moveAxis.z));
        }


    }
}
