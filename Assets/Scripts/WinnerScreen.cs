using UnityEngine;
using UnityEngine.UI;

public class WinnerScreen : MonoBehaviour
{
    public Text text;
    public bool fadeOut;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Init(Color color)
    {
        text.color = color;
        GetComponent<RectTransform>().localPosition = new Vector3();
        transform.localPosition = new Vector3();
    }

    public void FadeOut()
    {
        anim.SetBool("FadeOut", true);
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
