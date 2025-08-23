using UnityEngine;

[System.Serializable]
public class PipeTile
{
    public PipeType type;
    public int rotation; // 0, 90, 180, 270
    public bool isConnected;
    public bool[] connections; // 상(0), 우(1), 하(2), 좌(3)
    public GameObject tileObject;

    public PipeTile(PipeType _type, int _rotation)
    {
        type = _type;
        rotation = _rotation;
        isConnected = false;
        connections = new bool[4];
        UpdateConnections();
    }

    public void Rotate()
    {
        // 시계방향으로 90도 회전
        rotation = (rotation + 90) % 360;
        UpdateConnections();
    }

    public void UpdateConnections()
    {
        // 모든 연결 초기화
        for (int i = 0; i < 4; i++)
            connections[i] = false;

        // 타입별 기본 연결 설정 (회전 0도 기준)
        bool[] baseConnections = new bool[4];

        switch (type)
        {
            case PipeType.Empty:
                // 연결 없음
                break;

            case PipeType.Straight:
                // 수평 연결 (좌-우)
                baseConnections[1] = true; // 우
                baseConnections[3] = true; // 좌
                break;

            case PipeType.Corner:
                // L자 연결 (상-우)
                baseConnections[0] = true; // 상
                baseConnections[1] = true; // 우
                break;

            case PipeType.TShape:
                // T자 연결 (상-우-하)
                baseConnections[0] = true; // 상
                baseConnections[1] = true; // 우
                baseConnections[2] = true; // 하
                break;

            case PipeType.Cross:
                // 십자 연결 (모든 방향)
                baseConnections[0] = true; // 상
                baseConnections[1] = true; // 우
                baseConnections[2] = true; // 하
                baseConnections[3] = true; // 좌
                break;

            case PipeType.Start:
                // 시작점 - 우측으로만 연결
                baseConnections[1] = true; // 우
                break;

            case PipeType.End:
                // 끝점 - 좌측으로만 연결
                baseConnections[3] = true; // 좌
                break;
        }

        // 회전 적용
        int rotationSteps = rotation / 90;
        for (int i = 0; i < 4; i++)
        {
            if (baseConnections[i])
            {
                int rotatedIndex = (i + rotationSteps) % 4;
                connections[rotatedIndex] = true;
            }
        }
    }
}

public enum PipeType
{
    Empty,
    Straight,
    Corner,
    TShape,
    Cross,
    Start,
    End
}