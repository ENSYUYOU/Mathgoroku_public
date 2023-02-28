using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;//ランダム変数用
using TMPro;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public TextMeshProUGUI item1;
    public TextMeshProUGUI item2;
    public TextMeshProUGUI item3;
    public TextMeshProUGUI item4;
    public TextMeshProUGUI item5;
    public int reverse;

    string[] itemlist = new string[] {"SKIPカード", "リバースカード", "保険証", "更地カード", "指定マスカード"};
    static int[] used = new int[] {1, 1, 1, 1, 1};

    public GameController gameobject;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("hello2");
        gameobject = GameObject.Find("GameMaster").GetComponent<GameController>();
        int players_turn = GameController.players_turn;
        if(used[0]!=0)item1.text = itemlist[0] + "×" + GameController.players_item[players_turn, 0].ToString();
        if(used[1]!=0)item2.text = itemlist[1] + "×" + GameController.players_item[players_turn, 1].ToString();
        if(used[2]!=0)item3.text = itemlist[2] + "×" + GameController.players_item[players_turn, 2].ToString();
        if(used[3]!=0)item4.text = itemlist[3] + "×" + GameController.players_item[players_turn, 3].ToString();
        if(used[4]!=0)item5.text = itemlist[4] + "×" + GameController.players_item[players_turn, 4].ToString();

    }

    // Update is called once per frame
    float currentTime = 0f;
    void Update()
    {
        //currentTime += Time.deltaTime;
        //if(currentTime>0.1f) ShiteiMasu();
    }
    
    public void ShiteiMasu(){//指定マスカード
        gameobject.Walk(3,0,0);
    }

    public void Skip(){//Skipカード
        GameController.players_turn += 1;
        GameController.players_turn %= GameController.PLAYERS_NUM;
    }

    public void Reverse(){//リバースカード
        reverse ^= 1;
    }

    public void Sarachi(){//更地カード

    }

    public void HokenSyo(){
        
    }
}
