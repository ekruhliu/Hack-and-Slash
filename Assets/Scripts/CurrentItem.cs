using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CurrentItem : MonoBehaviour, IPointerClickHandler
{
    public int i;
    public Inventory inventory;
    public GameObject Player;
    public Text description;
    public Text rarity;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button.Equals(PointerEventData.InputButton.Left))
        {
            Player.GetComponent<PlayerController>().currentWeapon = inventory.item[i].type;
            for (int i = 0; i < inventory.item.Count; i++)
                inventory.item[i].isCurreunt = false;
            inventory.item[i].isCurreunt = true;
            rarity.text = "  Rarity: " + inventory.item[i].Rarity;
            if (inventory.item[i].Rarity.Equals("Rare"))
                rarity.color = Color.blue;
            else if (inventory.item[i].Rarity.Equals("Legendary"))
                rarity.color = new Color(255,100,0,255);
            else
                rarity.color = new Color(255,255,255,255);
            description.text = " Damage: " + inventory.item[i].Damage + "\n Attack Speed: " +
                                   inventory.item[i].AttackSpeed;
        }
        if (eventData.button.Equals(PointerEventData.InputButton.Right))
        {
            GameObject newItem = Instantiate(inventory.item[i].prefub, Player.transform.position, Quaternion.identity);
            if (inventory.item[i].countItem > 1)
                inventory.item[i].countItem--;
            else
                inventory.item[i] = new Item();
            inventory.DisplayItems();
            Player.GetComponent<PlayerController>().currentWeapon = 0;
        }
    }
}
