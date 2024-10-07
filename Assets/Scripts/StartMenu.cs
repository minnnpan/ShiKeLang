using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayableDirector startCutScene;
    public GameObject CreditPageObj;
    public void StartButton()
    {
        startCutScene.Play();
    }

    public void CreditButton()
    {
        CreditPageObj.SetActive(true);
    }

    public void CreditPageBackButton()
    {
        CreditPageObj.SetActive(false);
    }

    public void TutorialOKButton()
    {
        SceneManager.LoadScene("dev_test");
    }
}
