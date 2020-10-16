using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa umożliwiająca otwieranie drzwi przy użyciu klucza
public class GateKeeper : MonoBehaviour
{
    //numery sprajtów przedstawiających zamkniete drzwi
    const int lockedR = 95; //prawa strona
    const int lockedUR = 81;// prawa górna strona
    const int lockedUL = 80;
    const int lockedL = 100;
    const int lockedDL = 101;
    const int lockedDR = 102;

    //numery dla drzwi otwartych
    const int openR = 48;
    const int openUR = 93;
    const int openUL = 92;
    const int openL = 51;
    const int openDL = 26;
    const int openDR = 27;

    private IKeyMaster keys;

    private void Awake()
    {
        keys = GetComponent<IKeyMaster>();
    }
    private void OnCollisionStay(Collision collision)
    {
        if (keys.keyCount < 1) return;

        //pobierz kafel, z którym zetknęła się postać
        Tile ti = collision.gameObject.GetComponent<Tile>();
        if (ti == null) return;

        int facing = keys.GetFacing();
        
        //dla podwójnych drzwi
        Tile ti2; 

        // sprawdzenie czy to kafel reprezentujący zamkniete drzwi
        switch (ti.tileNum)
        {
            case lockedR:
                if (facing != 0) return;//jesli nie patrzy na drzwi, wyjdz
                ti.SetTile(ti.x, ti.y, openR);//to samo położenie, inny numer sprajta
                break;
            case lockedUR:
                if (facing != 1) return;
                ti.SetTile(ti.x, ti.y, openUR);//to samo położenie, inny numer sprajta
                ti2 = TileCamera.TILES[ti.x - 1, ti.y];// górne i dolne drzwi składając się z dwóch sprajtów
                ti2.SetTile(ti2.x, ti2.y, openUL);
                break;
            case lockedUL:
                if (facing != 1) return;
                ti.SetTile(ti.x, ti.y, openUL);
                ti2 = TileCamera.TILES[ti.x + 1, ti.y];// górne i dolne drzwi składają się z dwóch sprajtów
                ti2.SetTile(ti2.x, ti2.y, openUR);
                break;
            case lockedL:
                if (facing != 2) return;
                ti.SetTile(ti.x, ti.y, openL);
                break;
            case lockedDL:
                if (facing != 3) return;
                ti.SetTile(ti.x, ti.y, openDL);
                ti2 = TileCamera.TILES[ti.x + 1, ti.y];
                ti2.SetTile(ti2.x, ti2.y, openDR);
                break;
            case lockedDR:
                if (facing != 3) return;
                ti.SetTile(ti.x, ti.y, openDR);
                ti2 = TileCamera.TILES[ti.x - 1, ti.y];
                ti2.SetTile(ti2.x, ti2.y, openDL);
                break;
            default:
                return;

        }
        keys.keyCount--;
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
