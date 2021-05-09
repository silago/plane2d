using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Text text;
    private bool _entered;
    public string Text
    {
        get => text.text;
        set => text.text = value;
    }
    public void SetActive(bool status) => gameObject.SetActive(status);
    public void OnPointerEnter(PointerEventData eventData)
    {
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //gameObject.SetActive(false);
    }
}
