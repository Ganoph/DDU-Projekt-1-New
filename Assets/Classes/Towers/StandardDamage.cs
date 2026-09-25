using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageMethod
{
    public bool DamageTick(Enemy Target);
    public void Init(float Damage, float Firerate);
}

public class StandardDamage : MonoBehaviour, IDamageMethod
{
    private float Damage;
    private float Firerate;
    private float Delay;
    public void Init(float Damage, float Firerate)
    {
        this.Damage = Damage;
        this.Firerate = Firerate;
        Delay = 1f / Firerate;
    }
    public bool DamageTick(Enemy Target)
    {
        if (Target)
        {
            if (Delay > 0f)
            {
                Delay -= Time.deltaTime;
                return false;
            }

            GameLoopManager.EnqueueDamageData(
                new EnemyDamageData(Target, Damage, Target.DamageResistance)
            );

            Delay = 1f / Firerate;
            return true;
        }

        return false;
    }
}