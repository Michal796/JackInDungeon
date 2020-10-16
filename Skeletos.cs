using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// klasa Skeletos dziedziczy po klasie enemy. Jest to jeden z rodzajów wroga
public class Skeletos : Enemy, IFacingMover
{
    [Header("definiowanie ręczne w panelu inspector: Skeletos")]
    public int speed = 2;
    public float timeThinkMin = 1f; //czas podjęcia decyzji o zmianie kierunku przez obiekt od 1 do 4 s
    public float timeThinkMax = 4f;
    [Header("definiowanie dynamiczne: Skeletos")]
    public int facing = 0;
    public float timeNextDecisions = 0;
    private InRoom inRm;

    protected override void Awake()
    {
        base.Awake();
        inRm = GetComponent<InRoom>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    //losowy kierunek poruszania się
    void DecideDirection()
    {
        facing = Random.Range(0, 4);
        timeNextDecisions = Time.time + Random.Range(timeThinkMin, timeThinkMax);
    }
    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        if (knockback) return;
        //zmiana kierunku poruszania się
        if (Time.time >= timeNextDecisions)
        {
            DecideDirection();
        }
        rigid.velocity = directions[facing] * speed;
    }

    //implementacja interfejsu IFancingMover
    public int GetFacing()
    {
        return facing;
    }
    public bool moving
    {
        get
        {
            return true;
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
}
