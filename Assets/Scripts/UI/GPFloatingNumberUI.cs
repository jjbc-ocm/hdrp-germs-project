using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPFloatingNumberUI : MonoBehaviour
{
    public TextMeshProUGUI m_text;
    public Image m_image;
    public float m_lifeTime = 1.0f;
    public float m_fadeOutDelay = 1.0f;
    public float m_fadeOutSpeed = 1.0f;
    public GPPunchTween m_punchTween;

    private void Start()
    {
        StartCoroutine(IEOnSpawn());
    }

    public void SetNumber(int number, string preStr)
    {
        m_text.text = preStr + number.ToString();
    }

    public void SetPosition(Vector3 worldPos, Camera perfectiveCamera)
    {
        Vector3 pos = perfectiveCamera.WorldToScreenPoint(worldPos);

        if (transform.position != pos) { transform.position = pos; }
    }

    public IEnumerator IEOnSpawn()
    {
        if (m_punchTween)
        {
            m_punchTween.PunchEffect();
        }
        yield return StartCoroutine(IEColorFadeOut(m_fadeOutDelay, m_lifeTime));
        Destroy(gameObject);
    }

    IEnumerator IEColorFadeOut(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            Color textColor = m_text.color;
            textColor.a = Mathf.Lerp(1, 0, (Time.time - startTime)/duration);
            m_text.color = textColor;

            Color imageColor = m_image.color;
            imageColor.a = Mathf.Lerp(1, 0, (Time.time - startTime) / duration);
            m_image.color = imageColor;

            yield return new WaitForFixedUpdate();
        }
    }

}
