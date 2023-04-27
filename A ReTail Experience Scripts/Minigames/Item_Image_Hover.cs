using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item_Image_Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator anim; // the animator for the image UI object

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered");
        anim.SetBool("isHovering", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Exited");
        anim.SetBool("isHovering", false);
    }
}
