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
            if(GameStats.currGameMode == GameStats.GameMode.TimeAttack)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                if(enemies.Length == 0)
                {
                    StopFrenzyMode();
                    gameController.SetWin(true);

                }
            }
           
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

        if(GameStats.currGameMode == GameStats.GameMode.TimeAttack)
        {
            enemySpawner.SpawnFrenzyWave();
            frenzyText.GetComponent<TextMeshProUGUI>().enabled = true;
            frenzyText.GetComponent<TextMeshProUGUI>().text = "Defeat the remaining enemies quickly!";
        }
       
    }

    public void StopFrenzyMode()
    {
        frenzyEffect.SetActive(false);    
        frenzy = false;
    }
}
