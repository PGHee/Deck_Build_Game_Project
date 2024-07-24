using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;

public class DamageText : MonoBehaviour
{
    public GameObject[] damageTextPrefab; // 데미지 텍스트 프리팹 배열

    public void ShowDamage(GameObject target, int fontIndex, int damage, int hitCount, float hitInterval)
    {
        MonsterState monsterState = target.GetComponent<MonsterState>();
        PlayerState playerState = target.GetComponent<PlayerState>();
        if (monsterState != null)
        {
            if(monsterState.reduceDamage > 0) damage = Mathf.RoundToInt(damage * (1-monsterState.reduceDamage));
        }
        else if (playerState != null)
        {
            if(playerState.reduceDamage > 0) damage = Mathf.RoundToInt(damage * (1-playerState.reduceDamage));
        }
        Vector3 targetPosition = target.transform.position;
        StartCoroutine(ShowDamageRoutine(targetPosition, fontIndex, damage, hitCount, hitInterval));
    }

    public void ShowAreaDamage(List<GameObject> targets, int fontIndex, int damage, int hitCount, float hitInterval)
    {
        List<Vector3> targetPositions = new List<Vector3>();
        foreach (var target in targets)
        {
            targetPositions.Add(target.transform.position);
        }
        StartCoroutine(ShowAreaDamageRoutine(targetPositions, fontIndex, damage, hitCount, hitInterval));
    }

    private IEnumerator ShowDamageRoutine(Vector3 targetPosition, int fontIndex, int damage, int hitCount, float hitInterval)
    {
        for (int i = 0; i < hitCount; i++)
        {
            yield return new WaitForSeconds(hitInterval);
            CreateAndAnimateDamageText(targetPosition, fontIndex, damage);
        }
    }

    private IEnumerator ShowAreaDamageRoutine(List<Vector3> targetPositions, int fontIndex, int damage, int hitCount, float hitInterval)
    {
        for (int i = 0; i < hitCount; i++)
        {
            yield return new WaitForSeconds(hitInterval);
            foreach (var position in targetPositions)
            {
                CreateAndAnimateDamageText(position, fontIndex, damage);
            }
        }
    }

    private void CreateAndAnimateDamageText(Vector3 position, int fontIndex, int damage)
    {
        // 랜덤 오프셋 생성
        Vector3 randomOffset = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
        Vector3 spawnPosition = position + randomOffset + new Vector3(0, 1.0f, 0);

        // 데미지 텍스트 인스턴스 생성
        GameObject textInstance = Instantiate(damageTextPrefab[fontIndex], spawnPosition, Quaternion.identity, transform);
        TextMeshProUGUI tmp = textInstance.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = damage.ToString();

        StartCoroutine(AnimateDamageText(textInstance)); // 애니메이션 시작
    }

    private IEnumerator AnimateDamageText(GameObject textInstance)
    {
        float duration = 0.5f; // 애니메이션 지속 시간
        float elapsedTime = 0f;
        Vector3 initialPosition = textInstance.transform.position;
        Vector3 targetPosition = initialPosition + new Vector3(0, 1.5f, 0); // 위로 이동되는 속도

        while (elapsedTime < duration)
        {
            textInstance.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 애니메이션이 끝나면 텍스트 삭제
        Destroy(textInstance);
    }
}
