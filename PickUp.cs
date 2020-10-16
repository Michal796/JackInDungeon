using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa umożliwiająca podnoszenie przedmiotów
public class PickUp : MonoBehaviour
{
    public enum eType { key, health, grappler}
    public static float COLLIDER_DELAY = 0.5f;

    [Header("definiowanie ręczne w panelu inspector")]
    public eType itemType;

    private void Awake()
    {
        //opóźnienie aktywacji collidera
        GetComponent<Collider>().enabled = false;
        Invoke("Activate", COLLIDER_DELAY); //aktywacja collidera po czasie 0.5s
    }
    void Activate()
    {
        GetComponent<Collider>().enabled = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
