using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;//ランダム変数用
using TMPro;
using UnityEngine.Tilemaps;//マス目を記録したタイルマップ
using UnityEngine.UI;
using System.Linq;//Containsメソッド用

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
    public TextMeshProUGUI message;//エンディング、アイテム使用時などのメッセージ
    public Tilemap tilemap;
    static List<Vector3Int> SarachiPos = new List<Vector3Int>();
    void Start(){
        for(int i=0; i<SarachiPos.Count; i++){
            tilemap.SetTile(SarachiPos[i], Gray);
        }
    }
    // Update is called once per frame

    public TileBase Gray;//選択しているタイル
    void Update()
    {   
        int players_turn = GameController.players_turn;
        if(used[0]!=0)item1.text = itemlist[0] + "×" + GameController.players_item[players_turn, 0].ToString();
        if(used[1]!=0)item2.text = itemlist[1] + "×" + GameController.players_item[players_turn, 1].ToString();
        if(used[2]!=0)item3.text = itemlist[2] + "×" + GameController.players_item[players_turn, 2].ToString();
        if(used[3]!=0)item4.text = itemlist[3] + "×" + GameController.players_item[players_turn, 3].ToString();
        if(used[4]!=0)item5.text = itemlist[4] + "×" + GameController.players_item[players_turn, 4].ToString();

        if (Input.GetMouseButtonDown(0) && sarachicount>=1 ){//更地カード関係
            Vector3 pos = Input.mousePosition;   
            Vector3Int selectCellPos = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos));
            var tile = tilemap.GetTile<Tile>(selectCellPos);
            if(tilemap.HasTile(selectCellPos) && tile.sprite.name!="castleCenter_rounded_naname"){
                tilemap.SetTile(selectCellPos, Gray);
                SarachiPos.Add(selectCellPos);
                sarachicount -= 1;
                if(sarachicount==0)StartCoroutine(Message("更地完了！"));
                else StartCoroutine(Message("残り" + sarachicount.ToString() + "マス！"));

            }
        }
    }


    public static int[] skip_flg = new int[] {0, 0, 0, 0};
    public void Skip(){//Skipカード
        if(GameController.players_item[GameController.players_turn, 0] >= 1){
            GameController.players_item[GameController.players_turn, 0] -= 1;
            ItemPanelClose.SetActive(false);
            ItemPanelPreOpen.SetActive(true);
            int next_player = GameController.players_turn;
            next_player += reverse;
            next_player %= GameController.PLAYERS_NUM;
            skip_flg[next_player] = 1;//スキップフラグを1に
            StartCoroutine(Message("スキップカード！"));
        }
    }

    public void Reverse(){//リバースカード
        if(GameController.players_item[GameController.players_turn, 1] >= 1){
            GameController.players_item[GameController.players_turn, 1] -= 1;
            ItemPanelClose.SetActive(false);
            ItemPanelPreOpen.SetActive(true);
            reverse *= -1;
            StartCoroutine(Message("リバースカード！"));
        }
    }


    public void HokenSyo(){//まだ実装してない
       if(GameController.players_item[GameController.players_turn, 2] >= 1){
            GameController.players_item[GameController.players_turn, 2] -= 1;
            ItemPanelClose.SetActive(false);
            ItemPanelPreOpen.SetActive(true);
            StartCoroutine(Message("保険証！", 2));
        }
    }


    int sarachicount;
    public void Sarachi(){//更地カード
        if(GameController.players_item[GameController.players_turn, 3] >= 1){
            GameController.players_item[GameController.players_turn, 3] -= 1;
            ItemPanelClose.SetActive(false);
            ItemPanelPreOpen.SetActive(true);
            StartCoroutine(Message("更地カード！", 3));
        }
    }


    public void ShiteiMasu(){//指定マスカード
        if(GameController.players_item[GameController.players_turn, 4] >= 1){
            GameController.players_item[GameController.players_turn,4] -= 1;
            ItemPanelClose.SetActive(false);//くろいほう
            ItemPanelPreOpen.SetActive(true);//あかいほう
            StartCoroutine(Message("指定マスカード！", 4));
        }
    }

    

    public TMP_InputField Masume;
    public GameObject ShiteiMasuPanel;
    public void InputMasume(){//ますを指定してインプットしたらコールバックされる
        string[] atai = new string[] {"1", "2", "3", "4", "5", "6"};
        if(atai.Contains(Masume.text))gamecontroller.Walk(int.Parse(Masume.text),0,0);
         ShiteiMasuPanel.SetActive(false);
    }
    

    public Button ItemPanelPreOpenButton;
    public GameObject ItemPanelPreOpen;
    //public Button ItemPanelOpenButton;
    public GameObject ItemPanelOpen;
    public GameObject ItemPanelPreClose;
    public GameObject ItemPanelClose;
    public GameObject ItemPanel;
    
    public AudioSource SoundEffedt;//オーディオソースは透明なゲームオブジェクトについてる。
    public AudioClip selectSound;//ボタン選択時の音

    IEnumerator Message(string newmessage, int cardid=-1){//メッセージ, 指定マスパネルのときはパネルを出すアクティブにする
        //ItemPanelOpenButton.interactable = false;
        ItemPanel.SetActive(false);
        message.text = newmessage;
        yield return new WaitForSeconds(1f);
        message.text = "";
        if(cardid==3){//更地カード
            message.text = "残り3マス!";
            yield return new WaitForSeconds(1f);
            message.text = "";
            sarachicount = 3;
        }else if(cardid==4){
            ShiteiMasuPanel.SetActive(true);
        }
    }

    public void ItemPanelPreActive(){//アイテムボタンを押すとボタンが赤くなる
        SoundEffedt.PlayOneShot(selectSound);
        ItemPanelPreOpen.SetActive(false);
        ItemPanelOpen.SetActive(true);
        Invoke("ItemPanelActive",0.5f);
    }

    public void ItemPanelActive(){//赤いアイテムボタンを押すとアイテムパネルが表示される
        ItemPanelOpen.SetActive(false);//黒いほう
        ItemPanelPreClose.SetActive(true);//あかいほう
        ItemPanel.SetActive(true);
    }

    public void ItemPanelPreActiveFalse(){//戻るボタンを押すとボタンが赤くなる
        SoundEffedt.PlayOneShot(selectSound);
        ItemPanelPreClose.SetActive(false);
        ItemPanelClose.SetActive(true);
        Invoke("ItemPanelActiveFalse",0.5f);
    }

    public void ItemPanelActiveFalse(){//赤い戻るボタンを押すとアイテムパネルが消える
        ItemPanelClose.SetActive(false);
        ItemPanel.SetActive(false);
        ItemPanelPreOpen.SetActive(true);
    }
}
