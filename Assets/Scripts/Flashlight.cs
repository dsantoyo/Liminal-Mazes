using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#region Required Components
[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioSource))]
#endregion
public sealed class Flashlight : MonoBehaviour
{
    #region Variables

    #region Keys

    [SerializeField] KeyCode reloadKey                          = KeyCode.R;
    [SerializeField] KeyCode toggleKey                          = KeyCode.F;

    #endregion

    #region Parameters

    [SerializeField] int        maxBatteries                        = 4;
    [SerializeField] int        batteries                           = 2;

    [SerializeField] bool       autoReduce                          = true;
    [SerializeField] float      reduceSpeed                         = 1.0f;

    [SerializeField] bool       autoIncrease                        = false;
    [SerializeField] float      increaseSpeed                       = 1.0f;

    [Range(0, 1)]
    [SerializeField] float      toggleOnWaitTillPercentageReached   = 0.05f;

    public const     float      minBatteryLife                      = 0.0f;
    [SerializeField] float      maxBatteryLife                      = 1.0f;

    [Range(1, 25)]
    [SerializeField] float      followSpeed                         = 5.0f;
    [SerializeField] Quaternion offset                              = Quaternion.identity;

    #endregion

    #region References

    #region Audio

    [SerializeField] AudioClip onSound      = null;
    [SerializeField] AudioClip offSound     = null;
    [SerializeField] AudioClip reloadSound  = null;

    #endregion

    #region UI

    [SerializeField] Image              stateIcon       = null;
    [SerializeField] Slider             lifeSlider      = null;
    [SerializeField] Image              lifeSliderFill  = null;
    [SerializeField] TextMeshProUGUI    reloadText      = null;
    [SerializeField] TextMeshProUGUI    countText       = null;
    [SerializeField] CanvasGroup        holder          = null;

    #region Colors

    [SerializeField] Color  fullLifeColor = Color.green;
    [SerializeField] Color  deadLifeColor = Color.red;

    #endregion

    #endregion

    #region Object

    [SerializeField] new Camera camera      = null;
    [SerializeField] GameObject flashlight  = null;

    #endregion

    #endregion

    #region Statistics

    [SerializeField]
    private float           batteryLife                         = 0.0f;
    [SerializeField]
    private bool            usingFlashlight                     = false;
    [SerializeField]
    private bool            outOfBattery                        = false;

    #endregion

    #region Private and Properties

    private IEnumerator     IE_UpdateBatteryLife                = null;

    Light                   _light                              = null;
    Light                   Light                               
    {
        get
        {
            if (_light == null)
            {
                _light = GetComponent<Light>();
                if (_light == null) { _light = gameObject.AddComponent<Light>(); }
                _light.type = LightType.Spot;
            }
            return _light;
        }
    }

    float                   defaultIntensity                    = 0.0f;

    AudioSource             _source                             = null;
    AudioSource             Source                              
    {
        get
        {
            if (_source == null)
            {
                _source = GetComponent<AudioSource>();
                if (_source == null) { _source = gameObject.AddComponent<AudioSource>(); }
                _source.playOnAwake = false;
            }
            return _source;
        }
    }

    float                   GetLifePercentage                   
    {
        get
        {
            return (batteryLife - minBatteryLife) / (maxBatteryLife - minBatteryLife);
        }
    }
    float                   GetLightIntensity                   
    {
        get
        {
            return defaultIntensity * GetLifePercentage;
        }
    }

    bool                    CanReload                           
    {
        get
        {
            return usingFlashlight && (batteries > 0 && batteryLife < maxBatteryLife);
        }
    }
    bool                    MoreThanNeededPercentage            
    {
        get
        {
            return GetLifePercentage >= toggleOnWaitTillPercentageReached;
        }
    }

    #endregion

    #endregion

    void Start()
    {
        Init();
    }
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight(!usingFlashlight, true);
        }
        if (Input.GetKeyDown(reloadKey) && CanReload)
        {
            Reload();
        }
        if (usingFlashlight)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, camera.transform.localRotation * offset, followSpeed * Time.deltaTime);
            flashlight.transform.rotation = transform.rotation;
        }
    }

    void ToggleFlashlight(bool state, bool playSound)
    {
        //Update the using state.
        usingFlashlight = state;

        //Update the flashlight object state.
        flashlight.SetActive(state);

        //Set the toggle object state.
        state = (outOfBattery && usingFlashlight) ? false : usingFlashlight;
        //Update the flashlight state.
        ToggleObject(state);

        //Check if holder is referenced.
        if (holder)
        {
            //Do a using flashlight switch.
            //switch (usingFlashlight)
            //{
                //case true:
                    //Set the holder canvas group alpha value to 1.
                    holder.alpha = 1.0f;
                    //Set the holder canvas group block raycasts state to true.
                    holder.blocksRaycasts = true;
                    //break;
                //case false:
                    //Set the holder canvas group alpha value to 0.
                    //holder.alpha = 0.0f;
                    //Set the holder canvas group block raycasts state to false.
                    //holder.blocksRaycasts = false;
                    //break;
            //}
        }

        //Check if needs to play sound.
        if (playSound)
        {
            //Call play sound effect method and pass in the right clip depending on the current using state.
            PlaySFX(usingFlashlight ? onSound : offSound);
        }
        //Update the battery life process.
        UpdateBatteryLifeProcess();
    }
    private void ToggleObject(bool state)
    {
        //Update the light component state.
        Light.enabled = state;
    }

    private void UpdateBatteryLifeProcess()
    {
        //Stop coroutine if currently running.
        if (IE_UpdateBatteryLife != null) { StopCoroutine(IE_UpdateBatteryLife); }

        //If we are using the flashlight and not out of battery.
        if (usingFlashlight && !outOfBattery)
        {
            //Check if needs to auto reduce the battery life.
            if (autoReduce)
            {
                //Cache the coroutine reference (Reduce process).
                IE_UpdateBatteryLife = ReduceBattery();
                //Start the coroutine.
                StartCoroutine(IE_UpdateBatteryLife);
            }
            //Return from this method.
            return;
        }
        //Check if needs to auto increase the battery life.
        if (autoIncrease)
        {
            //Cache the coroutine reference (Increase Process).
            IE_UpdateBatteryLife = IncreaseBattery();
            //Start the coroutine.
            StartCoroutine(IE_UpdateBatteryLife);
        }
    }
    IEnumerator ReduceBattery()
    {
        //While battery life is not equal to zero.
        while (batteryLife > 0.0f)
        {
            //Get the new reduced value.
            var newValue = batteryLife - reduceSpeed * Time.deltaTime;

            //Update the battery life value.
            batteryLife = Mathf.Clamp(newValue, 0, maxBatteryLife);
            //Update the light intensity value.
            Light.intensity = GetLightIntensity;

            //Update the battery life UI slider.
            UpdateSlider();

            //Yield for a single frame.
            yield return null;
        }

        /// When out of battery ///
        
        //Update the battery state to true (meaning is out of battery).
        UpdateBatteryState(true);
        //Update the battery process.
        UpdateBatteryLifeProcess();
    }
    IEnumerator IncreaseBattery()
    {
        //While battery life is less than max battery life.
        while (batteryLife < maxBatteryLife)
        {
            //Get the new increased value.
            var newValue = batteryLife + increaseSpeed * Time.deltaTime;

            //Update the battery life value.
            batteryLife = Mathf.Clamp(newValue, 0, maxBatteryLife);
            //Update the light intensity value.
            Light.intensity = GetLightIntensity;

            //Do a battery life check.
            BatteryLifeCheck();

            //Update the battery life UI slider.
            UpdateSlider();

            //Yield for a single frame.
            yield return null;
        }
    }
    private void BatteryLifeCheck()
    {
        //If the percentage is met and is currently out of battery.
        if (MoreThanNeededPercentage && outOfBattery)
        {
            //Update the battery state.
            UpdateBatteryState(false);
            //Update the battery life process
            UpdateBatteryLifeProcess();
        }
    }

    private void UpdateBatteryState(bool isDead)
    {
        //Update out of battery state.
        outOfBattery = isDead;

        //Check if reload text is referenced.
        if (reloadText)
        {
            //Update battery life text component state.
            reloadText.enabled = isDead;
        }

        if (stateIcon)
        { stateIcon.color = isDead ? new Color(1,1,1,.25f) : Color.white; }

        //Create object new state.
        var state = outOfBattery 
            ? false 
            : usingFlashlight ? true : false;

        //Update object state.
        ToggleObject(state);
    }
    public void Reload()
    {
        //Set battery life to max battery life.
        batteryLife             = maxBatteryLife;
        //Update the flashlight intensity.
        Light.intensity         = GetLightIntensity;
        //Reduce batteries count.
        batteries--;

        #region Update UI
        //Update battery count text.
        UpdateCountText();
        //Update the battery life UI slider.
        UpdateSlider();
        #endregion

        //Update the battery state to false (meaning is not out of battery).
        UpdateBatteryState(false);

        //Update the battery life process
        UpdateBatteryLifeProcess();

        //Play reload sound effect.
        PlaySFX(reloadSound);
    }

    private void UpdateCountText()
    {
        //Check if count text is referenced.
        if (countText)
        {
            //Create a new StringBuilder.
            StringBuilder countString = new StringBuilder("Batteries: ");
            //Append current batteries value.
            countString.Append(batteries);
            //Append the slash character.
            countString.Append(" / ");
            //Append the max batteries value.
            countString.Append(maxBatteries);

            //Update the count text component text value to string builder text value.
            countText.text = countString.ToString();
        }
    }
    private void UpdateSlider()
    {
        //Check if life slider is referenced and if so update the life slider's value to battery life value.
        if (lifeSlider) { lifeSlider.value = batteryLife; }
        //Check if life slider fill is referenced.
        if (lifeSliderFill)
        {
            //Update the life slider fill UI image color.
            lifeSliderFill.color = Color.Lerp(deadLifeColor, fullLifeColor, GetLifePercentage);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        //Check if argument is equal to null, if so return from this method.
        if (clip == null) return;

        //Set the source clip to the passed as an argument clip.
        Source.clip = clip;
        //Play the sound.
        Source.Play();
    }

    private void Init()
    {
        //Cache the default max intensity.
        defaultIntensity = Light.intensity;
        //Set the start battery life to a max battery life.
        batteryLife = maxBatteryLife;

        UpdateBatteryState(false);

        //Toggle off the flashlight at the start of the game.
        ToggleFlashlight(false, false);

        UpdateCountText();

        lifeSlider.minValue = minBatteryLife;
        lifeSlider.maxValue = maxBatteryLife;
        lifeSlider.value = batteryLife;
        UpdateSlider();

        if (!camera)
        {
            camera = Camera.main;
        }
        reloadText.text = "RELOAD (" + reloadKey + ")";
    }
}