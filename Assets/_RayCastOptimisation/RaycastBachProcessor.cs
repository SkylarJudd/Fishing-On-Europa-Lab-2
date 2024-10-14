using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Europa
{
    public class RaycastBachProcessor : Singleton<RaycastBachProcessor>
    {
        [SerializeField] int maxRayCastsPerJob = 1000;

        NativeArray<RaycastCommand> commandsList;
        NativeArray<RaycastHit> hitResults;

        public void PerformRayCasts(Vector3[] origins, Vector3[] directions, int layerMask, bool hitBackFaces, bool HitTriggers, bool HitMultiFace, Action<RaycastHit[]> callback)
        {
            const float maxDistance = 0.4f;
            int rayCount = Mathf.Min(origins.Length, maxRayCastsPerJob);

            QueryTriggerInteraction queryTriggerInteraction = HitTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            using (commandsList = new NativeArray<RaycastCommand>(rayCount, Allocator.TempJob))
            {
                QueryParameters parameters = new QueryParameters
                {
                    layerMask = layerMask,
                    hitBackfaces = hitBackFaces,
                    hitTriggers = queryTriggerInteraction,
                    hitMultipleFaces = HitMultiFace,
                };

                for (int i = 0; i < rayCount; i++)
                {
                    commandsList[i] = new RaycastCommand(origins[i], directions[i], parameters, maxDistance);
                }

                ExecuteRaycasts(commandsList, callback);
            }
        }

        private void ExecuteRaycasts(NativeArray<RaycastCommand> raycastCommands, Action<RaycastHit[]> callback)
        {
            int maxHitsPerRaycast = 1;
            int titalHitsNeeded = raycastCommands.Length * maxHitsPerRaycast;

            using (hitResults = new NativeArray<RaycastHit>(titalHitsNeeded, Allocator.TempJob))
            {
                foreach (RaycastCommand t in raycastCommands)
                {
                    Debug.DrawLine(t.from, t.from + t.direction * 1f, Color.red, 0.5f);
                }

                JobHandle raycastJobHandle = RaycastCommand.ScheduleBatch(raycastCommands, hitResults, maxHitsPerRaycast);
                raycastJobHandle.Complete();

                if (hitResults.Length > 0)
                {
                    RaycastHit[] results = hitResults.ToArray();

                    for (int i = 0; i < results.Length; i++)
                    {
                        if (results[i].collider != null)
                        {
                            Debug.Log($"Hit {results[i].collider.name} at {results[i].point}");
                            Debug.DrawLine(raycastCommands[i].from, results[i].point, Color.green, 1.0f);
                        }
                    }
                    callback?.Invoke(results);
                }
            }
        }
    }
}
