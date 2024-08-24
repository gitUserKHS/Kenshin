using Rito.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class StatUiSlot : MonoBehaviour, IPointerClickHandler
{
    public enum SlotType
    {
        Weapon,
        Armor
    }

    [SerializeField] SlotType slotType;
    [SerializeField] ArmorType armorType;
    [SerializeField] Image IconImage;
    
    //WeaponType weaponType = WeaponType.None;

    Color defaultColor;
    bool isColorChanged = false;

    PlayerController playerController;

    void Start()
    {
        playerController = Managers.Game.GetPlayer().GetComponent<PlayerController>();
    }

    void Update()
    {
        if(playerController == null)
        {
            Debug.Log("statUiSlot: player controller is null");
            return;
        }

        switch (slotType)
        {
           case SlotType.Weapon:
                if (playerController.PlayerWeapons[(int)playerController.PlayerWeaponType] == null)
                    IconImage.sprite = null;
                else if (IconImage.sprite != playerController.PlayerWeapons[(int)playerController.PlayerWeaponType].IconSprite)
                    IconImage.sprite = playerController.PlayerWeapons[(int)playerController.PlayerWeaponType].IconSprite;
                break;
            case SlotType.Armor:
                if (playerController.PlayerArmors[(int)armorType] == null)
                {
                    IconImage.sprite = null;
                }
                else if (IconImage.sprite != playerController.PlayerArmors[(int)armorType].IconSprite)
                    IconImage.sprite = playerController.PlayerArmors[(int)armorType].IconSprite;
                break;
        }

        if (IconImage.sprite != null && !isColorChanged)
        {
            defaultColor = IconImage.color;
            IconImage.color = new Color(255, 255, 255);
            isColorChanged = true;
        }
        else if(IconImage.sprite == null && isColorChanged)
        {
            IconImage.color = defaultColor;
            isColorChanged= false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (IconImage.sprite == null)
            return;

        PlayerController pc = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        switch (slotType)
        {
            case SlotType.Weapon:
                WeaponItemData wid = pc.PlayerWeapons[(int)pc.PlayerWeaponType];
                pc.PlayerWeapons[(int)pc.PlayerWeaponType] = null;
                pc.InventoryManager.Add(wid);
                pc.PlayerWeaponType = WeaponType.None;
                break;
            case SlotType.Armor:
                ArmorItemData aid = pc.PlayerArmors[(int)armorType]; 
                pc.OnChangeArmor((int)armorType, false);
                pc.PlayerArmors[(int)armorType] = null;
                pc.InventoryManager.Add(aid);
                break;
        }
    }
}
