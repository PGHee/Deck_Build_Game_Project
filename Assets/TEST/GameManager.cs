using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo
{
    public int stageNum;
    public int[] stageDropGold;
    public GameObject[][] stageMonsters;
    public int mapNum;
    public int[] mapList;

    public StageInfo(int stageNum, int[] stageDropGold, GameObject[][] stageMonsters, int mapNum, int[] mapList)
    {
        this.stageNum = stageNum;   // �������� ��ȣ, �������� �Ѿ�� +1 ���ٰ�, �������� ���� �ε����� ���� 
        this.stageDropGold = stageDropGold; // �ش� ������������ ����ϴ� ����� ����Ʈ(5�� �� ���� ���)
        this.stageMonsters = stageMonsters; // �� ������������ ������ �� �ִ� ���� �������� 2�� �迭
        this.mapNum = mapNum;
        this.mapList = mapList; // �̵� ������ ���� ����, Ȯ���� ������ ����
    }
}


public class GameManager : MonoBehaviour
{
    public StageManager stageManager;
    public DeckManager deckManager;
    public StageInfo stageInfos;

    // Start is called before the first frame update
    void Start()
    {
        stageInfos = new StageInfo(1, new int[] { 10, 20, 30, 40, 50 }, null, 1, new int[] { 1, 2, 3, 4, 5 });
        stageInfos.stageMonsters = stageMonstersList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetBattle()
    {
        // ���� ���� ����
        stageInfos.stageMonsters = stageMonstersList(); // �������� �� ���� �迭 �ʱ�ȭ

        // ����� �ʱ�ȭ

        // ���� �ʱ�ȭ

        // ���� �ʱ�ȭ

        // ī�� �ʱ�ȭ

        // ����׿� ��� �κ�
    }

    public GameObject[][] stageMonstersList()
    {
        GameObject[] dest = new GameObject[1];
        GameObject[][] dest2 = new GameObject[5][];
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < 1; i++)  //�ݺ��� ���� ���� ���� �ٸ��� ���� ����
            {
                dest[i] = Resources.Load<GameObject>($"Prefabs/Monster1");
            }
            dest2[j] = dest;
        }
        return dest2;
    }
}
