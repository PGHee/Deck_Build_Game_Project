using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingPanelManager : MonoBehaviour
{
    public GameObject endingPanel;

    // 플레이어 부문 텍스트, 속성 부문 텍스트, 스테이지 부문 텍스트, 몬스터 부문 텍스트
    public TextMeshProUGUI playerSectionText;
    public TextMeshProUGUI attributeSectionText;
    public TextMeshProUGUI stageSectionText;
    public TextMeshProUGUI monsterSectionText;
    public TextMeshProUGUI finalScoreText;

    // 성취도 게이지 및 별 아이콘
    public Slider achievementGauge;
    public Image[] achievementStars; // 5개 별 오브젝트
    public Image[] sectionBorder;
    public TextMeshProUGUI achievementText;
    public Image endingImage;
    public Sprite[] endingSprite;

    // 점수 계산 배수
    public float levelMultiplier = 200f;
    public float experienceMultiplier = 2f;
    public float crystalMultiplier = 2f;
    public float artifactMultiplier = 300f;
    public float attributeLevelMultiplier = 80f;
    public float attributeExperienceMultiplier = 1f;
    public float stageMultiplier = 300f;
    public float fightMultiplier = 20f;
    public float eventMultiplier = 25f;
    public float monsterMultiplier = 20f;
    public float eliteMultiplier = 50f;
    public float bossMultiplier = 100f;

    // 플레이어 상태 및 게임 디렉터 참조
    private PlayerState playerState;
    private GameDirector gameDirector;
    private ArtifactManager artifactManager;
    private int totalScore = 0;

    // 성취도 단계
    private int[] achievementThresholds = { 500, 1000, 2000, 3000, 4000 };

    private void Start()
    {
        // 플레이어 상태 및 게임 디렉터 참조
        playerState = FindObjectOfType<PlayerState>();
        gameDirector = FindObjectOfType<GameDirector>();
        artifactManager = FindObjectOfType<ArtifactManager>();

        endingPanel.SetActive(false);
    }

    // 엔딩 패널 활성화
    public void ActivateEndingPanel()
    {
        endingPanel.SetActive(true);
        endingImage.enabled = false;
        StartCoroutine(DisplayScoresWithDelay());
    }

    public void HideEndingPanel()
    {
        endingPanel.SetActive(false);
        SceneManager.LoadScene("StartScene");
    }

    // 점수 계산 및 시간차 출력
    private IEnumerator DisplayScoresWithDelay()
    {
        yield return new WaitForSeconds(2f);
        // 1. 플레이어 부문 점수 계산 및 출력
        sectionBorder[0].enabled = true;
        if(playerState.currentHealth >= 0)
        {
            int playerLevelScore = Mathf.Max(0, (playerState.level - 3) * (int)levelMultiplier);
            int playerExperienceScore = Mathf.FloorToInt(playerState.experience * experienceMultiplier);
            int playerCrystalScore = Mathf.FloorToInt(playerState.crystal * crystalMultiplier);
            int artifactScore = artifactManager.artifactGrade + 1; 
            
            playerSectionText.text = $"◈레벨: {playerLevelScore}점\n◈경험치: {playerExperienceScore}점\n◈마석: {playerCrystalScore}점\n";
            playerSectionText.text = $"◈레벨: {playerLevelScore}\n◈경험치: {playerExperienceScore}\n◈마석: {playerCrystalScore}\n◈아티팩트 등급: {artifactScore}";
            //int playerTotalScore = playerLevelScore + playerExperienceScore + playerCrystalScore;
            int playerTotalScore = playerLevelScore + playerExperienceScore + playerCrystalScore + artifactScore;
            StartCoroutine(SmoothUpdateAchievementGauge(totalScore, playerTotalScore));
            totalScore += playerTotalScore;
        }
        else playerSectionText.text = "플레이어의 사망으로 점수를 획득할 수 없습니다.";

        yield return new WaitForSeconds(2f);

        // 2. 속성 부문 점수 계산 및 출력
        sectionBorder[0].enabled = false;
        sectionBorder[1].enabled = true;
        if(playerState.currentHealth >= 0)
        {
            int attributeLevelScore = 0;
            int attributeExperienceScore = 0;

            foreach (var attribute in playerState.attributeMastery)
            {
                attributeLevelScore += (attribute.Value - 1) * (int)attributeLevelMultiplier;
                attributeExperienceScore += Mathf.FloorToInt(playerState.attributeExperience[attribute.Key] * attributeExperienceMultiplier);
            }
            attributeSectionText.text = $"◈속성 레벨: {attributeLevelScore}점\n◈속성 경험치: {attributeExperienceScore}점";
            int attributeTotalScore = attributeLevelScore + attributeExperienceScore;
            StartCoroutine(SmoothUpdateAchievementGauge(totalScore, attributeTotalScore));
            totalScore += attributeTotalScore;
        }
        else attributeSectionText.text = "플레이어의 사망으로 점수를 획득할 수 없습니다.";
        
        yield return new WaitForSeconds(2f);

        // 3. 스테이지 부문 점수 계산 및 출력
        sectionBorder[1].enabled = false;
        sectionBorder[2].enabled = true;
        int stageScore = Mathf.FloorToInt((gameDirector.currentStage - 1) * stageMultiplier);
        int fightScore = Mathf.FloorToInt(gameDirector.enterFight * fightMultiplier);
        int eventScore = Mathf.FloorToInt(gameDirector.enterEvent * eventMultiplier);

        stageSectionText.text = $"◈도달한 스테이지: {stageScore}점\n◈전투 횟수: {fightScore}점\n◈이벤트 횟수: {eventScore}점";
        int stageTotalScore = stageScore + fightScore + eventScore;
        StartCoroutine(SmoothUpdateAchievementGauge(totalScore, stageTotalScore));
        totalScore += stageTotalScore;
        yield return new WaitForSeconds(2f);

        // 4. 몬스터 부문 점수 계산 및 출력
        sectionBorder[2].enabled = false;
        sectionBorder[3].enabled = true;
        int monsterScore = Mathf.FloorToInt(gameDirector.killMonster * monsterMultiplier);
        int eliteScore = Mathf.FloorToInt(gameDirector.killElite * eliteMultiplier);
        int bossScore = Mathf.FloorToInt(gameDirector.killBoss * bossMultiplier);

        monsterSectionText.text = $"◈처치한 몬스터: {monsterScore}점\n◈처치한 정예: {eliteScore}점\n◈처치한 보스: {bossScore}점";
        int monsterTotalScore = monsterScore + eliteScore + bossScore;
        StartCoroutine(SmoothUpdateAchievementGauge(totalScore, monsterTotalScore));
        totalScore += monsterTotalScore;

        yield return new WaitForSeconds(2f);
        sectionBorder[3].enabled = false;
        sectionBorder[4].enabled = true;
        finalScoreText.text = $"최종 점수: {totalScore}점";
        endingImage.enabled = true;
    }

    // 성취도 게이지 및 별 아이콘 업데이트 (실시간으로 부드럽게 증가)
    private IEnumerator SmoothUpdateAchievementGauge(int previousScore, int newScore)
    {
        float duration = 1.5f; // 점수 증가에 소요될 시간
        float elapsed = 0f; 
        int initialScore = previousScore;

        int targetScore = previousScore + newScore; // 목표 점수
        int maxThreshold = achievementThresholds[achievementThresholds.Length - 1]; // 마지막 성취도 단계 기준

        int remainingScore = targetScore;

        // 성취도 단계별로 점수를 나눠서 계산
        for (int i = 0; i < achievementStars.Length; i++)
        {
            // 현재 단계의 최대 값 계산
            int threshold = achievementThresholds[i];

            // 이전 단계에서 남은 점수와 현재 단계 점수 비교
            if (remainingScore >= threshold)
            {
                // 남은 점수가 현재 성취도 단계의 최대값을 넘는다면, 별을 획득하고 남은 점수 계산
                remainingScore -= threshold;
                UpdateAchievementStars(threshold); // 별 획득
                achievementGauge.value = 1f; // 게이지를 가득 채움

                // 게이지 초기화
                achievementGauge.value = 0f;
                achievementText.text = $"0 / {threshold}";
            }
            else
            {
                // 남은 점수가 현재 단계의 최대값보다 작다면 게이지를 해당 값만큼 채움
                elapsed = 0f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / duration;
                    int currentScore = Mathf.FloorToInt(Mathf.Lerp(0, remainingScore, t)); // 남은 점수를 부드럽게 증가시킴

                    // 게이지 값 및 텍스트 업데이트
                    achievementGauge.value = (float)currentScore / threshold;
                    achievementText.text = $"{currentScore} / {threshold}";

                    yield return null; // 한 프레임 대기
                }

                // 남은 점수에 따른 최종 업데이트
                achievementGauge.value = (float)remainingScore / threshold;
                achievementText.text = $"{remainingScore} / {threshold}";

                // 다음 단계로 넘어갈 필요 없으므로 종료
                break;
            }
        }
    }

    // 별 아이콘 업데이트 함수
    private void UpdateAchievementStars(int score)
    {
        for (int i = 0; i < achievementStars.Length; i++)
        {
            if (score >= achievementThresholds[i])
            {
                achievementStars[i].enabled = true; // 별 활성화
                endingImage.sprite = endingSprite[i];
            }
            else
            {
                achievementStars[i].enabled = false; // 별 비활성화
            }
        }
    }

}
