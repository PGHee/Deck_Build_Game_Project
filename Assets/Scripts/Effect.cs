using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour
{
    public GameObject[] effects;        // 각 카드에 사용할 이펙트 프리팹 배열
    public AudioClip[] effectSounds;    // 각 이펙트에 사용할 사운드 클립 배열
    private AudioSource audioSource;    // 사운드를 재생할 AudioSource

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();   // AudioSource 컴포넌트를 추가합니다.
    }

    // 특정 카드 효과를 적용하는 메소드
    public void ApplyEffect(GameObject targetMonster, int effectIndex, int hitCount, float hitInterval)
    {
        if (effectIndex < 0 || effectIndex >= effects.Length)
        {
            Debug.LogError("Invalid effect index");
            return;
        }

        Vector3 targetPosition = targetMonster.transform.position;
        StartCoroutine(ShowEffectRoutine(targetPosition, effectIndex, hitCount, hitInterval));
    }

    public void ApplyAreaEffect(List<GameObject> targetMonsters, int effectIndex, int hitCount, float hitInterval)
    {
        if (effectIndex < 0 || effectIndex >= effects.Length)
        {
            Debug.LogError("Invalid effect index");
            return;
        }

        List<Vector3> targetPositions = new List<Vector3>();
        foreach (var target in targetMonsters)
        {
            targetPositions.Add(target.transform.position);
        }
        StartCoroutine(ShowAreaEffectRoutine(targetPositions, effectIndex, hitCount, hitInterval));
    }

    private IEnumerator ShowEffectRoutine(Vector3 targetPosition, int effectIndex, int hitCount, float hitInterval)
    {
        for (int i = 0; i < hitCount; i++)
        {
            yield return new WaitForSeconds(hitInterval);
            StartCoroutine(SpawnEffect(targetPosition, effectIndex, hitInterval));
        }
    }

    private IEnumerator ShowAreaEffectRoutine(List<Vector3> targetPositions, int effectIndex, int hitCount, float hitInterval)
    {
        for (int i = 0; i < hitCount; i++)
        {
            yield return new WaitForSeconds(hitInterval);
            foreach (var position in targetPositions)
            {
                StartCoroutine(SpawnEffect(position, effectIndex, hitInterval)); // 타격 간격 조정
            }
        }
    }

    private IEnumerator SpawnEffect(Vector3 targetPosition, int effectIndex, float delay)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        Vector3 spawnPosition = targetPosition + randomOffset;

        // 이펙트 인스턴스 생성
        GameObject effectInstance = Instantiate(effects[effectIndex], spawnPosition, Quaternion.identity);

        // 이펙트 사운드 재생
        if (effectSounds != null && effectSounds.Length > effectIndex)
        {
            audioSource.PlayOneShot(effectSounds[effectIndex]);
        }

        var spriteRenderer = effectInstance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Effects";
            spriteRenderer.sortingOrder = 10;
        }
        
        // 애니메이션이 끝날 때까지 대기
        var animator = effectInstance.GetComponent<Animator>();
        float animationDuration = animator != null ? animator.GetCurrentAnimatorStateInfo(0).length : 1.0f;
        yield return new WaitForSeconds(animationDuration);

        Destroy(effectInstance);
    }
}
