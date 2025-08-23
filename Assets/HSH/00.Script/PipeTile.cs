using UnityEngine;

[System.Serializable]
public enum PipeType
{
    Empty,      // �� Ÿ��
    Straight,   // ���� ������ (�� �Ǵ� ��)
    Corner,     // �ڳ� ������ (��, ��, ��, ��)
    TShape,     // T�� ������ (��, ��, ��, ��)
    Cross,      // ���� ������ (��)
    Start,      // ������
    End         // ����
}

[System.Serializable]
public class PipeTile
{
    public PipeType type;
    public int rotation; // 0, 90, 180, 270 degrees
    public bool isConnected;
    public GameObject tileObject;

    // �� ������ ���� ���� ���� (��, ��, ��, ��)
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
        // ���� ���� �ʱ�ȭ
        for (int i = 0; i < 4; i++) connections[i] = false;

        switch (type)
        {
            case PipeType.Straight:
                if (rotation % 180 == 0) // ����
                {
                    connections[1] = true; // ��
                    connections[3] = true; // ��
                }
                else // ����
                {
                    connections[0] = true; // ��
                    connections[2] = true; // ��
                }
                break;

            case PipeType.Corner:
                int rotIndex = rotation / 90;
                switch (rotIndex)
                {
                    case 0: // ��
                        connections[0] = true; // ��
                        connections[1] = true; // ��
                        break;
                    case 1: // ��
                        connections[1] = true; // ��
                        connections[2] = true; // ��
                        break;
                    case 2: // ��
                        connections[2] = true; // ��
                        connections[3] = true; // ��
                        break;
                    case 3: // ��
                        connections[3] = true; // ��
                        connections[0] = true; // ��
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
                // ��� ���� ���� ����
                for (int i = 0; i < 4; i++)
                    connections[i] = true;
                break;
        }
    }
}
