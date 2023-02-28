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
    public const int PLAYERS_NUM = 4;
    public Tilemap tilemap;//地図のタイルマップを取得。地図のタイルマップとワールド座標は異なるためGetCellCentorWordlでタイルマップの中心の位置に変換する必要がある。
    public TextMeshProUGUI endingtext;
    

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


    void Start(){
        var builder = new StringBuilder();//タイルマップ表示用プログラム
        var bound = tilemap.cellBounds;
        /*
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
            if(ProblemController.ans >= 0){
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
        yield return new WaitForSeconds(waitTime);
        player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
        audioSource.PlayOneShot(walkSound);
        if(nokori==0){
            yield return new WaitForSeconds(waitTime);//目的地を変えてから直ぐにターン変更すると次のプレイヤーが動いてしまう
            players_turn += ItemController.reverse + GameController.PLAYERS_NUM;
            players_turn %= PLAYERS_NUM;
            turn.interactable = true;
            turnImage.sprite = turnImages[players_turn];
        }
    }
    
    void Update(){
        float speed = 0.5f;
        Vector3 delta = new Vector3(0,0.5f,0);//パネルの上に立ってるように見える補正
        Vector3 dist = player_destination[players_turn] + delta;
        players[players_turn].transform.position = Vector3.MoveTowards(players[players_turn].transform.position,  dist, speed);//player_destination[players_turn], speed);

        if (Input.GetMouseButtonDown(0)){
            Vector3 pos = Input.mousePosition;   
            Vector3Int posI = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos));
            Debug.Log(tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)));
            var tile = tilemap.GetTile<Tile>(posI);
            Debug.Log(tile.name.GetType());
        }
    }


    public TileBase m_tileGray;
    public TileBase m_tileYellow;
    IEnumerator WaitInput (int nokori, List<List<int>> Nexts) {
        int nexts_index = 0;
        Vector3Int before;
        Vector3Int selectCellPos = new Vector3Int(Nexts[0][0],Nexts[0][1],0);
        tilemap.SetTile(selectCellPos,m_tileYellow);
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
                        tilemap.SetTile(selectCellPos,m_tileYellow);
                        tilemap.SetTile(before,m_tileGray);
                        before = selectCellPos;
                    }
                }
            }
            yield return null;
        }
        tilemap.SetTile(selectCellPos,m_tileGray);
        Walk(nokori, 1, nexts_index);//無限ループ防止用フラグ(分岐で移動しなくなる)
    }

    
    public void Walk(int ans, int flg=0, int nexts_index=0){
        if(ans==0){
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
                StartCoroutine(Change(players_position[players_turn, 0], players_position[players_turn, 1], 0, 0.3f));
                return;
            }
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
    

}   