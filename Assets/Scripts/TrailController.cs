using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Control the length of the trails shown on the models with a slider
public class TrailController : MonoBehaviour
{
    // Instantiation of variables
    TrailRenderer trail;
    Slider trailSlider;
    Text label;
    Toggle showTrails;

    float startingValue = 0.5f;

    private bool isShown = false;

    // Retrieve objects
    void Awake()
    {
        trail = gameObject.GetComponent<TrailRenderer>(); // Grabs the trail renderer for this trail
        trailSlider = GameObject.Find("Trail Length").GetComponent<Slider>(); // Grabs the trail slider by name
        label = GameObject.Find("Length Label").GetComponent<Text>();
        if (GameObject.Find("Toggle Trails"))
        {
            showTrails = GameObject.Find("Toggle Trails").GetComponent<Toggle>();
        }
    }

    // Assign listeners and starting change values
    void Start()
    {
        // Assign variables from the world
        trail.time = startingValue;
        trailSlider.value = startingValue;

        label.text = startingValue.ToString("0.0") + "s";

        // Adds listener that updates the trail length whenever the slider is updated
        trailSlider.onValueChanged.AddListener(delegate { ChangeTrailLength(trailSlider.value); });

        // Gets the trail toggle, which exists only on the recording page
        // Initailizes everything as hidden
        if (showTrails)
        {
            trailSlider.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
            showTrails.onValueChanged.AddListener(delegate { ToggleSliderState(); });
        }
    }

    public void ToggleSliderState()
    {
        isShown = !isShown;

        if (isShown)
        {
            this.gameObject.SetActive(true);
            trailSlider.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
            trailSlider.gameObject.SetActive(false);
        }
    }

    public void ChangeTrailLength(float newLength)
    {
        trail.time = newLength;

        // The text gets changed twice (once per trail), but that doesn't really matter
        label.text = newLength.ToString("0.0") + "s";
    }
}
