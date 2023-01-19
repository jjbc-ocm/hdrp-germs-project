using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPCoinMovement : MonoBehaviour
{
    [Header("Movement settings")]
    public float m_grabRadius = 1.0f;
    public float m_moveSpeed = 1.0f;

    private bool m_pursue = false; // set to true to start following the player

    public Transform m_target; // target to go to
    public GameObject m_object; // coin model game object to which apply rotation effects
    public float m_rotationSpeed = 100.0f;
    private float m_elapsedTime = 0;

    float m_delayCounter = 0;
    float m_delayTime = 0.5f;

    float destroyTimeCounter = 0.0f;
    float m_destroyTime = 60.0f; //destroy after one minute

    [Header("Sound Settings")]
    public AudioClip m_pickSFX;

    // Update is called once per frame
    void Update()
    {
        if (m_pursue)
        {
            m_delayCounter += Time.deltaTime;
            if (m_delayCounter > m_delayTime) //start following target after delay finishes
            {
                seekPosition(m_target.position);
            }
        }
        else
        {
            // destroy after some time just in case
            destroyTimeCounter += Time.deltaTime; 
            if (destroyTimeCounter > m_destroyTime)
            {
                Destroy(gameObject);
            }
        }

        m_elapsedTime += Time.deltaTime;

        //rotate
        Vector3 rotation = m_object.transform.localEulerAngles;
        rotation.y += m_rotationSpeed * Time.deltaTime;
        m_object.transform.localEulerAngles = rotation;
    }


    /// <summary>
    /// Moves in the direction of a world position.
    /// </summary>
    /// <param name="position"></param>
    void seekPosition(Vector3 position)
    {
        Vector3 dir = position - transform.position;
        if (dir.magnitude < m_grabRadius)
        {
            Pick();
        }
        transform.position += dir.normalized * m_moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Plays pick sfx and destroys the Coin.
    /// </summary>
    void Pick()
    {
        AudioManager.Instance.Play3D(m_pickSFX, transform.position, 0.2f);
        Destroy(gameObject);
    }

    /// <summary>
    /// Dispersates the coin to a initial target position
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public IEnumerator Dispersate(Vector3 targetPos)
    {
        Vector3 startPosition = transform.position;
        float timeCounter = 0;
        float duration = 0.5f;

        while (timeCounter <= duration)
        {
            timeCounter += Time.deltaTime;
            float lerpFactor = SofterInSofterOut01(timeCounter / duration);
            //move to target
            transform.position = Vector3.Lerp(startPosition,
                                              targetPos,
                                              lerpFactor);
            yield return new WaitForFixedUpdate();
        }

        m_pursue = true;

    }

    /// <summary>
    /// Cool interpolation factor calcualtion.
    /// </summary>
    /// <param name="valueFrom0to1"></param>
    /// <returns></returns>
    public static float SofterInSofterOut01(float valueFrom0to1)
    {
        return (-Mathf.Cos(valueFrom0to1 * Mathf.PI)) * 0.5f + 0.5f;
    }

}
