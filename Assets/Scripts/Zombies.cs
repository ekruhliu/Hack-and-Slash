using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Zombies : MonoBehaviour
{
    NavMeshAgent agent;
    private Animator anim;

    public GameObject player;

    public PlayerController playerStats;

    private Vector3 rayPos;
    private Vector3 playerPos;

    public float distance;

    public IStats Zstats;

    private bool dead;

    public GameObject hpSphere;

    public GameObject[] Weapons;

    private void Awake()
    {
        Zstats = GetComponent<IStats>();
        playerStats = player.GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent> ();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (!gameObject.tag.Equals("Boss"))
        {
            Zstats.lvl = playerStats.stats.lvl;
            Zstats.Strengh = 7 * Zstats.lvl;
            Zstats.Agility = 7 * Zstats.lvl;
            Zstats.Constitution = 12 + Zstats.lvl * 2;
            Zstats.health = 5 * Zstats.Constitution;
            Zstats.Armor = 2;
            Zstats.minDamage = Zstats.Strengh / 2;
            Zstats.maxDamage = Zstats.minDamage + 4;
            Zstats.HitChance = 0;
            Zstats.BasicDamage = Random.Range(Zstats.minDamage, Zstats.maxDamage);
        }
        else
        {
            Zstats.Strengh = (7 * Zstats.lvl) * 5;
            Zstats.Agility = (7 * Zstats.lvl) * 5;
            Zstats.minDamage = Zstats.Strengh / 2;
            Zstats.maxDamage = Zstats.minDamage + 450;
            Zstats.HitChance = 0;
            Zstats.BasicDamage = Random.Range(Zstats.minDamage, Zstats.maxDamage);
            Zstats.BasicDamage += 500;
        }
    }

    void Update()
    {
        if (dead)
            transform.Translate(Vector3.down * 1f * Time.deltaTime);
        if (!gameObject || anim.GetBool("dead"))
            return;
        rayPos = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        playerPos = new Vector3(player.transform.position.x, player.transform.position.y + 2f, player.transform.position.z);
        distance = Vector3.Distance(rayPos, playerPos);
        Debug.DrawRay(rayPos, playerPos - rayPos, Color.red);
        if (distance < 15f && distance > 2f && !anim.GetBool("dead"))
        {
            transform.LookAt(player.transform.position);
            agent.destination = player.transform.position;
            anim.SetBool("move", true);
        }
        else
            anim.SetBool("move", false);
        if (distance <= 2f && !anim.GetBool("dead"))
        {
            transform.LookAt(player.transform.position);
            anim.SetBool("move", false);
            anim.SetBool("attack", true);
            
        }
        else
            anim.SetBool("attack", false);
        if (Zstats.Armor > 150)
            Zstats.Armor = 150;
    }

    public void MinusZdorovie()
    {
        if (!gameObject)
            return;
        Zstats.health -= playerStats.stats.BasicDamage * (1 - playerStats.stats.Armor / 200);
        if (Zstats.health <= 0)
        {
            playerStats.stats.exp += 20;
            playerStats.stats.money += Random.Range(5, 21);
            anim.SetBool("move", false);
            anim.SetBool("attack", false);
            anim.SetBool("dead", true);
            Invoke("kill", 5);
        }
    }
    
    public void OffAnim()
    {
        if (!gameObject)
            return;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        anim.enabled = false;
        dead = true;
    }
    
    void kill()
    {
        if (!gameObject)
            return;
        int chance = Random.Range(0, 3);
        int type = Random.Range(0, 3);
        int chanceCommon = Random.Range(0, 11);
        int chanceRare = Random.Range(0, 21);
        int chanceLegendary = Random.Range(0, 51);
        GameObject dropWeapon;
        if (chance.Equals(1))
            Instantiate(hpSphere, new Vector3(transform.position.x, 30.5f, transform.position.z), Quaternion.identity);
        if (chanceCommon.Equals(10))
        {
            dropWeapon = Instantiate(Weapons[type], new Vector3(transform.position.x, 30.5f, transform.position.z), Quaternion.identity);
            dropWeapon.GetComponent<Item>().Damage = Weapons[type].GetComponent<Item>().Damage + playerStats.stats.lvl * 2;
            dropWeapon.GetComponent<Item>().Rarity = "Common";
        }
        else if (chanceRare.Equals(20))
        {
            dropWeapon = Instantiate(Weapons[type], new Vector3(transform.position.x, 30.5f, transform.position.z), Quaternion.identity);
            dropWeapon.GetComponent<Item>().Damage = (Weapons[type].GetComponent<Item>().Damage + playerStats.stats.lvl * 2) * 2;
            dropWeapon.GetComponent<Item>().Rarity = "Rare";
        }
        else if (chanceLegendary.Equals(50))
        {
            dropWeapon = Instantiate(Weapons[type], new Vector3(transform.position.x, 30.5f, transform.position.z), Quaternion.identity);
            dropWeapon.GetComponent<Item>().Damage = (Weapons[type].GetComponent<Item>().Damage + 50) + playerStats.stats.lvl * 10;
            dropWeapon.GetComponent<Item>().Rarity = "Legendary";
        }
        Destroy(gameObject);
    }

    public void damage()
    {
        if (!gameObject)
            return;
        playerStats.stats.health -= Zstats.BasicDamage * (1 - playerStats.stats.Armor / 200);
    }
}
