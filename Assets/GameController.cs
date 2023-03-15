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
    public MasuController masucontroller;
    public const int PLAYERS_NUM = 1;
    public Tilemap tilemap;//地図のタイルマップを取得。地図のタイルマップとワールド座標は異なるためGetCellCentorWordlでタイルマップの中心の位置に変換する必要がある。

    public TextMeshProUGUI message;//エンディング、アイテム使用時などのメッセージ

    public TextMeshProUGUI syojikin;

    public static GameObject player1;
    public static GameObject player2;
    public static GameObject player3;
    public static GameObject player4;
    public Button turn;
    List<GameObject> players = new List<GameObject>();

    public static int players_turn = 0;//今誰のターンか
    static int[,] players_position;
    static int[,,] used;
    public static int[] players_coin = new int[]{10, 10, 10, 10};//追加2/8(伊藤)
    public static int[] players_medal = new int[PLAYERS_NUM];//それぞれのプレイヤーのメダルの数
    public static int[,] players_item = new int[PLAYERS_NUM, 5];//それぞれのプレイヤーのアイテムの数

    static List<Vector3> player_destination = new List<Vector3>();

    System.Random saikoro = new System.Random();

   

    static bool syokika = true;
    public AudioSource audioSource;//オーディオソースは透明なゲームオブジェクトについてる。
    public AudioClip BGM;//BGM用のpublic変数
    static float bgmTime;//シーンに映るときにBGMが初めに戻らないようにする変数。

    public Button ItemPanelButton;
    public GameObject path1;
    public GameObject path2;
    public GameObject path3;
    public GameObject path4;
    public GameObject path5;
    void Start(){
        var bound = tilemap.cellBounds;
        /*
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
        */
        player1 = GameObject.Find("fox");
        player2 = GameObject.Find("fox_red");
        player3 = GameObject.Find("fox_yellow");
        player4 = GameObject.Find("fox_blue");
        players = new List<GameObject>() {player1, player2, player3, player4};//プレイヤーのゲームオブジェクトを配列として保持している。プレイヤーのゲームオブジェクトを配列として保持している。

        if (syokika){
            bgmTime = 0f;//BGMを初めから
            int sx = -5;//スタート地点の座標。
            int sy = -1;
            players_position = new int[,]{{sx,sy}, {sx,sy}, {sx,sy}, {sx,sy}};//それぞれのプレイヤーのいるマス目の座標。
            player_destination = new List<Vector3>() {tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0)), 
                                                      tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0)), 
                                                      tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0)),
                                                      tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0))};
            used = new int[PLAYERS_NUM, bound.max.x-bound.min.x, bound.max.y-bound.min.y];//プレイヤー数、縦、横
            for(int i=0; i<PLAYERS_NUM; i++)used[i, sx-bound.min.x, sy-bound.min.y] = 1;//xを+8, yを+4した値にする
            syokika = false;
        }else{
            Vector3 delta = new Vector3(0,0.5f,0);
            for(int i=0; i<PLAYERS_NUM; i++)players[i].transform.position = delta + player_destination[i];
        }
       
        CameraControl2.MoveCamera();
        if(ProblemController.isWalk){
            turn.interactable = false;
            ItemPanelButton.interactable = false;
            if(ProblemController.ans >= 0){
                Walk(ProblemController.ans, 1);//歩き始めにフラグを1にする
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
    public AudioClip coinSound;//歩く音
    public static bool canChange;
    private IEnumerator Change(int x, int y, int nokori, float waitTime, bool rev=false){
        yield return new WaitForSeconds(waitTime);
        player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
        audioSource.PlayOneShot(walkSound);
        canChange = true;
        if(nokori==0){
            if(rev==false){
                var tile = tilemap.GetTile<Tile>(tilemap.WorldToCell(player_destination[players_turn]));
                if(tile.sprite.name.Contains("blue")) {//プラスます
                    masucontroller.CoinPlus();
                    audioSource.PlayOneShot(coinSound);
                }else if(tile.sprite.name.Contains("red")){//マイナスます
                    masucontroller.CoinMinus();
                    audioSource.PlayOneShot(coinSound);
                }else if(tile.sprite.name.Contains("green")){//ショップます
                    yield return new WaitForSeconds(1f);
                    canChange = false;
                    masucontroller.EventMasu();
                }else if(tile.sprite.name.Contains("yellow")){
                    yield return new WaitForSeconds(1f);
                    canChange = false;
                    masucontroller.ShopMasu();
                }else if(tile.sprite.name.Contains("sekisho")){//関所ます
                    yield return new WaitForSeconds(1f);
                    canChange = false;
                    masucontroller.SekisyoMasu();
                }
            
                yield return new WaitForSeconds(1f);//目的地を変えてから直ぐにターン変更すると次のプレイヤーが動いてしまう
                while(!canChange)yield return null;//店にいる間は動かない

                if(tile.sprite.name.Contains("kakushi1")){//ショップ優先でその後隠します解除
                    yield return new WaitForSeconds(1f);
                    path1.SetActive(false);
                }else if(tile.sprite.name.Contains("kakushi2")){
                    yield return new WaitForSeconds(1f);
                    path2.SetActive(false);
                }else if(tile.sprite.name.Contains("kakushi3")){
                    yield return new WaitForSeconds(1f);
                    path3.SetActive(false);
                }else if(tile.sprite.name.Contains("kakushi4")){
                    yield return new WaitForSeconds(1f);
                    path4.SetActive(false);
                }else if(tile.sprite.name.Contains("kakushi5")){
                    yield return new WaitForSeconds(1f);
                    path5.SetActive(false);
                }
            }
            players_turn += ItemController.reverse + GameController.PLAYERS_NUM;
            players_turn %= PLAYERS_NUM;
            if(ItemController.skip_flg[players_turn]==1){//スキップフラグ
                turnImage.sprite = turnImages[players_turn];
                yield return new WaitForSeconds(1f);
                message.text = "スキップ！";
                yield return new WaitForSeconds(2f);
                players_turn += ItemController.reverse + GameController.PLAYERS_NUM;
                players_turn %= PLAYERS_NUM;
                message.text = "";
            }
            turnImage.sprite = turnImages[players_turn];
            turn.interactable = true;
            ItemPanelButton.interactable = true;
        }
    }
    

    void Update(){
        syojikin.text = "×" + players_coin[players_turn].ToString();
        float speed = 0.5f;
        Vector3 delta = new Vector3(0,0.5f,0);//パネルの上に立ってるように見える補正
        Vector3 dist = player_destination[players_turn] + delta;
        players[players_turn].transform.position = Vector3.MoveTowards(players[players_turn].transform.position,  dist, speed);//player_destination[players_turn], speed);
        
        if (Input.GetMouseButtonDown(0)){
                Vector3 pos = Input.mousePosition;   
                var bound = tilemap.cellBounds;
                var tile = tilemap.GetTile<Tile>(tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)));
                //Debug.Log(tile.sprite.name);
                /*Debug.Log(used[players_turn, tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)).x-bound.min.x,
                                                                 tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)).y-bound.min.y]);*/
                //Debug.Log(tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)).x);
        }
    }

    public TileBase m_tileYellow;//選択しているタイル
    IEnumerator WaitInput (int nokori, List<List<int>> Nexts) {
        int nexts_index = 0;
        Vector3Int before;
        Vector3Int selectCellPos = new Vector3Int(Nexts[0][0],Nexts[0][1],0);
        TileBase beforeTile = tilemap.GetTile<Tile>(selectCellPos);
        tilemap.SetTile(selectCellPos, m_tileYellow);
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
                        tilemap.SetTile(before, beforeTile);
                        before = selectCellPos;
                        beforeTile =  tilemap.GetTile<Tile>(selectCellPos);
                        tilemap.SetTile(selectCellPos, m_tileYellow);
                    }
                }
            }
            yield return null;
        }
        tilemap.SetTile(selectCellPos, beforeTile);
        Walk(nokori, 1, nexts_index);//無限ループ防止用フラグ(分岐で移動しなくなる)
    }

    
    public void Walk(int ans, int flg=0, int nexts_index=0){//答え、歩き初めに1にするフラグ、分岐の時の次のマス
        if(ans==0){//残りゼロマスだとそのまま
            StartCoroutine(Change(players_position[players_turn, 0], players_position[players_turn, 1], 0, 0.3f));
            return;
        }
        int[,] delta = new int[,] {{0,-1}, {1,0}, {0,1}, {-1,0},};//下右上左の方向、jが変わるとアクセスされるデルタが変わる
        var bound = tilemap.cellBounds;
        for(int i=0; i<ans; i++){//ansが正のときでないと動かない
            var tile = tilemap.GetTile<Tile>(tilemap.WorldToCell(player_destination[players_turn]));
            string tilename = tile.sprite.name;
            List<List<int>> Nexts = new List<List<int>>();
            int x, y, nx_kouho, ny_kouho;
            x = players_position[players_turn, 0];
            y = players_position[players_turn, 1];
        
            /*if(tilename.Contains("gouryu")){
                List<int> next = new List<int>();
                nx_kouho = x + delta[tilename[6]-'0', 0];
                ny_kouho = y + delta[tilename[6]-'0', 1];
                next.Add(nx_kouho);
                next.Add(ny_kouho);
                Nexts.Add(next);
            }else{*/
                for(int j=0; j<4; j++){//上下左右の探索
                    List<int> next = new List<int>();
                    nx_kouho = x + delta[j, 0];
                    ny_kouho = y + delta[j, 1];
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
            //}

            int nx, ny;
            if(tilename.Contains("bunki")){//現在分岐ます
                if(flg==0){//フラグを1にしないとnx, nyを変更できない
                    StartCoroutine(WaitInput(ans-i, Nexts));//黄色のポインタを出す
                return;
                }else{
                    nx = Nexts[nexts_index][0];
                    ny = Nexts[nexts_index][1];
                }
            }else if(tilename.Contains("sekisho")&&flg==0){
                StartCoroutine(Change(x, y, 0, 0.3f*i));//関所かつ歩き始めではない時にとまる
                return;
            }else{
                nx = Nexts[0][0];
                ny = Nexts[0][1];
            }

            if(tilename.Contains("sekisho")&&flg==1)used[players_turn, x-bound.min.x, y-bound.min.y]=0;//Walkrevで関所より前に戻らないように

            flg=0;
            players_position[players_turn, 0] = nx;
            players_position[players_turn, 1] = ny;
            used[players_turn, nx-bound.min.x, ny-bound.min.y] = 1;//通った道を記録
            StartCoroutine(Change(nx, ny, ans-i-1, 0.3f*i));
            player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(nx, ny, 0));//タイル換算の位置にしている
        }
    }

    private void WalkRev(int ans, int flg=0, int nexts_index=0){
        int[,] delta = new int[,] {{0,-1}, {1,0}, {0,1}, {-1,0},};//下右上左の方向、jが変わるとアクセスされるデルタが変わる
        var bound = tilemap.cellBounds;
        for(int i=0; i<ans; i++){
            int x, y;
            x = players_position[players_turn, 0];
            y = players_position[players_turn, 1];
            List<List<int>> Nexts = new List<List<int>>();
            for(int j=0; j<4; j++){//上下左右の探索
                List<int> next = new List<int>();
                int nx_kouho = x + delta[j, 0];
                int ny_kouho = y + delta[j, 1];
                if (!tilemap.HasTile(new Vector3Int(nx_kouho, ny_kouho, 0)))continue;
                if (used[players_turn, nx_kouho-bound.min.x, ny_kouho-bound.min.y] == 0)continue;
                next.Add(nx_kouho);
                next.Add(ny_kouho);
                Nexts.Add(next);
            } 
            if(Nexts.Count==0){//行き止まりはスタートとして判定
                StartCoroutine(Change(players_position[players_turn, 0], players_position[players_turn, 1], 0, 0.3f));
                return;
            }
            int nx, ny;
            nx = Nexts[0][0];
            ny = Nexts[0][1];
            used[players_turn, x-bound.min.x, y-bound.min.y] = 0;//通った道を記録
            players_position[players_turn, 0] = nx;
            players_position[players_turn, 1] = ny;
            StartCoroutine(Change(nx, ny, ans-i-1, 0.3f*i, true));
            player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(nx, ny, 0));//タイル換算の位置にしている
        }
    }
    

    void Ending(){
        message.text = "Player" + players_turn.ToString() + " Wins!";
    }
    
    public void Turn(){
        bgmTime = audioSource.time;
        turn.interactable = false;
        SceneManager.LoadScene("problem");
    }
    

}   