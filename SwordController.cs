using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa obsługująca miecz postaci
public class SwordController : MonoBehaviour
{
    private GameObject sword;
    private Jack jack;
    // Start is called before the first frame update
    void Start()
    {
        sword = transform.Find("Sword").gameObject;
        jack = transform.parent.GetComponent<Jack>();
        sword.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //obrócenie miecza zależnie od kierunku, w którym patrzy postać
        transform.rotation = Quaternion.Euler(0, 0, 90 * jack.facing);
        sword.SetActive(jack.mode == Jack.eMode.attack); // aktywny tylko podczas ataku
        
    }
}
