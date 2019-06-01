using UnityEngine;

public class RotateScript : MonoBehaviour
{
    public Vector3 axis = new Vector3(0, 0, 10);
    public float speed = 2;

    void Update()
    {
        transform.Rotate(axis * speed, Space.Self);
    }
}
