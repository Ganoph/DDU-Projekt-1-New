using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int NodeIndex;

    public float DamageResistance = 1f;
    public float MaxHealth;
    public float Health;
    public float Speed;
    public int ID;
    private AudioSource _audioSource;


    public void Init()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 1f;
        Health = MaxHealth;
        transform.position = GameLoopManager.NodePositions[0];
        NodeIndex = 0;
        _audioSource.Play();
    }
}