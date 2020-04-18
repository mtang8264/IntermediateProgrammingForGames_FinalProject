using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    KeyCode heartbeatKey = KeyCode.H;   // This is the key that you use to make the heart pump
    float timer;    // This is the timer which tracks when you last beat the heart
    Animator animator;
    [SerializeField]
    float minTime, maxTime; // The max and min times you are allowed to beat the heart and have it be good
    [SerializeField]
    HeartBar bar;   // The display bar which shows the player when to beat the heart

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        timer += Time.deltaTime;    // Increase the timer

        // These if statements tell the heart bar if right now is early, late, or good to press the heartbeat button
        if (timer < minTime)
            bar.SetQuality(HeartBar.Timing.EARLY);
        else if (timer > maxTime)
            bar.SetQuality(HeartBar.Timing.LATE);
        else
            bar.SetQuality(HeartBar.Timing.GOOD);

        animator.SetBool("Asphyxiating", timer > maxTime);  // If it is late to push the button we tell the animator to start playing the asphyxiating animation

        // If you press the heartbeat key you beat the heart
        if(Input.GetKeyDown(heartbeatKey))
        {
            Heartbeat();
        }

        // This just tells the bar how far to put the indicator on a scale of [0,1] with 0 representing the far left and 1 representing the far right
        bar.SetPosition(timer/maxTime);
    }

    void Heartbeat()
    {
        animator.SetTrigger("Beat");    // Trigger the beat animation
        timer = 0f; // Reset the timer
    }
}
