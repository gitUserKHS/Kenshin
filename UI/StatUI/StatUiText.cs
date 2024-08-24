using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatUiText : MonoBehaviour
{
    [SerializeField] TMP_Text dmgText;
    [SerializeField] TMP_Text defText;
    [SerializeField] TMP_Text hpText;

    PlayerController playerController;

    private void Start()
    {
        playerController = Managers.Game.GetPlayer().GetComponent<PlayerController>();
    }

    private void Update()
    {
        dmgText.text = $"Damage: {playerController.Stat.Attack + playerController.AdditionalDmg}";
        defText.text = $"Defense: {playerController.Stat.Defense + playerController.AdditionalDef}";
        hpText.text = $"Hp: {playerController.Stat.Hp}";
    }
}
