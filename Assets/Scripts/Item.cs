using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string nameItem;
    public int id;
    public int countItem;
    public bool isStack;
    [Multiline(5)]
    public string descriptionItem;
    public Sprite Icon;
    public GameObject prefub;
    public int type; // 0 -sword 1 - axe 2- dagger
    public bool isCurreunt;
    
    public int Damage;
    public float AttackSpeed;
    public string Rarity;
}
