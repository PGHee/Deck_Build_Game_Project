**단일 캐릭터**

- 초반 전투 후 원하는 속성 카드 2가지 선택 제공
- 숙련도 3레벨 이후 해당 속성 등장 확률 증가

**5대 보스**

- 탐욕, 분노, 나태, 색욕, 악마의 왕(사탄)
- 1스테이지는 색욕의 악마
- 2스테이지는 탐욕의 악마
- 3스테이지는 나태의 악마
- 4스테이지는 분노의 악마
- 5스테이지는 “악마의 왕 - 사탄”

**전투 시스템**

- 카드: 사용 즉시 발동
- 코스트: 서클 단일 사용

**속성**

| 속성 | 화 | 수 | 목 | 금 | 지 |
| --- | --- | --- | --- | --- | --- |
| 특징 | 한방 딜 | 치유 | 지속 딜 | 소환 | 방어 |
| 속성 | 뇌 | 풍 | 광 | 흑 | 무 |
| 특징 | 광역 | 다중타격 | 버프 | 디버프 | 특수, 보조 |
- 금(소환) 마법은 영구 유지되는 골렘 1기 소환.
    - 카드는 골렘의 스탯을 상승시키는 형태
        - 공격: 한 턴마다 골렘이 직접 타격하여 적에게 쌓인 공격력만큼 피해를 줌
        - 방어: 한 턴마다 골렘이 플레이어를 보호하여 쌓인 방어력만큼 보호막을 걸어줌
        - 특수: 스택을 쌓는 카드와 별개의 매커니즘으로 동작하는 카드

**몬스터 매커니즘**

- 몬스터들의 전체 행동을 규정해놓은 클래스 1개를 몬스터마다 상속받는 형태로 구현
- 몬스터마다 전체 행동 클래스를 상속받아 그 중에 사용할 동작을 몬스터마다 구현
    - 각 몬스터 개체 별로 어떤 행동을 할 것인지는 랜덤적으로 진행
        - ex. A몬스터의 행동 패턴이 공격, 방어, 버프가 있다면 이 중에 하나를 랜덤 수행
            - 해당 몬스터가 할 행동은 그 해당 턴에 플레이어에게 보임
    - 몬스터 스폰은 1~3마리가 랜덤으로 스폰됨
    - 스폰되는 몬스터는 스테이지마다 다르나 한 스테이지에서 스폰되는 몬스터는 해당 스테이지에서 뜰 수 있는 몬스터 중에 랜덤으로 스폰됨
        - ex. 1스테이지에서 뜨는 몬스터가 3스테이지에서도 스폰 (X)
        - ex. 1스테이지의 1번 맵에서 뜬 몬스터가 1스테이지 4번 맵에서 스폰(O)
- 몬스터의 조직화는 작업량의 사유로 폐지

**숙련도 시스템**

- 같은 속성을 반복적으로 사용할수록 숙련도 포인트가 쌓여 성장하는 방식
- 1레벨부터 최대 10레벨까지 등극 가능
    - 1~9레벨은 해당 속성의 반복 사용으로 성장 가능
    - 10레벨은 초월을 통해 등극 가능.
        - 초월의 조건: 9서클 등극 + 두 가지 속성의 숙련도 9레벨 달성
- 속성 별 특성이 두드러진 특수 패시브를 3레벨, 6레벨, 10레벨에 획득 가능
    - 특수 패시브를 통해 사용하는 해당 속성의 능력들을 강화할 수 있음
- 숙련도 레벨이 2레벨 단위가 될 때마다 해당 속성의 카드를 선택하여 획득 가능
    - ex. 2레벨 달성 시 1~2 서클 카드 중 획득, 4레벨 달성 시 3~4서클 카드 중 획득, ..

**화폐**

- 마나 결정(일명 마석)으로 통일
    - 몬스터 체내에 응축된 마나가 결정 형태로 드랍된다는 컨셉
- 마나 결정을 통해 전투 시 필요에 따라 서클 회복으로 치환 가능
    - 정확한 교환 비는 추후에 결정이 필요
- 마나 결정은 쉼터에서 카드 구매 및 제거, 하위 아티팩트 구매, 치료비로 쓰일 예정

**맵**

- 2갈래의 길 중에 선택하여 나아가는 방식
    - 순간마다의 선택으로 굴려나가는 느낌
- 맵은 일반 전투, 엘리트 전투, 쉼터, 물음표(랜덤)으로 구성될 예정
    - A 갈래는 (일반 전투: 70%, 랜덤: 10%, 엘리트 전투: 10%, 쉼터: 10%)로 등장
    - B 갈래는 (일반 전투: 30%, 랜덤: 30%, 엘리트 전투: 20%, 쉼터: 20%)로 등장

**아티팩트**

- 하위 아이템을 모아서 조합하는 형태의 빌드 시스템
- 조합한 상위 아이템은 하위 아이템의 능력이 점차 강화되는 형태로 빌드
    - 1단계: 약한 패시브
    - 2단계: 패시브 + 액티브
    - 3단계: 강한 패시브 + 강한 액티브 + 특수 능력
- 아티팩트의 액티브는 서클(코스트)을 소모
    - 액티브마다 소모되는 코스트는 다를 예정
- 하위 아티팩트는 엘리트 몬스터(중간 보스)를 통해서 획득

**물속성 변경사항 ※회의록에 작성되지 않은 내용임을 유의※**

- 스택형 치유에서 턴 치유로 변경
- 사유는 밸런스를 잡기 어려움으로 인함

**카드 밸런스 사항 ※회의록에 작성되지 않은 내용임을 유의※**

- 등급과 코스트 간의 격차는 최대 4서클까지만 허용함
    
    ex. 9등급 마법은 최소 6서클의 코스트를 소비하도록 설계되어야 함
    
    ex2. 5등급 마법은 최소 1서클의 코스트를 소비하도록 설계되어야 함
    
- 코스트(소모 서클) 당 데미지는 6으로 고정
    - 별개의 특성은 가중치를 곱하는 방식 (치유, 방어, 독 등)

**튜토리얼 스테이지**

- 첫 회차 플레이에 1~2등급 구간 튜토리얼 스테이지부터 시작
    - 튜토리얼 스테이지 구간에서 3번의 전투를 치룬다.
        - 매 전투 클리어마다 원하는 속성 카드를 1종 획득한다.
        - 획득하는 카드의 속성은 중복 불가능하며, 총 3개의 속성 카드를 획득한다.
    - 2회차 플레이 때는 튜토리얼 스테이지가 스킵된다. (스토리 상의 루프 시작점)
        - 3등급부터 모험을 시작하여 3코스트의 자원으로 전투를 한다.
        - 튜토리얼이 스킵된 경우 전부 다른 속성의 선택 카드 3종을 받고 시작한다.
    - 몬스터의 개체 수는 1스테이지부터 1마리에서 3마리까지 출현 가능하다.
        - 초반은 3마리 등장이 적으며, 스테이지가 오를수록 많이 나올 확률이 증가한다.
    - 1스테이지의 몬스터는 덱 빌드를 위한 구간으로 설정해 약한 개체로만 등장한다.

**숙련도 이점 - 금 속성 더 고려해야 됨 (3레벨 비용 6, 6레벨 비용 21 기준으로 설계됨)**

- 불 속성: (3레벨) 화 속성 카드 데미지 25% 증가 / (6레벨) 데미지 50% 증가
- 물 속성: (3레벨) 턴 당 치유 + 4 / (6레벨) 턴 당 치유 + 14
- 목 속성: (3레벨) 목 속성 카드 지속딜 스택 +1 / (6레벨) 지속딜 스택 +3
- 금 속성: (3레벨) 대상 지정 / (6레벨) 초반 스택 공방 + 10 (고려)
- 지 속성: (3레벨) 턴 당 방어 + 4 / (6레벨) 턴 당 방어 + 14
- 뇌 속성: (3레벨) 뇌 속성 카드 스턴 확률 +15% / (6레벨) 스턴 확률 +30%
    - 보스는 원래 확률의 절반만 적용
- 풍 속성: (3레벨) 풍 속성 카드 타수 +1 / (6레벨) 풍 속성 카드 타수 +2
- 광 속성: (3레벨) 매 턴 안 쓴 카드 덱에 광 속성 카드 1종을 랜덤으로 드로우 / 
              (6레벨) 매 턴 안 쓴 카드 덱에 광 속성 카드 1종을 선택하여 드로우
- 흑 속성: (3레벨) 매 턴 이미 사용한 카드 묘지에서 흑 속성 카드 1종을 랜덤으로 드로우 / 
              (6레벨) 매 턴 이미 사용한 카드 묘지에서 흑 속성 카드 1종을 선택하여 드로우
- 무 속성: (3레벨) 마나 결정 획득량 50% 증가  / (6레벨) 획득량 100% 증가
