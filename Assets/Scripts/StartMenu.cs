using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class StartMenu : MonoBehaviour
{

    public Animator crossFade;
    public void StartSimulation()
    {
        Debug.Log("Start Called");
        StartCoroutine(ActualStart());
    }

    public IEnumerator ActualStart()
    {
        crossFade.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    private void Start()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
        //EditorApplication.isPlaying = false;
    }
}
