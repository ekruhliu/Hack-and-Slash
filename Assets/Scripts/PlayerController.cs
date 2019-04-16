using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    
    public Slider enemySlider;
    public Slider hpValue;
    public Slider expValue;
    
    public Text lvltext;
    public Text hpText;
    public Text enemyText;
    public Text die;
    public Text moneyText;

    public GameObject charPanel;
    public Text levelNpoints;
    public Text goldPanel;
    public Text strText;
    public Text agiText;
    public Text conText;
    public Text armorText;
    public Text minDmgText;
    public Text maxDmgText;
    public Text xpText;

    public int itemNum;

    public ParticleSystem lvlUp;


    private GameObject zombie;
    public Zombies zombieStats;

    public IStats stats;
    public int levelPoints;

    public Inventory Inventory;

    public Item item;

    public GameObject[] weapons;
    public int currentWeapon;


    private void Awake()
    {
        stats = GetComponent<IStats>();
        stats.Strengh = 10;
        stats.Agility = 10;
        stats.Constitution = 20;
        stats.exp = 0;
        stats.lvl = 1;
        stats.health = 5 * stats.Constitution;
        stats.Armor = 5;
        stats.minDamage = stats.Strengh / 2;
        stats.maxDamage = stats.minDamage + 4;
        stats.money = 0;
        stats.HitChance = 0;
        stats.BasicDamage = Random.Range(stats.minDamage, stats.maxDamage);
    }

    void Start ()
    {
        currentWeapon = 0;
        itemNum = 1;
        lvlUp.Stop();
        charPanel.SetActive(false);
        levelPoints = 0;
        anim = GetComponent<Animator> ();
        agent = GetComponent<NavMeshAgent> ();
        enemyText.gameObject.SetActive(false);
        enemySlider.gameObject.SetActive(false);
        die.gameObject.SetActive(false);
        hpValue.maxValue = 5 * stats.Constitution;
        StartCoroutine(healing());
    }
    
    void Update () {
        if(Input.GetMouseButtonDown(0) && !charPanel.activeSelf && !Inventory.cells.activeSelf)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag.Equals("Enemy"))
                {
                    zombie = hit.transform.gameObject;
                    zombieStats = zombie.transform.gameObject.GetComponent<Zombies>();
                }
                else
                {
                    zombie = null;
                    zombieStats = null;
                }
                if (hit.collider.GetComponent<Item>())
                    item = hit.collider.GetComponent<Item>();
                else
                    item = null;
                agent.destination = hit.point;
                transform.LookAt(hit.point);
            }
        }

        if (Input.GetKeyUp(KeyCode.L))
            stats.exp += 100;
        if (Input.GetKeyUp(KeyCode.P))
            levelPoints += 10;
        if (Input.GetKeyUp(KeyCode.H))
            stats.health = 5 * stats.Constitution;
        TakeItem();
        ShowPanel();
        if (Input.GetKeyUp(KeyCode.C) && !charPanel.activeSelf)
        {
            charPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else if (Input.GetKeyUp(KeyCode.C) && charPanel.activeSelf)
        {
            charPanel.SetActive(false);
            Time.timeScale = 1;
        }
        Recalculate();
        Attacking();
        YouDied();
        LevelUp();
        ShowGUI();
        ActiveWeapon();
    }

    void ActiveWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(false);
        if (currentWeapon > 0)
        {
            int currItemDmg = 0;
            float currAttackSpd = 1f;
            weapons[currentWeapon].SetActive(true);
            for (int i = 0; i < Inventory.item.Count; i++)
            {
                if (Inventory.item[i].isCurreunt)
                {
                    currItemDmg = Inventory.item[i].Damage;
                    currAttackSpd = Inventory.item[i].AttackSpeed;
                    break;
                }
            }

            if (currItemDmg > 0)
            {
                weapons[currentWeapon].GetComponent<WeaponStats>().Damage = currItemDmg;
                weapons[currentWeapon].GetComponent<WeaponStats>().AttackSpeed = currAttackSpd;
            }
        }
    }

    void TakeItem()
    {
        if (item && Vector3.Distance(transform.position, item.transform.position) < 3f)
        {
            for (int i = 0; i < Inventory.item.Count; i++)
            {
                if (Inventory.item[i].id == 0)
                {
                    Inventory.item[i] = item;
                    item.id = itemNum;
                    itemNum++;
                    Inventory.DisplayItems();
                    Destroy(item.gameObject);
                    break;
                }
            }
        }
    }

    private void ShowPanel()
    {
        if (charPanel.activeSelf)
        {
            levelNpoints.text = "Level: " + stats.lvl + " / Points: " + levelPoints;
            goldPanel.text = "Gold: " + stats.money;
            strText.text = stats.Strengh.ToString();
            agiText.text = stats.Agility.ToString();
            conText.text = stats.Constitution.ToString();
            armorText.text = "Armor: " + stats.Armor;
            minDmgText.text = "Min Damage: " + stats.minDamage;
            maxDmgText.text = "Max Damage: " + stats.maxDamage;
            xpText.text = "XP: " + stats.exp + "/100";
        }
    }
    
    public void UpSTR()
    {
        if (levelPoints > 0)
        {
            stats.Strengh++;
            levelPoints--;
        }
    }
    public void UpAGI()
    {
        if (levelPoints > 0)
        {
            stats.Agility++;
            levelPoints--;
        }
    }
    public void UpCON()
    {
        if (levelPoints > 0)
        {
            stats.Constitution++;
            levelPoints--;
        }
    }
/*    public void UpMANA()
    {
        if (levelPoints > 0)
        {
            stats.Strengh++;
            levelPoints--;
        }
    }*/

    public void UbitNevernogo()
    {
        if (zombie)
            zombie.GetComponent<Zombies>().MinusZdorovie();
    }

    void Restart() { Application.LoadLevel("SampleScene"); }

    IEnumerator healing()
    {
        if (stats.health < 5 * stats.Constitution)
            stats.health += 5;
        if (stats.health > 5 * stats.Constitution)
            stats.health = 5 * stats.Constitution;
        yield return new WaitForSeconds(20f);
        StartCoroutine(healing());
    }

    void Recalculate()
    {
        hpValue.maxValue = 5 * stats.Constitution;
        if (currentWeapon > 0)
        {
            stats.minDamage = (stats.Strengh / 2) + weapons[currentWeapon].GetComponent<WeaponStats>().Damage;
            stats.maxDamage = (stats.minDamage + 4) + weapons[currentWeapon].GetComponent<WeaponStats>().Damage;
        }
        else
        {
            stats.minDamage = stats.Strengh / 2;
            stats.maxDamage = stats.minDamage + 4;
        }
        stats.BasicDamage = Random.Range(stats.minDamage, stats.maxDamage);
    }
    
    void LevelUp()
    {
        if (stats.exp == 100 && stats.lvl < 50)
        {
            lvlUp.Play();
            Invoke("OffPS", 4);
            levelPoints += 5;
            stats.health = 5 * stats.Constitution;
            stats.exp = 0;
            stats.lvl++;
        }
    }

    void OffPS() { lvlUp.Stop(); }
    
    void YouDied()
    {
        if (stats.health <= 0)
        {
            anim.SetBool("death", true);
            die.gameObject.SetActive(true);
            Invoke("Restart", 10);
        }
    }

    void Attacking()
    {
        if (zombie && zombieStats)
        {
            stats.HitChance = 75 + stats.Agility - zombieStats.Zstats.Agility;
            enemyText.gameObject.SetActive(true);
            enemyText.text = "Zombie level " + zombieStats.Zstats.lvl;
            enemySlider.gameObject.SetActive(true);
            enemySlider.maxValue = 5 * zombieStats.Zstats.Constitution;
            enemySlider.value = zombieStats.Zstats.health;
            if (zombie.GetComponent<Animator>().GetBool("dead"))
            {
                enemyText.gameObject.SetActive(false);
                enemySlider.gameObject.SetActive(false);
            }
        }
        if (zombie && Vector3.Distance(transform.position, zombie.transform.position) < 3f && !zombie.GetComponent<Animator>().GetBool("dead"))
        {
            transform.LookAt(zombie.transform.position);
            agent.velocity = Vector3.zero;
            anim.SetBool("attack", true);
            if (currentWeapon > 0)
                anim.speed = weapons[currentWeapon].GetComponent<WeaponStats>().AttackSpeed;
            else
                anim.speed = 1;
        }
        else if (zombie && !zombie.GetComponent<Animator>().GetBool("dead"))
        {
            transform.LookAt(zombie.transform.position);
            agent.destination = zombie.transform.position;
            anim.SetBool("attack", false);
            anim.speed = 1;
        }
        else
        {
            anim.SetBool("attack", false);
            anim.speed = 1;
        }
    }

    void ShowGUI()
    {
        hpText.text = stats.health.ToString();
        moneyText.text = stats.money.ToString();
        hpValue.value = stats.health;
        expValue.value = stats.exp;
        lvltext.text = "Level " + stats.lvl;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("hpSphere"))
        {
            stats.health += 30;
            if (stats.health > 5 * stats.Constitution)
                stats.health = 5 * stats.Constitution;
            Destroy(other.gameObject);
        }
    }
}
