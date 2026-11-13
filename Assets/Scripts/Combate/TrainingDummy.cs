using System.Collections;
using UnityEngine;

public class TrainingDummy : MonoBehaviour, IDamageable, IWeaknessReceiver
{
    public int health = 5;
    public SpriteRenderer sprite;

    private void Awake()
    {
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        health -= amount;
        Debug.Log(name + " recibio " + amount + " de dano. Vida: " + health);
        StartCoroutine(FlashRoutine());
        if (health <= 0) Destroy(gameObject);
    }

    public void ApplyWeakness(WeaknessType type, Vector2 sourcePosition)
    {
        Debug.Log(name + " recibio la debilidad: " + type);
        TakeDamage(2, sourcePosition);
    }

    private IEnumerator FlashRoutine()
    {
        if (sprite == null) yield break;
        Color old = sprite.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sprite != null) sprite.color = old;
    }
}