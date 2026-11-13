using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Envuelve al Animator para que no salgan advertencias si todavia no has creado un parametro
/// o si aun no tienes Animator. Asi puedes probar el juego con cuadrados de color.
/// </summary>
public class SafeAnimator
{
    private readonly Animator anim;
    private readonly HashSet<int> hashes = new HashSet<int>();

    public SafeAnimator(Animator animator)
    {
        anim = animator;
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            foreach (AnimatorControllerParameter p in anim.parameters)
                hashes.Add(p.nameHash);
        }
    }

    public void SetFloat(int hash, float value) { if (hashes.Contains(hash)) anim.SetFloat(hash, value); }
    public void SetBool(int hash, bool value) { if (hashes.Contains(hash)) anim.SetBool(hash, value); }
    public void SetInt(int hash, int value) { if (hashes.Contains(hash)) anim.SetInteger(hash, value); }
    public void SetTrigger(int hash) { if (hashes.Contains(hash)) anim.SetTrigger(hash); }
}
