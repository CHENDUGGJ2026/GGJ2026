using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassController : MonoBehaviour
{

    [Range(0f, 1f)] public float ExternalInfluenceStrength = 0.3f;
    public float easeInTime = 0.2f;
    public float easeOutTime = 0.2f;
    public float velocity = 4f;//触发草飘逸临界的速度

    private int _externalInfluence = Shader.PropertyToID("_ExternalInfluence");

    public void InfluenceGrass(Material mat , float xVelocity)
    {
        mat.SetFloat(_externalInfluence, xVelocity);
    }
}
