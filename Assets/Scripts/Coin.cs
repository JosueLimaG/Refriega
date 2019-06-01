using UnityEngine;

public class Coin : MonoBehaviour
{
    [HideInInspector] public int value;

    private void Start()
    {
        Init(Random.Range(1, 11) > 8 ? true : false);
    }

    public void Init(bool super)
    {
        GetComponent<Renderer>().material.SetColor("_MainColor", super ? Color.green : Color.yellow);
        value = super ? 5 : 1;
        Destroy(this.gameObject, 15);
    }
}
