using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameUpdater : MonoBehaviour
{
    public CustomText customText;
    private string lastName = "";
    private void Update()
    {
        if (lastName != Database.databaseStruct.playerAccount)
        {
            customText.SetString(Database.databaseStruct.playerAccount);
            lastName = Database.databaseStruct.playerAccount;
        }
    }
}
