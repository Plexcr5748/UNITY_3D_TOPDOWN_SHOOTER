using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player_SoundFX: 플레이어의 사운드 효과를 관리하는 클래스
public class Player_SoundFX : MonoBehaviour
{
    public AudioSource weaponReady; // 무기 준비 완료 시 재생되는 소리
    public AudioSource walkSFX; // 걷는 동안 재생되는 소리
    public AudioSource runSFX; // 달리는 동안 재생되는 소리
}
