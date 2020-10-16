using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//skrypt utrzymujący wrogów w pomieszczeniu
public class InRoom : MonoBehaviour
{
    //każdy pokój ma te same wymiary
    static public float ROOM_W = 16;
    static public float ROOM_H = 11;
    static public float WALL_T = 2; //grubość ściany

    static public int MAX_RM_X = 9;
    static public int MAX_RM_Y = 9; //GRANICE MAPY;

    static public Vector2[] DOORS = new Vector2[] //możliwe położenia drzwi w grze (w pomieszczeniu)
    {
        new Vector2(14,5),
        new Vector2(7.5f,9),
        new Vector2(1,5),
        new Vector2(7.5f,1),
     };

    [Header("definiowanie ręczne w panelu inspector")]
    public bool keepInRoom = true;
    public float gridMult = 1;

    private void LateUpdate()
    {
        if (keepInRoom)
        {
            Vector2 rPos = roomPos;
            //utrzymanie pozycji pomiędzy ścianami
            rPos.x = Mathf.Clamp(rPos.x, WALL_T, ROOM_W - 1 - WALL_T);
            rPos.y = Mathf.Clamp(rPos.y, WALL_T, ROOM_H - 1 - WALL_T);
            roomPos = rPos;
        }
    }
    //określenie lokalizacji postaci w pokoju
    public Vector2 roomPos
    {
        //zastosowanie % sprawia, że wartość roomPos nie zależy od pomieszczenia, w którym znajduje się postać
        get
        {
            Vector2 tPos = transform.position;
            tPos.x %= ROOM_W;//reszta z dzielenia
            tPos.y %= ROOM_H;
            return tPos;
        }
        set
        {
            Vector2 rm = roomNum;
            rm.x *= ROOM_W;
            rm.y *= ROOM_H;
            rm += value;
            transform.position = rm;
        }

    }
    //okreslenie pokoju, w którym znajduje się ta postać
    public Vector2 roomNum
    {
        get
        {
            Vector2 tPos = transform.position;
            tPos.x = Mathf.Floor(tPos.x / ROOM_W);
            tPos.y = Mathf.Floor(tPos.y / ROOM_H);
            return tPos;
        }
        set
        {
            Vector2 rPos = roomPos; //pozycja w pomieszczeniu
            Vector2 rm = value;
            rm.x *= ROOM_W;
            rm.y *= ROOM_H;
            transform.position = rm + rPos;           
        }
    }
    //określenie najbliższej lokalizacji siatki dla tej postaci
    public Vector2 GetRoomPosOnGrid(float mult = -1)
    {
        if (mult == -1)
        {
            mult = gridMult;
        }
        Vector2 rPos = roomPos;
        rPos /= mult;
        rPos.x = Mathf.Round(rPos.x);
        rPos.y = Mathf.Round(rPos.y);
        rPos *= mult;
        return rPos;
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
