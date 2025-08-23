using UnityEngine;

[System.Serializable]
public enum PipeType
{
    Empty,      // 빈 타일
    Straight,   // 직선 파이프 (━ 또는 ┃)
    Corner,     // 코너 파이프 (┗, ┏, ┓, ┛)
    TShape,     // T자 파이프 (┣, ┳, ┫, ┻)
    Cross,      // 십자 파이프 (╋)
    Start,      // 시작점
    End         // 끝점
}

[System.Serializable]
public class PipeTile
{
    public PipeType type;
    public int rotation; // 0, 90, 180, 270 degrees
    public bool isConnected;
    public GameObject tileObject;

    // 각 방향의 연결 가능 여부 (상, 우, 하, 좌)
    public bool[] connections = new bool[4];

    public PipeTile(PipeType pipeType, int rot = 0)
    {
        type = pipeType;
        rotation = rot;
        isConnected = false;
        UpdateConnections();
    }

    public void Rotate()
    {
        rotation = (rotation + 90) % 360;
        UpdateConnections();
    }

    public void UpdateConnections()
    {
        // 연결 정보 초기화
        for (int i = 0; i < 4; i++) connections[i] = false;

        switch (type)
        {
            case PipeType.Straight:
                if (rotation % 180 == 0) // 수평
                {
                    connections[1] = true; // 우
                    connections[3] = true; // 좌
                }
                else // 수직
                {
                    connections[0] = true; // 상
                    connections[2] = true; // 하
                }
                break;

            case PipeType.Corner:
                int rotIndex = rotation / 90;
                switch (rotIndex)
                {
                    case 0: // ┗
                        connections[0] = true; // 상
                        connections[1] = true; // 우
                        break;
                    case 1: // ┏
                        connections[1] = true; // 우
                        connections[2] = true; // 하
                        break;
                    case 2: // ┓
                        connections[2] = true; // 하
                        connections[3] = true; // 좌
                        break;
                    case 3: // ┛
                        connections[3] = true; // 좌
                        connections[0] = true; // 상
                        break;
                }
                break;

            case PipeType.TShape:
                for (int i = 0; i < 4; i++)
                {
                    if (i != (rotation / 90))
                        connections[i] = true;
                }
                break;

            case PipeType.Cross:
                for (int i = 0; i < 4; i++)
                    connections[i] = true;
                break;

            case PipeType.Start:
            case PipeType.End:
                // 모든 방향 연결 가능
                for (int i = 0; i < 4; i++)
                    connections[i] = true;
                break;
        }
    }
}
