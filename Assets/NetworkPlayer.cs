using Mirror;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    void Update()
    {
        if (isOwned) //���������, ���� �� � ��� ����� �������� ���� ������
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float speed = 5f * Time.deltaTime;
            transform.Translate(new Vector2(h * speed, v * speed)); //������ ���������� ��������
        }
    }
}
