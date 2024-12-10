using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update

    // TODO: remove if this gets set by a menu later
    void Awake()
    {
        GameStats.gameDifficulty = GameStats.GameDifficulty.tutorial;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerResourceManager>().Essence = 400;
    }


}
