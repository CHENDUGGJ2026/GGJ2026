using UnityEngine;

public class NPCMoveController : MonoBehaviour
{
    private INPCAnim nPCAnim;
    private Rigidbody2D rb;
    [SerializeField] float speed;
    private float aliveTime; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        nPCAnim = GetComponent<INPCAnim>();
        aliveTime = 30f;
    }

    private void Update()
    {
        if(aliveTime > 0)
        {
            aliveTime -= Time.deltaTime;
        }else
        {
            Destroy(gameObject);
        }
    }
    public void NPCMove(float x,float y)
    {
        float horizontal = x;
        float vertical = y;
        Vector3 moveDir = new Vector3(horizontal, vertical, 0).normalized;
        rb.velocity = moveDir * speed;

        // 方向定义：1=上 2=下 3=左 4=右（你可以根据自己的动画参数调整）
        if (vertical > 0)
        {
            nPCAnim.Walk(1); // 向上走
        }
        else if (vertical < 0)
        {
            nPCAnim.Walk(2); // 向下走
        }
        else if (horizontal < 0)
        {
            nPCAnim.Walk(3); // 向左走
        }
        else if (horizontal > 0)
        {
            nPCAnim.Walk(4); // 向右走
        }
    }

}
