using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningController : MonoBehaviour
{
    public GameObject OpeningLogo;
    public GameObject OpeningBack;
    public GameObject GamePreStartButton;
    public Button GamePreStart;
    public GameObject GameStartButton;
    public Button GameStart;
    
    public AudioSource audioSource;//オーディオソースは透明なゲームオブジェクトについてる。
    public AudioClip StartBGM;//BGM用のpublic変数

    void Start(){
        Invoke("StartButton", 2f);
        audioSource.clip = StartBGM;
        audioSource.Play();
    }

    public void StartButton(){
        GamePreStartButton.SetActive(true);
    }

    public AudioClip selectSound;//ボタン選択時の音
    public void StartSwitch1(){
        audioSource.PlayOneShot(selectSound);
        GamePreStartButton.SetActive(false);
        GameStartButton.SetActive(true);
        Invoke("StartSwitch2",0.25f);
    }

    public void StartSwitch2(){

    }

    /*void Update()
    {
        
    }*/
}
