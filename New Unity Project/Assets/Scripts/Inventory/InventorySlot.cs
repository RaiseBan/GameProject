using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemData item;
    public int amount;
    public bool isEmpty = true;
    public GameObject iconGO;
    public TMP_Text itemAmountText;
    public float itemDurability;
    [SerializeField] private Image _durabilityBar;

    private void Awake()
    {
        iconGO = transform.GetChild(0).GetChild(0).gameObject;
        itemAmountText = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
    }
    public void SetIcon(Sprite icon)
    {
        iconGO.GetComponent<Image>().color = new Color(1,1,1,1);
        iconGO.GetComponent<Image>().sprite = icon; 
    }
    public void UpdateDurabilityBar()
    {
        _durabilityBar.fillAmount = itemDurability / 100;
    }
}
