using UnityEngine;
[ExecuteInEditMode]
public class DebugRotation : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        print(Vector2.Angle((transform.position - target.position), transform.up));
    }
}
