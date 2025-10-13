using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Button openButton; 
    public Button closeButton; 

    void Start()
    {
        if (openButton != null) openButton.onClick.AddListener(OpenInventory);
        if (closeButton != null) closeButton.onClick.AddListener(CloseInventory);
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            Debug.Log("InventoryPanel set to inactive at start");
        }
    }

    public void OpenInventory()
    {
        if (inventoryPanel != null && !inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            if (openButton != null) openButton.interactable = false; 
            if (closeButton != null) closeButton.interactable = true; 
            Debug.Log("Inventory opened");
        }
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null && inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
            if (openButton != null) openButton.interactable = true; 
            if (closeButton != null) closeButton.interactable = false; 
            Debug.Log("Inventory closed");
        }
    }

}
