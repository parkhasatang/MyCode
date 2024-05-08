using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public GameObject[] floors;  // 바닥 타일 배열
    public Transform player;     // 플레이어의 위치
    public float tileSize = 40f; // 타일의 세로 크기
    public float tileWidth = 30f; // 타일의 가로 크기

    // 타일이 최근에 이동했는지 추적
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

            // 플레이어가 타일의 상단 경계를 넘어섰는지 확인
            if (distanceY > tileSize && player.position.y > floor.transform.position.y && !hasMoved)
            {
                // 타일을 맵의 하단으로 이동
                floor.transform.position += new Vector3(0, tileSize * floors.Length, 0);
                hasMoved = true;
            }
            // 플레이어가 타일의 하단 경계를 넘어섰는지 확인
            else if (distanceY > tileSize && player.position.y < floor.transform.position.y && !hasMoved)
            {
                // 타일을 맵의 상단으로 이동
                floor.transform.position -= new Vector3(0, tileSize * floors.Length, 0);
                hasMoved = true;
            }
            // 플레이어가 타일의 우측 경계를 넘어섰는지 확인
            else if (distanceX > tileWidth && player.position.x > floor.transform.position.x && !hasMoved)
            {
                // 타일을 맵의 좌측으로 이동
                floor.transform.position += new Vector3(tileWidth * floors.Length, 0, 0);
                hasMoved = true;
            }
            // 플레이어가 타일의 좌측 경계를 넘어섰는지 확인
            else if (distanceX > tileWidth && player.position.x < floor.transform.position.x && !hasMoved)
            {
                // 타일을 맵의 우측으로 이동
                floor.transform.position -= new Vector3(tileWidth * floors.Length, 0, 0);
                hasMoved = true;
            }
        }

        // 타일이 이동한 후, 한 프레임 대기
        if (hasMoved)
        {
            Invoke("ResetMoveFlag", 0.1f);
        }
    }

    // 이동 플래그 재설정
    void ResetMoveFlag()
    {
        hasMoved = false;
    }
}
