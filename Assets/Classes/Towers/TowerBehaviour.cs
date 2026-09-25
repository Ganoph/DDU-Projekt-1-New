using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.VFX;

public class TowerBehaviour : MonoBehaviour
{
    public LayerMask EnemiesLayer;

    public Enemy Target;
    public Transform TowerPivot;
    public Animator TowerAnimator;
    public VisualEffect FireVFX;
    private AudioSource _audioSource;

    public float Damage;
    public float Firerate;
    public float Range;

    private float Delay;
    private IDamageMethod CurrentDamageMethodClass;


    void Awake()
    {
        TowerAnimator = GetComponentInChildren<Animator>();

        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 0.7f;

        if (TowerAnimator == null)
        {
            Debug.LogError("TOWER: No Animator found on tower or its children!");
        }
    }

    void Start()
    {
        CurrentDamageMethodClass = GetComponent<IDamageMethod>();

        if (CurrentDamageMethodClass == null)
        {
            Debug.LogError("TOWERS: No damage class attached to given tower!");
        }
        else
        {
            CurrentDamageMethodClass.Init(Damage, Firerate);
        }

        Delay = 1 / Firerate;
    }

    public void Tick()
    {
        if (Target != null)
        {
            bool Fired = CurrentDamageMethodClass.DamageTick(Target);

            if (Fired)
            {
                TowerAnimator.SetTrigger("ShootTrigger");
                Debug.Log("SENDING VFX EVENT");
                FireVFX.SendEvent("MuzzleFlash");
                _audioSource.Play();
            }

            TowerPivot.rotation =
                Quaternion.LookRotation(
                    Target.transform.position - transform.position
                );
        }
    }
}