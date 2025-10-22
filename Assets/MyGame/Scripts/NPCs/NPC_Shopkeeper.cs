using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Shopkeeper : NPC
{
    [SerializeField] private GameObject shopPanel; // Panel_NPCshop trong GameUI

    protected override void Start()
    {
        // Không cần hiển thị quest icon, nên override và không gọi base.Start()
        if (questAvailableIcon != null) questAvailableIcon.SetActive(false);
        if (questTurnInIcon != null) questTurnInIcon.SetActive(false);
    }

    protected override void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
           
            ShopDialogueManager.Instance.StartShopDialogue(npcData, this);
        }
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }
}
