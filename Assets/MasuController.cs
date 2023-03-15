using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;//ランダム変数用
using UnityEngine.UI;

public class MasuController : MonoBehaviour
{
    System.Random saikoro = new System.Random();

    int nameid1;//イベントマス関係
    int nameid2;
    int nameid3;
    int nameid4;
    bool moveName;
    int nameID;
    float currentTime;
    public TextMeshProUGUI name1;
    public TextMeshProUGUI name2;
    public TextMeshProUGUI name3;
    public TextMeshProUGUI name4;
    public TextMeshProUGUI sugakusyacomment;
    public Button SugakusyaCommentButton;
    string[] sugakusya = {"ニュートン", "アルキメデス", "ノイマン", "ピタゴラス", "チューリング"};
    List<List<string>> sugakusya_comment_list = new List<List<string>>() {
        new List<string> {"「私はニュートンである」","ニュートンは万有引力の法則を発見した人としてよく語られますが、数学にも多大な功績を残しています。","自らが発見した運動の法則や万有引力の法則、ケプラーが発見した惑星の法則を定式化して証明するために、ニュートンは微分法、積分法の考え方を生み出しました","そんなニュートンですが、学生時代には成績不振を馬鹿にされ、いじめにあっていたこともあったそうです。","「君の顔は昔、私のことをいじめた人間によく似ているような気がするのだ」","「私は今、当時を思い出して無性に腹が立ってきたのだ」","「君を1発殴らせてくれないか。もしくはその代わりにそのメダルを1枚よこしたまえ」","ニュートンはメダルを奪って去って行った。"},
        new List<string> {"「我はアルキメデスと申す」","あああ","よろしく"},
        new List<string> {"「私の名前はフォン・ノイマンだ」","フォン・ノイマンは戦前から戦後にかけて活躍したハンガリー出身の数学者です。","驚異的な計算能力をもち、独特な思考方法をしていたと言われ、「悪魔の頭脳」とも評されました。","ゲーム理論の成立やコンピュータの開発に貢献するなど、マルチな活躍を見せました。","「私のことを『悪魔の頭脳』と呼ぶ輩もいるそうじゃないか」","「悪魔と言われるのはあまり気分の良いことではないな」","「どうだ、君にこのアイテムをやるから、私の『悪魔』というのを変えてきてくれないか」","フォン・ノイマンから無効化カードをもらった！"},
        new List<string> {"「ワシの名はピタゴラスじゃ」","ピタゴラスの定理（三平方の定理）で知られるピタゴラスですが、実際には相当な秘密主義者であったと言われています。","ピタゴラスは自ら教団を組織し、教団内の情報を外部に持ち出すことは固く禁じました。","「このワシに逆らって秘密を外に漏らした者に容赦はしない」","「今から貴様を船から海に突き落としてやる！」","しまった！　ピタゴラスは秘密が漏れたことを知って、たいそうお怒りのようです！","ここはメダルを差し出して何とかやりすごしましょう！","「メダルをよこすのなら今回だけだ、命まではとらぬ、さっさと行け！」","ピタゴラスはメダルを奪って去っていった。"},
        new List<string> {"「私はチューリングです」","チューリングは"},};
    int selected_sugakusya_id;
    int sugakusyacommentid;


    string[] ITEMS = new string[]{"SKIPカード", "リバースカード", "保険証", "更地カード", "指定マスカード"};//アイテムます関係
    int[] ITEMPRICE = new int[]{5, 5, 5, 10, 10};
    int selected_item;
    int item1, item2, item3;
    public Image nekoImage;
    public Sprite[] nekoImages;
    public TextMeshProUGUI nekoserihu;//ショップの猫のセリフ
    public TextMeshProUGUI syojikin;//右下のコインの枚数のテキスト
    public TextMeshProUGUI shopitem1;//ショップますで使うボタンのテキスト
    public TextMeshProUGUI shopitem2;
    public TextMeshProUGUI shopitem3;

    //関所マス関係
    string[] comment = new string[] {"こんにちは", "ここは関所です", "コイン50枚でメダルに交換できます。", "交換しますか?"};
    int commentid=0;
    public TextMeshProUGUI Dirichletcomment;
    public GameObject yes;
    public GameObject no;
    public GameObject SekisyoHaikeiButton;
    public TextMeshProUGUI DirichleSyojikin;


    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime>0.1f && moveName){
            nameID+=1;
            nameID%=4;//4アイテム選択
            currentTime = 0f;
            name1.color = new Color(0, 0, 0, 1f);
            name2.color = new Color(0, 0, 0, 1f);
            name3.color = new Color(0, 0, 0, 1f);
            name4.color = new Color(0, 0, 0, 1f);
            if(nameID==0)name1.color = new Color(1f, 0.92f, 0.016f, 1f);
            if(nameID==1)name2.color = new Color(1f, 0.92f, 0.016f, 1f);
            if(nameID==2)name3.color = new Color(1f, 0.92f, 0.016f, 1f);
            if(nameID==3)name4.color = new Color(1f, 0.92f, 0.016f, 1f);
        }
    }


    public void CoinPlus(){
        GameController.players_coin[GameController.players_turn] += 5;
    }

    public void CoinMinus(){
        GameController.players_coin[GameController.players_turn] -= 5;
        GameController.players_coin[GameController.players_turn] = Math.Max(GameController.players_coin[GameController.players_turn], 0);
    }

    /*
    Syop画面の残りの作業
    ・アイテムの値段を決めて、購入後に所持金を減らす
    ・購入後の画面の切り替えとか色々
    */
    public GameObject ShopHaikei;
    public void ShopMasu(){//入荷アイテム3種類。今選択したアイテム。
        ShopHaikei.SetActive(true);
        while(item1==item2 || item2==item3 || item3==item1){
            item1 = saikoro.Next(0, 5);
            item2 = saikoro.Next(0, 5);
            item3 = saikoro.Next(0, 5);
        }
        selected_item = -1;//はじめは存在しないアイテムで初期化
        shopitem1.text = ITEMS[item1] + "(" + ITEMPRICE[item1].ToString() + "P)";
        shopitem2.text = ITEMS[item2] + "(" + ITEMPRICE[item2].ToString() + "P)";
        shopitem3.text = ITEMS[item3] + "(" + ITEMPRICE[item3].ToString() + "P)";
        syojikin.text = "×"+ GameController.players_coin[GameController.players_turn].ToString();
    }


    //ItemButton1とかはアイテムリストのボタンを押したときに反応
    public void ItemButton1(){//全部黒にしてから選んだものだけ黄色にする
        shopitem1.color = new Color(0, 0, 0, 1f);
        shopitem2.color = new Color(0, 0, 0, 1f);
        shopitem3.color = new Color(0, 0, 0, 1f);
        shopitem1.color = new Color(1f, 0.92f, 0.016f, 1f);
        Debug.Log("hello");
        if(selected_item==item1){
            if(GameController.players_coin[GameController.players_turn] >= ITEMPRICE[item1]){
                GameController.players_coin[GameController.players_turn] -= ITEMPRICE[item1];
                GameController.players_item[GameController.players_turn, item1] += 1;
                nekoserihu.text = "お買い上げありがとうございます！";
                nekoImage.sprite = nekoImages[5];
                syojikin.text = "×"+ GameController.players_coin[GameController.players_turn].ToString();
                StartCoroutine(ReturnToSugoroku());
            }else{
                nekoserihu.text = "お金が足りません";
            }
        }else{
            nekoserihu.text = ITEMS[item1] + "にしますか?";
            selected_item = item1;
            nekoImage.sprite = nekoImages[saikoro.Next(0,5)];
        }
    }

    public void ItemButton2(){
        shopitem1.color = new Color(0, 0, 0, 1f);
        shopitem2.color = new Color(0, 0, 0, 1f);
        shopitem3.color = new Color(0, 0, 0, 1f);
        shopitem2.color = new Color(1f, 0.92f, 0.016f, 1f);
        if(selected_item==item2){
            if(GameController.players_coin[GameController.players_turn] >= ITEMPRICE[item2]){
                GameController.players_coin[GameController.players_turn] -= ITEMPRICE[item2];
                GameController.players_item[GameController.players_turn, item2] += 1;
                nekoserihu.text = "お買い上げありがとうございます！";
                nekoImage.sprite = nekoImages[5];
                syojikin.text = "×"+ GameController.players_coin[GameController.players_turn].ToString();
                StartCoroutine(ReturnToSugoroku());
            }else{
                nekoserihu.text = "お金が足りません";
            }
        }else{
            nekoserihu.text = ITEMS[item2] + "にしますか?";
            selected_item = item2;
            nekoImage.sprite = nekoImages[saikoro.Next(0,5)];
        }
    }

    public void ItemButton3(){
        shopitem1.color = new Color(0, 0, 0, 1f);
        shopitem2.color = new Color(0, 0, 0, 1f);
        shopitem3.color = new Color(0, 0, 0, 1f);
        shopitem3.color = new Color(1f, 0.92f, 0.016f, 1f);
        if(selected_item==item3){
            if(GameController.players_coin[GameController.players_turn] >= ITEMPRICE[item3]){
                GameController.players_coin[GameController.players_turn] -= ITEMPRICE[item3];
                GameController.players_item[GameController.players_turn, item3] += 1;
                nekoserihu.text = "お買い上げありがとうございます！";
                nekoImage.sprite = nekoImages[5];
                syojikin.text = "×"+ GameController.players_coin[GameController.players_turn].ToString();
                StartCoroutine(ReturnToSugoroku());
            }else{
                nekoserihu.text = "お金が足りません";
            }
        }else{
            nekoserihu.text = ITEMS[item3] + "にしますか?";
            selected_item = item3;
            nekoImage.sprite = nekoImages[saikoro.Next(0,5)];
        }
    }

    public void ReturnFromShop(){//戻るボタンを押すと作動
        nekoserihu.text = "お気をつけてお帰りください";
        StartCoroutine(ReturnToSugoroku());
    }
    
    public void SekisyoMasu(){//とりあえずクリックしたときに呼ばれる
        SekisyoHaikeiButton.SetActive(true);
        DirichleSyojikin.text = "×" + GameController.players_coin[GameController.players_turn].ToString();
        if (commentid < 3){
            commentid += 1;
            Dirichletcomment.text = comment[commentid];
        }else if(commentid==3){
            commentid += 1;
            yes.SetActive(true);
            no.SetActive(true);
        }
    }

    public void yesfunction(){
        if(GameController.players_coin[GameController.players_turn] < 50){
            Dirichletcomment.text = "お金が足りません";
        }else{
            GameController.players_coin[GameController.players_turn] -= 50;
            GameController.players_medal[GameController.players_turn] += 1;
            yes.SetActive(false);
            no.SetActive(false);
            Dirichletcomment.text = "まいどありがとうございます";
            DirichleSyojikin.text = "×" + GameController.players_coin[GameController.players_turn].ToString();
            StartCoroutine(ReturnToSugoroku());
        }
    }

    public void nofunction(){
        Dirichletcomment.text = "さようなら";
        yes.SetActive(false);
        no.SetActive(false);
        StartCoroutine(ReturnToSugoroku());
    }




    public GameObject EventHaikei;
    public GameObject EventStartRouletteButton;
    public GameObject EventHukidashi;
    public GameObject EventNeko;
    public GameObject EventTextBox;
    public GameObject EventHaikeiButton;
    public void EventMasu(){
        EventHaikei.SetActive(true);
    }
    
    public void StartRoulette(){
        EventTextBox.SetActive(true);
        EventStartRouletteButton.SetActive(false);
        EventHukidashi.SetActive(false);
        EventNeko.SetActive(false);
        SugakusyaCommentButton.interactable = false;
        while(nameid1==nameid2 || nameid2==nameid3 || nameid3==nameid4 || nameid4==nameid1){
            nameid1 = saikoro.Next(0, 5);
            nameid2 = saikoro.Next(0, 5);
            nameid3 = saikoro.Next(0, 5);
            nameid4 = saikoro.Next(0, 5);
        }
        name1.text = sugakusya[nameid1];
        name2.text = sugakusya[nameid2];
        name3.text = sugakusya[nameid3];
        name4.text = sugakusya[nameid4];
        moveName = true;
    }

    public void StopNameRoulette(){//数学者のルーレットを止めた時の関数
        if (sugakusyacommentid!=0)return;
        moveName = false;
        int [] selected_list = {nameid1, nameid2, nameid3, nameid4};
        selected_sugakusya_id = selected_list[nameID];
        sugakusyacomment.text = sugakusya_comment_list[selected_sugakusya_id][sugakusyacommentid];
        SugakusyaCommentButton.interactable = true;
        Invoke("EventSwitch", 2f);
    }

    public void EventSwitch(){
        EventTextBox.SetActive(false);
        EventHukidashi.SetActive(true);
        EventHaikeiButton.SetActive(true);
    }

    public void SugakusyaCommentFunc(){//ボタンを押すと反応。数学者のコメント
        sugakusyacommentid += 1;
        if(sugakusyacommentid < sugakusya_comment_list[selected_sugakusya_id].Count){
            sugakusyacomment.text = sugakusya_comment_list[selected_sugakusya_id][sugakusyacommentid];
        }else{
            SugakusyaCommentButton.interactable = false;
            StartCoroutine(ReturnToSugoroku());
        }
    }

    private IEnumerator ReturnToSugoroku(){
        yield return new WaitForSeconds(2f);
        GameController.canChange = true;
        ShopHaikei.SetActive(false);
        EventHaikei.SetActive(false);
        SekisyoHaikeiButton.SetActive(false);
    }
}
