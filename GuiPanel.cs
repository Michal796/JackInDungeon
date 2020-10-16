using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//klasa opdowiada za wyświetlanie informacji o bohaterze w bocznym panelu 
public class GuiPanel : MonoBehaviour
{
    [Header("definiowanie ręczne w panelu inspector")]
    public Jack jack;
    public Sprite healthEmpty;
    public Sprite healthHalf;
    public Sprite healthFull;

    Text keyCountText;
    List<Image> healthImages;
    // Start is called before the first frame update
    void Start()
    {
        Transform trans = transform.Find("Key Count");
        keyCountText = trans.GetComponent<Text>();

        Transform healthPanel = transform.Find("Health Panel");
        healthImages = new List<Image>();
        //przetworzenie wszystkich obrazków zdrowia na panelu (5)
        if(healthPanel != null)
        {
            for (int i=0; i< 20; i++)
            {
                trans = healthPanel.Find("H_"+i);
                //ten kod wykona się tylko dla i<=4, ponieważ jest tylko 5 obrazków
                if (trans == null) break;
                healthImages.Add(trans.GetComponent<Image>());

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        keyCountText.text = jack.numKeys.ToString();

        int health = jack.health;
        //przetworzenie każdego z obrazków
        for (int i=0; i<healthImages.Count; i++)
        {
            if (health > 1)
            {
                healthImages[i].sprite = healthFull;

            }
            else if (health == 1){
                healthImages[i].sprite = healthHalf;
            }
            else
            {
                healthImages[i].sprite = healthEmpty;
            }
            health -= 2; // zmniejszenie lokalnej zmiennej, nie zdrowia jacka
        }
    }
}
