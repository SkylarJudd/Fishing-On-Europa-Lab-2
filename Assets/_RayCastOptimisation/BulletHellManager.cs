
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Pool;
using UnityUtils;



namespace Europa
{
    public class BulletHellManager : Singleton<BulletHellManager>
    {
        #region Settings And Refs
        [SerializeField] private int bulletCount = 100; // Number Of Bullets to spawn
        [SerializeField] private float bulletSpeed = 10f; // Speed Of Each Bullet
        [SerializeField] private float bulletMaxDistance = 30f; // Maximum Distance Each Bullet Can Travel
        [SerializeField] private LayerMask collisionMask; //Layer Mas For The Ray Cast Collision
        [SerializeField] private Transform bulletOrigin; // Origin Point For Bullet Spawning; 

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject impacckEffectPrefab;
        #endregion


        private ObjectPool<Bullet> bulletPool;
        private BulletPatternGenerator patternGenerator;

        [SerializeField] List<Bullet> activeProjectiles = new List<Bullet>();
        readonly List<Bullet> bulletsToReturn = new List<Bullet>();

        TransformAccessArray bulletTransform;


        private void Start()
        {
            patternGenerator = new BulletPatternGenerator(new RadialPattern());

            bulletPool = new ObjectPool<Bullet>(
                createFunc: () =>
                {
                    GameObject bulletObj = Instantiate(bulletPrefab);
                    bulletObj.SetActive(false);
                    return bulletObj.GetOrAdd<Bullet>();
                },

                actionOnGet: bullet => bullet.gameObject.SetActive(true),
                actionOnRelease: bullet => bullet.gameObject.SetActive(false),
                actionOnDestroy: bullet => DestroyBullet(bullet),
                collectionCheck: false,
                defaultCapacity: bulletCount,
                maxSize: bulletCount * 10
                );

        }

        void DestroyBullet(Bullet bullet)
        {
            if (bullet)
            {
                Destroy(bullet.gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                print("Space Pressed");
                SpawnBulletPattern();
            }

            int subSteps = 4;

            float substepTime = Time.deltaTime / subSteps;


            using (bulletTransform = new TransformAccessArray(activeProjectiles.Count))
            {
                for (int i = activeProjectiles.Count; i -- > 0;)
                {
                    Bullet Bullet = activeProjectiles[i];

                    if (Bullet.HasTraveledMaxDistance())
                    {
                        ReturnBullet(Bullet);
                        continue;
                    }

                    bulletTransform.Add(Bullet.transform);

                }

                for (int step = 0; step < subSteps; step++)
                {
                    var job = new BulletMoveJob
                    {
                        deltaTime = substepTime,
                        speed = bulletSpeed,
                    };

                    JobHandle jobHandle = job.Schedule(bulletTransform);
                    jobHandle.Complete();

                    HandleCollisions();
                }

            }
        }

        private void HandleCollisions()
        {
            Vector3[] origins = new Vector3[activeProjectiles.Count];
            Vector3[] directions = new Vector3[activeProjectiles.Count];

            for (int i = 0; i < activeProjectiles.Count; i++)
            {
                Bullet bullet = activeProjectiles[i];
                origins[i] = bullet.transform.position;
                directions[i] = bullet.direction;
            }

            RaycastBachProcessor.instance.PerformRayCasts(origins, directions, collisionMask.value, false, false, false, OnRayCastResults);
        }

        void OnRayCastResults(RaycastHit[] hits)
        {
            for(int i = hits.Length; i-- > 0;)
            {
                if(hits[i].collider != null)
                {
                    ReturnBullet(activeProjectiles[i]); 
                }
            }
        }

        private void ReturnBullet(Bullet bullet)
        {
            bulletsToReturn.Add(bullet);
            activeProjectiles.Remove(bullet);
        }

        [BurstCompile]
        struct BulletMoveJob : IJobParallelForTransform
        {
            public float deltaTime;
            public float speed;

            public void Execute(int index, TransformAccess transform)
            {
                Vector3 forward = transform.rotation * Vector3.forward;
                transform.position += forward * speed * deltaTime;
            }
        }

        public void SpawnBulletPattern()
        {
            BulletHellProjectile[] newBullets = patternGenerator.GeneratePattern(bulletOrigin.position, bulletCount, bulletSpeed);

            foreach (BulletHellProjectile projectile in newBullets)
            {
                Bullet bullet = bulletPool.Get();
                bullet.Initialize(projectile.Position, projectile.Direction, bulletMaxDistance);
                activeProjectiles.Add(bullet);
            }
        }

        public void SetPattern(IBulletPattern pattern) => patternGenerator.SetPattern(pattern);

        private void LateUpdate()
        {
            foreach (var bullet in bulletsToReturn)
            {
                bulletPool.Release(bullet);
            }
            bulletsToReturn.Clear();
        }

        private void OnDestroy()
        {
            bulletPool.Dispose();
        }

    }
}
