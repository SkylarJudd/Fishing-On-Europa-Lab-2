using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Europa
{
    public class TempMiniGame : GameBehaviour
    {
        [SerializeField] MiniGameState miniGameState;

        [SerializeField] GameObject player;
        [SerializeField] GameObject lure;

        [SerializeField] Transform lureTargetPostion;
        [SerializeField] Vector3 rotationAxis;

        [SerializeField] PullDirections miniGamePullDirection;
        [SerializeField] PullDirections playerPullDirection;

        [SerializeField] float startingAngle;
        [SerializeField] float currentAngleOffset;
        [SerializeField] float maxAngle = 20;

        [SerializeField] int miniGameFightCount;
        [SerializeField] int maxFightCount;

        

        [SerializeField] float hybridSideChangeMax = 6;
        [SerializeField] float hybridSideChangeMin = 3;
        [SerializeField] float hybridMiniGameSpeed = 10;
        [SerializeField] float hybridCurretStamina;
        [SerializeField] float hybridMaxStamina = 20;


        [SerializeField] float pullDirectionBuffer = 3;
        [SerializeField] float currentPullDirectionBuffer;


        [SerializeField] float currentFishingRodHP;
        [SerializeField] float fishingRodHP;

        private Vector3 _playerStartLocation;

        

        private Coroutine fighingMiniGameCorutine;
        private Coroutine setHybridDirectionCoroutine;

        private void Start()
        {
            StartFightingMiniGame();
        }

        private void Update()
        {
            startingAngle = Vector3.Angle(player.transform.forward, lure.transform.position - player.transform.position);
        }

        [ContextMenu("StartMiniGame")]
        public void StartFightingMiniGame()
        {
            miniGameState = MiniGameState.HybridPulling;
            fighingMiniGameCorutine = StartCoroutine(FighingMiniGameCorutine());
        }

        public void StartHybridTiredMiniGame()
        {
            miniGameState = MiniGameState.HybridTied;

        }

        public void OnHybridEscape()
        {
            miniGameState = MiniGameState.HybridEscape;
            print("GoodBye");
            StopCoroutine(fighingMiniGameCorutine);
        }

        private IEnumerator FighingMiniGameCorutine()
        {
            miniGameFightCount++;
            if (miniGameFightCount >= maxFightCount) miniGameFightCount = maxFightCount;

            SetUpMiniGamePulling();

            while (hybridCurretStamina > 0)
            {
                yield return new WaitForEndOfFrame();
                RotateLure();
                CaculateRodHP();


                hybridCurretStamina -= Time.deltaTime;
            }

            EndMiniGamePulling();
        }
        private void RotateLure()
        {

            float deltaAngle = 10 * Time.deltaTime;
            currentAngleOffset = Vector3.Angle(player.transform.forward, lureTargetPostion.transform.position - player.transform.position) - startingAngle;

            // Calculate the shortest path to rotate back within the valid angle range
            if (Mathf.Abs(currentAngleOffset) < maxAngle)
            {
                lureTargetPostion.transform.RotateAround(player.transform.position, rotationAxis, deltaAngle);
            }
            else
            {
                // Gradually rotate back towards the maximum allowed angle
                float correctionAngle = Mathf.Sign(currentAngleOffset) * deltaAngle;

                if (rotationAxis == Vector3.up)
                {
                    lureTargetPostion.transform.RotateAround(player.transform.position, rotationAxis, -correctionAngle);
                }
                else
                {
                    lureTargetPostion.transform.RotateAround(player.transform.position, rotationAxis, correctionAngle);
                }

            }

        }

        private bool CaculateRodHP()
        {
            if (playerPullDirection != miniGamePullDirection)
            {
                if (currentPullDirectionBuffer <= pullDirectionBuffer)
                {
                    currentPullDirectionBuffer += Time.deltaTime;
                }
                else
                {
                    currentFishingRodHP -= 10 * Time.deltaTime;
                    if (currentFishingRodHP <= 0)
                    {
                        print("I called Hybrid Escaped");
                        OnHybridEscape();
                        return true;
                    }
                }
            }
            else
            {
                currentPullDirectionBuffer = 0;
            }

            return false;
        }


        private void SetUpMiniGamePulling()
        {
            SetupHybrid();
            SetUpMiniGame();
        }

        private void SetupHybrid()
        {
            hybridCurretStamina = hybridMaxStamina;
        }
        private void SetUpMiniGame()
        {
            setHybridDirectionCoroutine = StartCoroutine(SetHybridDirectionCoroutine());

            _playerStartLocation = player.transform.position;
            startingAngle = Vector3.Angle(player.transform.forward, lure.transform.position - player.transform.position);
            lureTargetPostion.position = lure.transform.position;

            currentFishingRodHP = fishingRodHP;


        }

        private void EndMiniGamePulling()
        {
            StopCoroutine(setHybridDirectionCoroutine);
            StartHybridTiredMiniGame();
        }

        private IEnumerator SetHybridDirectionCoroutine()
        {
            int random = UnityEngine.Random.Range(1, 3);
            rotationAxis = random == 1 ? rotationAxis = Vector3.up : rotationAxis = Vector3.down;
            miniGamePullDirection = random == 1 ? PullDirections.Left : PullDirections.Right;

            yield return new WaitForSeconds(UnityEngine.Random.Range(hybridSideChangeMin * 0.5f, hybridSideChangeMax * 0.5f));

            while (miniGameState == MiniGameState.HybridPulling)
            {
                FlipRotationAxis();
                yield return new WaitForSeconds(UnityEngine.Random.Range(hybridSideChangeMin, hybridSideChangeMax));
            }
        }

        private void FlipRotationAxis()
        {
            Debug.Log($"Rotation was {rotationAxis}");
            rotationAxis = rotationAxis == Vector3.down ? Vector3.up : Vector3.down;
            miniGamePullDirection = rotationAxis == Vector3.down ? PullDirections.Right : PullDirections.Left;
            Debug.Log($"Rotation is now {rotationAxis}");
        }
    }
}
