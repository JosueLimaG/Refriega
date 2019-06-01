using UnityEngine;
using UnityEngine.UI;

public class HUDOpacity : MonoBehaviour
{
    public Image master;

    private Image children;

    void Start()
    {
        children = GetComponent<Image>();
    }

    void Update()
    {
        Color color = children.color;
        color.a = master.color.a;
        children.color = color;
    }
}
