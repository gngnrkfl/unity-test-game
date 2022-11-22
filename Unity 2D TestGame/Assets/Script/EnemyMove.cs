using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextmove;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D colli;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colli = GetComponent<CapsuleCollider2D>();
        Invoke("Think", 5); //"함수이름"을 5초후에 호출
    }

    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextmove, rigid.velocity.y);

        //플랫폼 체크
        Vector2 frontVec = new Vector2(rigid.position.x + nextmove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0)); //rigid.position 즉 오브젝트의 중심에서 녹색 레이저를 아래로 쏨
        /*아래의 RaycastHit은 레이저를 쏘지만 보이지 않기 때문에 위의 디버그를 이용하여 눈에 보이게 표시*/
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); //레이저가 오브젝트에 닿았을 때
        if (rayHit.collider == null) //레이저가 오브젝트를 벗어났을때
            Turn();
    }

    //재귀 함수
    void Think()
    {
        // 다음활동 조정
        nextmove = Random.Range(-1, 2);
        
        // 애니메이션
        anim.SetInteger("WalkSpeed", nextmove);
        // 애니메이션 방향 조정
        if (nextmove != 0)
            spriteRenderer.flipX = nextmove == 1;

        //재귀(Recursive)
        float nextThinkTime = Random.Range(2, 5);
        Invoke("Think", nextThinkTime);
    }

    // 벽에 부딫혔을때 회전함
    void Turn()
    {
        nextmove *= -1; //방향을 반대로
        spriteRenderer.flipX = nextmove == 1;
        CancelInvoke(); //작동중인 Invoke를 멈춤
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        // 스프라이트 색
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // 스프라이트 flipY
        spriteRenderer.flipY = true;
        // Collider 비활성화
        colli.enabled = false;
        // 죽는 효과
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // 완전 비활성화
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
