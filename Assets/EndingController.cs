using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;//max, min

public class EndingController : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    GameObject Panel1st;
    GameObject Panel2nd;
    GameObject Panel3rd;
    GameObject Panel4th;
    public GameObject Result;
    List<GameObject> JuniPanels;
    // Start is called before the first frame update
    public AudioSource SoundEffect;//オーディオソースは透明なゲームオブジェクトについてる。
    public AudioClip Don;
    public AudioClip Hue;
    public AudioClip Kansei;

    public Animator orange;
    public Animator red;
    public Animator blue;
    public Animator yellow;

    public TextMeshProUGUI MedalNum1;
    public TextMeshProUGUI MedalNum2;
    public TextMeshProUGUI MedalNum3;
    public TextMeshProUGUI MedalNum4;
    List<TextMeshProUGUI> MedalList;

    public TextMeshProUGUI CoinNum1;
    public TextMeshProUGUI CoinNum2;
    public TextMeshProUGUI CoinNum3;
    public TextMeshProUGUI CoinNum4;
    List<TextMeshProUGUI> CoinList;
    int[] juni_list= new int[] {10000000, 10000000, 10000000, 10000000};//インデックス版目のプレイヤーが何位であるか
    int[] juni_list_last= new int[] {10000000, 10000000, 10000000, 10000000};//インデックス版目のプレイヤーが何位であるか
    int[] points = new int[] {-1, -1, -1, -1};
    int[] sorted_points = new int[] {-1, -1, -1, -1};
    void JuniKettei(){
        for(int i=0; i<GameController.PLAYERS_NUM; i++){
            points[i] = 1000*GameController.players_medal[i] + GameController.players_coin[i];
            sorted_points[i] = 1000*GameController.players_medal[i] + GameController.players_coin[i];
        }
        Array.Sort(sorted_points);
        Array.Reverse(sorted_points);
        int before = sorted_points[0];
        int juni = 1;
      
        for(int i=0; i<GameController.PLAYERS_NUM; i++){//ソートしたものを上から
            for(int j=0; j<GameController.PLAYERS_NUM; j++){//全員のポイントを走査
                if(sorted_points[i]==points[j]){
                    if(sorted_points[i]!=before)juni+=1;
                    juni_list[j] = juni;
                    juni_list_last[j] = juni;
                    before = sorted_points[i];
                }
            }
        }
    }


    IEnumerator Start()
    {
        JuniPanels = new List<GameObject>() {Panel1st, Panel2nd, Panel3rd, Panel4th};
        MedalList = new List<TextMeshProUGUI>() {MedalNum1, MedalNum2, MedalNum3, MedalNum4};
        CoinList = new List<TextMeshProUGUI>() {CoinNum1, CoinNum2, CoinNum3, CoinNum4};
        Panel1st = GameObject.Find("1stPanel");
        Panel2nd = GameObject.Find("2ndPanel");
        Panel3rd = GameObject.Find("3rdPanel");
        Panel4th = GameObject.Find("4thPanel");
        Panel1st.SetActive(false);
        Panel2nd.SetActive(false);
        Panel3rd.SetActive(false);
        Panel4th.SetActive(false);
        JuniKettei();
        
        
        Debug.Log(sorted_points[0]);
        Debug.Log(sorted_points[1]);
        Debug.Log(sorted_points[2]);
        Debug.Log(sorted_points[3]);

        Debug.Log(juni_list[0]);
        Debug.Log(juni_list[1]);
        Debug.Log(juni_list[2]);
        Debug.Log(juni_list[3]);
        for(int i=0; i<GameController.PLAYERS_NUM; i++){//何枚目のパネルに記入するか
            for(int j=0; j<GameController.PLAYERS_NUM; j++){//それぞれのプレイヤーを走査
                if(juni_list[j] == juni_list.Min()){
                    MedalList[i].text = GameController.players_medal[j].ToString();
                    CoinList[i].text = GameController.players_coin[j].ToString();
                    juni_list[j] = 100;
                    break;
                }
            }
        }
        Debug.Log("---");
         Debug.Log(juni_list[0]);
        Debug.Log(juni_list[1]);
        Debug.Log(juni_list[2]);
        Debug.Log(juni_list[3]);
        SoundEffect.PlayOneShot(Hue);
        if(GameController.PLAYERS_NUM >=4){
            yield return new WaitForSeconds(1f);
            Panel4th.SetActive(true);
            SoundEffect.PlayOneShot(Don);
        }
        if(GameController.PLAYERS_NUM >=3){
            yield return new WaitForSeconds(1f);
            Panel3rd.SetActive(true);
            SoundEffect.PlayOneShot(Don);
        }
        if(GameController.PLAYERS_NUM >=2){
            yield return new WaitForSeconds(1f);
            Panel2nd.SetActive(true);
            SoundEffect.PlayOneShot(Don);
        }
        if(GameController.PLAYERS_NUM >=1){
            yield return new WaitForSeconds(1f);
            Panel1st.SetActive(true);
            SoundEffect.PlayOneShot(Don);
        }
        yield return new WaitForSeconds(1f);
        Result.SetActive(false);
        SoundEffect.PlayOneShot(Kansei);//いえーい

        if(juni_list_last[0]==1)orange.SetBool("cheer", true);
        if(juni_list_last[1]==1)red.SetBool("fox_red_cheer", true);
        if(juni_list_last[2]==1)blue.SetBool("fox_blue_cheer", true);
        if(juni_list_last[3]==1)yellow.SetBool("fox_yellow_cheer", true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            Vector3 pos = Input.mousePosition;  
            Debug.Log(pos);
        }
    }
}
