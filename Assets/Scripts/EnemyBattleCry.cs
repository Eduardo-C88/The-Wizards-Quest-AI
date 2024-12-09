using UnityEngine;

public class EnemyBattleCry : MonoBehaviour
{
    public AudioClip[] battleCries; // Lista de gritos (variedade)
    public float minInterval = 15f; // Tempo mínimo entre gritos
    public float maxInterval = 30f; // Tempo máximo entre gritos

    private AudioSource audioSource;
    private float nextCryTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ScheduleNextCry();
    }

    private void Update()
    {
        if (Time.time >= nextCryTime)
        {
            PlayBattleCry();
            ScheduleNextCry();
        }
    }

    private void PlayBattleCry()
    {
        if (battleCries.Length > 0)
        {
            int randomIndex = Random.Range(0, battleCries.Length);
            audioSource.PlayOneShot(battleCries[randomIndex]);
        }
    }

    private void ScheduleNextCry()
    {
        nextCryTime = Time.time + Random.Range(minInterval, maxInterval);
    }
}
