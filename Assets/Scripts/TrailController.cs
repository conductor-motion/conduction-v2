using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrailController : MonoBehaviour
{
    // Instantiation of varaibles
    TrailRenderer trail;
    Slider trailSlider;
    Text label;

    float startingValue = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Assign variables from the world
        trail = gameObject.GetComponent<TrailRenderer>(); // Grabs the trail renderer for this trail
        trailSlider = GameObject.Find("Trail Length").GetComponent<Slider>(); // Grabs the trail slider by name
        trail.time = startingValue;
        trailSlider.value = startingValue;

        label = GameObject.Find("Length Label").GetComponent<Text>();
        label.text = startingValue.ToString("0.0") + "s";

        // Adds listener that updates the trail length whenever the slider is updated
        trailSlider.onValueChanged.AddListener(delegate { ChangeTrailLength(trailSlider.value); });
    }

    
    public void ChangeTrailLength(float newLength)
    {
        trail.time = newLength;

        // The text gets changed twice (once per trail), but that doesn't really matter
        label.text = newLength.ToString("0.0") + "s";
    }
}
