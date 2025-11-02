using System.Collections.Generic;
using UnityEngine;

public class WaveGraph : MonoBehaviour
{
    [Header("Oscillators")]
    public Oscillator oscillatorA;
    public Oscillator oscillatorB;

    [Header("Graph Settings")]
    public LineRenderer lineA;
    public LineRenderer lineB;
    public float timeWindow = 5f; // seconds visible on the graph
    public int resolution = 200;  // number of points across the window
    public float verticalScale = 1f;
    public float horizontalScale = 2f;
    public Vector3 graphOrigin = new Vector3(0, 0, 0);

    private float startTime;

    void Start()
    {
        startTime = Time.time;
        lineA.positionCount = resolution;
        lineB.positionCount = resolution;
    }

    void Update()
    {
        DrawWave(lineA, oscillatorA.phaseShiftDegrees);
        DrawWave(lineB, oscillatorB.phaseShiftDegrees);
    }

    void DrawWave(LineRenderer line, float phaseDeg)
    {
        float phaseRad = phaseDeg * Mathf.Deg2Rad;
        float frequency = oscillatorA.frequency;
        float amplitude = oscillatorA.amplitude;
        float timeNow = Time.time - startTime;

        for (int i = 0; i < resolution; i++)
        {
            float t = (i / (float)(resolution - 1)) * timeWindow;
            float angle = (timeNow - t) * frequency * 2f * Mathf.PI + phaseRad;
            float y = amplitude * Mathf.Cos(angle) * verticalScale;
            float x = (i / (float)(resolution - 1)) * timeWindow * horizontalScale;

            // Position the line relative to origin
            line.SetPosition(i, graphOrigin + new Vector3(x, y, 0));
        }
    }
}
