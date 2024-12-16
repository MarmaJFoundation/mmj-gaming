using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionUpdater : MonoBehaviour
{
    public NearHelper nearHelper;
    public CustomText customText;
    private string lastName = "";
    private void Update()
    {
        if (lastName != Application.version)
        {
            if (nearHelper.Testnet)
            {
                customText.SetString($"Testnet Version: {Application.version}");
            }
            else
            {
                customText.SetString($"Mainnet Beta V. {Application.version}");
            }
            lastName = Application.version;
        }
    }
}
