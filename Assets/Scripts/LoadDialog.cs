using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LoadDialog : MonoBehaviour
{

    public TMP_Dropdown patternName;
    public SaveLoadManager saveLoadManager;
    public void LoadPattern()
    {
        Debug.Log("load pattern called");
        EventManager.TriggerEvent("LoadPattern");
        saveLoadManager.isActive = false;

        gameObject.SetActive(false);
    }

    public void QuitDialog()
    {
        saveLoadManager.isActive = false;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        ReloadOptions();
    }

    private void OnEnable()
    {
        ReloadOptions();
    }
    private void ReloadOptions()
    {
        List<string> options = new List<string>();

        string[] filePaths = Directory.GetFiles(@"patterns/");

        for (int i = 0; i < filePaths.Length; i++) 
        {
            string filename = filePaths[i].Substring(filePaths[i].LastIndexOf('/') + 1);
            string extension = Path.GetExtension(filename);

            filename = filename.Substring(0, filename.Length - extension.Length);
            options.Add(filename);
        }

        patternName.ClearOptions();
        patternName.AddOptions(options);
        Debug.Log(options.Count);
    }
}
