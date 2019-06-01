using UnityEngine;
using UnityEngine.Audio;
public class SoundtrackScript : MonoBehaviour
{
    public static SoundtrackScript instance;
    public bool menu;
    public AudioMixerSnapshot main;
    public AudioMixerSnapshot sec;
    public AudioMixerSnapshot _clock;
    public AudioMixerSnapshot _gravity;
    public AudioMixerSnapshot ambos;
    public AudioMixerSnapshot ninguno;
    public AudioClip spawn, coin, dead, shield, shieldOff, bomb, explode, color, electric, hitWall, botin, power;
    public GameObject soundPrefab;

    private void Start()
    {
        instance = this;
    }

    void Update()
    {
        menu = GameManager.instance.players.Count > 0;
        var gravity = GameManager.instance.gravityChange;
        var clock = GameManager.instance.slow;

        if (menu)
            sec.TransitionTo(0.5f);
        else
            main.TransitionTo(0.5f);

        if (gravity && clock)
            ambos.TransitionTo(0.5f);
        else if (gravity)
            _gravity.TransitionTo(0.5f);
        else if (clock)
            _clock.TransitionTo(0.5f);
        else
            ninguno.TransitionTo(0.5f);
    }

    public void CreateSound(int type)
    {
        GameObject temp = Instantiate(soundPrefab, transform);
        AudioSource _as = temp.GetComponent<AudioSource>();
        _as.clip = GetSound(type);
        _as.Play();
        Destroy(temp, 1.5f);
    }

    AudioClip GetSound(int type)
    {
        switch (type)
        {
            case 0:     //Coin
                return coin;
            case 1:     //Muerte
                return dead;
            case 2:     //ChocarCasco
                return shieldOff;
            case 3:     //ChocarPared
                return hitWall;
            case 4:     //PlantarBomba
                return bomb;
            case 5:     //Electrocutado
                return electric;
            case 6:     //TomarEscudo
                return shield;
            case 7:     //PowerUp
                return color;
            case 8:     //Spawn
                return spawn;
            case 9:     //Exlposion
                return explode;
            case 10:    //Botin
                return botin;
            default:    //PoderGenerico
                return power;
        }
    }
}
