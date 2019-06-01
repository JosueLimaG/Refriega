using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float multiplier;

    private Material mat;
    private float speed = 0.05f;
    private float value;

    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        value += speed * Time.deltaTime * multiplier;
        mat.mainTextureOffset = new Vector2(0, value);
    }
}
