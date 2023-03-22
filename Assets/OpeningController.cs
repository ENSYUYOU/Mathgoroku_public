using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningController : MonoBehaviour
{

    public GameObject GameStartButton;
    public GameObject PlayerNumSelect;
    public TextMeshProUGUI GameStartButtonText;
    public AudioSource audioSource;//オーディオソースは透明なゲームオブジェクトについてる。
    public AudioClip StartBGM;//BGM用のpublic変数
    public AudioClip Pirororn;//BGM用のpublic変数

    IEnumerator Start(){
        audioSource.clip = StartBGM;
        audioSource.Play();
        yield return new WaitForSeconds(2f);
        GameStartButton.SetActive(true);
        
    }

   

    public AudioClip selectSound;//ボタン選択時の音
    public void StartSwitch(){
        audioSource.PlayOneShot(selectSound);
        GameStartButtonText.color = new Color(1.0f,0.0f,0.0f,1.0f);
        StartCoroutine(PlayerNumSelectOn());
        
    }

    IEnumerator PlayerNumSelectOn(){//人数選択画面ON
        yield return new WaitForSeconds(1f);
        PlayerNumSelect.SetActive(true);
    }

    public void One(){
        audioSource.PlayOneShot(Pirororn);
        GameController.PLAYERS_NUM = 1;
        StartCoroutine(LoadSugoroku());
    }

    public void Two(){
        audioSource.PlayOneShot(Pirororn);
        GameController.PLAYERS_NUM = 2;
        StartCoroutine(LoadSugoroku());
    }

    public void Three(){
        audioSource.PlayOneShot(Pirororn);
        GameController.PLAYERS_NUM = 3;
        StartCoroutine(LoadSugoroku());
    }

    public void Four(){
        audioSource.PlayOneShot(Pirororn);
        GameController.PLAYERS_NUM = 4;
        StartCoroutine(LoadSugoroku());
    }

    IEnumerator LoadSugoroku(){
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("SampleScene");
    }

  
}
