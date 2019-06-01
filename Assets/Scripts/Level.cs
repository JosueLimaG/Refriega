using System.Collections;
using UnityEngine;

public class Level : MonoBehaviour
{
    public float speed;
    public Material[] mats;
    public Texture[] texs;

    private float desiredRot;
    private Texture _cameraTexture;

    private void Start()
    {
        _cameraTexture = Resources.Load<Texture>("CameraTexture");
        ChangeColor(Color.gray);
    }

    private void Update()
    {
        desiredRot += Time.deltaTime * speed;
        var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, desiredRot, transform.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime);
    }

    public void ChangeColor(Color color)
    {
        bool draw = color == Color.gray;
        color = new Color((color.r / 2 + 0.5f), (color.g / 2 + 0.5f), (color.b / 2 + 0.5f));

        foreach (Material mat in mats)
        {
            int prob = Random.Range(-5, 2);

            if (prob > 0 && !draw)
                StartCoroutine(Change(mat));
            else
                StartCoroutine(Change(mat, color));
        }
    }

    IEnumerator Change(Material mat)
    {
        yield return new WaitForSeconds(Random.Range(0, 1.5f));
        mat.SetTexture("_Texture2", _cameraTexture);
        mat.SetColor("_Color2", Color.white);
    }

    IEnumerator Change(Material mat, Color color)
    {
        yield return new WaitForSeconds(Random.Range(0, 1.5f));
        mat.SetTexture("_Texture2", texs[Random.Range(0, texs.Length)]);
        mat.SetColor("_Color2", color);
    }
}
