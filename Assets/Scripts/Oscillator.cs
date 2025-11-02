using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [Header("Oscillation Settings")]
    public float amplitude = 1f;
    public float frequency = 1f;
    [Range(0f, 360f)] public float phaseShiftDegrees = 0f;
    public bool useCircularMotion = false;
    public bool projectToAxis = true;

    private float phaseShift => phaseShiftDegrees * Mathf.Deg2Rad;

    void Update()
    {
        float time = Time.time;
        float angle = (time * frequency * 2f * Mathf.PI) + phaseShift;

        if (useCircularMotion)
        {
            // Move in a circle (for visualizing uniform circular motion)
            float x = amplitude * Mathf.Cos(angle);
            float y = amplitude * Mathf.Sin(angle);
            transform.localPosition = new Vector3(x, y, 0);
        }
        else if (projectToAxis)
        {
            // Move along x-axis to simulate SHM
            float x = amplitude * Mathf.Cos(angle);
            transform.localPosition = new Vector3(x, 0, 0);
        }
    }
}
