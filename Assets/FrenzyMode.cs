using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FrenzyMode : MonoBehaviour
{
    private int damage = 50;
    private float damageTimer = 0;
    private float damageRate = 1;
    private bool frenzy = false;

    PlayerHealth playerhealthScript;
    SpawnEnemy enemySpawner;
    [SerializeField] GameControl gameController;

    [SerializeField] ScriptableRendererFeature frenzyEffect;
    [SerializeField] Material effectMaterial;
    [SerializeField] GameObject frenzyText;
    // Start is called before the first frame update
    void Start()
    {
        playerhealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        enemySpawner = GetComponent<SpawnEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if(frenzy)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if(enemies.Length == 0)
            {
                if(GameStats.currGameMode == GameStats.GameMode.TimeAttack)
                {
                    //StopFrenzyMode();
                    gameController.SetWin(true);
                }
                else if(GameStats.currGameMode == GameStats.GameMode.Endless)
                {
                    gameController.ResetMap();
                }

            }
            // if(GameStats.currGameMode == GameStats.GameMode.TimeAttack)
            // {
            //     GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            //     if(enemies.Length == 0)
            //     {
            //         //StopFrenzyMode();
            //         gameController.SetWin(true);

            //     }
            // }
           
            damageTimer += Time.deltaTime;

            if(damageTimer >= damageRate)
            {
                playerhealthScript.TakeDamage(damage);
                damageTimer = 0;
            }
            
        }
    }

    void OnDisable()
    {
        frenzyEffect.SetActive(false);
    }

    public void StartFrenzyMode()
    {
        frenzy = true;
        frenzyEffect.SetActive(true);

        if(GameStats.currGameMode == GameStats.GameMode.TimeAttack || GameStats.currGameMode == GameStats.GameMode.Endless)
        {
            enemySpawner.SpawnFrenzyWave();
            frenzyText.GetComponent<TextMeshProUGUI>().enabled = true;
            frenzyText.GetComponent<TextMeshProUGUI>().text = "Defeat the remaining enemies quickly!";
        }
        // else if(GameStats.currGameMode == GameStats.GameMode.Survival)
        // {
        //     StartCoroutine(FrenzyDamageRampUp());
        // }
       
    }

    public void StopFrenzyMode()
    {
        frenzyEffect.SetActive(false);    
        frenzy = false;
        frenzyText.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    public void IncreaseFrenzyDamage(int damageIncrease)
    {
        damage += damageIncrease;
    }

    // IEnumerator FrenzyDamageRampUp()
    // {
    //     while(frenzy)
    //     {
    //         yield return new WaitForSeconds(30);
    //         damage += 10;
    //     }
        
    // }
}
