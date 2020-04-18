using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBar : MonoBehaviour
{
    [SerializeField]
    Transform indicator;
    float position;
    [SerializeField]
    Color earlyIndicator, goodIndicator, lateIndicator;
    public enum Timing {EARLY, GOOD, LATE};
    Timing timing;

    void Start()
    {
        
    }

    void Update()
    {
        float xPos = Mathf.Lerp(-0.5f, 0.5f, position);
        Vector3 newPos = new Vector3(xPos,indicator.localPosition.y, indicator.localPosition.z);
        indicator.localPosition = newPos;

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

    public void SetPosition(float percentage)
    {
        position = percentage;
    }

    public void SetQuality(Timing t)
    {
        timing = t;
    }
}
