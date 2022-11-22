using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartButton;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString(); //포인트 UI 변경
    }

    public void NextStage() // 다음 스테이지로 넘어갈 때
    {
        // 스테이지 변경
        if (stageIndex + 1 < Stages.Length)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1).ToString(); // 스테이지 UI 변경
        }
        else // 게임 클리어
        {
            Time.timeScale = 0; // 시간 정지
            Debug.Log("게임 클리어");
            
            Text btnText = RestartButton.GetComponentInChildren<Text>();
            btnText.text = "Clear!"; //버튼 구문 변경
            RestartButton.SetActive(true); //리스타트 버튼 활성화
        }
        

        // 포인트 총합
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if(health > 1)
        {
            health--;
        }
        else
        {
            // 캐릭터 죽음 이펙트
            player.OnDie();
            // 결과창
            Debug.Log("게임 오버");
            //리스타트 버튼 활성화
            Invoke("timebutton", 2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (health > 1) // 체력이 1이상일 때만 원위치
            {
                PlayerReposition();
            }
            HealthDown(); // 체력감소
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(2, 0.5f, 0); //리포지션
        player.VeloctyZero(); //낙하속도 0
    }

    public void Restart() //게임 재시작
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    void timebutton()
    {
        RestartButton.SetActive(true);
    }
}
