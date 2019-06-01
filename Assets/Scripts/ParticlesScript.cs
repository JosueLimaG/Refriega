using UnityEngine;

public class ParticlesScript : MonoBehaviour
{
    public float currentValue;

    private ParticleSystem.MainModule main;

    void Start()
    {
        main = GetComponent<ParticleSystem>().main;
    }

    void Update()
    {
        float temp = 0;
        bool grav = GameManager.instance.gravityChange;
        bool force = GameManager.instance.slow;

        if (grav && force)
            temp = -0.5f;
        else if (grav && !force)
            temp = -2.5f;
        else if (!grav && force)
            temp = 0.5f;
        else if (!grav && !force)
            temp = 2.5f;

        if (temp != currentValue)
            ChangeGravity(temp);

        currentValue = temp;
    }

    void ChangeGravity(float value)
    {
        main.startSpeedMultiplier = value;
    }
}
