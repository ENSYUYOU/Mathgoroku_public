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
    public static int reverse = 1;//リバースフラグ。-1の時逆順になる
    public static int[] skip = new int[] {0, 0, 0, 0};//それぞれの人のスキップフラグ

    string[] itemlist = new string[] {"SKIPカード", "リバースカード", "保険証", "更地カード", "指定マスカード"};
    static int[] used = new int[] {1, 1, 1, 1, 1};//使ったアイテムは???から実際の名前にする

    public GameController gamecontroller;
  

    // Update is called once per frame

    void Update()
    {   
        int players_turn = GameController.players_turn;
        if(used[0]!=0)item1.text = itemlist[0] + "×" + GameController.players_item[players_turn, 0].ToString();
        if(used[1]!=0)item2.text = itemlist[1] + "×" + GameController.players_item[players_turn, 1].ToString();
        if(used[2]!=0)item3.text = itemlist[2] + "×" + GameController.players_item[players_turn, 2].ToString();
        if(used[3]!=0)item4.text = itemlist[3] + "×" + GameController.players_item[players_turn, 3].ToString();
        if(used[4]!=0)item5.text = itemlist[4] + "×" + GameController.players_item[players_turn, 4].ToString();
    }
    
    public void ShiteiMasu(){//指定マスカード
        gamecontroller.Walk(3,0,0);
    }


    public static int[] skip_flg = new int[] {0, 1, 0, 0};
    public void Skip(){//Skipカード
        int next_player = GameController.players_turn;
        next_player += reverse;
        next_player %= GameController.PLAYERS_NUM;
        skip_flg[next_player] = 1;//スキップフラグを1に
    }

    public void Reverse(){//リバースカード
        reverse *= -1;
    }

    public void Sarachi(){//更地カード

    }

    public void HokenSyo(){

    }

    public GameObject ItemPanel;
    public void ItemPanelActive(){
        if(ItemPanel.activeSelf) ItemPanel.SetActive(false);
        else ItemPanel.SetActive(true);
    }
}
