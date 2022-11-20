using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed; //최대 스피드
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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
}
