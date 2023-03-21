using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    public GameObject Panel1st;
    public GameObject Panel2nd;
    public GameObject Panel3rd;
    public GameObject Panel4th;
    public GameObject Result;
    // Start is called before the first frame update
    public AudioSource SoundEffect;//オーディオソースは透明なゲームオブジェクトについてる。
    public AudioClip Don;
    public AudioClip Hue;
    public AudioClip Kansei;

    public Animator orange;
    public Animator red;
    public Animator blue;
    public Animator yellow;
    IEnumerator Start()
    {
        SoundEffect.PlayOneShot(Hue);
        yield return new WaitForSeconds(2f);
        Panel4th.SetActive(true);
        SoundEffect.PlayOneShot(Don);
        yield return new WaitForSeconds(2f);
        Panel3rd.SetActive(true);
        SoundEffect.PlayOneShot(Don);
        yield return new WaitForSeconds(2f);
        Panel2nd.SetActive(true);
        SoundEffect.PlayOneShot(Don);
        yield return new WaitForSeconds(2f);
        Panel1st.SetActive(true);
        SoundEffect.PlayOneShot(Don);
        yield return new WaitForSeconds(2f);
        Result.SetActive(false);
        SoundEffect.PlayOneShot(Kansei);
        orange.SetBool("cheer", true);
        
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
