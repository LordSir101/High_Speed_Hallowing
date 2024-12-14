using UnityEngine;

public class PlayerCooldowns : MonoBehaviour
{
    PlayerMovement playerMovement;
    PlayerGrapple playerGrapple;
    PlayerAudio playerAudio;
    PlayerAnimation playerAnimation;

    float dashCooldown = 3;
    float dashCooldownTimer = 0;
    //[SerializeField] GameObject dashCooldownIcon;
    [SerializeField] CooldownUI cooldownUI;

    float grappleCooldown = 3.5f;
    float grappleCooldownTimer = 0;
    //[SerializeField] GameObject grappleCooldownIcon;

    bool dashCooldownStarted = false;
    bool grappleCooldownStarted = false;

    void Update()
    {
        if(!playerMovement.canDash)
        {
            //dashCooldownIcon.SetActive(false);
            dashCooldownTimer += Time.deltaTime;
            if(dashCooldownTimer >= dashCooldown)
            {
                //dashCooldownIcon.SetActive(true);
                playerMovement.canDash = true;
                dashCooldownTimer = 0;
                //dashCooldownStarted = false;
            }
        }

        if(!playerGrapple.canGrapple)
        {
            //grappleCooldownIcon.SetActive(false);
            grappleCooldownTimer += Time.deltaTime;
            if(grappleCooldownTimer >= grappleCooldown)
            {
                //grappleCooldownIcon.SetActive(true);
                playerGrapple.canGrapple = true;
                grappleCooldownTimer = 0;
            }
        }
    }
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerGrapple = GetComponent<PlayerGrapple>();
        playerAudio = GetComponentInChildren<PlayerAudio>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }
    // public void StartCooldown(float cooldownTime, System.Action callback)
    // {
    //     // Debug.Log("grapple on cooldown");
    //     // Debug.Log(cooldownTime);
    //     // yield return new WaitForSeconds(cooldownTime);
    //     // Debug.Log("grapple done cooldown");
    //     // callback();
    //     this.Invoke(callback, cooldownTime);
        
    // }
    public void StartDashCooldown()
    {
        cooldownUI.StartDashCooldown(dashCooldown);

    }

    public void StartGrappleCooldown()
    {
        cooldownUI.StartGrappleCooldown(grappleCooldown);

    }
    public void EndAllCooldowns()
    {
        StopAllCoroutines();
        PlayCooldownRefeshAnimations();
        playerMovement.canDash = true;
        playerGrapple.canGrapple = true;
        //grappleCooldownIcon.SetActive(true);
        //dashCooldownIcon.SetActive(true);
        cooldownUI.EndAllCooldowns();

        
        

    }

    private void PlayCooldownRefeshAnimations()
    {
        // only play the animation if a cooldown was actually affected 

        if(!playerMovement.canDash || !playerGrapple.canGrapple)
        {
            playerAnimation.PlayCooldownRefreshAnimation();
            playerAudio.PlayCooldownRefreshAudio();
        }
        
    }
}
