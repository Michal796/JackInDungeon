using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ta klasa odpowiada za linkę która umożliwia graczowi przyciąganie się do ścian, co pozwala mu
//ominąć pola lawy przez które normalnie nie może przejść
public class Grapple : MonoBehaviour
{
    //możliwe stany linki
    public enum eMode { none, gOut, gInMiss, gInHit}

    [Header("Definiowanie ręczne w panelu inspector")]
    public float grappleSpd = 10;
    public float grappleLength = 7;
    public float grappleInLength = 0.5f;
    public int unsafeTileHealthPenality = 2; //gdy gracz przyciągnie się do ściany w miejscu gdzie jest lawa, otrzyma karę do punktów zdrowia,
    //a także nastąpi zresetowanie jego pozycji w pokoju
    public TextAsset mapGrappleable;

    [Header("definiowanie dynamiczne")]
    public eMode mode = eMode.none;
    public List<int> grappleTiles; //numer sprajtów z którymi może się zderzyć hak
    public List<int> unsafeTiles; //numery sprajtów z niebezpiecznymi sprajtami

    private Jack jack;
    private Rigidbody rigid;
    private Animator anim;
    private Collider jackColld;

    private GameObject grapHead;
    private LineRenderer grapLine;
    private Vector3 p0, p1;
    private int facing;

    private Vector3[] directions = new Vector3[]
    {
        Vector3.right, Vector3.up, Vector3.left, Vector3.down
    };
    private void Awake()
    {
        //inicjalizacja podstawowych zmiennych
        string gTiles = mapGrappleable.text;
        gTiles = Utils.RemoveLineEndings(gTiles);
        grappleTiles = new List<int>();
        unsafeTiles = new List<int>();
        for(int i=0; i<gTiles.Length; i++)
        {
            switch (gTiles[i])
            {
                case 'S':
                    grappleTiles.Add(i);
                    break;
                case 'X':
                    unsafeTiles.Add(i);
                    break;
            }
        }
        jack = GetComponent<Jack>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        jackColld = GetComponent<Collider>();

        Transform trans = transform.Find("Grappler");
        grapHead = trans.gameObject;
        grapLine = grapHead.GetComponent<LineRenderer>();
        grapHead.SetActive(false); //hak widać tylko wtedy gdy postać użyje linki
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!jack.hasGrapple) return;

        switch (mode)
        {
            case eMode.none:
                if (Input.GetKeyDown(KeyCode.X)) //naciśnięcie X spowoduje użycie linki
                {
                    StartGrapple();
                }
                break;
        }
    }
    void StartGrapple()
    {
        facing = jack.GetFacing();
        jack.enabled = false; //blokowanie kontroli nad jackiem
        anim.CrossFade("Dray_Attack_" + facing, 0);
        jackColld.enabled = false; //aby żaden obiekt nie zatrzymał jacka
        rigid.velocity = Vector3.zero;

        grapHead.SetActive(true);

        p0 = transform.position + (directions[facing] * 0.5f); //położenie początkowe linki
        p1 = p0;
        grapHead.transform.position = p1;
        grapHead.transform.rotation = Quaternion.Euler(0, 0, 90 * facing);

        grapLine.positionCount = 2;
        grapLine.SetPosition(0, p0); //ustawienie wartości obu punktów (w tym momencie to ta sama pozycja)
        grapLine.SetPosition(1, p1);
        mode = eMode.gOut;
    }

    private void FixedUpdate()
    {
        switch (mode)
        {
            case eMode.gOut:
                p1 += directions[facing] * grappleSpd * Time.fixedDeltaTime;
                grapHead.transform.position = p1;
                grapLine.SetPosition(1, p1); //aktualizacja linii

                //czy hak w cos trafil
                int tileNum = TileCamera.GET_MAP(p1.x, p1.y); //numer kafla który został trafiony
                if(grappleTiles.IndexOf(tileNum) != -1) //jeśli numer trafionego kafla zawiera się w liście grappleTiles - można się przyciągnąć
                {
                    mode = eMode.gInHit;
                    break;
                }
                if((p1-p0).magnitude>= grappleLength) //jesli hak osiagnął maksymalną dlugość i nie znalazł odpowiedniego kafla
                {
                    mode = eMode.gInMiss;
                }
                break;
            case eMode.gInMiss:
                p1 -= directions[facing] * 2 * grappleSpd * Time.fixedDeltaTime; //wraca z podwójną prędkością
                if (Vector3.Dot((p1 - p0), directions[facing]) > 0) //dopóki hak znajduje się przed jackiem (iloczyn skalarny)
                {
                    //zaktualizuj położenie haka
                    grapHead.transform.position = p1;
                    grapLine.SetPosition(1, p1);
                }
                else
                {
                    StopGrapple();
                }
                break;
            case eMode.gInHit:
                float dist = grappleInLength + grappleSpd * Time.fixedDeltaTime; //grappleInLenght jest wymiarem haka, a nie zasięgiem linki
                if(dist > (p1 - p0).magnitude) // jeśli jacek przebył dystans większy niż różnica p0-p1 - zakoncz przyciaganie
                {
                    p0 = p1 - (directions[facing] * grappleInLength); // polozenie ustawione na punkt docelowy minus wymiar haka
                    transform.position = p0;
                    StopGrapple();
                    break;
                }
                //przyciąganie 
                p0 += directions[facing] * grappleSpd * Time.fixedDeltaTime;
                transform.position = p0;
                grapLine.SetPosition(0, p0); //aktualizacja pierwszego punktu linii
                grapHead.transform.position = p1;
                break;
        }

    }
    void StopGrapple()
    {
        jack.enabled = true;
        jackColld.enabled = true;

        //czy sprajt na którym stanął jack jest niebezpieczny

        int tileNum = TileCamera.GET_MAP(p0.x, p0.y);
        if(mode == eMode.gInHit && unsafeTiles.IndexOf(tileNum) != -1) //jeśli nastąpiło przyciągnięcie do niebezpiecznego sprajtu
        {
            jack.ResetInRoom(unsafeTileHealthPenality);
        }
        grapHead.SetActive(false);
        mode = eMode.none;
    }
    private void OnTriggerEnter(Collider other)
    {
        Enemy e = other.GetComponent<Enemy>();
        if (e == null) return;
        mode = eMode.gInMiss;
    }
}

