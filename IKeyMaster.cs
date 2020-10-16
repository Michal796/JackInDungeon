using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interfejs umożliwiający otwieranie drzwi
public interface IKeyMaster
{
    int keyCount { get; set; }//liczba kluczy
    int GetFacing();
}
