using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//podstawowa klasa wroga
public class Enemy : MonoBehaviour
{
    protected static Vector3[] directions = new Vector3[]
    {
        Vector3.right, Vector3.up, Vector3.left, Vector3.down
    };

    [Header("definiowanie ręczne w panelu inspekcyjnym: Enemy")]
    public float maxHealth = 1;
    public GameObject[] randomItemDrops; //tablica, na podsatwie której generowany jest losowy przedmiot, wypadający z wroga po jego pokonaniu
    public float knockbackSpeed = 10;
    public float knockbackDuration = 0.25f;
    public float invincibleDuration = 0.5f;
    public GameObject guaranteedItemDrop = null; //upuszczenie tego przedmiotu przez pokonanego wroga jest pewne

    [Header("definiowanie dynamiczne: Enemy")]
    public float health;
    public bool invincible = false;
    public bool knockback = false;

    private float invincibleDone = 0; //czas zakończenia niewrażliwości oraz odrzutu
    private float knockbackDone = 0;
    private Vector3 knockbackVel;

    protected Animator anim;
    protected Rigidbody rigid;
    protected SpriteRenderer sRend;

    protected virtual void Awake()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        sRend = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (invincible && Time.time > invincibleDone) invincible = false;
        //postać w stanie nieśmiertelności zmienia kolor
        sRend.color = invincible ? Color.red : Color.white;

        //odrzuć postać
        if (knockback)
        {
            rigid.velocity = knockbackVel;
            if (Time.time < knockbackDone) return;
        }
        anim.speed = 1;
        knockback = false;
    }
    private void OnTriggerEnter(Collider colld)
    {
        if (invincible) return;
        DamageEffect dEf = colld.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return;

        health -= dEf.damage;
        if (health <= 0) Die();

        invincible = true;
        invincibleDone = Time.time + invincibleDuration;

        if (dEf.knockback)
        {
            Vector3 delta = transform.position - colld.transform.root.position;
            if(Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                delta.x = (delta.x > 0) ? 1 : -1;
                delta.y = 0; //odrzucenie w bok
            }
            else
            {
                delta.x = 0; //odrzucenie "w górę" lub "w dół"
                delta.y = (delta.y > 0) ? 1 : -1;
            }
            knockbackVel = delta * knockbackSpeed;
            rigid.velocity = knockbackVel;

            knockback = true;
            knockbackDone = Time.time + knockbackDuration;
            anim.speed = 0;
        }
    }
    void Die()
    {
        Destroy(gameObject);
        GameObject go;
        //upuść przedmiot gwarantowany lub losowy
        if (guaranteedItemDrop != null)
        {
            go = Instantiate<GameObject>(guaranteedItemDrop);
            go.transform.position = transform.position;
        }
        else if (randomItemDrops.Length > 0)
        {
            int n = Random.Range(0, randomItemDrops.Length);
            GameObject prefab = randomItemDrops[n];
            if(prefab != null)
            {
                go = Instantiate<GameObject>(prefab);
                go.transform.position = transform.position;
            }
        }
    }
}
