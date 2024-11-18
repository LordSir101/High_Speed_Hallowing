using System;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;


public abstract class TelegraphedHitbox : MonoBehaviour
{
    
    public float WindupTime { get; set; }
    public float ActiveTime { get; set;}
    public float CooldownTime { get; set;}
    public float WindupTimer { get; set;} = 0f;
    public float CooldownTimer { get; set;} = 0f;
    public int Damage { get; set;}

    public float Size { get; set;}
    public float StartingTelegaphPercentSize { get; set;} // is a percent of max size
    public float AnimationStartPercent { get; set;} //the percentage of hitbox filled when the attack animation starts

    public bool attackStarted { get; set;} = false;
    public bool attackEnded { get; set;} = false;
    public bool startAnimation { get; set;} = false;
    public bool attackReady = false;

    public float windupProgress = 0;

    GameObject telegraphSprite;

    public void Init(float windupTime, float activeTime, float cooldownTime, float size, int damage, float startingTelegaphPercentSize = 0f, float animationStartPercent = 1f)
    {
        WindupTime = windupTime;
        ActiveTime = activeTime;
        CooldownTime = cooldownTime;
        Size = size;
        Damage = damage;
        StartingTelegaphPercentSize = startingTelegaphPercentSize;
        AnimationStartPercent = animationStartPercent;
        
        
        // The sprite that fills up the hitbox to show the player when the attack will come
        telegraphSprite = Utils.FindGameObjectInChildWithTag(gameObject, "TelegraphSprite");

        gameObject.GetComponent<Renderer>().enabled = false;
        telegraphSprite.GetComponent<Renderer>().enabled = false;

        transform.localScale = new Vector3(Size, Size, 0);
        telegraphSprite.transform.localScale = new Vector3(startingTelegaphPercentSize, startingTelegaphPercentSize,0);

        SetAllCollidersStatus(false);

        //Setup();

    }

    //public abstract void Setup();

    public void SetAllCollidersStatus (bool isActive) {
        foreach(Collider2D c in GetComponentsInChildren<Collider2D>()) {
            c.enabled = isActive;
        }
    }

    public void Update()
    {
        
        UpdateHitboxPosition();
        if(attackStarted)
        {
            WindupTimer += Time.deltaTime;

            windupProgress = WindupTimer / WindupTime;
            float interpolatedScale = Mathf.Lerp(StartingTelegaphPercentSize, 1, windupProgress);

            Transform windupSpriteTransform = telegraphSprite.transform;
            windupSpriteTransform.localScale = new Vector3(interpolatedScale, interpolatedScale, 0);

            // if(WindupTimer >= AnimationStartPercent)
            // {
            //     startAnimation = true;
            // }

            if(WindupTimer >= WindupTime)
            {
                attackStarted = false;
                //
                WindupTimer = 0f;

                StartCoroutine(StartActiveAttackFrames());
            }
        }
    }

    public  virtual void UpdateHitboxPosition()
    {
        transform.position = transform.parent.transform.position;
    }

    public IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(CooldownTime);
        attackReady = true;
    }

    public void StartAttack()
    {
        attackStarted = true;
        attackReady = false;
        attackEnded = false;
        gameObject.GetComponent<Renderer>().enabled = true;
        telegraphSprite.GetComponent<Renderer>().enabled = true;
    }

    public void EndAttack()
    {
        StartCoroutine(StartCooldown());
        // gameObject.GetComponent<Renderer>().enabled = false;
        // telegraphSprite.GetComponent<Renderer>().enabled = false;
        attackEnded = true;
        windupProgress = 0;
    }

    public virtual IEnumerator StartActiveAttackFrames()
    {
        SetAllCollidersStatus(true);
        gameObject.GetComponent<Renderer>().enabled = false;
        telegraphSprite.GetComponent<Renderer>().enabled = false;

        yield return new WaitForSeconds(ActiveTime);
        
        SetAllCollidersStatus(false);
        EndAttack();
    }

    // public static GameObject FindGameObjectInChildWithTag (GameObject parent, string tag)
	// {
	// 	Transform t = parent.transform;

	// 	for (int i = 0; i < t.childCount; i++) 
	// 	{
	// 		if(t.GetChild(i).gameObject.tag == tag)
	// 		{
	// 			return t.GetChild(i).gameObject;
	// 		}
				
	// 	}
			
	// 	return null;
	// }
}
