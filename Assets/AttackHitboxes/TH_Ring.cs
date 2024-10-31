using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TH_Ring : MonoBehaviour, TelegraphedHitbox
{
    public float WindupTime { get; set; }
    public float ActiveTime { get; set;}
    public float CooldownTime { get; set;}
    public float WindupTimer { get; set;} = 0f;
    public float CooldownTimer { get; set;} = 0f;

    public float MaxSize { get; set;}
    public float MinSize { get; set;} // is a percent of max size

    private bool attackStarted = false;

    GameObject telegraphSprite; 

    // [SerializeField]
    // private Sprite hitboxSprite;
    public Sprite HitboxSprite { get; set; }

    public void Init(float windupTime, float activeTime, float cooldownTime, float minSize, float maxSize)
    {
        WindupTime = windupTime;
        ActiveTime = activeTime;
        CooldownTime = cooldownTime;
        MinSize = minSize;
        MaxSize = maxSize;

        telegraphSprite = gameObject.transform.GetChild(1).gameObject;

        gameObject.GetComponent<Renderer>().enabled = false;
        telegraphSprite.GetComponent<Renderer>().enabled = false;

        transform.localScale = new Vector3(MaxSize, MaxSize, 0);
        telegraphSprite.transform.localScale = new Vector3(MinSize, MinSize,0);
        gameObject.transform.GetChild(0).localScale = new Vector3(MinSize, MinSize,0);

    }

    public void OnEnable()
    {
        //HitboxSprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        // min size is the same size as the mask
        //MinSize = gameObject.transform.GetChild(0).localScale.x;

        StartCoroutine(StartCooldown());
        
    }

    IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(CooldownTime);
        StartAttack();
    }

    public void Update()
    {
        if(attackStarted)
        {
            WindupTimer += Time.deltaTime;
            float interpolationRatio = WindupTimer / WindupTime;

            float interpolatedScale = Mathf.Lerp(MinSize, 1, interpolationRatio);

            Transform windupSpriteTransform = telegraphSprite.transform;
            windupSpriteTransform.localScale = new Vector3(interpolatedScale, interpolatedScale, 0);

            
            if(WindupTimer >= WindupTime)
            {
                attackStarted = false;
                WindupTimer = 0f;
                DoAttack();
            }
        }
    }

    private void DoAttack()
    {
        StartCoroutine(ActiveAttackTime());
        
    }

    IEnumerator ActiveAttackTime()
    {
        yield return new WaitForSeconds(ActiveTime);
        EndAttack();
    }

    public void StartAttack()
    {
        attackStarted = true;
        gameObject.GetComponent<Renderer>().enabled = true;
        telegraphSprite.GetComponent<Renderer>().enabled = true;
    }

    private void EndAttack()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        telegraphSprite.GetComponent<Renderer>().enabled = false;
        StartCoroutine(StartCooldown());
    }

}
