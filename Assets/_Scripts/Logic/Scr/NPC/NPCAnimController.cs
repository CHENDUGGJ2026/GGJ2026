using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INPCAnim
{
    void Walk(int dis);
}
public class NPCAnimController : MonoBehaviour, INPCAnim
{
    [SerializeField] Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Walk(int dis)
    {
        animator.SetInteger("Dis", dis);
    }
}
