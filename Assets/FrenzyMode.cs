using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FrenzyMode : MonoBehaviour
{
    private int damage = 75;
    private float damageTimer = 0;
    private float damageRate = 2;
    private bool frenzy = false;

    PlayerHealth playerhealthScript;
    SpawnEnemy enemySpawner;
    [SerializeField] GameOverPanel gameOverPanel;

    [SerializeField] ScriptableRendererFeature frenzyEffect;
    [SerializeField] Material effectMaterial;
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
                frenzyEffect.SetActive(false);
                gameOverPanel.SetWin(true);
            }
            else
            {
                damageTimer += Time.deltaTime;

                if(damageTimer >= damageRate)
                {
                    playerhealthScript.TakeDamage(damage);
                    damageTimer = 0;
                }
            }
            
        }
    }

    public void StartFrenzyMode()
    {
        frenzy = true;
        enemySpawner.SpawnFrenzyWave();
        frenzyEffect.SetActive(true);
    }
}
