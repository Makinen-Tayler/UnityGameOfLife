using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveDialog : MonoBehaviour
{
    public TMP_InputField patternName;
    public SaveLoadManager saveLoadManager;
    public void SavePattern()
    {

        EventManager.TriggerEvent("SavePattern");
        saveLoadManager.isActive = false;
        gameObject.SetActive(false);
    }

    public void QuitDialog()
    {
        saveLoadManager.isActive = false;
        gameObject.SetActive(false );
    }
}
