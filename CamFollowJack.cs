using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//kamera podążająca za postacią do kolejnych pomieszczeń
public class CamFollowJack : MonoBehaviour
{
    static public bool TRANSITIONING = false;

    [Header("definiowanie ręczne w panelu inspector")]
    public InRoom jackInRm;
    public float transTime = 0.5f;

    private Vector3 p0, p1; //położenie początkowe i docelowe kamery

    private InRoom inRm;
    private float transStart;

    private void Awake()
    {
        inRm = GetComponent<InRoom>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TRANSITIONING)
        {
            //płynny ruch kamery
            float u = (Time.time - transStart) / transTime;
            if (u >= 1)
            {
                u = 1;
                TRANSITIONING = false;
            }
            transform.position = (1 - u) * p0 + u * p1;
        }
        else
        // inicjacja ruchu kamery
        {
            if(jackInRm.roomNum != inRm.roomNum)
            {
                TransitionTo(jackInRm.roomNum);
            }
        }
        
    }
    void TransitionTo(Vector2 rm)
    {
        p0 = transform.position;
        inRm.roomNum = rm;
        p1 = transform.position + (Vector3.back * 10);
        transform.position = p0;
        transStart = Time.time;
        TRANSITIONING = true;
    }
}
