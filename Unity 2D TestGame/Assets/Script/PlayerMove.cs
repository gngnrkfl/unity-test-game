using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
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
        // Ű���忡�� ���� ���� �ӵ��� �ް��� ����
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // ������ȯ(Direction Sprite)        
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; //����Ű ������ filpX�� �� (fipX�� X�� ����)

        // �����̰ų� ������ �ִϸ��̼�
        if (Mathf.Abs(rigid.velocity.x) < 0.3) //�ӵ��� 0.3 ���Ϸ� ��������
            anim.SetBool("isWalking", false); //�ִϸ������� isWalking �Ķ���͸� ��
        else
            anim.SetBool("isWalking", true); //�ִϸ������� isWalking �Ķ���͸� ��
    }
    void FixedUpdate()
    {
        // �����϶�
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse); //�Էµ� �������� ���� ����

        //�ִ� ���ǵ� ����
        if(rigid.velocity.x > maxSpeed) //������ Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) //���� Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
    }
}
