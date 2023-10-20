using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public SaveDialog saveDialog;
    public LoadDialog loadDialog;
    public bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        saveDialog.gameObject.SetActive(false);
        loadDialog.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSaveDialog()
    {
        isActive = true;
        saveDialog.gameObject.SetActive(true);
    }

    public void ShowLoadDialog()
    {
        isActive = true;
        loadDialog.gameObject.SetActive(true);
    }
}
