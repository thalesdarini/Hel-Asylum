using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public static AudioClip attack_made, attack_blood, attack_charge, enemy_death, player_death, steps, taser, song_final, song_theme, song_game;

    private static AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    void Start()
    {
        attack_made = Resources.Load<AudioClip>("attack_made");
        attack_blood = Resources.Load<AudioClip>("attack_blood");
        attack_charge = Resources.Load<AudioClip>("attack_charge2");
        enemy_death = Resources.Load<AudioClip>("enemy_death");
        player_death = Resources.Load<AudioClip>("player_death");
        steps = Resources.Load<AudioClip>("steps");
        taser = Resources.Load<AudioClip>("taser");
        song_final = Resources.Load<AudioClip>("song_final");
        song_theme = Resources.Load<AudioClip>("song_theme");
        song_game = Resources.Load<AudioClip>("song_game2");

        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "attack_made":
                audioSource.PlayOneShot(attack_made, 2f);
                break;
            case "attack_blood":
                audioSource.PlayOneShot(attack_blood, 0.6f);
                break;
            case "attack_charge":
                audioSource.PlayOneShot(attack_charge, 0.05f);
                break;
            case "enemy_death":
                audioSource.PlayOneShot(enemy_death, 0.1f);
                break;
            case "player_death":
                audioSource.PlayOneShot(player_death, 0.1f);
                break;
            case "steps":
                audioSource.PlayOneShot(steps, 0.6f);
                break;
            case "taser":
                audioSource.PlayOneShot(taser, 2f);
                break;
            default:
                break;
        }
    }

    public static void ChangeMusic(string music)
    {
        switch (music)
        {
            case "song_final":
                audioSource.clip = song_final;
                audioSource.Play();
                break;
            case "song_theme":
                audioSource.clip = song_theme;
                audioSource.Play();
                break;
            case "song_game":
                audioSource.clip = song_game;
                audioSource.Play();
                break;
            default:
                break;
        }
    }
}
