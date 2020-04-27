using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 100f)]
    float oxygen;
    public float OxygenLevel { get { return oxygen; } }
    Slider oxygenBar;   // The display bar which shows the oxygen level

    float inefficiencyMulti = 1f;   // This variable lowers the amount of oxygen you recieve from breathing based on how long your eyes have been open
                                    // You can see how it is set in the VisionLoss function in the Eye Functions region

    #region Lung Variables
    [Header("Lungs")]
    [SerializeField]
    bool lungsActive;   // This determines if the lungs are used in this level
    GameObject lungs;   // The game object reference for the lungs
    public enum LungState { EMPTY, INHALING, HOLDING, EXHALING };   // The possible states the lungs can be in
    LungState lungState;
    [Range(0f, 1f)]
    float lungCapacity; // The capacity of the lungs. This is like how much they are between empty(0f) and full(1f)
    [SerializeField]
    KeyCode inhaleKey, exhaleKey;   // The keys used to inhale and exhale
    [SerializeField]
    float inhaleSpeed, exhaleSpeed; // The rate at which the lungs inhale and exhale
    [SerializeField]
    float oxygenGainSpeed;  // The speed at which the oxygen bar fills up while inhaling
    [SerializeField]
    float emptyLungSize;    // The scale that the lungs reach when they are empty. We assume they should reach 1f scale while full.
    float oxygenLossTimer;  // This is the timer used to calculate when you should start losing oxygen
    [SerializeField]
    AnimationCurve oxygenLossRate;  // This curve defines how quickly you lose oxygen while not inhaling
    [SerializeField]
    float oxygenLossMultiplier; // This is a number multiplied by the oxygen loss rate
    Slider lungCapacityBar;
    Image lungCapacityBarFill, lungCapacityBarHandle;
    [SerializeField]
    Gradient lungCapacityBarGradient;
    #endregion
    #region Eyes Variables
    [Header("Eyes")]
    [SerializeField]
    bool eyesActive;    // Helps track if the eyes are used in this level
    [SerializeField]
    Image bloodshotImage, blurImage;    // These are the images on the UI which show the player how bloodshot or blurry their vision is
    float eyeTimer; // The timer used to track how long the player has gone without blinking
    bool eyesOpen = true;  // The state of the eyes as either open or closed
    [SerializeField]
    AnimationCurve bloodshotOpacityCurve, blurSizeCurve;    // The animation curves which help the bloodshot and blur effect come in gradually and the faster
    [SerializeField]
    float timeToFullBloodshot, timeToFullBlur;  // The timer until the player's eyes are fully bloodshot or blurry
    [SerializeField]
    Animator blinkingAnimator;  // The animator which controlls the eye lids
    #endregion

    void Awake()
    {
        oxygenBar = GameObject.FindWithTag("OxygenBar").GetComponent<Slider>();

        // If the level uses the lungs set up for the lungs
        if (lungsActive)
        {
            lungs = GameObject.FindWithTag("Lungs");
            lungState = LungState.EMPTY;
            lungCapacity = 0f;
            lungCapacityBar = GameObject.FindWithTag("LungCapacityBar").GetComponent<Slider>();
            Image[] images = lungCapacityBar.GetComponentsInChildren<Image>();
            foreach (Image i in images)
            {
                if (i.name == "Fill")
                    lungCapacityBarFill = i;
                else if (i.name == "Handle")
                    lungCapacityBarHandle = i;
            }
        }
    }

    void Update()
    {
        oxygenBar.value = oxygen;

        if (lungsActive)
        {
            DrawLungs();
            UpdateLungState();
            OxygenLoss();
        }
        if (eyesActive)
        {
            DrawEyes();
            UpdateEyesState();
            VisionLoss();
        }
    }

    #region Lung Functions
    public LungState GetCurrentLungState()
    {
        return lungState;
    }
    /// <summary>
    /// This function updates the visual display for the lungs.
    /// The size of the lungs is just a lerp based on the current lung capacity.
    /// </summary>
    void DrawLungs()
    {
        float s = Mathf.Lerp(emptyLungSize, 1f, lungCapacity);
        Vector3 scale = new Vector3(s, s, s);
        lungs.transform.localScale = scale;
        lungCapacityBar.value = lungCapacity;
        lungCapacityBarFill.color = lungCapacityBarGradient.Evaluate(lungCapacity);
        lungCapacityBarHandle.color = lungCapacityBarGradient.Evaluate(lungCapacity);
    }
    /// <summary>
    /// This function updates the lungState based on which key is pressed.
    /// </summary>
    void UpdateLungState()
    {
        switch (lungState)
        {
            case LungState.EMPTY:   // If the lungs are empty
                if (Input.GetKeyDown(inhaleKey))    // The lungs will begin to inhal when you press the inhale key
                {
                    lungState = LungState.INHALING;
                    BreathingSoundController.instance.StartInhaling();
                }
                break;
            case LungState.INHALING:    // If the lungs are inhaling
                oxygenLossTimer = 0f;
                lungCapacity += inhaleSpeed * Time.deltaTime;   // Lung capacity increases at a constant rate while inhaling
                oxygen += oxygenGainSpeed * Time.deltaTime * inefficiencyMulti; // While the lungs are inhaling the oxygen level also increases
                if (Input.GetKey(inhaleKey) == false || lungCapacity >= 1f) // If the player releases the inhale key or the max lung capacity is reached the lungs begin to hold
                {
                    lungState = LungState.HOLDING;
                    BreathingSoundController.instance.StopInhaling();
                }
                if (Input.GetKeyDown(exhaleKey)) // If you press the exhale key while still inhaling you will start exhaling
                {
                    lungState = LungState.EXHALING;
                    BreathingSoundController.instance.StartExhaling();
                }
                break;
            case LungState.HOLDING: // If the lungs are holding
                if (Input.GetKeyDown(exhaleKey))    // Pressing the exhale key will begin to exhale
                {
                    lungState = LungState.EXHALING;
                    BreathingSoundController.instance.StartExhaling();
                }
                break;
            case LungState.EXHALING:    // If the lungs are exhaling
                if (Input.GetKey(exhaleKey))    // The lungs exhale at a constant rate as long as you continue to hold the exhale key
                {
                    lungCapacity -= exhaleSpeed * Time.deltaTime;
                    BreathingSoundController.instance.ResumeExhaling();
                }
                else
                {
                    BreathingSoundController.instance.PauseExhaling();
                }
                if (lungCapacity <= Mathf.Epsilon)  // If the lungs capcity reaches 0 then the lungs are empty
                {
                    lungState = LungState.EMPTY;
                    BreathingSoundController.instance.PauseExhaling();
                }
                break;
        }
    }
    /// <summary>
    /// This function enforces the loss of oxygen while not breathing
    /// </summary>
    void OxygenLoss()
    {
        if (lungState == LungState.INHALING)
        {
            oxygenLossTimer = 0f;
            return;
        }
        oxygenLossTimer += Time.deltaTime;
        oxygen -= oxygenLossRate.Evaluate(oxygenLossTimer) * oxygenLossMultiplier * Time.deltaTime;
    }
    #endregion
    #region Eye Functions
    /// <summary>
    /// This function just allows other functions to read if they eyes are open
    /// </summary>
    /// <returns>Returns true if eyes are open and false if they eyes are closed.</returns>
    public bool GetEyesOpen()
    {
        return eyesOpen;
    }
    /// <summary>
    /// This function handles the visual aspects of the eyes.
    /// This includes blinking as well as the bloodshot and blur effects.
    /// </summary>
    void DrawEyes()
    {
        blinkingAnimator.SetBool("Open", eyesOpen);
        if(eyesOpen)    // The bloodshot and blur effects only need to change while the eyes are open
        {
            // Bloodshot
            float t = Mathf.Lerp(0f, 1f, eyeTimer / timeToFullBloodshot);
            t = bloodshotOpacityCurve.Evaluate(t);
            bloodshotImage.color = new Color(1f, 1f, 1f, t);
            // Blur
            float s = eyeTimer - timeToFullBloodshot;
            s = s / (timeToFullBlur - timeToFullBloodshot);
            s = Mathf.Lerp(0f, 1f, s);
            s = blurSizeCurve.Evaluate(s);
            blurImage.rectTransform.localScale = new Vector3(s, s, s);
        }
    }
    /// <summary>
    /// This function changes the eyesOpen variable when the correct keys are pressed.
    /// </summary>
    void UpdateEyesState()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.UpArrow) && eyesOpen)   // Close eyes
        {
            eyesOpen = false;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow) && !eyesOpen) // Open eyes
        {
            eyesOpen = true;
        }
    }
    /// <summary>
    /// This function handles the timer for vision loss.
    /// It also controls the inefficiency multiplier due to loss of vision.
    /// </summary>
    void VisionLoss()
    {
        // If the eyes are open then the timer continues and if they are closed the timer is held at 0
        if(eyesOpen)
        {
            eyeTimer += Time.deltaTime;
        }
        else
        {
            eyeTimer = 0f;
        }

        // This calculates how much of your breathing and other effects are reduced by based on how long you are in to the blur effect
        float t = eyeTimer - timeToFullBloodshot;
        t = t / (timeToFullBlur - timeToFullBloodshot);
        inefficiencyMulti = Mathf.Lerp(1f, 0f, t);
    }
    #endregion
}
