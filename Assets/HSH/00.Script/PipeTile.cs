using UnityEngine;

public class PipeTile
{
    public PipeType type;
    public int rotation; // 0, 90, 180, 270
    public bool isConnected;
    public bool[] connections; // ��(0), ��(1), ��(2), ��(3)
    public GameObject tileObject;

    public PipeTile(PipeType _type, int _rotation)
    {
        type = _type;
        rotation = _rotation % 360; // ����ȭ
        isConnected = false;
        connections = new bool[4];
        UpdateConnections();
    }

    public void Rotate()
    {
        // �ð�������� 90�� ȸ��
        rotation = (rotation + 90) % 360;
        UpdateConnections();
    }

    public void UpdateConnections()
    {
        // ���� �迭 �ʱ�ȭ
        connections = new bool[4] { false, false, false, false };

        // Ÿ�Ժ� �⺻ ���� ����
        switch (type)
        {
            case PipeType.Empty:
                // ���� ����
                break;

            case PipeType.Straight:
                // ȸ������ ���� ����
                if (rotation == 0 || rotation == 180)
                {
                    // ���� ���� (��-��)
                    connections[1] = true; // ��
                    connections[3] = true; // ��
                }
                else if (rotation == 90 || rotation == 270)
                {
                    // ���� ���� (��-��)
                    connections[0] = true; // ��
                    connections[2] = true; // ��
                }
                break;

            case PipeType.Corner:
                // ȸ���� ���� ���� ����
                if (rotation == 180)
                {
                    // �� ��� (��-��)
                    connections[0] = true; // ��
                    connections[1] = true; // ��
                }
                else if (rotation == 270)
                {
                    // �� ��� (��-��)
                    connections[1] = true; // ��
                    connections[2] = true; // ��
                }
                else if (rotation == 0)
                {
                    // �� ��� (��-��)
                    connections[2] = true; // ��
                    connections[3] = true; // ��
                }
                else if (rotation == 90)
                {
                    // �� ��� (��-��)
                    connections[3] = true; // ��
                    connections[0] = true; // ��
                }
                break;

            case PipeType.TShape:
                // T�� ���� (3����)
                if (rotation == 0)
                {
                    // �� ��� (��-��-��)
                    connections[0] = true; // ��
                    connections[1] = true; // ��
                    connections[3] = true; // ��
                }
                else if (rotation == 90)
                {
                    // �� ��� (��-��-��)
                    connections[0] = true; // ��
                    connections[1] = true; // ��
                    connections[2] = true; // ��
                }
                else if (rotation == 180)
                {
                    // �� ��� (��-��-��)
                    connections[1] = true; // ��
                    connections[2] = true; // ��
                    connections[3] = true; // ��
                }
                else if (rotation == 270)
                {
                    // �� ��� (��-��-��)
                    connections[0] = true; // ��
                    connections[2] = true; // ��
                    connections[3] = true; // ��
                }
                break;

            case PipeType.Cross:
                // ���� ���� (��� ����)
                connections[0] = true; // ��
                connections[1] = true; // ��
                connections[2] = true; // ��
                connections[3] = true; // ��
                break;

            case PipeType.Start:
                // ������ - �������θ� ����
                connections[1] = true; // ��
                break;

            case PipeType.End:
                // ���� - �������θ� ����
                connections[3] = true; // ��
                break;
        }

        // ����� ���
        Debug.Log($"[{type}] ȸ��: {rotation}��, ����: ��({connections[0]}) ��({connections[1]}) ��({connections[2]}) ��({connections[3]})");
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