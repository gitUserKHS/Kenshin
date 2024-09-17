# 유니티를 이용한 3D RPG 게임 만들기

### 개발 기간 
---
2024.03.16 ~ 2024.08.26

### 개발 인원수
---
1인

### 시현 영상
---
<https://www.youtube.com/watch?v=UDkuBbVpJzU>

### 구현된 기능
---
3인칭 라운드 뷰 카메라 구현
![기능-원형카메라](https://github.com/user-attachments/assets/02ca2032-1415-42e1-8828-8c6124cdd5e4)


CameraController.cs의 HandleRoundView 메서드
```cs
private void HandleRoundView()
{
    mouseDeltaX = Input.GetAxis("Mouse X");
    mouseDeltaY = Input.GetAxis("Mouse Y");
    mouseDeltaX = Mathf.Clamp(mouseDeltaX, -3, 3);

    float cameraUpperLimitY = focusPos.y + focusDist * Mathf.Sin(yAngleUpperLimit);
    float cameraLowerLimitY = focusPos.y + focusDist * Mathf.Sin(yAngleLowerLimit);

    Vector3 deltaCameraMove = transform.right * -mouseDeltaX;
    if (false == (transform.position.y > cameraUpperLimitY && mouseDeltaY < 0 || transform.position.y < cameraLowerLimitY && mouseDeltaY > 0))
        deltaCameraMove += transform.up * -mouseDeltaY;
    camDir = (camDir + deltaCameraMove * sensitivity * Time.deltaTime).normalized;

    transform.position = focusPos + camDir * focusDist;
    transform.LookAt(focusPos);

    Zoom();
}
```

![원형카메라](https://github.com/user-attachments/assets/7ea24105-bbf4-4821-b5eb-10bdf5e6d85b)

설명: HandleRoundView 메서드에서 우선 GetAxis를 이용하여 x축과 y축 방향의 입력 값을 받아옵니다. 그 다음에 1번에 해당하는 현재 카메라의 위치에 입력 값의 방향 만큼을 더한 값을 계산한 2번에 해당하는 위치의 벡터값을 구합니다. 그리고, 벡터를 정규화한 이후에 초점과의 거리를 곱하여 최종적으로 카메라를 3번의 위치로 이동시킨 다음 LookAt함수를 호출하여 카메라를 초점의 방향으로 바라보게끔 하였습니다. <br>
카메라의 높이에 제한을 주기 위하여 최대 각도와 최소 각도 값을 설정하였으며 카메라의 최대 높이는 Sin함수를 이용하여 계산하였습니다.


3인칭, 1인칭 시점 변경 기능
![기능-1인칭카메라](https://github.com/user-attachments/assets/eb8c3565-e63a-4d3d-870c-0f9732e7c719)

CameraController.cs의 CamMode 프로퍼티
```cs
CameraMode camMode = CameraMode.RoundView;
public CameraMode CamMode { 
    get { return camMode; } 
    set
    {
        if(camMode == value) 
            return;

        if (value == CameraMode.FirstPersonView)
        {
            transform.parent = player.transform;
            transform.position = player.transform.TransformPoint(camDir_FirstPerson);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if(value == CameraMode.RoundView)
        {
            camDir = player.transform.forward * -3.5f + player.transform.up * 1.5f;
            transform.parent = Managers.Scene.CurrentScene.transform;
            transform.position = focusPos + camDir;
            transform.rotation = Quaternion.LookRotation(focusPos - transform.position);
        }

        camMode = value;
    }
}
```
설명: 우선, CameraMode라는 enum을 선언하였고, RoundView, FirstPersonView라는 값을 정의하였습니다. <br>
3인칭에서 1인칭으로 바꿀 경우에는 camDir_FirstPerson이라는 로컬 좌표계 기준의 위치값을 transform.TransfromPoint 메서드를 이용하여 글로벌 좌표계 값으로 변환한 후에 transform.position 값을 설정하였습니다. <br>
반대로 1인칭에서 3인칭으로 바꿀 경우에는 초점 위치(focusPos)에 camDir을 더하여 transform.position값을 설정하였습니다.


몬스터가 일정 범위를 벗어났을 때 스폰포인트로 귀환시키기
![기능-몬스터리턴](https://github.com/user-attachments/assets/66c4b075-1de4-4ffb-a851-0e15ae4b0526)

WarriorController.cs의 CoReturn 코루틴
```cs
IEnumerator CoReturn()
{
    while (Mathf.Abs((transform.position - warriorSpawnPoint.transform.position).magnitude) >= 0.5f)
    {
        Vector3 dir= warriorSpawnPoint.transform.position - transform.position;

        rb.MovePosition(transform.position + dir.normalized * Time.deltaTime * Stat.MoveSpeed);
        transform.rotation = Quaternion.LookRotation(dir);

        yield return null;
    }

    State = CreatureState.Idle;
    Stat.RestoreHP(Stat.MaxHp - Stat.Hp);
    coReturn = null;
}
```
설명: 몬스터와 몬스터 스폰포인트와의 거리가 매우 작아질 때까지 스폰포인트를 향해 이동하도록 하였습니다. <br>
이는 따로 귀환하는 state(상태)를 정의함으로써 해결할 수도 있었겠지만, 간단한 작업이기 때문에 UpdateMoving 함수에서 코루틴을 호출하는 것으로 해결하였습니다.


아이템 착용/해제 기능
![기능-아이템착용](https://github.com/user-attachments/assets/543ad56c-adb3-4835-8785-b8f165cc1505)

PlayerController.cs의 일부
```cs
public ArmorItemData[] PlayerArmors { get; set; } = new ArmorItemData[(int)ArmorType.Count];
public WeaponItemData[] PlayerWeapons { get; set; } = new WeaponItemData[(int)WeaponType.Count];
```

설명: 인벤토리에서 해당 아이템을 우클릭하면 플레이어의 스탯값을 변경시키고 스탯창에서 표시되도록 하였습니다. <br>
플레이어가 착용한 아이템의 정보는 플레이어의 스크립트인 PlayerController.cs에서 배열을 선언하여 보관하도록 하였습니다.


게임 오버 기능
![기능-게임오버](https://github.com/user-attachments/assets/7af7c57e-aa38-4f5c-a21b-2ab128fbfe3c)

설명: 게임 오버 시 플레이어를 움직이게 못하도록 만들고 몬스터가 공격하지 않도록 하였습니다. Respawn버튼을 누르면 플레이어가 다시 움직일 수 있으며 몬스터도 다시 공격할 수 있게 됩니다.

### 문제 상황 및 해결책
---
1. 3인칭 라운드 뷰 카메라 모드에서 플레이어가 방향을 바꾸며 이동하면 플레이어가 떨리는 현상이 발견되었습니다: <br>이는 카메라의 LateUpdate문과 플레이어의 Update문의 싱크가 맞지 않아서 생기는 문제라고 생각하여 플레이어의 조작을 담당하는 함수들을 CameraController.cs 안에다가 넣어서 호출되게끔 하였습니다.
```cs
private void LateUpdate()
{
    focusPos = player.transform.position + Vector3.up * 1f;

    switch (CamMode)
    {
        case CameraMode.RoundView:
            HandleRoundView();
            playerController.HandleCharacterInput();
            playerController.ProcessRotation();
            break;
        case CameraMode.FirstPersonView:
            HandleFirstPersonView();
            playerController.HandleCharacterInput();
            break;
    }
}
```

2. 오토타겟팅 기능을 어떻게 구현할 것인가: <br>
이는 Physics.OverlapSphere 메서드를 활용하여 구현하였습니다. 일정 반경 내에 몬스터들이 있으면 for루프를 돌려서 몬스터가 공격가능한지 여부를 체크하고 첫번째로 조건을 만족하는 몬스터를 선정하므로써 해결하였습니다. <br>
PlayerController.cs의 FindTargetToAttack 메서드
```cs
Transform FindTargetToAttack()
{
    float attackRange = 0f;
    switch (PlayerWeaponType)
    {
        case WeaponType.Sword:
            attackRange = swordAttackRange; 
            break;
        case WeaponType.CrossBow:
            attackRange = bowAttackRange;
            break;
    }

    Collider[] monsters = Physics.OverlapSphere(transform.position + Vector3.up, attackRange, LayerMask.GetMask("Monster"));
    LayerMask mask = 1 << (int)Layer.Block;
    
    foreach(Collider monster in monsters)
    {
        Vector3 delta = monster.transform.position - transform.position;

        // 검으로 공격 가능한 높이에 있는가
        if (delta.y > swordAttackRange * Mathf.Sin(Mathf.Deg2Rad * 45) && PlayerWeaponType == WeaponType.Sword)
            continue;

        // 몬스터가 죽었는지 확인
        if (monster.GetComponent<CreatureController>().State == CreatureState.Die)
            continue;

        // 앞에 장애물이 가로막고 있는지 확인
        if(Physics.Raycast(transform.position + Vector3.up, delta, delta.magnitude, mask) == false)
        {
            return monster.transform;
        }
    }
    return null;
}
```
