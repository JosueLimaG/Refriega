using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public List<PlayerScript> players = new List<PlayerScript>();
    public Transform radius;

    private SphereCollider coll;
    private Material mat;
    private float timer;

    void Start()
    {
        coll = GetComponent<SphereCollider>();
        mat = GetComponent<Renderer>().material;
        mat.color = Color.white;
        coll.isTrigger = true;
        StartCoroutine(Explode());
        coll.radius = 0;
        radius.localScale = new Vector3();
    }

    void Update()
    {
        timer += Time.deltaTime / 4;
        mat.color = Color.Lerp(mat.color, Color.red, Time.deltaTime / 8);
        coll.radius = timer;
        float size = Time.deltaTime * 0.5f;
        radius.localScale += new Vector3(size, size, size);
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(8);
        SoundtrackScript.instance.CreateSound(9);

        foreach (PlayerScript player in players)
        {
            if (player.Die())
            {
                GameManager.instance.ContarMuerte(2);
            }
        }

        GameObject temp = Instantiate(Resources.Load<GameObject>("Explosion"), transform.position, Quaternion.identity);
        Destroy(temp, 5);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
            players.Add(collision.GetComponent<PlayerScript>());
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
            players.Remove(collision.GetComponent<PlayerScript>());
    }
}
