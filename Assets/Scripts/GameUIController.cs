using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance;

    [Header("References")]
    public TextMeshProUGUI clipAmmoCount;
    public TextMeshProUGUI spareAmmoCount;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public static void UpdateClipAmmo(int value)
    {
        Instance.clipAmmoCount.text = value.ToString();
    }
    public static void UpdateSpareAmmo(int value)
    {
        Instance.spareAmmoCount.text = value.ToString();
    }

}
