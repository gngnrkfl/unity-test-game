using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager manager;
    public float maxSpeed; //최대 스피드
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D playercolli;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        playercolli= GetComponent<CapsuleCollider2D>();
    }
    void Update()
    {
        // 점프
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        // 키보드에서 손을 떼면 속도를 급격히 내림
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // 방향전환(Direction Sprite)        
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; //왼쪽키 누르면 filpX를 켬 (fipX는 X축 반전)

        // 움직이거나 정지시 애니메이션
        if (Mathf.Abs(rigid.velocity.x) < 0.3) //속도가 0.3 이하로 내려가면
            anim.SetBool("isWalking", false); //애니메이터의 isWalking 파라미터를 끔
        else
            anim.SetBool("isWalking", true); //애니메이터의 isWalking 파라미터를 켬
    }
    void FixedUpdate()
    {
        // 움직일때
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse); //입력된 방향으로 힘을 더함

        //최대 스피드 설정
        if(rigid.velocity.x > maxSpeed) //오른쪽 Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) //왼쪽 Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //플랫폼에 닿았을 때
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); //rigid.position 즉 오브젝트의 중심에서 녹색 레이저를 아래로 쏨
            /*아래의 RaycastHit은 레이저를 쏘지만 보이지 않기 때문에 위의 디버그를 이용하여 눈에 보이게 표시*/
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); //레이저가 오브젝트에 닿았을 때
            if (rayHit.collider != null)
            { //레이저가 오브젝트에 맞았을때
                if (rayHit.distance < 0.5f) // 오브젝트에 닿았을 때 ray의 길이가 0.5이하가 되면(0.5는 플레이어의 collider 길이의 절반)
                    anim.SetBool("isJumping", false);
            }
        }
    }

    /* 다른 오브젝트와 충돌하는지 체크(물리적 첩촉)
     OnCollisionEnter2D : 2개의 충돌체의 isTrigger가 꺼져 있으면 호출=>물리적 접촉시 */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy") //충돌한 오브젝트의 tag가 Enemy라면
        {
            // 몬스터 공격
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else //공격 당함
                OnDamaged(collision.transform.position);
        }
    }

    /* OnTriggerEnter2D : 2개중 하나의 충돌체의 isTrigger가 켜져 있으면 호출 
    =>물리적 접촉이 아닌 통과될때 (오브젝트에 isTrigger 체크해야함!)*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item") //동전과 닿았을 때
        {
            // 점수획득
            bool isBronze = collision.gameObject.name.Contains("Bronze"); //Bronze라는 단어를 포함하고 있는가
            bool isSilver = collision.gameObject.name.Contains("Silver"); //Silver라는 단어를 포함하고 있는가
            bool isGold = collision.gameObject.name.Contains("Gold"); //Gold라는 단어를 포함하고 있는가
            
            if (isBronze)
                manager.stagePoint += 50;
            else if (isSilver)
                manager.stagePoint += 100;
            else if (isGold)
                manager.stagePoint += 200;

            // 아이템 제거
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            manager.NextStage();
            // 다음 스테이지로

        }
    }

    // 적을 공격했을 때
    void OnAttack(Transform enemy)
    {
        // 점수 획득
        manager.stagePoint += 100;

        // 적을 밟을 때 반발력
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        // EnemyMove 파일의 객체를 생성 후 OnDamaged() 함수 실행
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    // 대미지를 입었을 때
    void OnDamaged(Vector2 targetPos)
    {
        // 체력 감소
        manager.HealthDown();

        // 레이어를 바꿈 (무적상태)
        gameObject.layer = 9;

        // 투명화
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 튕겨져 나감
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 10, ForceMode2D.Impulse);

        //애니메이션
        anim.SetTrigger("Damaged");

        Invoke("OffDamaged", 2); //무적시간 종료
    }

    // 대미지를 회복함
    void OffDamaged()
    {
        gameObject.layer = 8; //레이어 변경 (기본상태)
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        // 스프라이트 색
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // 스프라이트 flipY
        spriteRenderer.flipY = true;
        // Collider 비활성화
        playercolli.enabled= false;
        // 죽는 효과
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VeloctyZero() //속도 초기화
    {
        rigid.velocity = Vector2.zero;
    }
}
