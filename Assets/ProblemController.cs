using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;//ランダム変数用
using UnityEngine.UI;

public class ProblemController : MonoBehaviour
{
    string[] problem_list = new string [] {"", "log<sub>2</sub>4=", "1+2=", "sin<sup>2</sup><i>Θ</i>+cos<sup>2</sup><i>Θ</i>=", "<i>a</i><sub>1</sub>=2, <i>a<sub>n</i>+1</sub>=2<i>a<sub>n</sub></i>+1, <i>a</i><sub>3</sub>=</i>", "(2+<i>i</i>)(2-<i>i</i>)=", "2sin(<i>π</i>/4)cos(<i>π</i>/4)=", "log<sub>3</sub>9=", "2<sup>2</sup>=", "-6cos<i>π</i>="};
    //string[] ans_list = new string [] {"", "90", "90", "90", "90", "90", "90", "90", "90", "90"};
    //string[] ans_list = new string [] {"", "15", "15", "15", "15", "15", "15", "15", "15", "15"};
    //string[] ans_list = new string [] {"", "1", "1", "1", "1", "1", "1", "1", "1", "1"};
    string[] ans_list = new string [] {"", "3", "3", "3", "3", "3", "3", "3", "3", "3"};
    public TextMeshProUGUI Problem;
    public TextMeshProUGUI Timer;
    public TMP_InputField Answer;
    bool isTimeUp = false;
    bool isAnswered = false;
    float time;
    public static bool isWalk;
    public static int ans;

    public TextMeshProUGUI problem1;
    public TextMeshProUGUI problem2;
    public TextMeshProUGUI problem3;
    public TextMeshProUGUI problem4;
    public TextMeshProUGUI problem5;
    public TextMeshProUGUI problem6;

    public GameObject blackboard;
    System.Random saikoro = new System.Random();
    int one;
    int two;
    int three;
    int four;
    int five;
    int six;
    int last_problem;
    int me = 0;
    public Button dice;
    public AudioSource SoundEffect;//ProblemControllerObjectに追加したオーディオソースコンポーネント
    public AudioSource BGM;//ProblemControllerObjectに追加したオーディオソースコンポーネント
    public AudioClip taikoSound;
    bool moveDice;
    public GameObject ProblemPanel;
    public void StartProblem()
    {
        BGM.volume /= 3f;
        Answer.text = "答えを入れよう！";
        inputfield.interactable = true;
        time = 1000000000000000000f;
        ProblemPanel.SetActive(true);
        moveDice=false;
        SoundEffect.PlayOneShot(taikoSound);
        dice.interactable = true;
        one = saikoro.Next(1,10);
        two = saikoro.Next(1,10);
        three = saikoro.Next(1,10);
        four = saikoro.Next(1,10);
        five = saikoro.Next(1,10);
        six = saikoro.Next(1,10);
        problem1.text = problem_list[one];
        problem2.text = problem_list[two];
        problem3.text = problem_list[three];
        problem4.text = problem_list[four];
        problem5.text = problem_list[five];
        problem6.text = problem_list[six];

        isAnswered = false;
        StartCoroutine(MoveDice());
    }

    IEnumerator MoveDice(){//サイコロの動き始めを遅らせる
        yield return new WaitForSeconds(1f);
        moveDice=true;
    }

    
    // Update is called once per frame
    float currentTime = 0f;
    public Image diceImage;
    public Sprite[] diceImages;
    public AudioClip SaikoroSound;//ピピピ音
    public AudioClip SaikoroEndSound;//ピコン
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime>0.1f && dice.interactable && moveDice && ProblemPanel.activeSelf){
            me+=1;
            me%=6;
            currentTime = 0f;
            diceImage.sprite = diceImages[me];
            SoundEffect.PlayOneShot(SaikoroSound);
        }
        
        if (0 < time && time<=10) {//10秒にセットされないと減らない。
            time -= Time.deltaTime;
            Timer.text = "Timer:"+time.ToString("F1");
        }else if (time < 0 && isTimeUp==false && isAnswered==false){
            SoundEffect.Stop();//時計の音を止める
            SoundEffect.PlayOneShot(batu);
            isTimeUp = true;
            GameController.players_coin[GameController.players_turn]+=3;
            gamecontroller.ChangeTurn();
            //GameController.players_turn += ItemController.reverse + GameController.PLAYERS_NUM;//時間切れのときはこのタイミングでターン変更
            //GameController.players_turn %= GameController.PLAYERS_NUM;
            StartCoroutine(Erase(3));//時間切れ
        }
    }
    public GameController gamecontroller;
    
    IEnumerator Erase(float time){
        if (isTimeUp && isAnswered==false)Problem.text = "Time up";
        yield return new WaitForSeconds(time);
        blackboard.SetActive(false);
        maru_image.SetActive(false);
        batu_image.SetActive(false);
        if(isAnswered)isWalk = true;
        ProblemPanel.SetActive(false);
        gamecontroller.ReturnFromProblem();
        BGM.volume = 10f;
        //SceneManager.LoadScene("SampleScene");
    }

    //InputFieldの文字が変更されたらコールバックされる。
    //TMProの、InputFieldである、AnswerWindow、のOn End Editによって、GameMasterの、この関数(InputText)を選択し、コールバックできるようにした
    public AudioClip maru;
    public AudioClip batu;
    public GameObject maru_image;
    public GameObject batu_image;
    public TMP_InputField inputfield;
    public void InputText(){
        inputfield.interactable = false;
        SoundEffect.Stop();//時計の音を止める
        Problem.text += ans_list[last_problem];//答えを表示する
        if(Answer.text == ans_list[last_problem] && isAnswered==false){
            SoundEffect.PlayOneShot(maru);
            maru_image.SetActive(true);
            ans = int.Parse(ans_list[last_problem]);
        }else if (isTimeUp==false && isAnswered==false){
            SoundEffect.PlayOneShot(batu);
            batu_image.SetActive(true);
            ans = -int.Parse(ans_list[last_problem]);
        }
        isAnswered = true;
        Timer.text = "";
        time =- 1;//タイマーが減らないようにする
        StartCoroutine(Erase(3f));
    }

    
    public void Dice(){
        SoundEffect.PlayOneShot(SaikoroEndSound);
        dice.interactable = false;
        int [] selected_problems = {one, two, three, four, five, six};
        last_problem = selected_problems[me];
        StartCoroutine(Show());
    }
    public AudioClip syutsudai;
    public AudioClip tokeiSound;
    public IEnumerator Show(){
        yield return new WaitForSeconds(3f);
        blackboard.SetActive(true);
        Problem.text = "Solve me!<br>"+problem_list[last_problem];
        SoundEffect.PlayOneShot(syutsudai);
        yield return new WaitForSeconds(0.5f);
        //time *= 3f;
        time = 10f;
        SoundEffect.PlayOneShot(tokeiSound);
    }

    public GameObject PreSceneBack;//ボタンオブジェクト
    public GameObject SceneBack;
    public AudioClip selectSound;//ボタン選択時の音
    public void PreBack(){
        SoundEffect.PlayOneShot(selectSound);
        PreSceneBack.SetActive(false);
        SceneBack.SetActive(true);
        Invoke("Back",0.25f);
    }
    public void Back(){
        gamecontroller.ReturnFromProblem();
         ProblemPanel.SetActive(false);
         PreSceneBack.SetActive(true);
        SceneBack.SetActive(false);
    }
    
}
