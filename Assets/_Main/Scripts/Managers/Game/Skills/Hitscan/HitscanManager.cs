using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanManager : MonoBehaviour
{
    [SerializeField]
    private SkillData data;

    protected Vector3 from;

    protected Vector3 to;

    public void Initialize(Vector3 from, Vector3 to)
    {
        this.from = from;

        this.to = to;

        transform.position = from;

        transform.forward = (to - from).normalized;

        AudioManager.Instance.Play3D(data.Sounds[0], from);
    }
}
