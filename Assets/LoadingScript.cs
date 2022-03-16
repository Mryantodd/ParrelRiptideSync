using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParrelSync;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScript : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public TextMeshProUGUI textMeshProUGUI2;
    void Start()
    {
        if (ClonesManager.IsClone())
        {
            textMeshProUGUI2.text = "Client";
        }
        else
        {
            textMeshProUGUI2.text = "Server";
        }
        StartCoroutine(ExampleCoroutine());
    }
    private IEnumerator ExampleCoroutine()
    {
        textMeshProUGUI.text += ".";
        yield return new WaitForSeconds(1.25f);
        if (ClonesManager.IsClone())
        {
            SceneManager.LoadScene("ClientMain", LoadSceneMode.Single);
        }
        else
        {
            // Automatically start server if this is the original editor
            SceneManager.LoadScene("ServerMain", LoadSceneMode.Single);
        }
    }
}
