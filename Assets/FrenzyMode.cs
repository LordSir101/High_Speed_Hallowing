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
                frenzyEffect.SetActive(false);
                gameController.SetWin(true);
                frenzy = false;
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
        frenzyText.SetActive(true);
        frenzyText.GetComponent<TextMeshProUGUI>().text = "Defeat the remaining enemies quickly!";
    }
}
