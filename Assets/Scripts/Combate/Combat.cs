using UnityEngine;

/// <summary>Los objetos de debilidad que la protagonista puede usar contra los entes (uno por mundo).</summary>
public enum WeaknessType
{
    None = 0,
    Linterna = 1,   // Mundo 2 - La Llorona
    Antorcha = 2,   // Mundo 1 - La Patasola
    Machete = 3     // Mundo 3 - La Madremonte
}

/// <summary>Todo lo que pueda recibir dano (enemigos, plantas, jefes) debe cumplir esto.</summary>
public interface IDamageable
{
    void TakeDamage(int amount, Vector2 sourcePosition);
}

/// <summary>Los enemigos que reaccionan a los objetos de debilidad cumplen esto.</summary>
public interface IWeaknessReceiver
{
    void ApplyWeakness(WeaknessType type, Vector2 sourcePosition);
}