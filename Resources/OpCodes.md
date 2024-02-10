# OpCodes

| Name                         | Hex                                                                     | Bytes |
|------------------------------|-------------------------------------------------------------------------|------:|
| End                          | 00 00                                                                   |     2 |
| If                           | 01 00                                                                   |     2 |
| Else                         | 02 00                                                                   |     2 |
| EndIf                        | 03 00                                                                   |     2 |
| Check                        | 04 00 0000 00 00                                                        |     6 |
| Set                          | 05 00 0000 00 00                                                        |     6 |
| CompareByte                  | 06 00 00 00                                                             |     4 |
| CompareWord                  | 07 00 00 00 0000                                                        |     6 |
| SetValueByte                 | 08 00 00 00                                                             |     4 |
| SetValueWord                 | 09 00 0000                                                              |     4 |
| SetWallAtari                 | 0A 00 00 00                                                             |     4 |
| SetEtcAtari                  | 0B 00 00 00                                                             |     4 |
| SetFloorAtari                | 0C 00 00 00                                                             |     4 |
| CheckDeath                   | 0D 00 0000                                                              |     4 |
| CheckItem                    | 0E 00 0000 00 00                                                        |     6 |
| ClearUseItem                 | 0F 00                                                                   |     2 |
| CheckUseItem                 | 10 00                                                                   |     2 |
| CheckPlayerItem              | 11 00                                                                   |     2 |
| SetCinematic                 | 12 00                                                                   |     2 |
| SetCamera                    | 13 00 00 00                                                             |     4 |
| EventOn                      | 14 00 0000                                                              |     4 |
| BGMOn                        | 15 00 00 00                                                             |     4 |
| BGMOff                       | 16 00                                                                   |     2 |
| SEOn                         | 17 00 00 00 0000 00 00                                                  |     8 |
| SEOff                        | 18 00                                                                   |     2 |
| VoiceOn                      | 19 00 0000 00 00 00 00                                                  |     8 |
| VoiceOff                     | 1A 00                                                                   |     2 |
| CheckADX                     | 1B 00 00 00 00 00                                                       |     6 |
| BGSEOn                       | 1C 00 0000 00 00                                                        |     6 |
| BGSEOff                      | 1D 00 00 00                                                             |     4 |
| CheckADXTime                 | 1E 00 0000 00 00                                                        |     6 |
| SetMessage                   | 1F 00 00 00                                                             |     4 |
| SetDisplayObject             | 20 00 00 00                                                             |     4 |
| CheckDeathEvent              | 21 00 0000 00 00                                                        |     6 |
| SetCheckEnemy                | 22 00 0000                                                              |     4 |
| SetCheckItem                 | 23 00 0000 00 00                                                        |     6 |
| SetInitModel                 | 24 00 00 00                                                             |     4 |
| SetEtcAtari2                 | 25 00 0000 00 00 00 00 00 00                                            |    10 |
| CheckArmsItem                | 26 00                                                                   |     2 |
| ChangeArmsItem               | 27 00                                                                   |     2 |
| SubStatus                    | 28 00                                                                   |     2 |
| SetCameraPause               | 29 00                                                                   |     2 |
| SetCamera2                   | 2A 00 00 00                                                             |     4 |
| SetMotionPause               | 2B 00                                                                   |     2 |
| SetEffect                    | 2C 00 0000                                                              |     4 |
| InitMotionPause              | 2D 00                                                                   |     2 |
| SetPlayerMotionPause         | 2E 00                                                                   |     2 |
| InitSetKage                  | 2F 00 00 00                                                             |     4 |
| InitMotionPauseEx            | 30 00 00 00                                                             |     4 |
| PlayerItemLost               | 31 00                                                                   |     2 |
| SetObjectLink                | 32 00 00 00 00 00 0000 0000 0000                                        |    12 |
| SetDoorCall                  | 33 00 0000 00 00 00 00                                                  |     8 |
| SetPlayerObjectLink          | 34 00 00 00 00 00 0000 0000 0000                                        |    12 |
| SetLight                     | 35 00 00 00                                                             |     4 |
| SetFade                      | 36 00000000 00                                                          |     6 |
| RoomCaseNo                   | 37 00                                                                   |     2 |
| CheckFrame                   | 38 00 00 00 0000                                                        |     6 |
| SetCameraInfo                | 39 00 00 00                                                             |     4 |
| SetPlayerMuteki              | 3A 00                                                                   |     2 |
| SetDefaultModel              | 3B 00 00 00 00 00                                                       |     6 |
| SetMask                      | 3C 00 00 00                                                             |     4 |
| SetLip                       | 3D 00 00 00                                                             |     4 |
| StartMask                    | 3E 00 00 00                                                             |     4 |
| StartLip                     | 3F 00 00 00                                                             |     4 |
| SetPlayerStartLookG          | 40 00 0000 0000 0000                                                    |     8 |
| SetPlayerStopLookG           | 41 00                                                                   |     2 |
| SetItemAspd                  | 42 00 00 00                                                             |     4 |
| SetEffectDisplay             | 43 00 00 00                                                             |     4 |
| SetEffectAmb                 | 44 00 00 00 00 00                                                       |     6 |
| DeleteObjectSE               | 45 00                                                                   |     2 |
| SetNextRoomBGM               | 46 00 00 00 0000 00 00                                                  |     8 |
| SetNextRoomBGSE              | 47 00 00 00 0000 0000 00 00                                             |    10 |
| CallFootSE                   | 48 00 00 00 00 00                                                       |     6 |
| CallWeaponSE                 | 49 00 00 00 00 00 00 00 00 00                                           |    10 |
| SetYakkyou                   | 4A 00 00 00 00 00 00 00                                                 |     8 |
| SetLightType                 | 4B 00 00 00                                                             |     4 |
| SetFogColor                  | 4C 00000000 00                                                          |     6 |
| CheckPlayerItemBlock         | 4D 00 00 00                                                             |     4 |
| SetEffectBlood               | 4E 00 00 00 0000 0000 0000 00 00 00 00                                  |    14 |
| SetCyoutenHenkei             | 4F 00 00 00 00 00                                                       |     6 |
| SetObjectMotion              | 50 00                                                                   |     2 |
| SetObjectEnemyLink           | 51 00 00 00 00 00 0000 0000 0000                                        |    12 |
| SetObjectItemLink            | 52 00 00 00 00 00 0000 0000 0000                                        |    12 |
| SetEnemyItemLink             | 53 00 00 00 00 00 0000 0000 0000                                        |    12 |
| SetEnemyEnemyLink            | 54 00 00 00 00 00 0000 0000 0000                                        |    12 |
| StartCyoutenHenkei           | 55 00                                                                   |     2 |
| SetEffectBloodPool           | 56 00 00 00 0000                                                        |     6 |
| FixEventCameraPlayer         | 57 00                                                                   |     2 |
| SetEffectBloodPool2          | 58 00 0000 0000 0000 00 00 00 00 0000                                   |    14 |
| SetObjectObjectLink          | 59 00 00 00 00 00 0000 0000 0000                                        |    12 |
| SetCameraYure                | 5A 00 0000                                                              |     4 |
| SetInitCamera                | 5B 00                                                                   |     2 |
| SetMessageDisplayEnd         | 5C 00                                                                   |     2 |
| CheckPad                     | 5D 00 00 00 00 00                                                       |     6 |
| StartMovie                   | 5E 00 00 00                                                             |     4 |
| StopMovie                    | 5F 00                                                                   |     2 |
| CheckTFrame                  | 60 00 00 00 0000                                                        |     6 |
| ClearEventTimer              | 61 00                                                                   |     2 |
| CheckCamera                  | 62 00 00 00 00 00                                                       |     6 |
| SetRandom                    | 63 00                                                                   |     2 |
| PlayerControl07              | 6407 0000 0000 0000                                                     |     8 |
| PlayerControl0D              | 640D 00 00 00 00                                                        |     6 |
| PlayerControl34              | 6434                                                                    |     2 |
| PlayerControl80              | 6480                                                                    |     2 |
| PlayerControl81              | 6481 00 00 00 00 00 00 00 00 00 00                                      |    12 |
| PlayerControl82              | 6482                                                                    |     2 |
| PlayerControl83              | 6483 00 00                                                              |     4 |
| PlayerControl89              | 6489 00 00                                                              |     4 |
| PlayerControl8B              | 648B                                                                    |     2 |
| PlayerControl8E              | 648E                                                                    |     2 |
| PlayerControl96              | 6496 00 00                                                              |     4 |
| LoadWork                     | 65 00 00 00                                                             |     4 |
| ObjectControl01              | 6601                                                                    |     2 |
| ObjectControl08              | 6608                                                                    |     2 |
| ObjectControl0C              | 660C 00 00 00 00                                                        |     6 |
| ObjectControl14              | 6614 00 00                                                              |     4 |
| SubControl80                 | 6780                                                                    |     2 |
| SubControl8F                 | 678F 00 00 00 00                                                        |     6 |
| SubControl93                 | 6793 00 00 00 00 00 00                                                  |     8 |
| SubControl94                 | 6794 00 00                                                              |     4 |
| LoadWork2                    | 68 00 00 00 00 00                                                       |     6 |
| CommonControl02              | 6902 00                                                                 |     3 |
| CommonControl03              | 6903 00                                                                 |     3 |
| CommonControl04              | 6904 00                                                                 |     3 |
| CommonControl05              | 6905 00 00 00 00                                                        |     6 |
| CommonControl06              | 6906 00 00 00 00                                                        |     6 |
| CommonControl07              | 6907 0000 0000 0000                                                     |     8 |
| CommonControl08              | 6908 00 00 0000 0000                                                    |     8 |
| CommonControl09              | 6909 00 00 00 00                                                        |     6 |
| CommonControl0A              | 690A 00 00 00 00                                                        |     6 |
| CommonControl0B              | 690B 00 00 00 00                                                        |     6 |
| CommonControl0C              | 690C 00 00 00 00                                                        |     6 |
| CommonControl0D              | 690D 00 00 00 00                                                        |     6 |
| CommonControl0F              | 690F 00 00 00 00                                                        |     6 |
| CommonControl10              | 6910 00 00 00 00                                                        |     6 |
| CommonControl11              | 6911 00 00                                                              |     4 |
| CommonControl12              | 6912 00 00                                                              |     4 |
| CommonControl13              | 6913 00 00                                                              |     4 |
| CommonControl17              | 6917 00 00                                                              |     4 |
| CommonControl18              | 6918 00 00 00 00 00 00                                                  |     8 |
| CommonControl19              | 6919 00 00 00 00 00 00                                                  |     8 |
| CommonControl1A              | 691A 00 00 0000 0000 0000                                               |    10 |
| CommonControl1B              | 691B 00 00 0000 0000                                                    |     8 |
| CommonControl1C              | 691C 00 00 00 00                                                        |     6 |
| CommonControl1D              | 691D 00                                                                 |     3 |
| CommonControl1E              | 691E 00                                                                 |     3 |
| CommonControl1F              | 691F 00                                                                 |     3 |
| CommonControl20              | 6920 00                                                                 |     3 |
| CommonControl21              | 6921 00                                                                 |     3 |
| CommonControl22              | 6922 00 00 00 00 00 00 00 00                                            |    10 |
| CommonControl24              | 6924                                                                    |     2 |
| CommonControl26              | 6926 00 00 0000 0000 0000                                               |    10 |
| CommonControl28              | 6928 00 00                                                              |     4 |
| CommonControl29              | 6929 00 00                                                              |     4 |
| CommonControl2A              | 692A 00 00                                                              |     4 |
| CommonControl2C              | 692C 00 00 00                                                           |     5 |
| CommonControl2E              | 692E 00 00                                                              |     4 |
| CommonControl2F              | 692F                                                                    |     2 |
| CommonControl30              | 6930 00 00                                                              |     4 |
| CommonControl31              | 6931 00 00                                                              |     4 |
| CommonControl32              | 6932 00 00                                                              |     4 |
| CommonControl33              | 6933 00 00 00 00                                                        |     6 |
| CommonControl34              | 6934 00 00                                                              |     4 |
| CommonControl35              | 6935 00 00                                                              |     4 |
| SetEventSkip                 | 6A 00                                                                   |     2 |
| DeleteYakkyou                | 6B 00                                                                   |     2 |
| SetObjectAlpha               | 6C 00 00 00 00 00 00 00                                                 |     8 |
| SetCyodan                    | 6D 00000000 00 0000 0000 0000 00 00 0000 0000 0000 00 00                |    22 |
| SetHEffect                   | 6E 00 00 00 0000 0000 0000 00 00 00 00 0000 0000 0000 0000              |    22 |
| SetObjectPlayerLink          | 6F 00 00 00 00 00 0000 0000 0000                                        |    12 |
| EffectPush                   | 70 00                                                                   |     2 |
| EffectPop                    | 71 00                                                                   |     2 |
| AreaSearchObject             | 72 00 00 00 0000 0000 00 00 0000 0000                                   |    14 |
| SetLightParameterC           | 73 00 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000                 |    22 |
| StartLightParameter          | 74 00 00 00                                                             |     4 |
| SetInitMidiSlot              | 75 00 00 00                                                             |     4 |
| Set3DSoundFlag               | 76 00 00 00                                                             |     4 |
| SetSoundVolume               | 77 00 00 00 00 00                                                       |     6 |
| SetLightParameter            | 78 00 00 00 0000 0000 0000 0000 0000                                    |    14 |
| EnemySEOn                    | 79 00 00 00 0000                                                        |     6 |
| EnemySEOff                   | 7A 00                                                                   |     2 |
| SetWallAtari2                | 7B 00 0000 00 00 00 00 00 00                                            |    10 |
| SetFloorAtari2               | 7C 00 0000 00 00 00 00 00 00                                            |    10 |
| SetEnemyPlayerMotionPos      | 7D 00 00 00                                                             |     4 |
| SetKageSw                    | 7E 00 00 00                                                             |     4 |
| SetSoundPan                  | 7F 00 00 00 00 00                                                       |     6 |
| SetInitPony                  | 80 00 00 00                                                             |     4 |
| CheckSubMapBusy              | 81 00                                                                   |     2 |
| SetDebugLoopEx               | 82 00 0000                                                              |     4 |
| SoundFadeOut                 | 83 00                                                                   |     2 |
| SetCyoutenHenkeiEx           | 84 00 00 00 00 00 0000                                                  |     8 |
| StartCyoutenHenkeiEx         | 85 00 0000                                                              |     4 |
| SetEasySE                    | 86 00 00 00 00 00 00 00 00 00 00 00 00 00 0000                          |    16 |
| SetSoundFlagReset            | 87 00                                                                   |     2 |
| SetEffectUV                  | 88 00 0000 0000 0000 0000                                               |    10 |
| SetChangePlayer              | 89 00                                                                   |     2 |
| CheckPlayerPoison            | 8A 00                                                                   |     2 |
| AddObjectSE                  | 8B 00 00 00 0000 0000 0000 0000                                         |    12 |
| RandomTest                   | 8C 00 0000 0000                                                         |     6 |
| SetEventCom                  | 8D 00                                                                   |     2 |
| CheckZombieUpDeath           | 8E 00 0000                                                              |     4 |
| SetFacePause                 | 8F 00 00 00                                                             |     4 |
| SetFaceRe                    | 90 00                                                                   |     2 |
| SetEffectMode                | 91 00 00 00                                                             |     4 |
| BGSEOff2                     | 92 00                                                                   |     2 |
| BGMOff2                      | 93 00                                                                   |     2 |
| BGSEOn2                      | 94 00 0000                                                              |     4 |
| BGMOn2                       | 95 00 00 00                                                             |     4 |
| SetEffectSensya              | 96 00                                                                   |     2 |
| SetEffectKokuen              | 97 00 0000 0000 0000                                                    |     8 |
| SetEffectSand                | 98 00 00 00 0000 0000 0000 00 00                                        |    12 |
| EnemyHPUp                    | 99 00 00 00                                                             |     4 |
| FaceRep                      | 9A 00 00 00                                                             |     4 |
| CheckMovie                   | 9B 00 00 00 00 00                                                       |     6 |
| SetItemMotion                | 9C 00                                                                   |     2 |
| SetObjectAspd                | 9D 00 00 00                                                             |     4 |
| SetVibrationFlag             | 9E 00                                                                   |     2 |
| StartVibration               | 9F 00                                                                   |     2 |
| MapSystemOn                  | A0 00 00 00 00 00                                                       |     6 |
| SetTrapDamage                | A1 00                                                                   |     2 |
| SetEventLighterFire          | A2 00 00 00                                                             |     4 |
| SetPlayerItemLink            | A3 00 00 00 00 00 0000 0000 0000                                        |    12 |
| PlayerKaidanMotion           | A4 00 00 00                                                             |     4 |
| SetEnemyRender               | A5 00 00 00                                                             |     4 |
| BGMOnEx                      | A6 00 00 00                                                             |     4 |
| BGMOn2Ex                     | A7 00 00 00                                                             |     4 |
| SetFogParameterC             | A8 00 00 00 00 00 00 00 00 00                                           |    10 |
| StartFogParameter            | A9 00                                                                   |     2 |
| SetEffectUV2                 | AA 00 0000 0000 0000 0000                                               |    10 |
| SetBGColor                   | AB 00000000 00                                                          |     6 |
| CheckMovieTime               | AC 00 0000 00 00                                                        |     6 |
| SetEffectType                | AD 00 00 00                                                             |     4 |
| PlayerPoison2Cr              | AE 00                                                                   |     2 |
| PlayerHandChange             | AF 00 00 00                                                             |     4 |
| SetHEffect2                  | B0 00 00 00 0000 0000 0000 00 00 00 00 0000 0000 0000 0000              |    22 |
| CheckObjectDpos              | B1 00 00 00 00 00                                                       |     6 |
| GetItem                      | B2 00 00 00                                                             |     4 |
| SetEtcAtariEnemyPos          | B3 00 00 00 00 00 00 00                                                 |     8 |
| SetEtcAtariEventPos          | B4 00 00 00                                                             |     4 |
| LoadWorkEx                   | B5 00 00 00                                                             |     4 |
| RoomSoundCase                | B6 00                                                                   |     2 |
| ItemPlayerToStorageBox       | B7 00                                                                   |     2 |
| ItemStorageBoxToInventoryBox | B8 00                                                                   |     2 |
| SetGridPos                   | B9 00 00 00 0000 0000 0000                                              |    10 |
| SetGridPosMoveC              | BA 00 0000 0000 0000 00 00 0000 0000 0000                               |    16 |
| StartGridPosMove             | BB 00                                                                   |     2 |
| EventKill                    | BC 00                                                                   |     2 |
| SetReTryPoint                | BD 00                                                                   |     2 |
| CheckPlayerDPos              | BE 00 00 00 00 00                                                       |     6 |
| PlayerItemLostEx             | BF 00 00 00                                                             |     4 |
| SetCyodanEx                  | C0 00000000 00000000 00 0000 0000 0000 00 00 0000 0000 0000 00 00 00 00 |    28 |
| SetArmsItem                  | C1 00                                                                   |     2 |
| GetItemEx                    | C2 00 00 00 0000                                                        |     6 |
| SetMatsumotoEffectSand       | C3 00 00 00                                                             |     4 |
| VoiceWait                    | C4 00 0000 00 00 00 00                                                  |     8 |
| StartVoice                   | C5 00                                                                   |     2 |
| SetGameOver                  | C6 00                                                                   |     2 |
| PlayerItemChangeM            | C7 00 00 00                                                             |     4 |
| SetEffectBakuDrm             | C8 00 0000 0000 0000                                                    |     8 |
| SetPlayerItemTama            | C9 00 0000                                                              |     4 |
| ClearEventEffect             | CA 00                                                                   |     2 |
| SetEventTimer                | CB 00                                                                   |     2 |
| SetEnemyLookFlag             | CC 00 00 00                                                             |     4 |
| ReturnTitleEvent             | CD 00                                                                   |     2 |
| SetCameraMode                | CE 00                                                                   |     2 |
| InitGameItemEx               | CF 00                                                                   |     2 |
| SetEnemyLifeM                | D0 00 0000                                                              |     4 |
| SetEffectSSize               | D1 00 00 00 0000 0000 0000                                              |    10 |
| SetEffectLinkOffset          | D2 00 00 00 0000 0000 0000                                              |    10 |
| CallRanking                  | D3 00                                                                   |     2 |
| CallSysSE                    | D4 00 0000                                                              |     4 |
| nop                          | D5                                                                      |     1 |
| nop                          | D6                                                                      |     1 |
| nop                          | D7                                                                      |     1 |
| nop                          | D8                                                                      |     1 |
| nop                          | D9                                                                      |     1 |
| nop                          | DA                                                                      |     1 |
| nop                          | DB                                                                      |     1 |
| nop                          | DC                                                                      |     1 |
| nop                          | DD                                                                      |     1 |
| nop                          | DE                                                                      |     1 |
| nop                          | DF                                                                      |     1 |
| nop                          | E0                                                                      |     1 |
| nop                          | E1                                                                      |     1 |
| nop                          | E2                                                                      |     1 |
| nop                          | E3                                                                      |     1 |
| nop                          | E4                                                                      |     1 |
| nop                          | E5                                                                      |     1 |
| nop                          | E6                                                                      |     1 |
| nop                          | E7                                                                      |     1 |
| nop                          | E8                                                                      |     1 |
| nop                          | E9                                                                      |     1 |
| nop                          | EA                                                                      |     1 |
| nop                          | EB                                                                      |     1 |
| nop                          | EC                                                                      |     1 |
| nop                          | ED                                                                      |     1 |
| nop                          | EE                                                                      |     1 |
| nop                          | EF                                                                      |     1 |
| nop                          | F0                                                                      |     1 |
| nop                          | F1                                                                      |     1 |
| nop                          | F2                                                                      |     1 |
| EWhile2                      | F3                                                                      |     1 |
| ComNext                      | F4                                                                      |     1 |
| nop                          | F5                                                                      |     1 |
| nop                          | F6                                                                      |     1 |
| nop                          | F7                                                                      |     1 |
| Sleep                        | F8 00 0000                                                              |     4 |
| Sleeping                     | F9 00 00                                                                |     3 |
| For                          | FA 00 0000                                                              |     4 |
| Next                         | FB 00                                                                   |     2 |
| While                        | FC 00                                                                   |     2 |
| EWhile                       | FD                                                                      |     1 |
| EventNext                    | FE                                                                      |     1 |
| EventEnd                     | FF 00                                                                   |     2 |