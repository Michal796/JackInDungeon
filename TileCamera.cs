using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa umożliwiająca podmianę kafelków, w celu wygodnego projektowania poziomów
//przykładowo: obrazek przedstawiający szkieleta w danym miejscu zostanie zamieniony na obrazek zwykłego podłoża 
//oraz instancję obiektu szkieleta
//definiowane ręcznie w panelu inspector
[System.Serializable]
public class TileSwap
{
    public int tileNum; //numer kafla do podmiany
    public GameObject swapPrefab;
    public GameObject guaranteedItemDrop;
    public int overrideTileNum = -1;
}
//klasa jest odpowiedzialna za przetwarzanie i przechowywanie sprajtów z pliku graficznego DeliverTiles
public class TileCamera : MonoBehaviour
{
    static private int W, H;
    static private int[,] MAP;
    static public Sprite[] SPRITES;
    static public Transform TILE_ANCHOR;
    static public Tile[,] TILES;
    static public string COLLISIONS;

    [Header("Definiowanie ręczne w panelu inspector")]
    public TextAsset mapData;
    public Texture2D mapTiles;
    public TextAsset mapCollisions;
    public Tile tilePrefab;
    public int defaultTileNum; // domyślny numer kafla czyli zwykła płytka podłogowa
    public List<TileSwap> tileSwaps; //lista kafli do podmiany

    private Dictionary<int, TileSwap> tileSwapDict; //słownik zwraca obiekt TileSwap na podstawie wartości liczbowej całkowitej
    private Transform enemyAnchor, itemAnchor;

    private void Awake()
    {
        PrepareTileSwapDict();
        enemyAnchor = (new GameObject("Enemy_Anchor")).transform;
        itemAnchor = (new GameObject("Item_Anchor")).transform;
        COLLISIONS = Utils.RemoveLineEndings(mapCollisions.text); //funkcja zamieniająca text z pliku text asset na 256-znakowy łańcuch
        //każemu sprajtowi z tablicy SPRITES[] zostanie przyporządkowany collider o odpowiednim kształcie
        LoadMap();
    }
    public void LoadMap()
    {
        //potomek wszystkich obiektów Tile
        GameObject go = new GameObject("TILE_ANCHOR");
        TILE_ANCHOR = go.transform;
        SPRITES = Resources.LoadAll<Sprite>(mapTiles.name);//wczytaj wszystkie sprajty obiektu DeliverTiles.png

        //wczytanie danych mapy
        string[] lines = mapData.text.Split('\n');
        H = lines.Length; //wysokość mapy w kaflach
        string[] tileNums = lines[0].Split(' ');// liczba kwadratów w jednym wierszu, długość mapy w kaflach
        W = tileNums.Length;
        System.Globalization.NumberStyles hexNum;
        hexNum = System.Globalization.NumberStyles.HexNumber;// system szesnastkowy
        //definiowanie nowej tablicy dwuwymiarowej, w celu identyfikacji każdego kafla mapy
        MAP = new int[W, H];
        for(int j=0; j<H; j++)
        {
            //umieszczenie numeru danego kafla odczytanego z pliku w tablicy
            tileNums = lines[j].Split(' ');
            for (int i=0; i<W; i++)
            {
                if(tileNums[i] == "..") //ten znak oznacza pusty kafelek
                {
                    MAP[i, j] = 0;
                }
                else
                {
                    MAP[i, j] = int.Parse(tileNums[i], hexNum); //odczytanie numeru kafla, zapisanego w systemie szesnastkowym
                }
                CheckTileSwaps(i, j); //sprawdz, czy ten obrazek należy podmienić
            }
        }
        //print("Przetworzono" + SPRITES.Length + " sprajtów");
        //print("roz" + W +"szer"+H+ " szer");
        ShowMap();

    }
    void PrepareTileSwapDict()
    {
        tileSwapDict = new Dictionary<int, TileSwap>();
        foreach(TileSwap ts in tileSwaps) //poszukaj kafli, które należy podmienić
        {
            tileSwapDict.Add(ts.tileNum, ts); //ich numer jest kluczem do słownika
        }
    }
    void CheckTileSwaps(int i, int j)
    {
        int tNum = GET_MAP(i, j);
        if (!tileSwapDict.ContainsKey(tNum)) return; // jesli nie jest to kafel do podmiany wyjdz

        TileSwap ts = tileSwapDict[tNum];
        //stwórz wroga jeśli powinien się tam znaleźć
        if (ts.swapPrefab != null)
        {
            GameObject go = Instantiate(ts.swapPrefab);
            Enemy e = go.GetComponent<Enemy>();
            if(e != null)
            {
                go.transform.SetParent(enemyAnchor);      
            }
            //jeśli to przedmiot
            else
            {
                go.transform.SetParent(itemAnchor);
            }
            go.transform.position = new Vector3(i, j, 0);
            //jeśli z tego wroga powinien wypaść określony przedmiot, dodaj go
            if(ts.guaranteedItemDrop != null){
                if (e != null)
                {
                    e.guaranteedItemDrop = ts.guaranteedItemDrop;
                }
            }
        }
        //jeśli to domyślny kafel podłoża
        if (ts.overrideTileNum == -1)
        {
            SET_MAP(i, j, defaultTileNum);
        }
        else
        {
            SET_MAP(i, j, ts.overrideTileNum);
        }
    }
    // zwróć numer kafla, którego trzeba użyć w miejscu o położeniu x, y
    static public int GET_MAP(int x, int y)
    {
        if (x < 0 || x >= W || y < 0 || y >= H)
        {
            return -1;
        }
        return MAP[x,y];
    }
    //przeciążenie zmiennoprzecinkowe
    static public int GET_MAP(float x, float y)
    {
        int tX = Mathf.RoundToInt(x);
        int tY = Mathf.RoundToInt(y - 0.25f);
        return GET_MAP(tX, tY);
    }
    //przypisanie numeru kafla, którego należy użyć dla miejsca o połozeniu x, y
    static public void SET_MAP(int x, int y, int tNUM)
    {
        if(x<0||x>=W || y < 0 || y >= H)
        {
            return;
        }
        MAP[x, y] = tNUM;
    }
    //wygenerowanie kafli dla całej mapy
    void ShowMap()
    {
        //tablica obiektów tile
        TILES = new Tile[W, H];
        for (int j = 0; j < H; j++)
        {
            for(int i = 0; i < W; i++)
            {
                if (MAP[i, j] != 0)
                {
                    Tile ti = Instantiate<Tile>(tilePrefab);
                    ti.transform.SetParent(TILE_ANCHOR);
                    ti.SetTile(i, j);
                    TILES[i, j] = ti;
                }
            }
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
