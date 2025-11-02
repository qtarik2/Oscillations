using UnityEngine;
using UnityEngine.UI;

public class OscillationController : MonoBehaviour
{
    public Oscillator oscillatorA;
    public Oscillator oscillatorB;

    public Slider phaseSlider;
    public Slider frequencySlider;
    public Slider amplitudeSlider;

    void Start()
    {
        phaseSlider.onValueChanged.AddListener(value =>
        {
            oscillatorB.phaseShiftDegrees = value;
        });

        frequencySlider.onValueChanged.AddListener(value =>
        {
            oscillatorA.frequency = oscillatorB.frequency = value;
        });

        amplitudeSlider.onValueChanged.AddListener(value =>
        {
            oscillatorA.amplitude = oscillatorB.amplitude = value;
        });
    }
}
