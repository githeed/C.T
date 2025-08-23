using UnityEngine;

[System.Serializable]
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
        rotation = _rotation;
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
        // ��� ���� �ʱ�ȭ
        for (int i = 0; i < 4; i++)
            connections[i] = false;

        // Ÿ�Ժ� �⺻ ���� ���� (ȸ�� 0�� ����)
        bool[] baseConnections = new bool[4];

        switch (type)
        {
            case PipeType.Empty:
                // ���� ����
                break;

            case PipeType.Straight:
                // ���� ���� (��-��)
                baseConnections[1] = true; // ��
                baseConnections[3] = true; // ��
                break;

            case PipeType.Corner:
                // L�� ���� (��-��)
                baseConnections[0] = true; // ��
                baseConnections[1] = true; // ��
                break;

            case PipeType.TShape:
                // T�� ���� (��-��-��)
                baseConnections[0] = true; // ��
                baseConnections[1] = true; // ��
                baseConnections[2] = true; // ��
                break;

            case PipeType.Cross:
                // ���� ���� (��� ����)
                baseConnections[0] = true; // ��
                baseConnections[1] = true; // ��
                baseConnections[2] = true; // ��
                baseConnections[3] = true; // ��
                break;

            case PipeType.Start:
                // ������ - �������θ� ����
                baseConnections[1] = true; // ��
                break;

            case PipeType.End:
                // ���� - �������θ� ����
                baseConnections[3] = true; // ��
                break;
        }

        // ȸ�� ����
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