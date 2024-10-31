using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TelegraphedHitbox
{
    public float WindupTime { get; set;}
    public float ActiveTime { get; set;}
    //public float Timer { get; set;}
    public float CooldownTime { get; set;}
    //public Sprite HitboxSprite { get; set;}

    public void StartAttack();
}
