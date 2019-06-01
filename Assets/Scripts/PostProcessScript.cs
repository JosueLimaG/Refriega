using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PostProcessScript : MonoBehaviour
{
    public GameManager manager;
    public Image rulesImage;

    private ColorGrading colorGradingLayer = null;
    private Vector4 customParameter = new Vector4();
    private float value = -0.45f;
    private float saturation;
    private float rules;
    private bool playing = false;
    private bool on;

    private void Start()
    {
        PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
        on = QualitySettings.GetQualityLevel() == 1;
        volume.enabled = on;
        Application.targetFrameRate = 60;
        
        if (on)
        {
            volume.profile.TryGetSettings(out colorGradingLayer);
            colorGradingLayer.enabled.value = true;
        }
    }

    private void Update()
    {
        playing = manager.players.Count > 0;
        rules += Time.deltaTime * (playing ? -1 : (manager.endGame ? -0.5f : 0.5f));
        rules = Mathf.Clamp(rules, 0, 1);
        rulesImage.color = new Color(1, 1, 1, rules);

        if (on) 
        {
            value += Time.deltaTime * (playing ? 1 : -0.5f);
            saturation += Time.deltaTime * (playing ? 200 : -100);
            value = Mathf.Clamp(value, -0.4f, 0.5f);
            saturation = Mathf.Clamp(saturation, -100, 75);
            customParameter.w = value;
            Vector4Parameter parameter = new Vector4Parameter();
            parameter.value = customParameter;
            colorGradingLayer.gamma.value = parameter;
            colorGradingLayer.saturation.value = saturation;
        }
    }
}