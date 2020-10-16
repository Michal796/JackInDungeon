using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ta klasa sprawia, że obiekty korzystające z interfejsu IFacingMover poruszają się po "siatce",
//co umożliwia bardziej precyzyjne przechodzenie przez drzwi
public class GridMove : MonoBehaviour
{
    private IFacingMover mover;

    private void Awake()
    {
        mover = GetComponent<IFacingMover>();
    }
    private void FixedUpdate()
    {
        if (!mover.moving) return;
        //określenie kierunku w celu dopasowania do siatki
        int facing = mover.GetFacing();

        Vector2 rPos = mover.roomPos;
        Vector2 rPosGrid = mover.GetRoomPosOnGrid();

        float delta = 0;
        //jeśli postać kieruje się w prawo lub w lewo, należy dopasować jej położenie w osi Y
        if (facing == 0 || facing == 2)
        {
            delta = rPosGrid.y - rPos.y;
        }
        else
        //w przeciwnym wypadku należy określić położenie na osi X
        {
            delta = rPosGrid.x - rPos.x;
        }
        if (delta == 0) return;
        //move - odległość, jaką pokona postać w jednej klatce
        float move = mover.GetSpeed() * Time.fixedDeltaTime;
        //jeśli delta jest mniejsza od wartości move, przesuń postać tylko o wartość delta
        move = Mathf.Min(move, Mathf.Abs(delta));

        if (delta < 0) move = -move;

        if (facing == 0 ||facing == 2)
        {
            rPos.y += move;
        }
        else
        {
            rPos.x += move;
        }
        mover.roomPos = rPos;
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
