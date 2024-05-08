using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public GameObject[] floors;  // �ٴ� Ÿ�� �迭
    public Transform player;     // �÷��̾��� ��ġ
    public float tileSize = 40f; // Ÿ���� ���� ũ��
    public float tileWidth = 30f; // Ÿ���� ���� ũ��

    // Ÿ���� �ֱٿ� �̵��ߴ��� ����
    private bool hasMoved = false;

    void Update()
    {
        CheckTilePosition();
    }

    void CheckTilePosition()
    {
        foreach (GameObject floor in floors)
        {
            float distanceY = Mathf.Abs(player.position.y - floor.transform.position.y);
            float distanceX = Mathf.Abs(player.position.x - floor.transform.position.x);

            // �÷��̾ Ÿ���� ��� ��踦 �Ѿ���� Ȯ��
            if (distanceY > tileSize && player.position.y > floor.transform.position.y && !hasMoved)
            {
                // Ÿ���� ���� �ϴ����� �̵�
                floor.transform.position += new Vector3(0, tileSize * floors.Length, 0);
                hasMoved = true;
            }
            // �÷��̾ Ÿ���� �ϴ� ��踦 �Ѿ���� Ȯ��
            else if (distanceY > tileSize && player.position.y < floor.transform.position.y && !hasMoved)
            {
                // Ÿ���� ���� ������� �̵�
                floor.transform.position -= new Vector3(0, tileSize * floors.Length, 0);
                hasMoved = true;
            }
            // �÷��̾ Ÿ���� ���� ��踦 �Ѿ���� Ȯ��
            else if (distanceX > tileWidth && player.position.x > floor.transform.position.x && !hasMoved)
            {
                // Ÿ���� ���� �������� �̵�
                floor.transform.position += new Vector3(tileWidth * floors.Length, 0, 0);
                hasMoved = true;
            }
            // �÷��̾ Ÿ���� ���� ��踦 �Ѿ���� Ȯ��
            else if (distanceX > tileWidth && player.position.x < floor.transform.position.x && !hasMoved)
            {
                // Ÿ���� ���� �������� �̵�
                floor.transform.position -= new Vector3(tileWidth * floors.Length, 0, 0);
                hasMoved = true;
            }
        }

        // Ÿ���� �̵��� ��, �� ������ ���
        if (hasMoved)
        {
            Invoke("ResetMoveFlag", 0.1f);
        }
    }

    // �̵� �÷��� �缳��
    void ResetMoveFlag()
    {
        hasMoved = false;
    }
}
