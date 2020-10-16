using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa odpowiada za każdy obiekt typu Sprite, z którego składa się mapa gry
public class Tile : MonoBehaviour
{
    [Header("Definiowanie dynamiczne")]
    public int x; //rząd sprajta
    public int y; //kolumna sprajta
    public int tileNum;

    private BoxCollider bColl;

    private void Awake()
    {
        bColl = GetComponent<BoxCollider>();
    }

    //funkcja ustawiająca obraz odpowiedniego sprajta w zadanym położeniu
    public void SetTile(int eX, int eY, int eTileNum = -1)
    {
        x = eX;
        y = eY;
        transform.localPosition = new Vector3(x, y, 0);
        //dzięki temu zapisowi, nazwa każdego obiektu tile oznacza jego położenie 
        gameObject.name = x.ToString("D3") + "x" + y.ToString("D3"); //ToString("D3) oznacza zapis dziesiętny i co najmniej 3 cyfry w zapisie
        //jeśli przekazano wartość -1, numer sprajta zostanie odczytany z mapy
        if (eTileNum == -1)
        {
            eTileNum = TileCamera.GET_MAP(x, y);
        }
        //w innym przypadku, zamień numer
        else
        {
            TileCamera.SET_MAP(x, y, eTileNum);
        }
        //przypisz numer kafla/sprajta i pobierz go z tablicy sprajtów
        tileNum = eTileNum;
        GetComponent<SpriteRenderer>().sprite = TileCamera.SPRITES[tileNum];

        SetCollider();
    }
    void SetCollider()
    {
        //przyporządkuj odpowiedni kształt Collidera, na podstawie informacji o typie collidera z pliku DeliverCollisions.txt
        bColl.enabled = true;
        char c = TileCamera.COLLISIONS[tileNum];
        switch (c)
        {
            case 'S': //cały kafelek
                bColl.center = Vector3.zero;
                bColl.size = Vector3.one;
                break;
            case 'W':  //collider w górnej części kafla
                bColl.center = new Vector3(0, 0.25f, 0);
                bColl.size = new Vector3(1, 0.5f, 1);
                break;
            case 'A':  //collider w lewej części kafla
                bColl.center = new Vector3(-0.25f, 0, 0);
                bColl.size = new Vector3(0.5f, 1, 1);
                break;
            case 'D':  //collider w prawej części kafla
                bColl.center = new Vector3(0.25f, 0, 0);
                bColl.size = new Vector3(0.5f, 1, 1);
                break;
            default:
                bColl.enabled = false; //każdy inny kafelek
                break;
        }
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
