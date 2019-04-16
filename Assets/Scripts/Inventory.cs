using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Item> item;
    public GameObject cells;

    public Text description;

    public PlayerController playerController;
    
    public Text rarity;

    private void Start()
    {
        item = new List<Item>();
        cells.SetActive(false);
        description.gameObject.SetActive(false);
        for (int i = 0; i < cells.transform.childCount; i++)
            item.Add(new Item());
        for (int i = 0; i < cells.transform.childCount; i++)
            cells.transform.GetChild(i).GetComponent<CurrentItem>().i = i;
        rarity.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            if (cells.activeSelf)
            {
                cells.SetActive(false);
                description.gameObject.SetActive(false);
                playerController.enabled = true;
                Time.timeScale = 1;
                rarity.gameObject.SetActive(false);
            }
            else
            {
                cells.SetActive(true);
                description.gameObject.SetActive(true);
                playerController.enabled = false;
                Time.timeScale = 0;
                rarity.gameObject.SetActive(true);
            }
        }
    }

    public void DisplayItems()
    {
        for (int i = 0; i < item.Count; i++)
        {
            Transform cell = cells.transform.GetChild(i);
            Transform icon = cell.GetChild(0);
            Image img = icon.GetComponent<Image>();
            if (!item[i].isStack)
                icon.GetComponentInChildren<Text>().text = null;
            if (item[i].id != 0)
            {
                img.gameObject.SetActive(true);
                img.sprite = item[i].Icon;
            }
            else
            {
                img.gameObject.SetActive(false);
                img.sprite = null;
            }
        }
    }
}
