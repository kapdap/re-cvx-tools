# Scripting

## Opcodes

USA PS2 release, the Scenario Jump Table is found at 0x00304FA0.<br />
Opcodes are read by the Scenario Check function located at 0x00171750.

| OpCode | Function                                                                        |
| ------ | ------------------------------------------------------------------------------- |
| 00     | End(u8)                                                                         |
| 01     | If(u8)                                                                          |
| 02     | Else(u8)                                                                        |
| 03     | EndIf(u8)                                                                       |
| 04     | Check(u8, u16, u8, u8)                                                          |
| 05     | Set(u8, u16, u8, u8)                                                            |
| 06     | CompareByte(u8, u8, u8)                                                         |
| 07     | CompareWord(u8, u8, u8, u16)                                                    |
| 08     | SetValueByte(u8, u8, u8)                                                        |
| 09     | SetValueWord(u8, u16)                                                           |
| 0A     | SetWallAtari(u8, u8, u8)                                                        |
| 0B     | SetEtcAtari(u8, u8, u8)                                                         |
| 0C     | SetFloorAtari(u8, u8, u8)                                                       |
| 0D     | CheckDeath(u8, u16)                                                             |
| 0E     | CheckItem(u8, u16, u8, u8)                                                      |
| 0F     | ClearUseItem(u8)                                                                |
| 10     | CheckUseItem(u8)                                                                |
| 11     | CheckPlayerItem(u8)                                                             |
| 12     | SetCinematic(u8)                                                                |
| 13     | SetCamera(u8, u8, u8)                                                           |
| 14     | EventOn(u8, u16)                                                                |
| 15     | BGMOn(u8, u8, u8)                                                               |
| 16     | BGMOff(u8)                                                                      |
| 17     | SEOn(u8, u8, u8, u16, u8, u8)                                                   |
| 18     | SEOff(u8)                                                                       |
| 19     | VoiceOn(u8, u16, u8, u8, u8, u8)                                                |
| 1A     | VoiceOff(u8)                                                                    |
| 1B     | CheckADX(u8, u8, u8, u8, u8)                                                    |
| 1C     | BGSEOn(u8, u16, u8, u8)                                                         |
| 1D     | BGSEOff(u8, u8, u8)                                                             |
| 1E     | CheckADXTime(u8, u16, u8, u8)                                                   |
| 1F     | SetMessage(u8, u8, u8)                                                          |
| 20     | SetDisplayObject(u8, u8, u8)                                                    |
| 21     | CheckDeathEvent(u8, u16, u8, u8)                                                |
| 22     | SetCheckEnemy(u8, u16)                                                          |
| 23     | SetCheckItem(u8, u16, u8, u8)                                                   |
| 24     | SetInitModel(u8, u8, u8)                                                        |
| 25     | SetEtcAtari2(u8, u16, u8, u8, u8, u8, u8, u8)                                   |
| 26     | CheckArmsItem(u8)                                                               |
| 27     | ChangeArmsItem(u8)                                                              |
| 28     | SubStatus(u8)                                                                   |
| 29     | SetCameraPause(u8)                                                              |
| 2A     | SetCamera2(u8, u8, u8)                                                          |
| 2B     | SetMotionPause(u8)                                                              |
| 2C     | SetEffect(u8, u16)                                                              |
| 2D     | InitMotionPause(u8)                                                             |
| 2E     | SetPlayerMotionPause(u8)                                                        |
| 2F     | InitSetKage(u8, u8, u8)                                                         |
| 30     | InitMotionPauseEx(u8, u8, u8)                                                   |
| 31     | PlayerItemLost(u8)                                                              |
| 32     | SetObjectLink(u8, u8, u8, u8, u8, f16, f16, f16)                                |
| 33     | SetDoorCall(u8, u16, u8, u8, u8, u8)                                            |
| 34     | SetPlayerObjectLink(u8, u8, u8, u8, u8, f16, f16, f16)                          |
| 35     | SetLight(u8, u8, u8)                                                            |
| 36     | SetFade(s32, u8)                                                                |
| 37     | RoomCaseNo(u8)                                                                  |
| 38     | CheckFrame(u8, u8, u8, u16)                                                     |
| 39     | SetCameraInfo(u8, u8, u8)                                                       |
| 3A     | SetPlayerMuteki(u8)                                                             |
| 3B     | SetDefaultModel(u8, u8, u8, u8, u8)                                             |
| 3C     | SetMask(u8, u8, u8)                                                             |
| 3D     | SetLip(u8, u8, u8)                                                              |
| 3E     | StartMask(u8, u8, u8)                                                           |
| 3F     | StartLip(u8, u8, u8)                                                            |
| 40     | SetPlayerStartLookG(u8, f16, f16, f16)                                          |
| 41     | SetPlayerStopLookG(u8)                                                          |
| 42     | SetItemAspd(u8, u8, u8)                                                         |
| 43     | SetEffectDisplay(u8, u8, u8)                                                    |
| 44     | SetEffectAmb(f8, f8, f8, u8, u8)                                                |
| 45     | DeleteObjectSE(u8)                                                              |
| 46     | SetNextRoomBGM(u8, u8, u8, u16, u8, u8)                                         |
| 47     | SetNextRoomBGSE(u8, u8, u8, u16, u16, u8, u8)                                   |
| 48     | CallFootSE(u8, u8, u8, u8, u8)                                                  |
| 49     | CallWeaponSE(u8, u8, u8, u8, u8, u8, u8, u8, u8)                                |
| 4A     | SetYakkyou(u8, u8, u8, u8, u8, u8, u8)                                          |
| 4B     | SetLightType(u8, u8, u8)                                                        |
| 4C     | SetFogColor(u8, u8, u8, u8, u8)                                                 |
| 4D     | CheckPlayerItemBlock(u8, u8, u8)                                                |
| 4E     | SetEffectBlood(u8, u8, u8, f16, f16, f16, u8, u8, u8, u8)                       |
| 4F     | SetCyoutenHenkei(u8, u8, u8, u8, u8)                                            |
| 50     | SetObjectMotion(u8)                                                             |
| 51     | SetObjectEnemyLink(u8, u8, u8, u8, u8, f16, f16, f16)                           |
| 52     | SetObjectItemLink(u8, u8, u8, u8, u8, f16, f16, f16)                            |
| 53     | SetEnemyItemLink(u8, u8, u8, u8, u8, f16, f16, f16)                             |
| 54     | SetEnemyEnemyLink(u8, u8, u8, u8, u8, f16, f16, f16)                            |
| 55     | StartCyoutenHenkei(u8)                                                          |
| 56     | SetEffectBloodPool(u8, u8, u8, u16)                                             |
| 57     | FixEventCameraPlayer(u8)                                                        |
| 58     | SetEffectBloodPool2(u8, f16, f16, f16, u8, u8, u8, u8, u16)                     |
| 59     | SetObjectObjectLink(u8, u8, u8, u8, u8, f16, f16, f16)                          |
| 5A     | SetCameraYure(u8, f16)                                                          |
| 5B     | SetInitCamera(u8)                                                               |
| 5C     | SetMessageDisplayEnd(u8)                                                        |
| 5D     | CheckPad(u8, u8, u8, u8, u8)                                                    |
| 5E     | StartMovie(u8, u8, u8)                                                          |
| 5F     | StopMovie(u8)                                                                   |
| 60     | CheckTFrame(u8, u8, u8, u16)                                                    |
| 61     | ClearEventTimer(u8)                                                             |
| 62     | CheckCamera(u8, u8, u8, u8, u8)                                                 |
| 63     | SetRandom(u8)                                                                   |
| 6407   | PlayerControl07(f16, f16, f16)                                                  |
| 640D   | PlayerControl0D(u8, u8, u8, u8)                                                 |
| 6434   | PlayerControl34()                                                               |
| 6480   | PlayerControl80()                                                               |
| 6481   | PlayerControl81(u8, u8, u8, u8, u8, u8, u8, u8, u8, u8)                         |
| 6482   | PlayerControl82()                                                               |
| 6483   | PlayerControl83(u8, u8)                                                         |
| 6489   | PlayerControl89(u8, u8)                                                         |
| 648B   | PlayerControl8B()                                                               |
| 648E   | PlayerControl8E()                                                               |
| 6496   | PlayerControl96(u8, u8)                                                         |
| 65     | LoadWork(u8, u8, u8)                                                            |
| 6601   | ObjectControl01()                                                               |
| 6608   | ObjectControl08()                                                               |
| 660C   | ObjectControl0C(u8, u8, u8, u8)                                                 |
| 6614   | ObjectControl14(u8, u8)                                                         |
| 6780   | SubControl80()                                                                  |
| 678F   | SubControl8F(u8, u8, u8, u8)                                                    |
| 6793   | SubControl93(u8, u8, u8, u8, u8, u8)                                            |
| 6794   | SubControl94(u8, u8)                                                            |
| 68     | LoadWork2(u8, u8, u8, u8, u8)                                                   |
| 6902   | CommonControl02(u8)                                                             |
| 6903   | CommonControl03(u8)                                                             |
| 6904   | CommonControl04(u8)                                                             |
| 6905   | CommonControl05(u8, u8, u8, u8)                                                 |
| 6906   | CommonControl06(u8, u8, u8, u8)                                                 |
| 6907   | CommonControl07(f16, f16, f16)                                                  |
| 6908   | CommonControl08(u8, u8, u16, u16)                                               |
| 6909   | CommonControl09(u8, u8, u8, u8)                                                 |
| 690A   | CommonControl0A(u8, u8, u8, u8)                                                 |
| 690B   | CommonControl0B(u8, u8, u8, u8)                                                 |
| 690C   | CommonControl0C(u8, u8, u8, u8)                                                 |
| 690D   | CommonControl0D(u8, u8, u8, u8)                                                 |
| 690F   | CommonControl0F(u8, u8, u8, u8)                                                 |
| 6910   | CommonControl10(u8, u8, u8, u8)                                                 |
| 6911   | CommonControl11(u8, u8)                                                         |
| 6912   | CommonControl12(u8, u8)                                                         |
| 6913   | CommonControl13(u8, u8)                                                         |
| 6917   | CommonControl17(u8, u8)                                                         |
| 6918   | CommonControl18(u8, u8, u8, u8, u8, u8)                                         |
| 6919   | CommonControl19(u8, u8, u8, u8, u8, u8)                                         |
| 691A   | CommonControl1A(u8, u8, f16, f16, f16)                                          |
| 691B   | CommonControl1B(u8, u8, f16, f16)                                               |
| 691C   | CommonControl1C(u8, u8, u8, u8)                                                 |
| 691D   | CommonControl1D(u8)                                                             |
| 691E   | CommonControl1E(u8)                                                             |
| 691F   | CommonControl1F(u8)                                                             |
| 6920   | CommonControl20(u8)                                                             |
| 6921   | CommonControl21(u8)                                                             |
| 6922   | CommonControl22(u8, u8, u8, u8, u8, u8, u8, u8)                                 |
| 6924   | CommonControl24()                                                               |
| 6926   | CommonControl26(u8, u8, f16, f16, f16)                                          |
| 6928   | CommonControl28(u8, u8)                                                         |
| 6929   | CommonControl29(u8, u8)                                                         |
| 692A   | CommonControl2A(u8, u8)                                                         |
| 692C   | CommonControl2C(u8, u8, u8)                                                     |
| 692E   | CommonControl2E(u8, u8)                                                         |
| 692F   | CommonControl2F()                                                               |
| 6930   | CommonControl30(u8, u8)                                                         |
| 6931   | CommonControl31(u8, u8)                                                         |
| 6932   | CommonControl32(u8, u8)                                                         |
| 6933   | CommonControl33(u8, u8, u8, u8)                                                 |
| 6934   | CommonControl34(u8, u8)                                                         |
| 6935   | CommonControl35(u8, u8)                                                         |
| 6A     | SetEventSkip(u8)                                                                |
| 6B     | DeleteYakkyou(u8)                                                               |
| 6C     | SetObjectAlpha(u8, u8, u8, s8, s8, f8, f8)                                      |
| 6D     | SetCyodan(f32, u8, f16, f16, f16, u8, u8, f16, f16, f16, u8, u8)                |
| 6E     | SetHEffect(u8, u8, u8, f16, f16, f16, u8, u8, u8, u8, f16, f16, f16, u16)       |
| 6F     | SetObjectPlayerLink(u8, u8, u8, u8, u8, f16, f16, f16)                          |
| 70     | EffectPush(u8)                                                                  |
| 71     | EffectPop(u8)                                                                   |
| 72     | AreaSearchObject(u8, u8, u8, f16, f16, u8, u8, f16, f16)                        |
| 73     | SetLightParameterC(u8, f16, f16, f16, f16, f16, f16, f16, f16, f16, f16)        |
| 74     | StartLightParameter(u8, u8, u8)                                                 |
| 75     | SetInitMidiSlot(u8, u8, u8)                                                     |
| 76     | Set3DSoundFlag(u8, u8, u8)                                                      |
| 77     | SetSoundVolume(u8, u8, u8, u8, u8)                                              |
| 78     | SetLightParameter(u8, u8, u8, f16, f16, f16, f16, f16)                          |
| 79     | EnemySEOn(u8, u8, u8, u16)                                                      |
| 7A     | EnemySEOff(u8)                                                                  |
| 7B     | SetWallAtari2(u8, u16, u8, u8, u8, u8, u8, u8)                                  |
| 7C     | SetFloorAtari2(u8, u16, u8, u8, u8, u8, u8, u8)                                 |
| 7D     | SetEnemyPlayerMotionPos(u8, u8, u8)                                             |
| 7E     | SetKageSw(u8, u8, u8)                                                           |
| 7F     | SetSoundPan(u8, u8, u8, u8, u8)                                                 |
| 80     | SetInitPony(u8, u8, u8)                                                         |
| 81     | CheckSubMapBusy(u8)                                                             |
| 82     | SetDebugLoopEx(u8, u16)                                                         |
| 83     | SoundFadeOut(u8)                                                                |
| 84     | SetCyoutenHenkeiEx(u8, u8, u8, u8, u8, f16)                                     |
| 85     | StartCyoutenHenkeiEx(u8, f16)                                                   |
| 86     | SetEasySE(u8, u8, u8, u8, u8, u8, u8, u8, u8, u8, u8, u8, u8, s16)              |
| 87     | SetSoundFlagReset(u8)                                                           |
| 88     | SetEffectUV(u8, f16, f16, f16, f16)                                             |
| 89     | SetChangePlayer(u8)                                                             |
| 8A     | CheckPlayerPoison(u8)                                                           |
| 8B     | AddObjectSE(u8, u8, u8, f16, f16, f16, u16)                                     |
| 8C     | RandomTest(u8, u16, u16)                                                        |
| 8D     | SetEventCom(u8)                                                                 |
| 8E     | CheckZombieUpDeath(u8, u16)                                                     |
| 8F     | SetFacePause(u8, u8, u8)                                                        |
| 90     | SetFaceRe(u8)                                                                   |
| 91     | SetEffectMode(u8, u8, u8)                                                       |
| 92     | BGSEOff2(u8)                                                                    |
| 93     | BGMOff2(u8)                                                                     |
| 94     | BGSEOn2(u8, u16)                                                                |
| 95     | BGMOn2(u8, u8, u8)                                                              |
| 96     | SetEffectSensya(u8)                                                             |
| 97     | SetEffectKokuen(u8, f16, f16, f16)                                              |
| 98     | SetEffectSand(u8, u8, u8, f16, f16, f16, u8, u8)                                |
| 99     | EnemyHPUp(u8, u8, u8)                                                           |
| 9A     | FaceRep(u8, u8, u8)                                                             |
| 9B     | CheckMovie(u8, u8, u8, u8, u8)                                                  |
| 9C     | SetItemMotion(u8)                                                               |
| 9D     | SetObjectAspd(u8, u8, u8)                                                       |
| 9E     | SetVibrationFlag(u8)                                                            |
| 9F     | StartVibration(u8)                                                              |
| A0     | MapSystemOn(u8, u8, u8, u8, u8)                                                 |
| A1     | SetTrapDamage(u8)                                                               |
| A2     | SetEventLighterFire(u8, u8, u8)                                                 |
| A3     | SetPlayerItemLink(u8, u8, u8, u8, u8, f16, f16, f16)                            |
| A4     | PlayerKaidanMotion(u8, u8, u8)                                                  |
| A5     | SetEnemyRender(u8, u8, u8)                                                      |
| A6     | BGMOnEx(u8, u8, u8)                                                             |
| A7     | BGMOn2Ex(u8, u8, u8)                                                            |
| A8     | SetFogParameterC(f8, f8, f8, f8, f8, f8, f8, f8, u8)                            |
| A9     | StartFogParameter(u8)                                                           |
| AA     | SetEffectUV2(u8, f16, f16, f16, f16)                                            |
| AB     | SetBGColor(u32, u8)                                                             |
| AC     | CheckMovieTime(u8, u16, u8, u8)                                                 |
| AD     | SetEffectType(u8, u8, u8)                                                       |
| AE     | PlayerPoison2Cr(u8)                                                             |
| AF     | PlayerHandChange(u8, u8, u8)                                                    |
| B0     | SetHEffect2(u8, u8, u8, f16, f16, f16, u8, u8, u8, u8, f16, f16, f16, u16)      |
| B1     | CheckObjectDpos(u8, u8, u8, u8, u8)                                             |
| B2     | GetItem(u8, u8, u8)                                                             |
| B3     | SetEtcAtariEnemyPos(u8, u8, u8, s8, u8, u8, u8)                                 |
| B4     | SetEtcAtariEventPos(u8, u8, u8)                                                 |
| B5     | LoadWorkEx(u8, u8, u8)                                                          |
| B6     | RoomSoundCase(u8)                                                               |
| B7     | ItemPlayerToStorageBox(u8)                                                      |
| B8     | ItemStorageBoxToInventoryBox(u8)                                                |
| B9     | SetGridPos(u8, u8, u8, f16, f16, f16)                                           |
| BA     | SetGridPosMoveC(u8, f16, f16, f16, u8, u8, f16, f16, f16)                       |
| BB     | StartGridPosMove(u8)                                                            |
| BC     | EventKill(u8)                                                                   |
| BD     | SetReTryPoint(u8)                                                               |
| BE     | CheckPlayerDPos(u8, u8, s8, u8, u8)                                             |
| BF     | PlayerItemLostEx(u8, u8, u8)                                                    |
| C0     | SetCyodanEx(s32, s32, u8, f16, f16, f16, u8, u8, f16, f16, f16, u8, u8, u8, u8) |
| C1     | SetArmsItem(u8)                                                                 |
| C2     | GetItemEx(u8, u8, u8, u16)                                                      |
| C3     | SetMatsumotoEffectSand(u8, u8, u8)                                              |
| C4     | VoiceWait(u8, u16, u8, u8, u8, u8)                                              |
| C5     | StartVoice(u8)                                                                  |
| C6     | SetGameOver(u8)                                                                 |
| C7     | PlayerItemChangeM(u8, u8, u8)                                                   |
| C8     | SetEffectBakuDrm(u8, f16, f16, f16)                                             |
| C9     | SetPlayerItemTama(u8, u16)                                                      |
| CA     | ClearEventEffect(u8)                                                            |
| CB     | SetEventTimer(u8)                                                               |
| CC     | SetEnemyLookFlag(u8, u8, u8)                                                    |
| CD     | ReturnTitleEvent(u8)                                                            |
| CE     | SetSyukanMode(u8)                                                               |
| CF     | InitGameItemEx(u8)                                                              |
| D0     | SetEnemyLifeM(u8, u16)                                                          |
| D1     | SetEffectSSize(u8, u8, u8, f16, f16, f16)                                       |
| D2     | SetEffectLinkOffset(u8, u8, u8, f16, f16, f16)                                  |
| D3     | CallRanking(u8)                                                                 |
| D4     | CallSysSE(u8, u16)                                                              |
| D5     | Empty()                                                                         |
| D6     | Empty()                                                                         |
| D7     | Empty()                                                                         |
| D8     | Empty()                                                                         |
| D9     | Empty()                                                                         |
| DA     | Empty()                                                                         |
| DB     | Empty()                                                                         |
| DC     | Empty()                                                                         |
| DD     | Empty()                                                                         |
| DE     | Empty()                                                                         |
| DF     | Empty()                                                                         |
| E0     | Empty()                                                                         |
| E1     | Empty()                                                                         |
| E2     | Empty()                                                                         |
| E3     | Empty()                                                                         |
| E4     | Empty()                                                                         |
| E5     | Empty()                                                                         |
| E6     | Empty()                                                                         |
| E7     | Empty()                                                                         |
| E8     | Empty()                                                                         |
| E9     | Empty()                                                                         |
| EA     | Empty()                                                                         |
| EB     | Empty()                                                                         |
| EC     | Empty()                                                                         |
| ED     | Empty()                                                                         |
| EE     | Empty()                                                                         |
| EF     | Empty()                                                                         |
| F0     | Empty()                                                                         |
| F1     | Empty()                                                                         |
| F2     | Empty()                                                                         |
| F3     | EWhile2()                                                                       |
| F4     | ComNext()                                                                       |
| F5     | Empty()                                                                         |
| F6     | Empty()                                                                         |
| F7     | Empty()                                                                         |
| F8     | Sleep(u8, u16)                                                                  |
| F9     | Sleeping(u8, u8)                                                                |
| FA     | For(u8, u16)                                                                    |
| FB     | Next(u8)                                                                        |
| FC     | While(u8)                                                                       |
| FD     | EWhile()                                                                        |
| FE     | EventNext()                                                                     |
| FF     | EventEnd(u8)                                                                    |
| FC     | While(u8)                                                                       |
| FD     | EWhile()                                                                        |
| FE     | EventNext()                                                                     |
| FF     | EventEnd(u8)                                                                    |

## Links

- [cbnj](https://www.dreamcast-talk.com/forum/memberlist.php?mode=viewprofile&u=16823) - [BH/RECV/X SCD SCRIPTING](https://www.dreamcast-talk.com/forum/viewtopic.php?f=52&t=17129)
- [@IntelOrca](https://github.com/IntelOrca) - [Scripting - Resident Evil Code Veronica](https://github.com/IntelOrca/biohazard-utils/wiki/Scripting-%E2%80%90-Resident-Evil-Code-Veronica)
