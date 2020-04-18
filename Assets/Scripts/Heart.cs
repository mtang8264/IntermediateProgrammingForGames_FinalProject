using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    KeyCode heartbeatKey = KeyCode.H;
    float timer;
    Animator animator;
    [SerializeField]
    float minTime, maxTime;
    [SerializeField]
    HeartBar bar;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < minTime)
            bar.SetQuality(HeartBar.Timing.EARLY);
        else if (timer > maxTime)
            bar.SetQuality(HeartBar.Timing.LATE);
        else
            bar.SetQuality(HeartBar.Timing.GOOD);

        animator.SetBool("Asphyxiating", timer > maxTime);

        if(Input.GetKeyDown(heartbeatKey))
        {
            Heartbeat();
        }

        bar.SetPosition(timer/maxTime);
    }

    void Heartbeat()
    {
        animator.SetTrigger("Beat");
        timer = 0f;
    }
}
