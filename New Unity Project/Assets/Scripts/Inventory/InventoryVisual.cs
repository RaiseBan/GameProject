using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryVisual : MonoBehaviour
{
    public GameObject UIPanel;
    public Transform inventoryPanel;
	public List<InventorySlot> slots = new List<InventorySlot>();
    public bool isOpened = false;
    public GameObject Player;
    public bool canTake;
    public int cantTake;

    private void Awake()
    {
        UIPanel.SetActive(true);
    }

    void Start()
    {
        for (int i = 0; i < inventoryPanel.childCount; i++)
		{
            if (inventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                slots.Add(inventoryPanel.GetChild(i).GetComponent<InventorySlot>());
            }
		}
        UIPanel.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpened = !isOpened;
            if (isOpened)
            {
                UIPanel.SetActive(true);
                inventoryPanel.gameObject.SetActive(true);
            }
            else
            {
                UIPanel.SetActive(false);
                inventoryPanel.gameObject.SetActive(false);
            }
        }

    }
    public void AddItem(ItemData _item, int _amount)
    {
        canTake = true;
        foreach (InventorySlot slot in slots)
        {
            if(slot.item == _item)
            {
                int canAdd = _item.maxAmount - slot.amount;
                int toAdd = Mathf.Min(canAdd, _amount);

                slot.amount += toAdd;
                _amount -= toAdd;

                slot.itemAmountText.text = slot.amount.ToString();

                if (_amount == 0) return;
            }
        }
        foreach (InventorySlot slot in slots)
        {
            if(slot.isEmpty) 
            {
                int toAdd = Mathf.Min(_item.maxAmount, _amount);

                slot.item = _item;
                slot.amount = toAdd;
                _amount -= toAdd;

                slot.itemAmountText.text = slot.amount.ToString();
                slot.isEmpty = false;
                slot.SetIcon(_item.icon);


                if (_amount == 0) return;
            }
        }
        if (_amount > 0)
        {
            canTake = false;
            cantTake = _amount;
        }
    }
}
