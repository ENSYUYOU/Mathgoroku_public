using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;//ランダム変数用
using TMPro;
using UnityEngine.Tilemaps;//マス目を記録したタイルマップ
using System.Text;//String Builderを使うためのもの
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{   
    const int PLAYERS_NUM = 3;
    public Tilemap tilemap;//地図のタイルマップを取得。地図のタイルマップとワールド座標は異なるためGetCellCentorWordlでタイルマップの中心の位置に変換する必要がある。
    public TextMeshProUGUI turntext;
    public TextMeshProUGUI endingtext;
    public TextMeshProUGUI shopitem1;//ショップますで使うボタンのテキスト
    public TextMeshProUGUI shopitem2;
    public TextMeshProUGUI shopitem3;
    

    public static GameObject player1;
    public static GameObject player2;
    public static GameObject player3;
    public Button turn;
    List<GameObject> players = new List<GameObject>();


    static int[,] players_position;
    public static int players_turn = 0;//今誰のターンか
    static int[,,] used;
    public static int[] players_coin = new int[]{10, 10, 10};//追加2/8(伊藤)
    static int[] players_medal = new int[PLAYERS_NUM];//それぞれのプレイヤーのメダルの数
    static int[,] players_item = new int[PLAYERS_NUM, 5];//それぞれのプレイヤーのアイテムの数

    static List<Vector3> player_destination = new List<Vector3>();

    System.Random saikoro = new System.Random();

   

    static bool syokika = true;
    public AudioSource audioSource;//オーディオソースは透明なゲームオブジェクトについてる。
    public AudioClip BGM;//BGM用のpublic変数
    static float bgmTime;//シーンに映るときにBGMが初めに戻らないようにする変数。
    void Start(){
        ShopMasu(0,1,2);
        var builder = new StringBuilder();//タイルマップ表示用プログラム
        var bound = tilemap.cellBounds;
        for (int y = bound.max.y-1; y >= bound.min.y; --y)
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                builder.Append(tilemap.HasTile(new Vector3Int(x, y, 0))? "■" : "□");
            }
            builder.Append("\n");
        }
    
        Debug.Log(builder);
        player1 = GameObject.Find("fox");
        player2 = GameObject.Find("fox_red");
        player3 = GameObject.Find("fox_yellow");
        players = new List<GameObject>() {player1, player2, player3};//プレイヤーのゲームオブジェクトを配列として保持している。プレイヤーのゲームオブジェクトを配列として保持している。
        //var bound = tilemap.cellBounds;
        if (syokika){
            bgmTime = 0f;//BGMを初めから
            int sx = -5;//スタート地点の座標。
            int sy = -1;
            players_position = new int[,]{{sx,sy}, {sx,sy}, {sx,sy}};//それぞれのプレイヤーのいるマス目の座標。
            player_destination = new List<Vector3>() {tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0)), tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0)), tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0))};
            
            used = new int[PLAYERS_NUM, bound.max.x-bound.min.x, bound.max.y-bound.min.y];//プレイヤー数、縦、横
            used[0, sx-bound.min.x, sy-bound.min.y] = 1;//xを+8, yを+4した値にする
            used[1, sx-bound.min.x, sy-bound.min.y] = 1;//幅優先探索用に訪れた頂点を初期化している
            used[2, sx-bound.min.x, sy-bound.min.y] = 1;
            syokika = false;
        }else{
            Vector3 delta = new Vector3(0,0.5f,0);
            player1.transform.position = delta + player_destination[0];//プレイヤーをワープさせる。
            player2.transform.position = delta + player_destination[1];
            player3.transform.position = delta+player_destination[2];
        }
       
        CameraControl2.MoveCamera();
        if(ProblemController.isWalk){
            turn.interactable = false;
            if(ProblemController.ans >= 0){
                Debug.Log("junkyuu");
                Walk(ProblemController.ans);
            }else if(ProblemController.ans < 0){
                WalkRev(Math.Abs(ProblemController.ans));
            }
        }
        ProblemController.isWalk = false;

        audioSource.clip = BGM;
        audioSource.time = bgmTime;
        audioSource.Play();
    }


    public Image turnImage;
    public Sprite[] turnImages;
    public AudioClip walkSound;//歩く音
    private IEnumerator Change(int x, int y, int nokori, float waitTime){
         Debug.Log("1111");
        yield return new WaitForSeconds(waitTime);
        player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
        audioSource.PlayOneShot(walkSound);
        Debug.Log("bbbb");
        if(nokori==0){
            yield return new WaitForSeconds(waitTime);//目的地を変えてから直ぐにターン変更すると次のプレイヤーが動いてしまう
            Debug.Log("aaaaaaaa");
            players_turn += 1;
            players_turn %= PLAYERS_NUM;
            turn.interactable = true;
            turnImage.sprite = turnImages[players_turn];
        }
    }
    
    float speed = 0.5f;

    void Update(){
        /*
        Debug.Log("-----------1----------");
        Debug.Log(players_coin[0]);
        Debug.Log("-----------2----------");
        Debug.Log(players_coin[1]);
        Debug.Log("-----------3----------");
        Debug.Log(players_coin[2]);
        Debug.Log(players_coin[players_turn]);*/
        Vector3 delta = new Vector3(0,0.5f,0);//パネルの上に立ってるように見える補正
        Vector3 dist = player_destination[players_turn] + delta;
        players[players_turn].transform.position = Vector3.MoveTowards(players[players_turn].transform.position,  dist, speed);//player_destination[players_turn], speed);

        if (Input.GetMouseButtonDown(0)){
                Vector3 pos = Input.mousePosition;   
                //Debug.Log(tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)));
                //EventMasu();
        }
    }



    public TileBase m_tileGray;
    public TileBase m_tileRed;
    IEnumerator WaitInput (int nokori, List<List<int>> Nexts) {
        int nexts_index = 0;
        Vector3Int before;
        Vector3Int selectCellPos = new Vector3Int(Nexts[0][0],Nexts[0][1],0);
        tilemap.SetTile(selectCellPos,m_tileGray);
        before = selectCellPos;
        bool canMove=false;
        while(!canMove) {
            if (Input.GetMouseButtonDown(0)){
                Vector3 pos = Input.mousePosition;   
                selectCellPos = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos));
                for(int i=0; i<Nexts.Count; i++){
                    if(selectCellPos.x == Nexts[i][0] && selectCellPos.y == Nexts[i][1]){
                        if(before==selectCellPos)canMove=true;
                        nexts_index = i;
                        tilemap.SetTile(selectCellPos,m_tileGray);
                        tilemap.SetTile(before,m_tileRed);
                        before = selectCellPos;
                    }
                }
            }
            yield return null;
        }
        tilemap.SetTile(selectCellPos,m_tileRed);
        Walk(nokori, 1, nexts_index);//無限ループ防止用フラグ(分岐で移動しなくなる)
    }

    
    private void Walk(int ans, int flg=0, int nexts_index=0){
        if(ans==0){
            Debug.Log("Walk");
            StartCoroutine(Change(players_position[players_turn, 0], players_position[players_turn, 1], 0, 0.3f));
            return;
        }
        int[,] delta = new int[,] {{0,-1}, {1,0}, {0,1}, {-1,0},};//下右上左の方向、jが変わるとアクセスされるデルタが変わる
        var bound = tilemap.cellBounds;
        for(int i=0; i<ans; i++){//ansが正のときでないと動かない
            List<List<int>> Nexts = new List<List<int>>();
            for(int j=0; j<4; j++){//上下左右の探索
                List<int> next = new List<int>();
                int nx_kouho = players_position[players_turn, 0] + delta[j, 0];
                int ny_kouho = players_position[players_turn, 1] + delta[j, 1];
                if (!tilemap.HasTile(new Vector3Int(nx_kouho, ny_kouho, 0)))continue;//タイルマップ上にタイルがあるか調べる
                if (used[players_turn, nx_kouho-bound.min.x, ny_kouho-bound.min.y] >= 1)continue;//n番目のプレイヤーの通った道の記録を呼び出して、
                next.Add(nx_kouho);
                next.Add(ny_kouho);
                Nexts.Add(next);
            } 
            if(Nexts.Count==0){//行き止まりはゴールとして判定
                Ending();
                return;
            }
            int nx, ny;
            int[,] bunki = {{-2, -1}};//分岐の座標を設定
            bool isBunki = false;//分岐の初期化
           
            for(int j=0; j<bunki.GetLength(0); j++){
                if((players_position[players_turn, 0]==bunki[j,0]&&players_position[players_turn, 1]==bunki[j,1]))isBunki = true;//プレイヤーの座標が分岐点の座標かどうか判定
            }

            if(isBunki){
                if(flg==0){//フラグを1にしないとnx, nyを変更できない
                    StartCoroutine(WaitInput(ans-i, Nexts));//黄色のポインタを出す
                return;
                }else{
                    nx = Nexts[nexts_index][0];
                    ny = Nexts[nexts_index][1];
                }
            }else{
                nx = Nexts[0][0];
                ny = Nexts[0][1];
            }
            
            players_position[players_turn, 0] = nx;
            players_position[players_turn, 1] = ny;
            used[players_turn, nx-bound.min.x, ny-bound.min.y] = 1;//通った道を記録
            StartCoroutine(Change(nx, ny, ans-i-1, 0.3f*i));
            player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(nx, ny, 0));//タイル換算の位置にしている
        }
    }

    private void WalkRev(int ans, int flg=0, int nexts_index=0){
        Debug.Log("walkrev");
        int[,] delta = new int[,] {{0,-1}, {1,0}, {0,1}, {-1,0},};//下右上左の方向、jが変わるとアクセスされるデルタが変わる
        var bound = tilemap.cellBounds;
        for(int i=0; i<ans; i++){//ansが負のときでないと動かない
            List<List<int>> Nexts = new List<List<int>>();
            for(int j=0; j<4; j++){//上下左右の探索
                List<int> next = new List<int>();
                int nx_kouho = players_position[players_turn, 0] + delta[j, 0];
                int ny_kouho = players_position[players_turn, 1] + delta[j, 1];
                if (!tilemap.HasTile(new Vector3Int(nx_kouho, ny_kouho, 0)))continue;
                if (used[players_turn, nx_kouho-bound.min.x, ny_kouho-bound.min.y] == 0)continue;
                next.Add(nx_kouho);
                next.Add(ny_kouho);
                Nexts.Add(next);
            } 
            if(Nexts.Count==0){//行き止まりはスタートとして判定
                Debug.Log("ikidomari");
                StartCoroutine(Change(players_position[players_turn, 0], players_position[players_turn, 1], 0, 0.3f));
                return;
            }
            Debug.Log("owari");
            int nx, ny;
            nx = Nexts[0][0];
            ny = Nexts[0][1];
            
            players_position[players_turn, 0] = nx;
            players_position[players_turn, 1] = ny;
            used[players_turn, nx-bound.min.x, ny-bound.min.y] = 0;//通った道を記録
            StartCoroutine(Change(nx, ny, ans-i-1, 0.3f*i));
            player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(nx, ny, 0));//タイル換算の位置にしている
        }
    }
    

    void Ending(){
        endingtext.text = "Player" + players_turn.ToString() + " Wins!";
    }
    
    public void Turn(){
        bgmTime = audioSource.time;
        turn.interactable = false;
        SceneManager.LoadScene("problem");
    }

    private void EventMasu(){//イベントマス用の関数
        /*eventid
        0: メダルをもらえる
        1: メダルを失う
        2: アイテムをもらえる
        */
        int eventid = saikoro.Next(0,3);
        if (eventid==0){
            players_medal[players_turn] += 1;
        }else if(eventid==1){
            players_medal[players_turn] -= 1;
            players_medal[players_turn] = Math.Max(0, players_medal[players_turn]);
        }else if(eventid==2){
            int itemid = saikoro.Next(0,4);
            players_item[players_turn, itemid] += 1; 
        }
    }


    private void CoinPlus(){
        players_coin[players_turn] += 1;
    }

    private void CoinMinus(){
        players_coin[players_turn] -= 1;
        players_coin[players_turn] = Math.Max(players_coin[players_turn], 0);
    }

    /*
    Syop画面の残りの作業
    ・アイテムの値段を決めて、購入後に所持金を減らす
    ・購入後の画面の切り替えとか色々
    */
    string[] ITEMS = new string[] {"アイテム1", "アイテム2", "アイテム3", "アイテム4", "アイテム5"};
    int selected_item;
    int item1, item2, item3;
    public Image nekoImage;
    public Sprite[] nekoImages;
    public TextMeshProUGUI nekoserihu;//ショップの猫のセリフ
    public TextMeshProUGUI syojikin;//右下のコインの枚数のテキスト
    private void ShopMasu(int newitem1, int newitem2, int newitem3){//入荷アイテム3種類。今選択したアイテム。
        item1 = newitem1;
        item2 = newitem2;
        item3 = newitem3;
        selected_item = -1;//はじめは存在しないアイテムで初期化
        shopitem1.text = ITEMS[item1];
        shopitem2.text = ITEMS[item2];
        shopitem3.text = ITEMS[item3];
        syojikin.text = "×"+ players_coin[players_turn].ToString();
    }
    //ItemButton1とかはアイテムリストのボタンを押したときに反応
    public void ItemButton1(){//全部黒にしてから選んだものだけ黄色にする
        shopitem1.color = new Color(0, 0, 0, 1f);
        shopitem2.color = new Color(0, 0, 0, 1f);
        shopitem3.color = new Color(0, 0, 0, 1f);
        shopitem1.color = new Color(1f, 0.92f, 0.016f, 1f);
        if(selected_item==0){
            players_item[players_turn, item1] += 1;
            nekoserihu.text = "お買い上げありがとうございます！";
            nekoImage.sprite = nekoImages[5];
            syojikin.text = "×"+ players_coin[players_turn].ToString();
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
        if(selected_item==1){
                players_item[players_turn, item2] += 1;
                nekoserihu.text = "お買い上げありがとうございます！";
                nekoImage.sprite = nekoImages[5];
                syojikin.text = "×"+ players_coin[players_turn].ToString();
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
        if(selected_item==2){
            players_item[players_turn, item3] += 1;
            nekoserihu.text = "お買い上げありがとうございます！";
            nekoImage.sprite = nekoImages[5];
            syojikin.text = "×"+ players_coin[players_turn].ToString();
        }else{
            nekoserihu.text = ITEMS[item3] + "にしますか?";
            selected_item = item3;
            nekoImage.sprite = nekoImages[saikoro.Next(0,5)];
        }
    }
}   