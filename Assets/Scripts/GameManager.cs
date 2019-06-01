using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string[] buttons = new string[8];
    public bool[] playing = new bool[8];
    public int[] puntos = new int[8];
    public Color[] colors = new Color[8];
    public Transform[] spawnPos = new Transform[8];
    public Image[] image = new Image[8];
    public Text[] text = new Text[8];
    public SecCam secundaryCam;
    public Canvas canvas;
    public Transform soundManager;
    public Text remainingPoints;
    public Text debug;
    [HideInInspector] public List<PlayerScript> players = new List<PlayerScript>();
    [HideInInspector] public Vector3 customGravity;
    [HideInInspector] public bool slow;
    [HideInInspector] public bool gravityChange;
    [HideInInspector] public bool empate;
    [HideInInspector] public bool endGame;

    private List<Rigidbody> rigids = new List<Rigidbody>();
    private PlayerScript exceptSlow;
    private Rigidbody exceptGrav;
    private Vector3 gravity = new Vector3(0, -9.81f, 0);
    private Transform currentWinner;

    private GameObject _power;
    private GameObject _coin;
    private GameObject _player;

    private int muertePisado;
    private int muerteBomba;
    private int muerteLaser;
    private bool show;

    void Start()
    {
        instance = this;

        _power = Resources.Load<GameObject>("Power");
        _coin = Resources.Load<GameObject>("Coin");
        _player = Resources.Load<GameObject>("Player");

        customGravity = gravity;
        int pos = 0;

        foreach (Text x in text)
        {
            image[pos].color = colors[pos];
            x.text = " Start ";
            pos++;
        }

        InvokeRepeating("SpawnItem", 2, 2);
        InvokeRepeating("UpdateRank", 5, 5);

        muertePisado = PlayerPrefs.GetInt("pisado", 0);
        muerteBomba = PlayerPrefs.GetInt("bomba", 0);
        muerteLaser = PlayerPrefs.GetInt("laser", 0);
    }

    void Update()
    {
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetButtonDown(buttons[i]) && !playing[i] && !endGame)
                SpawnPlayer(i);

            text[i].text = playing[i] ? puntos[i].ToString() : text[i].text;
        }

        customGravity = new Vector3(0, Mathf.Lerp(customGravity.y, gravityChange ? -gravity.y : gravity.y, Time.deltaTime * 4), 0);
        Physics.gravity = customGravity;

        foreach (Rigidbody rb in rigids)
        {
            rb.AddForce(rb != exceptGrav ? customGravity : gravity * 4f, ForceMode.Acceleration);
        }

        foreach (PlayerScript player in players)
        {
            if (player != exceptSlow)
                player.multiplier = Mathf.Lerp(player.multiplier, slow ? 0.4f : 1, Time.deltaTime * 20);
            else
                player.multiplier = Mathf.Lerp(player.multiplier, 1, Time.deltaTime * 20);
        }

        remainingPoints.text = (50 - puntos.Max()).ToString();
        debug.text = "Pisados: " + muertePisado + "   Bomba: " + muerteBomba + "    Laser: " + muerteLaser;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            show = !show;
            debug.gameObject.SetActive(show);
        }
    }

    public void SpawnPlayer(int id)
    {
        GameObject temp = Instantiate(_player, spawnPos[id].position, Quaternion.identity);
        PlayerScript script = temp.GetComponent<PlayerScript>();
        rigids.Add(temp.GetComponent<Rigidbody>());
        players.Add(script);
        script.Init(id, buttons[id], colors[id], soundManager);
        playing[id] = true;
    }

    void SpawnItem()
    {
        if (players.Count > 0)
        {
            int prob = Random.Range(-7, 15);
            switch (prob)
            {
                case 1:
                    SpawnItem(powerType.Color, false);
                    break;
                case 3:
                    SpawnItem(powerType.Clone, false);
                    break;
                case 5:
                    SpawnItem(powerType.SlowingField, false);
                    break;
                case 7:
                    SpawnItem(powerType.Gravity, true);
                    break;
                case 9:
                    SpawnItem(powerType.Shield, false);
                    break;
                default:
                    SpawnCoin();
                    break;
            }
        }
    }

    void SpawnItem(powerType item, bool inverse)
    {
        GameObject temp = Instantiate(_power, new Vector3(Random.Range(-8, 8), inverse ? -5.85f : 5.85f, 0), Quaternion.identity);
        temp.GetComponent<PowerUp>().Init(item);
    }

    void SpawnCoin()
    {
        if (players.Count > 0)
        {
            GameObject temp = Instantiate(_coin, new Vector3(Random.Range(-8, 8), 5.85f, 0), Quaternion.identity);
        }
    }

    public void SlowPlayers(PlayerScript except)
    {
        exceptSlow = except;
        slow = true;
        StartCoroutine(NormalVelocity());
    }

    IEnumerator NormalVelocity()
    {
        yield return new WaitForSeconds(10);
        slow = false;
    }

    public void ChangeGravity(Rigidbody except)
    {
        if (exceptGrav != null)
            exceptGrav.GetComponent<PlayerScript>().gravException = false;

        exceptGrav = except;
        exceptGrav.GetComponent<PlayerScript>().gravException = true;
        gravityChange = true;
        StartCoroutine(NormalGravity());
    }

    IEnumerator NormalGravity()
    {
        yield return new WaitForSeconds(10);
        gravityChange = false;

        if (exceptGrav != null)
            exceptGrav.GetComponent<PlayerScript>().gravException = false;

        exceptGrav = null;
    }

    public void ChangeColor(Material sr, PlayerScript _player)
    {
        int id = _player.ID;
        Color color = new Color();
        do
        {
            color = colors[(int)Random.Range(0, 8)];
        } while (sr.GetColor("_Color2") == color);

        foreach (PlayerScript player in players)
            if (player.ID != id)
                player.ChangeColor(color);

        StartCoroutine(NormalColor());
    }

    IEnumerator NormalColor()
    {
        yield return new WaitForSeconds(10);
        foreach (PlayerScript player in players)
                player.ChangeColor(colors[player.GetComponent<PlayerScript>().ID]);
    }

    public bool Dead(PlayerScript id)
    {
        players.Remove(id);
        rigids.Remove(id.GetComponent<Rigidbody>());

        if (!Playing(id.ID))
        {
            puntos[id.ID] = 0;
            StartCoroutine(CanSpawn(id.ID));
        }
        
        CheckRank();
        return Playing(id.ID);
    }

    IEnumerator CanSpawn(int id)
    {
        yield return new WaitForSeconds(2);
        playing[id] = false;
        text[id].text = " Start ";
    }

    public void Puntos(int ID, int puntos)
    {
        this.puntos[ID] += puntos;
        CheckRank();
    }

    bool Playing(int id)
    {
        foreach (PlayerScript player in players)
            if (player.ID == id)
                return true;

        return false;
    }

    void CheckRank()
    {
        Transform temp = LookWinner();

        if (temp != currentWinner)
        {
            secundaryCam.SetWinner(temp);
            currentWinner = temp;
        }
    }

    void UpdateRank()
    {
        currentWinner = LookWinner();
        secundaryCam.SetWinner(currentWinner);
    }

    Transform LookWinner()
    {
        empate = false;
        int temp = 0;
        int winner = 0;

        for (int i = 0; i < 8; i++)
        {
            if (puntos[i] > temp)
            {
                winner = i;
                temp = puntos[i];
                empate = false;
            }
            else if (puntos[i] == temp)
                empate = true;
        }

        if (empate)
            return null;
        else
            foreach(PlayerScript p in players)
                if (p.ID == winner)
                    return p.transform;

        print("No se encontro al ganador");
        return null;
    }

    public void KillAll()
    {
        foreach (PlayerScript victim in players)
        {
            StartCoroutine(victim.Kill());
        }
    }

    public void WinGame(PlayerScript winner)
    {
        KillAll();
        endGame = true;
        GameObject temp = Instantiate(Resources.Load<GameObject>("WinnerScreen"), new Vector3(), Quaternion.identity, canvas.transform);
        WinnerScreen screen = temp.GetComponent<WinnerScreen>();
        screen.Init(colors[winner.ID]);
        StartCoroutine(CanStart(screen));
    }

    IEnumerator CanStart(WinnerScreen screen)
    {
        yield return new WaitForSeconds(12);
        endGame = false;
        screen.FadeOut();
    }

    public void ContarMuerte(int tipo)
    {
        switch (tipo)
        {
            case 0:
                muertePisado++;
                break;
            case 1:
                muerteBomba++;
                break;
            case 2:
                muerteLaser++;
                break;
        }

        PlayerPrefs.SetInt("pisado", muertePisado);
        PlayerPrefs.SetInt("bomba", muerteBomba);
        PlayerPrefs.SetInt("laser", muerteLaser);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
