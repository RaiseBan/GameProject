using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryVisual : MonoBehaviour
{
    public GameObject UIBG;
    public Transform inventoryPanel;
	public List<InventorySlot> slots = new List<InventorySlot>();
    public bool isOpened = false;
    public Transform quickslotPanel;
    public GameObject Player;
    public bool canTake;
    public int cantTake;

    private void Awake()
    {
        UIBG.SetActive(true);
    }

    void Start()
    {
        slots.AddRange(inventoryPanel.GetComponentsInChildren<InventorySlot>());
        for (int i = 0; i < quickslotPanel.childCount; i++)
        {
            if (quickslotPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                slots.Add(quickslotPanel.GetChild(i).GetComponent<InventorySlot>());
            }
        }
        UIBG.SetActive(false);
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
                UIBG.SetActive(true);
                inventoryPanel.gameObject.SetActive(true);
            }
            else
            {
                UIBG.SetActive(false);
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
