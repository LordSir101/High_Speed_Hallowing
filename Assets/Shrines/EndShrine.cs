using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class EndShrine : MonoBehaviour
{
    List<GameObject> gems;
    [SerializeField] GameObject gemParent;
    [SerializeField] Sprite cleansedGemOutlineSprite;
    [SerializeField] private InputActionReference interact;
    [SerializeField] private ShrineManager shrineManager;
    Sprite uncleansedOutlineSprite;
    Color32 defaultGemLightColor;
    Color32 defaultGemColor;
    TextMeshProUGUI interactText;
    private GameObject player;
    private int cost = 0;

    int shrinesCleansed = 0;
    // Start is called before the first frame update
    void Start()
    {
        gems = new List<GameObject>();
        foreach(Transform gem in gemParent.transform)
        {
            gems.Add(gem.gameObject);
        }

        interactText = Utils.FindGameObjectInChildWithTag(gameObject.GetComponentInChildren<Canvas>().gameObject, "InteractText").GetComponent<TextMeshProUGUI>();
        player = GameObject.FindGameObjectWithTag("Player");

        cost  = 1500 * gems.Count;

        uncleansedOutlineSprite = gems[0].GetComponent<SpriteRenderer>().sprite;
        defaultGemLightColor = gems[0].GetComponentInChildren<Light2D>().color;
        defaultGemColor = gems[0].transform.GetChild(0).GetComponent<SpriteRenderer>().color;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void TurnOnGem()
    {
        shrinesCleansed += 1;
        gems[shrinesCleansed -1].GetComponent<SpriteRenderer>().sprite = cleansedGemOutlineSprite;
        gems[shrinesCleansed -1].GetComponentInChildren<Light2D>().color = new Color32(0, 183, 12, 255);
        gems[shrinesCleansed -1].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(0, 183, 12, 255);
    }

     private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            // check shrines -1 since totalShrines includes the end shrine
            if(shrineManager.shrinesActivated == GameStats.totalShrines - 1)
            {
                // ShowTrText();
                // interact.action.performed += Upgrade;
                interactText.enabled = true;
                interactText.text = $"Tribute to cleanse the area ({shrineManager.interactKeybind}) ({cost})" ;
                interact.action.performed += TributeBigShrine;
            }
            else
            {
                interactText.enabled = true;
                interactText.text = $"The king is asleep..." ;
            }
        }
        
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(shrineManager.shrinesActivated == GameStats.totalShrines - 1)
            {
                interact.action.performed -= TributeBigShrine;
            }

            interactText.enabled = false;
        }
        
    }

    private void TributeBigShrine(InputAction.CallbackContext context)
    {
        PlayerResourceManager rm = player.GetComponent<PlayerResourceManager>();

        if(rm.Essence >= cost)
        {
            GetComponent<ParticleSystem>().Play();
            shrineManager.PlayCleanseSound();
            
            rm.Essence -= cost;

            GameObject damageTextParent = Instantiate(shrineManager.paymentText, new Vector2 (transform.position.x, transform.position.y + 1), Quaternion.identity);
            damageTextParent.GetComponentInChildren<TextMeshPro>().text = $"-{cost}";
            damageTextParent.GetComponentInChildren<TextMeshPro>().color = Color.white;
            
            GameStats.IncreaseShrinesCleansed();
            //gameObject.GetComponent<CircleCollider2D>().enabled = false;
            //interactText.enabled = false;

            // allows upgrade text to appear right away without moving away from shrine
            interact.action.performed -= TributeBigShrine;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            shrineManager.StartFrenzyMode();
            //gameObject.GetComponent<SpriteRenderer>().sprite = cleanseSprite;

            // ChangeIcon();

            //shrineManager.SpawnEnemiesAtShrine(transform.position, 3);
        }   
    }

    public void ResetShrine()
    {
        shrinesCleansed = 0;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;

        foreach(GameObject gem in gems)
        {
            gem.GetComponent<SpriteRenderer>().sprite = uncleansedOutlineSprite;
            gem.GetComponentInChildren<Light2D>().color = defaultGemLightColor;
            gem.transform.GetChild(0).GetComponent<SpriteRenderer>().color = defaultGemColor;
        }
    }
}
