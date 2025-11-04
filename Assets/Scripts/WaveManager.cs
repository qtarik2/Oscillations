using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Renderers")]
    [SerializeField] private LineRenderer wave1;    // Blue
    [SerializeField] private LineRenderer wave2;    // Red
    [SerializeField] private LineRenderer sumWave;  // Green

    [Header("Connector")]
    [SerializeField] private LineRenderer connector1; // from point1 to wave1
    [SerializeField] private LineRenderer connector2; // from point2 to wave2

    [Header("Circle")]
    [SerializeField] private LineRenderer circleRenderer;
    [SerializeField] private Transform point1; // Blue point y1
    [SerializeField] private Transform point2; // Red point y2
    [SerializeField] private LineRenderer radiusLine1;
    [SerializeField] private LineRenderer radiusLine2;
    [SerializeField] private float circleXOffset = -3f;

    [Header("UI Controls")]
    [SerializeField] private TextMeshProUGUI amplitudeText;
    [SerializeField] private TextMeshProUGUI thetaText;
    [SerializeField] private Slider amplitudeSlider;
    [SerializeField] private Slider thetaSlider;
    [SerializeField] private Toggle movingToggle;
    [SerializeField] private Toggle stationaryToggle;
    [SerializeField] private Toggle sumToggle;

    [Header("Wave Settings")]
    [SerializeField] private int resolution = 400;
    [SerializeField] private float waveLength = 10f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float circleSpeed = 1f;
    [SerializeField] private float verticalScale = 1f;

    private float Omega => 2f * Mathf.PI * frequency;
    private float WaveNumber => 2f * Mathf.PI / waveLength;

    private float currentAmplitude;
    private float currentPhase;

    private void Start()
    {
        InitializeLines();
        InitializeUI();
    }

    private void InitializeLines()
    {
        SetupLineRenderer(wave1, false, 0.05f, resolution);
        SetupLineRenderer(wave2, false, 0.05f, resolution);
        SetupLineRenderer(sumWave, false, 0.05f, resolution);

        SetupLineRenderer(circleRenderer, true, 0.03f, resolution);

        SetupLineRenderer(radiusLine1, false, 0.03f, 2);
        SetupLineRenderer(radiusLine2, false, 0.03f, 2);
        SetupLineRenderer(connector1, false, 0.02f, 2);
        SetupLineRenderer(connector2, false, 0.02f, 2);
    }

    private void Update()
    {
        DrawCircle();
        DrawWaves();
        UpdateConnectors();
    }

    private void InitializeUI()
    {
        amplitudeSlider.minValue = 0f;
        amplitudeSlider.maxValue = 1f;
        amplitudeSlider.value = 1f;
        currentAmplitude = amplitudeSlider.value;

        thetaSlider.minValue = 0f;
        thetaSlider.maxValue = 360f;
        thetaSlider.value = 90f;
        currentPhase = thetaSlider.value * Mathf.Deg2Rad;

        movingToggle.isOn = true;
        stationaryToggle.isOn = false;
        sumToggle.isOn = false;

        amplitudeText.text = $"Amplitude = {amplitudeSlider.value:F2}";
        thetaText.text = $"θ = {thetaSlider.value:F0}° = {thetaSlider.value * Mathf.Deg2Rad:F2} rad";

        amplitudeSlider.onValueChanged.AddListener(OnAmplitudeChange);
        thetaSlider.onValueChanged.AddListener(OnThetaChange);
    }

    private void OnThetaChange(float value)
    {
        currentPhase = value * Mathf.Deg2Rad;
        thetaText.text = $"θ = {value:F0}° = {currentPhase:F2} rad";
    }

    private void OnAmplitudeChange(float value)
    {
        currentAmplitude = value;
        amplitudeText.text = $"Amplitude = {value:F2}";
    }

    private void DrawWaves()
    {
        float time = Time.timeSinceLevelLoad * speed;
        bool showMoving = movingToggle.isOn;
        bool showStationary = stationaryToggle.isOn;
        bool showSum = sumToggle.isOn;

        float x, y1, y2;
        for (int i = 0; i < resolution; i++)
        {
            x = i / (float)(resolution - 1) * waveLength;
            float phase = (WaveNumber * x) - (Omega * time);

            if (showMoving)
            {
                y1 = currentAmplitude * Mathf.Cos(phase);
                y2 = currentAmplitude * Mathf.Cos(phase + currentPhase);
            }
            else if (showStationary)
            {
                // Standing wave = sum of two opposite moving waves
                y1 = currentAmplitude * Mathf.Cos(WaveNumber * x);
                y2 = currentAmplitude * Mathf.Cos(WaveNumber * x + currentPhase);
            }
            else
            {
                y1 = y2 = 0;
            }

            // // Invert Y to match oPhysics direction
            // y1 *= -1f;
            // y2 *= -1f;

            wave1.SetPosition(i, new Vector3(x, y1 * verticalScale, 0));
            wave2.SetPosition(i, new Vector3(x, y2 * verticalScale, 0));

            if (showSum)
            {
                float ySum = y1 + y2;
                sumWave.SetPosition(i, new Vector3(x, ySum * verticalScale, 0));
            }

            wave1.enabled = showMoving || showStationary;
            wave2.enabled = showMoving || showStationary;
            sumWave.enabled = showSum;
        }
    }

    private void DrawCircle()
    {
        if (circleRenderer == null) return;

        Vector3 center = new(circleXOffset, 0, 0);

        for (int i = 0; i < resolution; i++)
        {
            float t = (float)i / resolution * 2 * Mathf.PI;
            float x = center.x + currentAmplitude * Mathf.Cos(t);
            float y = center.y + currentAmplitude * Mathf.Sin(t); // Anticlockwise rotation
            circleRenderer.SetPosition(i, new Vector3(x, y, 0));
        }

        float time = Time.timeSinceLevelLoad * circleSpeed;
        float angle1 = Omega * time;
        float angle2 = Omega * time + currentPhase;

        // Match circle rotation direction
        Vector3 p1 = new(center.x + currentAmplitude * Mathf.Cos(angle1), center.y + currentAmplitude * Mathf.Sin(angle1), center.z);
        Vector3 p2 = new(center.x + currentAmplitude * Mathf.Cos(angle2), center.y + currentAmplitude * Mathf.Sin(angle2), center.z);

        point1.localPosition = p1;
        point2.localPosition = p2;

        radiusLine1.SetPosition(0, center);
        radiusLine1.SetPosition(1, p1);

        radiusLine2.SetPosition(0, center);
        radiusLine2.SetPosition(1, p2);
    }

    private void UpdateConnectors()
    {
        if (wave1.positionCount == 0 || wave2.positionCount == 0) return;

        connector1.SetPosition(0, point1.position);
        connector2.SetPosition(0, point2.position);

        if (stationaryToggle.isOn)
        {
            // Let endpoints slide along the waves to visualize standing wave pattern
            float t = (Mathf.Sin(Time.time * 0.5f) + 1f) * 0.5f; // oscillate 0–1
            int idx = Mathf.FloorToInt(t * (resolution - 1));

            connector1.SetPosition(1, wave1.GetPosition(idx));
            connector2.SetPosition(1, wave2.GetPosition(idx));
        }
        else
        {
            connector1.SetPosition(1, wave1.GetPosition(0));
            connector2.SetPosition(1, wave2.GetPosition(0));
        }
    }

    private void SetupLineRenderer(LineRenderer lr, bool loop, float width, int posCount = 0, bool worldSpace = true)
    {
        if (lr == null) return;
        lr.loop = loop;
        lr.useWorldSpace = worldSpace;
        lr.alignment = LineAlignment.View;
        lr.numCornerVertices = 0;
        lr.numCapVertices = 0;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        if (posCount > 0) lr.positionCount = posCount;
    }
}
