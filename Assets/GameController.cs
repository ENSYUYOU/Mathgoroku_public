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
    public static int PLAYERS_NUM = 2;
    public Tilemap tilemap;//地図のタイルマップを取得。地図のタイルマップとワールド座標は異なるためGetCellCentorWordlでタイルマップの中心の位置に変換する必要がある。

    public TextMeshProUGUI message;//エンディング、アイテム使用時などのメッセージ

    public TextMeshProUGUI syojikin;
    public TextMeshProUGUI syojimedal;
    public static GameObject player1;
    public static GameObject player2;
    public static GameObject player3;
    public static GameObject player4;
    public GameObject preTurnButton;
    public Button preTurn;
    public GameObject turnButton;
    public Button turn;
    List<GameObject> players = new List<GameObject>();

    public static int players_turn = 0;//今誰のターンか
    static int[,] players_position;
    static int[,,] used;
    public static int[] players_coin = new int[]{50, 50, 50, 50};//追加2/8(伊藤)
    public static int[] players_medal = new int[PLAYERS_NUM];//それぞれのプレイヤーのメダルの数
    public static int[,] players_item = new int[PLAYERS_NUM, 5];//それぞれのプレイヤーのアイテムの数

    static List<Vector3> player_destination = new List<Vector3>();

    System.Random saikoro = new System.Random();

   

    public AudioSource SoundEffect;//オーディオソースは透明なゲームオブジェクトについてる。
    public AudioSource BGM;//オーディオソースは透明なゲームオブジェクトについてる。
    public AudioClip BGMClip;//BGM用のpublic変数
    

    public Button ItemPanelButton;
    public GameObject path1;
    public GameObject path2;
    public GameObject path3;
    public GameObject path4;
    public GameObject path5;
    void Start(){
        var bound = tilemap.cellBounds;
        players_item[0, 0] = 1;
        players_item[0, 1] = 1;
        players_item[0, 2] = 1;
        players_item[0, 3] = 1;
        players_item[0, 4] = 1;
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


        //int sx = -5;//スタート地点の座標。
        //int sy = -1;
        int sx = 21;//スタート地点の座標。
        int sy = 3;
        //int sx = 46;//スタート地点の座標。
        //int sy = 3;
        //int sx = 60;//スタート地点の座標。
        //int sy = -1;
        players_position = new int[,]{{sx,sy}, {sx,sy}, {sx,sy}, {sx,sy}};//それぞれのプレイヤーのいるマス目の座標。
        player_destination = new List<Vector3>() {tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0)), 
                                                    tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0)), 
                                                    tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0)),
                                                    tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0))};
        used = new int[PLAYERS_NUM, bound.max.x-bound.min.x, bound.max.y-bound.min.y];//プレイヤー数、縦、横
        for(int i=0; i<PLAYERS_NUM; i++)used[i, sx-bound.min.x, sy-bound.min.y] = 1;//xを+8, yを+4した値にする    
       

        BGM.clip = BGMClip;
        BGM.Play();
    }
    public void ReturnFromProblem(){
        preTurnButton.SetActive(true);
        turnButton.SetActive(false);
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
    }


    public AudioClip walkSound;//歩く音
    public AudioClip coinSound;
    public AudioClip PoPiSound;
    public static bool canChange;
    public TileBase tileBunkiBlue;//普通の青タイル
    private IEnumerator Change(int x, int y, int nokori, float waitTime, bool rev=false){
        yield return new WaitForSeconds(waitTime);
        player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
        if(nokori!=0)SoundEffect.PlayOneShot(walkSound);
        canChange = true;
        if(nokori==0){
            if(rev==false){
                Debug.Log(x);
                Debug.Log(y);
                if(x==58 && y==-6)StartCoroutine(Ending(1f));
                
                var tile = tilemap.GetTile<Tile>(tilemap.WorldToCell(player_destination[players_turn]));
                if(tile.sprite.name.Contains("blue") && !SarachiMap.HasTile(new Vector3Int(x, y, 0))) {//プラスます
                    StartCoroutine(masucontroller.CoinPlus(5));
                }else if(tile.sprite.name.Contains("red") && !SarachiMap.HasTile(new Vector3Int(x, y, 0))){//マイナスます
                        StartCoroutine(masucontroller.CoinMinus(5));
                }else if(tile.sprite.name.Contains("green") && !SarachiMap.HasTile(new Vector3Int(x, y, 0))){//イベントます
                    yield return new WaitForSeconds(1f);
                    canChange = false;
                    masucontroller.EventMasu();
                }else if(tile.sprite.name.Contains("yellow") && !SarachiMap.HasTile(new Vector3Int(x, y, 0))){//ショップ
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
                if(tile.sprite.name.Contains("kakushi1")){//ショップ優先でその後隠します解除。以後普通の分岐ますに
                    yield return new WaitForSeconds(1f);
                    path1.SetActive(false);
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileBunkiBlue);
                }else if(tile.sprite.name.Contains("kakushi2")){
                    yield return new WaitForSeconds(1f);
                    path2.SetActive(false);
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileBunkiBlue);
                }else if(tile.sprite.name.Contains("kakushi3")){
                    yield return new WaitForSeconds(1f);
                    path3.SetActive(false);
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileBunkiBlue);
                }else if(tile.sprite.name.Contains("kakushi4")){
                    yield return new WaitForSeconds(1f);
                    path4.SetActive(false);
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileBunkiBlue);
                }else if(tile.sprite.name.Contains("kakushi5")){
                    yield return new WaitForSeconds(1f);
                    path5.SetActive(false);
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileBunkiBlue);
                }
            }
            ChangeTurn();
            if(ItemController.skip_flg[players_turn]==1){//スキップフラグ
                ItemController.skip_flg[players_turn]=0;
                yield return new WaitForSeconds(1f);
                SoundEffect.PlayOneShot(PoPiSound);
                message.text = "スキップ！";
                yield return new WaitForSeconds(2f);
                ChangeTurn();
                message.text = "";
            }
            turn.interactable = true;
            ItemPanelButton.interactable = true;
        }
    }
    
    public Image turnImage;
    public Sprite[] turnImages;
    void Update(){
        if(BGM.time > 131f)BGM.time = 0;//BGMをループ
        turnImage.sprite = turnImages[players_turn];
        syojikin.text = "×" + players_coin[players_turn].ToString();
        syojimedal.text = "×" + players_medal[players_turn].ToString();
        float speed = 0.5f;
        Vector3 delta = new Vector3(0,0.5f,0);//パネルの上に立ってるように見える補正
        Vector3 dist = player_destination[players_turn] + delta;
        players[players_turn].transform.position = Vector3.MoveTowards(players[players_turn].transform.position,  dist, speed);//player_destination[players_turn], speed);
        
        if (Input.GetMouseButtonDown(0)){
                Vector3 pos = Input.mousePosition;   
                var bound = tilemap.cellBounds;
                var tile = tilemap.GetTile<Tile>(tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)));
                Debug.Log(tile.sprite.name);
                //Debug.Log(used[players_turn, tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)).x-bound.min.x,
                //                                                 tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)).y-bound.min.y]);
                Debug.Log(tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)));
        }
    }

    public TileBase m_tileYellow;//選択しているタイル
    public Tilemap SarachiMap;//更地マップと黄色ますは同じものを使う
    IEnumerator WaitInput (int nokori, List<List<int>> Nexts) {
        int nexts_index = 0;
        Vector3Int before;
        Vector3Int selectCellPos = new Vector3Int(Nexts[0][0],Nexts[0][1],0);
        TileBase beforeTile = SarachiMap.GetTile<Tile>(selectCellPos);
        SarachiMap.SetTile(selectCellPos, m_tileYellow);
        before = selectCellPos;
        bool canMove=false;
        while(!canMove) {
            if (Input.GetMouseButtonDown(0)){
                Vector3 pos = Input.mousePosition;   
                selectCellPos = SarachiMap.WorldToCell(Camera.main.ScreenToWorldPoint(pos));
                for(int i=0; i<Nexts.Count; i++){
                    if(selectCellPos.x == Nexts[i][0] && selectCellPos.y == Nexts[i][1]){
                        if(before==selectCellPos)canMove=true;
                        nexts_index = i;
                        SarachiMap.SetTile(before, beforeTile);
                        before = selectCellPos;
                        beforeTile =  SarachiMap.GetTile<Tile>(selectCellPos);
                        SarachiMap.SetTile(selectCellPos, m_tileYellow);
                    }
                }
            }
            yield return null;
        }
        SarachiMap.SetTile(selectCellPos, null);
        Walk(nokori, 2, nexts_index);//無限ループ防止用フラグ(分岐で移動しなくなる)
    }

    int[,] kakushi_next = new int[,]{{0, 1}, {1, 0}, {0, 1}, {1, 0}, {0, -1}};//隠しますが見えていない時に進むます
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
        
            if(tilename.Contains("gouryu")){
                List<int> next = new List<int>();
                nx_kouho = x + delta[tilename[6]-'0', 0];
                ny_kouho = y + delta[tilename[6]-'0', 1];
                next.Add(nx_kouho);
                next.Add(ny_kouho);
                Nexts.Add(next);
            }else if (tilename.Contains("kakushi")){//まだ隠しますが見えていない
                List<int> next = new List<int>();
                nx_kouho = x + kakushi_next[tilename[7]-'1', 0];
                ny_kouho = y + kakushi_next[tilename[7]-'1', 1];
                next.Add(nx_kouho);
                next.Add(ny_kouho);
                Nexts.Add(next);
            }else{
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
                    StartCoroutine(Ending(0.3f*i));
                    return;
                }
            }
            
            Debug.Log(tilename);
            Debug.Log(flg);
            int nx, ny;
            if(tilename.Contains("bunki")){//現在分岐ます
                if(flg!=2){//フラグが2の時は分岐からの始動nx, nyを変更できない
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

            flg=0;//歩き始め以外はフラグは0
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
    

    IEnumerator Ending(float t){
        yield return new WaitForSeconds(t);
        message.text = "Player" + players_turn.ToString() + " Goal!";
        yield return new WaitForSeconds(t+2f);
        message.text = "";
        Goaled[players_turn] = true;

        if(Goaled[0] && Goaled[1] && Goaled[2] && Goaled[3])message.text = "GameClear";
        else ChangeTurn();
    }
    
    public AudioClip selectSound;//ボタン選択時の音
    public void PreTurn(){
        SoundEffect.PlayOneShot(selectSound);
        preTurnButton.SetActive(false);
        turnButton.SetActive(true);
        Invoke("Turn",0.25f);
    }
    public ProblemController problemcontroller;
    public void Turn(){
        turn.interactable = false;
        problemcontroller.StartProblem();
        //SceneManager.LoadScene("problem");
    }
    static bool[] Goaled = new bool[]{false, false, false, false};
    public void ChangeTurn(){
        players_turn += ItemController.reverse + GameController.PLAYERS_NUM;
        players_turn %= PLAYERS_NUM;
        while (Goaled[players_turn]){//ゴールした人を飛ばす
            players_turn += ItemController.reverse + GameController.PLAYERS_NUM;
            players_turn %= PLAYERS_NUM;
        }//一つ進める
    }
}   