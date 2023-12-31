# 내일배움캠프 게임개발 심화 팀 프로젝트

# 만든 사람들
<a href="https://github.com/berylstar/Team_OneHundredEight/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=berylstar/Team_OneHundredEight" />
</a>

<!-- # [🎮다운로드]() -->
# [🎞 시연 영상 보러가기](https://www.youtube.com/watch?v=kELoADzN44g)

### 목차

1. 게임 개요 및 개발 기간
2. 구현 기능
3. 기능 명세서
4. 사용 에셋

## 플레이
<p>
    <img
      src="https://user-images.githubusercontent.com/115692722/276831840-6c424eee-3c8a-48d9-a051-0d5200a685ba.gif"
      width="45%"
      height="350"/>
      <img
  src="https://user-images.githubusercontent.com/115692722/276831856-b0cdc686-87b0-49d5-ad66-480fd093c7e2.gif"
  width="45%"
  height="350"/>
</p>
<p>
    <img
  src="https://user-images.githubusercontent.com/115692722/276831870-66fcd2b3-8945-47bf-8a9d-14be4f0591b4.gif"
  width="45%"
  height="350"/>
      <img
  src="https://user-images.githubusercontent.com/115692722/276835169-91073f5e-9f9a-40fc-a4a8-0c1f49d025b4.gif"
  width="45%"
  height="350"/>
    </p>
<img
  src="https://user-images.githubusercontent.com/115692722/276838131-a6f8545d-234b-4748-bbab-5165797f3329.gif"
    width="45%"
  height="350"
  />
    



---
<br>
<br>

# 1. 게임 개요 및 개발 기간

- **게임명** : `미니대난투`
- **설명** : Photon 멀티 PvP 게임
- **개요** : 상대를 죽여야 내가 사는 이곳에서. 나만의 방식으로 무기를 강화해보자.
- **게임 방법**
    - [WASD] : 이동
    - [SPACE] : 점프
    - [Left Click] : 공격
    - [Right Click] : 폭탄
- **플레이타임** : 5분 이내
- **개발 환경** : Unity 2022.3.2f1
- **타겟 플랫폼** : PC
- **개발 기간** `2023.10.13 ~ 2023.10.20`

---
<br>
<br>

# 2. 구현 기능

## **게임 로비**
- 게임 시작시 실행되는 씬


## **메인 게임**
- 게임은 두가지 단계로 구성된다. `증강 선택`, `배틀`
- 증강 선택에서는 일정한 순서에 따라 놓여진 5개의 증강 중에서 한가지를 선택할 수 있다. 순서는 전 라운드에서 먼저 탈락한 사람부터 먼저 고를 수 있다.
- 증강을 선택해 자신의 무기를 다양한 방식으로 강화시킬수 있다.
- 배틀에서는 플레이어들끼리 전투를 벌이며, 마지막 한 명이 남을 때까지 전투가 진행된다. 맵의 장외로 벗어나는 것도 탈락으로 간주된다.
- 동일한 방식으로 3라운드를 진행하여 승자를 가린다.

---
<br>
<br>

# 3. 기능 명세서

### - **매니저**

| 스크립트 | 내용 | 기여자 |
| -- | -- | -- |
| GameManager | 싱글톤, 전체 게임 로직 관리 | 김민상 |
| ObjectPooling | 포톤 내에서 오브젝트 풀링을 처리 | 김민상 |

### - **플레이어**

| 스크립트 | 내용 | 기여자 |
| -- | -- | -- |
| PlayerController | InputAction을 이용한 플레이어 입력과 이동 처리 | 김민상 |
| PlayerStatHandler | 플레이어 체력, 이동속도, 점프력 등을 관리 | 김민상 |

<!-- [PlayerController](Assets\Scripts\Minsang\PlayerController.cs) -->

### - **무기**

| 스크립트 | 내용 | 기여자 |
| -- | -- | -- |
| ProjectileController | 무기에서 발사된 총알 관리, 오브젝트 풀링 적용 | 김민상 |
| Boom | 폭탄 기능(적 밀고 1.5초 스턴) | 김대민 |

### - **증강**

| 스크립트 | 내용 | 기여자 |
| -- | -- | -- |
| EnhancementManager |증강에 필요한 동작과 ui상태 및 네트워크 동기화 작업 | 김대열 |
| EnhanceUI |상태에 따른 증강 ui 를 그리는 클래스  | 김대열 |
| EnhancementData | 증강 데이터 모델링  | 김대열 |

### - **맵**

| 스크립트 | 내용 | 기여자 |
| -- | -- | -- |
| Break | 시간이 지나면 떨어지는 발판 (시간제한) | 이홍준 |
| Fall | 플레이어가 밟으면 떨어지는 지형 | 이홍준 |
| StageManager | 매 라운드 다른 스테이지, 호스트가 맵을 고르고 포톤으로 모든 접속자에게 동기화 | 이홍준 |

### - **로비 씬**

| 스크립트 | 내용 | 기여자 |
| -- | -- | -- |
| RoomPanel | 대기방 UI 출력  | 김대열, 이세진 |
| ParticipantManager | 참가자들의 상태와 대기방의 상태를 저장하고 통신하는 클래스 | 김대열, 이세진 |
| nickName Panel | 플레이어의 로컬 닉네임을 정함 | 이세진 |
| main Panel | 매인 UI | 이세진 |
| Lobby Panel | 현재 만들어진 룸의 정보를 UI로 구현 | 이세진 |


### - **아이템**

| 스크립트 | 내용 | 기여자 |
| -- | -- | -- |
| DeadZone | 피를 지형에 그리는 기능  | 김대민 |
| Item | 실질적으로 플레이어에 스텟을 적용하는 기능 | 김대민 |
| ItemSpawner | 픽업 아이템을 생성하는 기능 | 김대민 |
| ItemStats.ItemStatSO | 플레이어에 적용될 아이템 스텟 | 김대민 |
| Pickup | 맵에 생성되는 아이템 | 김대민 |

### - **이펙트**

| 스크립트 | 내용 | 기여자 |
| -- | -- | -- |
| BloodDraw | 피를 지형에 그리는 기능  | 김대민 |
| ShakeCamera | 카메라 쉐이킹 | 김대민 |
| Boom | 폭탄 이펙트 구현 | 김대민 |

### - **에셋**
| 스크립트 | 내용 | 기여자 |
| -- | -- | -- |
| 플레이어 스프라이트 | 플레이어 스프라이트, 애니메이션  | 이세진 |
---
<br>
<br>

4. 후기

| 이름 | 내용 |
| -- | -- |
| 김민상 | 포톤을 다루는 팀 프로젝트였던만큼 힘들기도 힘들고 재밌기도 재밌었습니다. 좋은 팀원덕분에 마무리까지 잘 되서 너무 다행입니다. |
| 김대열 | 디버깅이 어려웠지만 통합 테스트하면서 다같이 즐길수 있어서 너무 재밌었습니다. 포톤을 잘 몰라서 힘들었지만 멀티 플레이어라서 다같이 재밌게 즐기면서 해서 좋았습니다. | 
| 김대민 | 여러가지 만들어보고싶은 이펙트적인 기능같은걸 만들어봐서 재미있었습니다 즐겁네요 포톤은 힘들었지만 즐거웠습니다. |
| 이홍준 | 실전압축으로 포톤을 야무지게 공부한 느낌이어서 좋았습니다. 팀원분들과 재밌게 테스트하는 것도 아주 즐거웠습니다. |
| 이세진 | 포톤 서버의 기본적인 구조를 배웠고 멀티플레이를 통해 재밌는 플레이가 좋았습니다 |

# 4. 사용 에셋
- 포톤 PUN 2
- https://assetstore.unity.com/packages/2d/environments/pixel-art-platformer-village-props-166114
- https://wallpapercave.com/1920x1080-desktop-pixel-art-wallpapers
