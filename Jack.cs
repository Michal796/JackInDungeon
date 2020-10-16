using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa odpowiedzialna za głównego bohatera gry
public class Jack : MonoBehaviour, IFacingMover, IKeyMaster
{
    public enum eMode { idle, move, attack, transition, knockback }

    [Header("definiowanie ręczne w panelu inspector")]
    public float speed = 5;
    public float attackDuration = 0.25f;
    public float attackDelay = 0.5f;
    public float transitionDelay = 0.5f;//opoznienie przejscia do następnego pokoju

    public int maxHealth = 10;
    public float knockbackSpeed = 10; //odrzucenie przy otrzymaniu obrazen
    public float knockbackDuration = 0.25f;
    public float invincibleDuration = 0.5f;

    private InRoom inRm;

    [Header("definiowanie dynamiczne")]
    public int dirHeld = -1; //kierunek poruszania się (0 - prawo, 1 - góra, 2 - lewo, 3 - dół)
    public int facing = 1; //zwrot postaci
    public eMode mode = eMode.idle;
    public int numKeys = 0;
    public bool invincible = false;
    public bool hasGrapple = false;
    public Vector3 lastSafeLoc;// położenie i facing z momentu wejścia do pomieszczenia
    public int lastSafeFacing;

    [SerializeField]
    private int _health;

    public int health
    {
        get { return _health; }
        set { _health = value; }
    }

    private float timeAtkDone = 0; //czas konca animacji ataku
    private float timeAtkNext = 0; // czas po którym można ponownie zaatakować
    private float transitionDone = 0;
    private Vector2 transitionPos;
    private float knockbackDone = 0;
    private float invincibleDone = 0;
    private Vector3 knockbackVel;

    private SpriteRenderer sRend;
    private Rigidbody rigid;
    private Animator anim;

    //na podstawie indeksu directions, ustalany jest kierunek ruchu postaci
    private Vector3[] directions = new Vector3[]
    {
        Vector3.right, Vector3.up, Vector3.left, Vector3.down
    };

    //do sterowania
    private KeyCode[] keys = new KeyCode[]
    {
        KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow
    };

    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        inRm = GetComponent<InRoom>();
        health = maxHealth;
        lastSafeLoc = transform.position;
        lastSafeFacing = facing;
    }

    //jeśli dojdzie do kolizji z obiektem isTrigger
    private void OnTriggerEnter(Collider other)
    {
        //sprawdź czy można go podnieść
        PickUp pup = other.GetComponent<PickUp>();
        if (pup == null) return;

        //uzdrów się, dodaj kluczyk lub podnieś linkę
        switch (pup.itemType)
        {
            case PickUp.eType.health:
                health = Mathf.Min(health + 1, maxHealth);
                break;
            case PickUp.eType.key:
                keyCount++;
                break;
            case PickUp.eType.grappler:
                hasGrapple = true;
                break;
        }
        Destroy(other.gameObject);
    }
    public void ResetInRoom(int healthLoss = 0)
    {
        //zresetowanie pozycji w pokoju (lasSafeLoc oraz LastSafeFacing są aktualizowane zawsze gdy jack zmienia pokój)
        transform.position = lastSafeLoc;
        facing = lastSafeFacing;
        health -= healthLoss;

        invincible = true;
        invincibleDone = Time.time + invincibleDuration;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //wyłącz nieśmiertelność, jeśli jej czas minął
        if (invincible && Time.time > invincibleDone) invincible = false;

        sRend.color = invincible ? Color.red : Color.white; // gdy niewrażliwy na ciosy - kolor czerwony, w przeciwnym przypadku biały

        //odrzuć jeśli trzeba
        if (mode == eMode.knockback)
        {
            rigid.velocity = knockbackVel;
            if (Time.time < knockbackDone) return;
        }
        //przechodzenie przez drzwi
        if(mode == eMode.transition)
        {
            rigid.velocity = Vector3.zero;
            anim.speed = 0;
            roomPos = transitionPos;
            if (Time.time < transitionDone) return;
            // jeśli czas przejścia minął
            mode = eMode.idle;
        }
        //ustalenie kierunku poruszania się
        dirHeld = -1;
        for (int i = 0; i<4; i++)
        {
            if (Input.GetKey(keys[i])) dirHeld = i;
        }
        //klawisz Z odpowiada za atak
        if (Input.GetKeyDown(KeyCode.Z) && Time.time >= timeAtkNext)
        {
            mode = eMode.attack;
            timeAtkDone = Time.time + attackDuration;
            timeAtkNext = Time.time + attackDelay;
        }
        if (Time.time >= timeAtkDone)
        {
            mode = eMode.idle;
        }
        if(mode != eMode.attack)
        {
            if(dirHeld == -1)
            {
                mode = eMode.idle;
            }
            else
            {
                facing = dirHeld;
                mode = eMode.move;
            }
        }

        //obsługa animacji postaci
        Vector3 vel = Vector3.zero;
        switch (mode)
        {
            case eMode.attack:
                anim.CrossFade("Jack_Attack_"+facing, 0);
                anim.speed = 0;
                break;
            case eMode.idle:
                anim.CrossFade("Jack_Walk_"+facing, 0);
                anim.speed = 0;
                break;
            case eMode.move:
                vel = directions[dirHeld];
                anim.CrossFade("Jack_Walk_"+facing, 0);
                anim.speed = 1;
                break;
        }
        rigid.velocity = vel * speed;
    }

    void LateUpdate()
    {
        //polozenie obiektu z dokladnoscia do polowy rozmiaru siatki
        Vector2 rPos = GetRoomPosOnGrid(0.5f);

        //czy postać znajduje się przy którychkolwiek drzwiach
        int doorNum;
        for(doorNum =0; doorNum < 4; doorNum++)
        {
            if(rPos == InRoom.DOORS[doorNum])
            {
                break;
            }
        }
        if (doorNum > 3 || doorNum != facing) return; //jesli nie stoi przy drzwiach lub nie jest skierowany w ich kierunku - wyjdź
        Vector2 rm = roomNum;
        // w zależności od drzwi, przejdź do poprzedniego lub następnego pokoju
        switch (doorNum)
        {
            case 0:
                rm.x += 1;
                break;
            case 1:
                rm.y += 1;
                break;
            case 2:
                rm.x -= 1;
                break;
            case 3:
                rm.y -= 1;
                break;
        }

        //upewnienie się ze pomieszczenie jest poprawnie zdefiniowane
        if (rm.x >=0 && rm.x<= InRoom.MAX_RM_X)
        {
            if(rm.y>=0 && rm.y <= InRoom.MAX_RM_Y)
            {
                roomNum = rm;
                transitionPos = InRoom.DOORS[(doorNum + 2) % 4];// sprawdzenie którymi drzwiami powinien wyjść 
                roomPos = transitionPos;
                lastSafeLoc = transform.position; //ustalenie bezpiecznej pozycji, na wypadek wpadnięcia do lawy
                lastSafeFacing = facing;
                mode = eMode.transition;
                transitionDone = Time.time + transitionDelay;
            }
        }
        
    }
    void OnCollisionEnter(Collision coll)
    {
        if (invincible) return;
        DamageEffect dEf = coll.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return;

        //zadanie obrażeń
        health -= dEf.damage;
        invincible = true;
        invincibleDone = Time.time + invincibleDuration;

        //jeśli trzeba, odrzuć postać
        if (dEf.knockback)
        {
            Vector3 delta = transform.position - coll.transform.position; //wektor kierunku
            if(Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                //odrzucenie poziome
                delta.x = (delta.x > 0) ? 1 : -1; //jesli z prawej strony wroga, odrzuc w prawo, jesli z lewej strony odrzuc w lewo
                delta.y = 0;
            }
            else
            {
                delta.x = 0;
                delta.y = (delta.y > 0) ? 1 : -1;
            }

            knockbackVel = delta * knockbackSpeed;

            mode = eMode.knockback;
            knockbackDone = Time.time + knockbackDuration;
        }
    }
    //implementacja interfejsu IFacingMover
    public int GetFacing()
    {
        return facing;
    }
    public bool moving {
        get
        { return (mode == eMode.move);
        }
    }

    public float GetSpeed()
    {
        return speed;
    }
    public float gridMult
    {
        get { return inRm.gridMult; }
    }
    public Vector2 roomPos
    {
        get { return inRm.roomPos; }
        set { inRm.roomPos = value; }
    }
    public Vector2 roomNum
    {
        get { return inRm.roomNum; }
        set { inRm.roomNum = value; }
    }
    public Vector2 GetRoomPosOnGrid(float mult = -1)
    {
        return inRm.GetRoomPosOnGrid(mult);
    }
    //implementacja interfejsu IKeyMaster
    public int keyCount
    {
        get { return numKeys; }
        set { numKeys = value; }
    }

}
