using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Button lureButton;
    [SerializeField] UnityEngine.UI.Button totemButton;
    [SerializeField] UnityEngine.UI.Button branchButton;
    [SerializeField] UnityEngine.UI.Text lureText;
    [SerializeField] UnityEngine.UI.Text totemText;
    [SerializeField] UnityEngine.UI.Text branchText;

    private void Awake()
    {
        UpdateValues(0, 0, 0);
    }

    public void UpdateValues(int lure, int branch, int totem)
    {
        lureButton.gameObject.SetActive(lure > 0);
        branchButton.gameObject.SetActive(branch > 0);
        totemButton.gameObject.SetActive(totem > 0);

        lureText.text = lure > 0 ? lure.ToString() : "";
        branchText.text = branch > 0 ? branch.ToString() : "";
        totemText.text = totem > 0 ? totem.ToString() : "";
    }
}
