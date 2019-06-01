using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public ParticleSystem ps;
    public GameObject shielded;
    public int puntos;
    public int ID;
    public float multiplier = 1;
    public bool forward;
    public bool gravException;
    public string input;

    private Material mat0;
    private Material mat1;
    private Rigidbody rb;
    private Animator anim;
    //private ParticleSystem.EmissionModule emit;
    private SoundtrackScript _audio;
    //public float particleEmit;
    private float vel = 6 ;
    private float holdTimer;
    private float currentFade = 1;
    private bool fadeIn = true;
    private bool dead;
    private bool shield;

    public void Init(int ID, string button, Color color, Transform sound)
    {
        mat0 = transform.GetChild(0).GetComponent<Renderer>().materials[1];
        mat1 = transform.GetChild(0).GetComponent<Renderer>().materials[0];
        transform.GetChild(1).GetComponent<Renderer>().material = mat0;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        //emit = ps.emission;
        anim.SetInteger("State", 0);
        shielded.SetActive(true);
        shield = true;
        name = "Player" + ID;
        this.ID = ID;
        input = button;
        forward = transform.position.x < 0;
        mat0.SetColor("_Color2", color);
        _audio = SoundtrackScript.instance;
        rb.velocity = new Vector3((forward ? vel : -vel) * multiplier, (gravException ? 5 : (GameManager.instance.customGravity.y < 0 ? 5 : -5)) * multiplier, 0);
        _audio.CreateSound(8);
    }

    void Update()
    {
        if (dead)
            vel = Mathf.Lerp(vel, 0, Time.deltaTime);
        else if (Input.GetButtonDown(input))
        {
            rb.velocity = new Vector3(rb.velocity.x, (gravException ? 10 : (GameManager.instance.customGravity.y < 0 ? 10 : -10)) * multiplier, 0);
            rb.AddForce(new Vector3((forward ? vel : -vel) * multiplier, 0, 0), ForceMode.Impulse);
            ps.Play();
        }

        if (Input.GetButtonUp(input) && holdTimer > 0.5f && !dead)
            Special(holdTimer);

        holdTimer += Input.GetButton(input) ? Time.deltaTime : -holdTimer;
        transform.eulerAngles = new Vector3(0, forward ? 90 : -90, 0);
        Fade();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!dead)
        {
            Transform colTran = collision.transform;

            switch (colTran.tag)
            {
                case "Wall":
                    //rb.velocity = new Vector3(-rb.velocity.x, rb.velocity.y, 0);
                    forward = !forward;
                    _audio.CreateSound(3);
                    break;
                case "Roof":
                    Die();
                    GameManager.instance.ContarMuerte(1);
                    _audio.CreateSound(5);
                    break;
                case "Coin":
                    AddPoint(colTran.GetComponent<Coin>().value);
                    Destroy(colTran.gameObject);
                    _audio.CreateSound(0);
                    break;
                case "Shield":
                    shielded.SetActive(true);
                    shield = true;
                    Destroy(colTran.gameObject);
                    _audio.CreateSound(6);
                    break;
                case "Gravity":
                    GameManager.instance.ChangeGravity(rb);
                    Destroy(colTran.gameObject);
                    _audio.CreateSound(20);
                    break;
                case "SlowingField":
                    GameManager.instance.SlowPlayers(this);
                    Destroy(colTran.gameObject);
                    _audio.CreateSound(20);
                    break;
                case "Color":
                    GameManager.instance.ChangeColor(mat0, this);
                    Destroy(colTran.gameObject);
                    _audio.CreateSound(7);
                    break;
                case "Clone":
                    GameManager.instance.SpawnPlayer(ID);
                    Destroy(colTran.gameObject);
                    break;
                case "Player":
                    PlayerScript colScript = collision.gameObject.GetComponent<PlayerScript>();

                    if (colScript.dead)
                    {
                        AddPoint((colScript.puntos / 2) + 1);
                        colTran.gameObject.GetComponent<PlayerScript>().Destroy();
                        print(name + " recogio un botin de los restos de " + colTran.name);
                        _audio.CreateSound(10);
                    }
                    else
                    {
                        float angulo = Vector2.Angle((transform.position - colTran.position), transform.up);

                        if (angulo >= 25 && angulo <= 155)
                        {
                            Vector2 vecTemp = colTran.GetComponent<Rigidbody>().velocity;
                            rb.velocity = new Vector3(-rb.velocity.x, rb.velocity.y + (Mathf.Abs(vecTemp.y) / 2), 0);
                            forward = !forward;
                        }
                        else if (angulo < 25)
                        {
                            //Aca no se hace nada :v r
                        }
                        else
                        {
                            if (Die())
                            {
                                colScript.AddPoint(5);
                                GameManager.instance.ContarMuerte(0);
                            }
                            //colScript.AddPoint(Die() ? 5 : 0);
                            print(name + "fue pisado por " + colTran.name + " en un angulo de " + angulo);
                        }
                    }
                    break;
            }
        }
        else
        {
            string colTag = collision.transform.tag;

            if (colTag == "Floor")
                anim.SetInteger("State", 2);
            else if (colTag != "Untagged" && colTag != "Player" && colTag != "Wall" && colTag != "Roof")
                Destroy(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !dead)
        {
            PlayerScript colScript = collision.gameObject.GetComponent<PlayerScript>();

            if (colScript.dead)
            {
                AddPoint(colScript.puntos / 2);
                colScript.Destroy();
                print(name + " recogio un botin de los restos de " + collision.transform.name);
            }
        }
        else if (collision.gameObject.CompareTag("Floor") && dead)
            anim.SetInteger("State", 2);

    }

    void Special(float power)
    {
        Instantiate(Resources.Load<GameObject>("Bomb"), transform.position, Quaternion.identity);
        forward = !forward;
        rb.velocity = new Vector3((forward ? vel : -vel) * multiplier, (gravException ? 10 : (GameManager.instance.customGravity.y < 0 ? 10 : -10)) * multiplier, 0);
        _audio.CreateSound(4);
    }

    void Fade()
    {
        if (fadeIn)
            currentFade = Mathf.Lerp(currentFade, 0, Time.deltaTime * 4);
        else
            currentFade = Mathf.Lerp(currentFade, 1, Time.deltaTime);

        mat0.SetFloat("_Fade", currentFade);
        mat1.SetFloat("_Fade", currentFade);

        if (currentFade > 0.9f && dead)
            Destroy(this.gameObject);
    }

    public bool Die()
    {
        if (!dead)
        {
            if (shield)
            {
                shield = false;
                shielded.SetActive(false);
                print(name + " perdio su proteccion al ser pisado");
                _audio.CreateSound(2);
                return false;
            }
            else
            {
                dead = true;
                mat0.SetColor("_Color2", Color.gray);
                anim.SetInteger("State", 1);
                name += " body";
                int puntosTemp = GameManager.instance.puntos[ID];
                puntos = GameManager.instance.Dead(this) ? 0 : puntosTemp;
                print(name + " murio al ser pisado");
                Invoke("Destroy", 10);
                _audio.CreateSound(1);
                return true;
            }
        }
        return false;
    }

    public IEnumerator Kill()
    {
        yield return new WaitForEndOfFrame();
        dead = true;
        mat0.SetColor("_Color2", Color.gray);
        anim.SetInteger("State", 1);
        name += " body";
        puntos = GameManager.instance.Dead(this) ? 0 : GameManager.instance.puntos[ID];
        print(name + " murio al ser pisado");
        Invoke("Destroy", 10);
    }

    public void AddPoint(int x)
    {
        puntos += x;
        GameManager.instance.Puntos(ID, x);
    }

    public void ChangeColor(Color color)
    {
        mat0.SetColor("_Color2", color);
    }

    public void Destroy()
    {
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        fadeIn = false;
    }
}
