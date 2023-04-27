using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Shipt_Goal : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Item_Finder _item_finder; //the item finder script. used in this script to alter the on_mouse_over bool

    public void OnPointerEnter(PointerEventData eventData)
    {
        _item_finder.mouse_over_goal = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _item_finder.mouse_over_goal = false;
    }

    private void Start()
    {
        _item_finder = transform.parent.parent.GetComponent<Item_Finder>();
    }
}
