using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour
{
    public GameObject[] effects;         // 각 카드에 사용할 이펙트 프리팹 배열
    public AudioClip[] effectSounds;     // 각 이펙트에 사용할 사운드 클립 배열
    public GameObject[] screenEffectPrefabs;  // 화면 전체에 사용할 이펙트 프리팹 배열 (최대 8개)
    private AudioSource audioSource;     // 사운드를 재생할 AudioSource

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("effectVolume", 1f);  // PlayerPrefs에 저장된 이펙트 볼륨을 불러와 설정
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

    // 화면 전체에 이펙트를 발동시키는 메소드
    public void PlayScreenEffect(int effectIndex)
    {
        // 인덱스 범위가 유효한지 확인 (screenEffectPrefabs 배열의 범위 내에 있어야 함)
        if (effectIndex < 0 || effectIndex >= screenEffectPrefabs.Length)
        {
            Debug.LogError("Invalid screen effect index");
            return;
        }

        StartCoroutine(SpawnScreenEffect(effectIndex));
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
            audioSource.volume = PlayerPrefs.GetFloat("effectVolume", 1f);
            audioSource.PlayOneShot(effectSounds[effectIndex]);
        }

        // 자식 오브젝트들의 ParticleSystem에 대한 처리
        var childParticleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>();
        if (childParticleSystems != null && childParticleSystems.Length > 0)
        {
            int baseSortingOrder = 45;  // 초기 레이어 값 설정
            float maxDuration = 0f;     // 자식 중 가장 긴 파티클 지속 시간

            // 모든 자식 오브젝트 순회
            for (int i = 0; i < childParticleSystems.Length; i++)
            {
                var particleSystem = childParticleSystems[i];
                var particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();

                if (particleSystemRenderer != null)
                {
                    // 순차적으로 레이어 값 설정 (각 자식마다 하나씩 증가)
                    particleSystemRenderer.sortingLayerName = "Effects";
                    particleSystemRenderer.sortingOrder = baseSortingOrder + i;
                }

                // 루프하는 이펙트인 경우 수동으로 종료
                var particleMain = particleSystem.main;
                if (particleMain.loop) particleMain.loop = false; // 무한 루프를 중단

                // 각 자식 오브젝트의 파티클 지속 시간을 계산하여 가장 긴 시간 저장
                float particleDuration = particleMain.duration + particleMain.startLifetime.constantMax;
                if (particleDuration > maxDuration) maxDuration = particleDuration;  // 가장 긴 지속 시간 저장
            }

            // 가장 긴 지속 시간만큼 대기
            yield return new WaitForSeconds(maxDuration);
        }
        else
        {
            // ParticleSystem이 없을 경우 기본 대기 시간 설정 (Fallback)
            yield return new WaitForSeconds(1.0f);
        }

        // 이펙트 오브젝트 삭제
        Destroy(effectInstance);
    }

    // 화면 전체 이펙트 생성 및 삭제 루틴
    private IEnumerator SpawnScreenEffect(int effectIndex)
    {
        // 지정된 인덱스에 해당하는 화면 전체 이펙트 프리팹을 인스턴스화
        GameObject screenEffectInstance = Instantiate(screenEffectPrefabs[effectIndex], Vector3.zero, Quaternion.identity);

        // 카메라 화면 크기에 맞게 이펙트 스케일 조정
        AdjustEffectScale(screenEffectInstance);

        // 화면 전체 이펙트의 ParticleSystem을 가져옴
        var particleSystems = screenEffectInstance.GetComponentsInChildren<ParticleSystem>();
        if (particleSystems != null && particleSystems.Length > 0)
        {
            float maxDuration = 0f;
            foreach (var ps in particleSystems)
            {
                var psRenderer = ps.GetComponent<ParticleSystemRenderer>();
                if (psRenderer != null)
                {
                    psRenderer.sortingLayerName = "Effects";
                    psRenderer.sortingOrder = 95;  // 화면 전체 이펙트는 레이어 값 95로 설정
                }

                // 파티클 지속 시간 계산
                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                if (duration > maxDuration) maxDuration = duration;

                // 무한 루프 제거
                var mainModule = ps.main;
                if (mainModule.loop) mainModule.loop = false; // 루프 중단
            }

            // 가장 긴 지속 시간만큼 대기
            yield return new WaitForSeconds(maxDuration);
        }

        // 이펙트 오브젝트 삭제
        Destroy(screenEffectInstance);
    }

    // 화면 크기에 맞게 이펙트 스케일 조정
    private void AdjustEffectScale(GameObject effectInstance)
    {
        // 카메라의 orthographicSize를 기반으로 화면의 월드 좌표 크기를 계산
        Camera mainCamera = Camera.main;
        float screenAspect = (float)Screen.width / Screen.height;
        float cameraHeight = 0.5f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * screenAspect;

        // 효과 오브젝트와 자식 오브젝트들의 스케일을 모두 조정
        var transforms = effectInstance.GetComponentsInChildren<Transform>();

        foreach (var childTransform in transforms)
        {
            childTransform.localScale = new Vector3(cameraWidth, cameraHeight, 1f);
        }
    }
}
