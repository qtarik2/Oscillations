using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Renderers")]
    [SerializeField] private LineRenderer wave1;    // Blue
    [SerializeField] private LineRenderer wave2;    // Red
    [SerializeField] private LineRenderer sumWave;  // Green

    [Header("UI Controls")]
    [SerializeField] private TextMeshProUGUI amplitudeText;
    [SerializeField] private TextMeshProUGUI thetaText;
    [SerializeField] private Slider amplitudeSlider;
    [SerializeField] private Slider thetaSlider;
    [SerializeField] private Toggle movingToggle;
    [SerializeField] private Toggle stationaryToggle;
    [SerializeField] private Toggle sumToggle;

    [Header("Wave Settings")]
    [SerializeField] private int resolution = 400;   // number of points in each line
    [SerializeField] private float waveLength = 10f; // horizontal length of the wave
    [SerializeField] private float frequency = 1f;   // fixed frequency
    [SerializeField] private float speed = 1f;       // animation speed
    [SerializeField] private float verticalScale = 1f;

    private float Omega => 2f * Mathf.PI * frequency;
    private float WaveNumber => 2f * Mathf.PI / waveLength;

    void Start()
    {
        // Setup default values
        amplitudeSlider.minValue = 0f;
        amplitudeSlider.maxValue = 1f;
        amplitudeSlider.value = 1f;


        thetaSlider.minValue = 0f;
        thetaSlider.maxValue = 360f;
        thetaSlider.value = 90f;

        movingToggle.isOn = true;
        stationaryToggle.isOn = false;
        sumToggle.isOn = false;

        wave1.positionCount = resolution;
        wave2.positionCount = resolution;
        sumWave.positionCount = resolution;

        amplitudeText.text = $"Amplitude = {amplitudeSlider.value:F2}";
        thetaText.text = $"θ in degrees= {thetaSlider.value:F0} = {thetaSlider.value * Mathf.Deg2Rad:F2} radians";


        amplitudeSlider.onValueChanged.AddListener((val) => amplitudeText.text = $"Amplitude = {val:F2}");
        thetaSlider.onValueChanged.AddListener((val) => thetaText.text = $"θ in degrees= {val:F0} = {val * Mathf.Deg2Rad:F2} radians");
    }

    void Update()
    {
        float A = amplitudeSlider.value;
        float theta = thetaSlider.value * Mathf.Deg2Rad;
        float t = Time.time * speed;

        bool showMoving = movingToggle.isOn;
        bool showStationary = stationaryToggle.isOn;
        bool showSum = sumToggle.isOn;

        // Clear previous frame (avoids residual lines)
        wave1.positionCount = resolution;
        wave2.positionCount = resolution;
        sumWave.positionCount = resolution;

        for (int i = 0; i < resolution; i++)
        {
            float x = i / (float)(resolution - 1) * waveLength;
            float phase = (WaveNumber * x) - (Omega * t);

            float y1 = 0f, y2 = 0f;

            if (showMoving)
            {
                y1 = A * Mathf.Cos(phase);
                y2 = A * Mathf.Cos(phase + theta);
            }
            else if (showStationary)
            {
                y1 = A * Mathf.Cos(WaveNumber * x);
                y2 = A * Mathf.Cos(WaveNumber * x + theta);
            }

            float ySum = y1 + y2;

            wave1.SetPosition(i, new Vector3(x, y1 * verticalScale, 0));
            wave2.SetPosition(i, new Vector3(x, y2 * verticalScale, 0));

            if (showSum)
                sumWave.SetPosition(i, new Vector3(x, ySum * verticalScale, 0));
            else
                sumWave.SetPosition(i, new Vector3(x, 0, 0));
        }

        // Enable only the visible waves
        wave1.enabled = showMoving || showStationary;
        wave2.enabled = showMoving || showStationary;
        sumWave.enabled = showSum;
    }
}
