using UnityEngine;

public enum powerType {Shield, Gravity, SlowingField, Color, Clone };

public class PowerUp : MonoBehaviour
{
    private Rigidbody rb;
    private bool inverse;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(powerType pow)
    {
        Instantiate(Resources.Load<GameObject>(pow.ToString()), transform.position, Quaternion.identity, transform);
        transform.tag = pow.ToString();
        transform.name = tag;

        if (pow == powerType.Gravity)
        {
            rb.useGravity = false;
            inverse = true;
        }

        Destroy(this.gameObject, 15);
    }

    private void Update()
    {
        if (inverse)
            rb.AddForce(0, 5f, 0, ForceMode.Acceleration);
    }
}
