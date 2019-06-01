using UnityEngine;
using UnityEngine.UI;

public class SecCam : MonoBehaviour
{
    public Level level;
    public Image winnerMark;

    private Transform target;
    private Color color;
    private PlayerScript winner;
    private float pos;

    void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + new Vector3(0, 0, -5);
    }

    private void Update()
    {
        float score;
        Color color;

        if (winner != null)
        {
            score = GameManager.instance.puntos[winner.ID];
            color = GameManager.instance.colors[winner.ID];
        }
        else
        {
            score = 0;
            color = Color.gray;
        }

        pos = (score * 0.8f) - 40;
        pos = Mathf.Min(pos, 0);
        winnerMark.transform.localPosition = new Vector3(0, pos, 0);
        winnerMark.color = color;

        if (score >= 50)
            GameManager.instance.WinGame(winner);
    }

    public void SetWinner(Transform target)
    {
        if (target != null)
        {
            PlayerScript player = target.GetComponent<PlayerScript>();
            winner = player;
            color = GameManager.instance.colors[player.ID];
            level.ChangeColor(color);
            this.target = target;
        }
        else
        {
            winner = null;
            level.ChangeColor(Color.gray);
            this.target = null;
        }
    }
}
