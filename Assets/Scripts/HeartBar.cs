using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBar : MonoBehaviour
{
    [SerializeField]
    Transform indicator;    // The circle indicator of when to push the button
    float position; // The position from left to right as a float [0,1]
    [SerializeField]
    Color earlyIndicator, goodIndicator, lateIndicator; // These are the colors the indicator turns based on when the player should push the button
    public enum Timing {EARLY, GOOD, LATE};
    Timing timing;

    void Start()
    {
        
    }

    void Update()
    {
        // This finds the local x position the indicator should be at
        float xPos = Mathf.Lerp(-0.5f, 0.5f, position);
        Vector3 newPos = new Vector3(xPos,indicator.localPosition.y, indicator.localPosition.z);
        indicator.localPosition = newPos;

        // Based on when in the timing process it is we change the color of the indicator
        switch(timing)
        {
            case Timing.EARLY:
                indicator.GetComponent<SpriteRenderer>().color = earlyIndicator;
                break;
            case Timing.GOOD:
                indicator.GetComponent<SpriteRenderer>().color = goodIndicator;
                break;
            case Timing.LATE:
                indicator.GetComponent<SpriteRenderer>().color = lateIndicator;
                break;
        }
    }

    // This updates where the indicator should be given a percentage from left to right
    public void SetPosition(float percentage)
    {
        position = percentage;
    }

    // This updates if the heartbeat will be early, late or good
    public void SetQuality(Timing t)
    {
        timing = t;
    }
}
