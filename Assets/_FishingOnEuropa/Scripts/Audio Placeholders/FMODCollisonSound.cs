using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[HelpURL("https://app.gitbook.com/s/5zKO0EvOjzUDeT2aiFk3/auto-hand/extras/collision-sounds")]
public class CollisionSound : MonoBehaviour
{

    [Tooltip("The layers that cause the sound to play")]
    public LayerMask collisionTriggers = ~0;
    [Tooltip("Source to play sound from")]
    public StudioEventEmitter source;

    [Space]
    [Tooltip("Source to play sound from")]
    public AnimationCurve velocityVolumeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public float soundRepeatDelay = 0.2f;

    Rigidbody body;
    bool canPlaySound = true;
    Coroutine playSoundRoutine;

    private void Start()
    {
        body = GetComponent<Rigidbody>();

        //So the sound doesn't play when falling in place on start
        StartCoroutine(SoundPlayBuffer(1f));
    }

    private void OnDisable()
    {
        if (playSoundRoutine != null)
            StopCoroutine(playSoundRoutine);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (body == null && !gameObject.CanGetComponent(out body))
            return;

        if (canPlaySound && collisionTriggers == (collisionTriggers | (1 << collision.gameObject.layer)))
        {
            if (source != null && source.enabled)
            {
                if (collision.collider.attachedRigidbody == null || collision.collider.attachedRigidbody.mass > 0.0000001f)
                {
                    if (source!= null)
                        source.Play();
                    if (playSoundRoutine != null)
                        StopCoroutine(playSoundRoutine);
                    playSoundRoutine = StartCoroutine(SoundPlayBuffer());
                }
            }
        }
    }

    IEnumerator SoundPlayBuffer()
    {
        canPlaySound = false;
        yield return new WaitForSeconds(soundRepeatDelay);
        canPlaySound = true;
        playSoundRoutine = null;
    }

    IEnumerator SoundPlayBuffer(float time)
    {
        canPlaySound = false;
        yield return new WaitForSeconds(time);
        canPlaySound = true;
        playSoundRoutine = null;
    }
}
