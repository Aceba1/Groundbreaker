using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleFPS : MonoBehaviour
{
    [SerializeField]
    Gradient fpsGradient;
    Text text;

    private void OnEnable()
    {
        text = GetComponent<Text>();
        if (text == null)
        {
            Debug.LogError("SimpleFPS: No UI Text attached!");
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float fps = 1f/Time.unscaledDeltaTime;
        text.text = $"FPS: {fps}";
        text.color = fpsGradient.Evaluate(fps/60f);
    }
}
