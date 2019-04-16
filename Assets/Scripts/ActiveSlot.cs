using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActiveSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private void OnDisable(){GetComponent<Image>().color = new Color(255,255,255,255);}

    public void OnPointerEnter(PointerEventData eventData){GetComponent<Image>().color = Color.yellow;}
    
    public void OnPointerExit(PointerEventData eventData){GetComponent<Image>().color = new Color(255,255,255,255);}
}
