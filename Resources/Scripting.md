# Scripting

## Opcodes

USA PS2 release, the Scenario Jump Table is found at 0x00304FA0.<br />
Opcodes are read by the Scenario Check function located at 0x00171750.

_ = unused argument

| OpCode | Function                                                                        | Bytes |
|--------|---------------------------------------------------------------------------------|------:|
| 00     | End(u8 _)                                                                       |     2 |
| 01     | If(u8 length)                                                                   |     2 |
| 02     | Else(u8 length)                                                                 |     2 |
| 03     | EndIf(u8 _)                                                                     |     2 |
| 04     | Check(u8, u16, u8, u8)                                                          |     6 |
| 05     | Set(u8, u16, u8, u8)                                                            |     6 |
| 06     | CompareByte(u8, u8, u8)                                                         |     4 |
| 07     | CompareWord(u8, u8, u8, u16)                                                    |     6 |
| 08     | SetValueByte(u8, u8, u8)                                                        |     4 |
| 09     | SetValueWord(u8, u16)                                                           |     4 |
| 0A     | SetWallAtari(u8, u8, u8)                                                        |     4 |
| 0B     | SetEtcAtari(u8, u8, u8)                                                         |     4 |
| 0C     | SetFloorAtari(u8, u8, u8)                                                       |     4 |
| 0D     | CheckDeath(u8, u16)                                                             |     4 |
| 0E     | CheckItem(u8, u16, u8, u8)                                                      |     6 |
| 0F     | ClearUseItem(u8)                                                                |     2 |
| 10     | CheckUseItem(u8)                                                                |     2 |
| 11     | CheckPlayerItem(u8)                                                             |     2 |
| 12     | SetCinematic(u8)                                                                |     2 |
| 13     | SetCamera(u8, u8, u8)                                                           |     4 |
| 14     | EventOn(u8, u16)                                                                |     4 |
| 15     | BGMOn(u8, u8, u8)                                                               |     4 |
| 16     | BGMOff(u8)                                                                      |     2 |
| 17     | SEOn(u8, u8, u8, u16, u8, u8)                                                   |     8 |
| 18     | SEOff(u8)                                                                       |     2 |
| 19     | VoiceOn(u8, u16, u8, u8, u8, u8)                                                |     8 |
| 1A     | VoiceOff(u8)                                                                    |     2 |
| 1B     | CheckADX(u8, u8, u8, u8, u8)                                                    |     6 |
| 1C     | BGSEOn(u8, u16, u8, u8)                                                         |     6 |
| 1D     | BGSEOff(u8, u8, u8)                                                             |     4 |
| 1E     | CheckADXTime(u8, u16, u8, u8)                                                   |     6 |
| 1F     | SetMessage(u8, u8, u8)                                                          |     4 |
| 20     | SetDisplayObject(u8, u8, u8)                                                    |     4 |
| 21     | CheckDeathEvent(u8, u16, u8, u8)                                                |     6 |
| 22     | SetCheckEnemy(u8, u16)                                                          |     4 |
| 23     | SetCheckItem(u8, u16, u8, u8)                                                   |     6 |
| 24     | SetInitModel(u8, u8, u8)                                                        |     4 |
| 25     | SetEtcAtari2(u8, u16, u8, u8, u8, u8, u8, u8)                                   |    10 |
| 26     | CheckArmsItem(u8)                                                               |     2 |
| 27     | ChangeArmsItem(u8)                                                              |     2 |
| 28     | SubStatus(u8)                                                                   |     2 |
| 29     | SetCameraPause(u8)                                                              |     2 |
| 2A     | SetCamera2(u8, u8, u8)                                                          |     4 |
| 2B     | SetMotionPause(u8)                                                              |     2 |
| 2C     | SetEffect(u8, u16)                                                              |     4 |
| 2D     | InitMotionPause(u8)                                                             |     2 |
| 2E     | SetPlayerMotionPause(u8)                                                        |     2 |
| 2F     | InitSetKage(u8, u8, u8)                                                         |     4 |
| 30     | InitMotionPauseEx(u8, u8, u8)                                                   |     4 |
| 31     | PlayerItemLost(u8)                                                              |     2 |
| 32     | SetObjectLink(u8, u8, u8, u8, u8, f16, f16, f16)                                |    12 |
| 33     | SetDoorCall(u8, u16, u8, u8, u8, u8)                                            |     8 |
| 34     | SetPlayerObjectLink(u8, u8, u8, u8, u8, f16, f16, f16)                          |    12 |
| 35     | SetLight(u8, u8, u8)                                                            |     4 |
| 36     | SetFade(s32, u8)                                                                |     6 |
| 37     | RoomCaseNo(u8)                                                                  |     2 |
| 38     | CheckFrame(u8, u8, u8, u16)                                                     |     6 |
| 39     | SetCameraInfo(u8, u8, u8)                                                       |     4 |
| 3A     | SetPlayerMuteki(u8)                                                             |     2 |
| 3B     | SetDefaultModel(u8, u8, u8, u8, u8)                                             |     6 |
| 3C     | SetMask(u8, u8, u8)                                                             |     4 |
| 3D     | SetLip(u8, u8, u8)                                                              |     4 |
| 3E     | StartMask(u8, u8, u8)                                                           |     4 |
| 3F     | StartLip(u8, u8, u8)                                                            |     4 |
| 40     | SetPlayerStartLookG(u8, f16, f16, f16)                                          |     8 |
| 41     | SetPlayerStopLookG(u8)                                                          |     2 |
| 42     | SetItemAspd(u8, u8, u8)                                                         |     4 |
| 43     | SetEffectDisplay(u8, u8, u8)                                                    |     4 |
| 44     | SetEffectAmb(f8, f8, f8, u8, u8)                                                |     6 |
| 45     | DeleteObjectSE(u8)                                                              |     2 |
| 46     | SetNextRoomBGM(u8, u8, u8, u16, u8, u8)                                         |     8 |
| 47     | SetNextRoomBGSE(u8, u8, u8, u16, u16, u8, u8)                                   |    10 |
| 48     | CallFootSE(u8, u8, u8, u8, u8)                                                  |     6 |
| 49     | CallWeaponSE(u8, u8, u8, u8, u8, u8, u8, u8, u8)                                |    10 |
| 4A     | SetYakkyou(u8, u8, u8, u8, u8, u8, u8)                                          |     8 |
| 4B     | SetLightType(u8, u8, u8)                                                        |     4 |
| 4C     | SetFogColor(u32, u8)                                                            |     6 |
| 4D     | CheckPlayerItemBlock(u8, u8, u8)                                                |     4 |
| 4E     | SetEffectBlood(u8, u8, u8, f16, f16, f16, u8, u8, u8, u8)                       |    14 |
| 4F     | SetCyoutenHenkei(u8, u8, u8, u8, u8)                                            |     6 |
| 50     | SetObjectMotion(u8)                                                             |     2 |
| 51     | SetObjectEnemyLink(u8, u8, u8, u8, u8, f16, f16, f16)                           |    12 |
| 52     | SetObjectItemLink(u8, u8, u8, u8, u8, f16, f16, f16)                            |    12 |
| 53     | SetEnemyItemLink(u8, u8, u8, u8, u8, f16, f16, f16)                             |    12 |
| 54     | SetEnemyEnemyLink(u8, u8, u8, u8, u8, f16, f16, f16)                            |    12 |
| 55     | StartCyoutenHenkei(u8)                                                          |     2 |
| 56     | SetEffectBloodPool(u8, u8, u8, u16)                                             |     6 |
| 57     | FixEventCameraPlayer(u8)                                                        |     2 |
| 58     | SetEffectBloodPool2(u8, f16, f16, f16, u8, u8, u8, u8, u16)                     |    14 |
| 59     | SetObjectObjectLink(u8, u8, u8, u8, u8, f16, f16, f16)                          |    12 |
| 5A     | SetCameraYure(u8, f16)                                                          |     4 |
| 5B     | SetInitCamera(u8)                                                               |     2 |
| 5C     | SetMessageDisplayEnd(u8)                                                        |     2 |
| 5D     | CheckPad(u8, u8, u8, u8, u8)                                                    |     6 |
| 5E     | StartMovie(u8, u8, u8)                                                          |     4 |
| 5F     | StopMovie(u8)                                                                   |     2 |
| 60     | CheckTFrame(u8, u8, u8, u16)                                                    |     6 |
| 61     | ClearEventTimer(u8)                                                             |     2 |
| 62     | CheckCamera(u8, u8, u8, u8, u8)                                                 |     6 |
| 63     | SetRandom(u8)                                                                   |     2 |
| 6407   | PlayerControl07(f16, f16, f16)                                                  |     8 |
| 640D   | PlayerControl0D(u8, u8, u8, u8)                                                 |     6 |
| 6434   | PlayerControl34()                                                               |     2 |
| 6480   | PlayerControl80()                                                               |     2 |
| 6481   | PlayerControl81(u8, u8, u8, u8, u8, u8, u8, u8, u8, u8)                         |    12 |
| 6482   | PlayerControl82()                                                               |     2 |
| 6483   | PlayerControl83(u8, u8)                                                         |     4 |
| 6489   | PlayerControl89(u8, u8)                                                         |     4 |
| 648B   | PlayerControl8B()                                                               |     2 |
| 648E   | PlayerControl8E()                                                               |     2 |
| 6496   | PlayerControl96(u8, u8)                                                         |     4 |
| 65     | LoadWork(u8, u8, u8)                                                            |     4 |
| 6601   | ObjectControl01()                                                               |     2 |
| 6608   | ObjectControl08()                                                               |     2 |
| 660C   | ObjectControl0C(u8, u8, u8, u8)                                                 |     6 |
| 6614   | ObjectControl14(u8, u8)                                                         |     4 |
| 6780   | SubControl80()                                                                  |     2 |
| 678F   | SubControl8F(u8, u8, u8, u8)                                                    |     6 |
| 6793   | SubControl93(u8, u8, u8, u8, u8, u8)                                            |     8 |
| 6794   | SubControl94(u8, u8)                                                            |     4 |
| 68     | LoadWork2(u8, u8, u8, u8, u8)                                                   |     6 |
| 6902   | CommonControl02(u8)                                                             |     3 |
| 6903   | CommonControl03(u8)                                                             |     3 |
| 6904   | CommonControl04(u8)                                                             |     3 |
| 6905   | CommonControl05(u8, u8, u8, u8)                                                 |     6 |
| 6906   | CommonControl06(u8, u8, u8, u8)                                                 |     6 |
| 6907   | CommonControl07(f16, f16, f16)                                                  |     8 |
| 6908   | CommonControl08(u8, u8, u16, u16)                                               |     8 |
| 6909   | CommonControl09(u8, u8, u8, u8)                                                 |     6 |
| 690A   | CommonControl0A(u8, u8, u8, u8)                                                 |     6 |
| 690B   | CommonControl0B(u8, u8, u8, u8)                                                 |     6 |
| 690C   | CommonControl0C(u8, u8, u8, u8)                                                 |     6 |
| 690D   | CommonControl0D(u8, u8, u8, u8)                                                 |     6 |
| 690F   | CommonControl0F(u8, u8, u8, u8)                                                 |     6 |
| 6910   | CommonControl10(u8, u8, u8, u8)                                                 |     6 |
| 6911   | CommonControl11(u8, u8)                                                         |     4 |
| 6912   | CommonControl12(u8, u8)                                                         |     4 |
| 6913   | CommonControl13(u8, u8)                                                         |     4 |
| 6917   | CommonControl17(u8, u8)                                                         |     4 |
| 6918   | CommonControl18(u8, u8, u8, u8, u8, u8)                                         |     8 |
| 6919   | CommonControl19(u8, u8, u8, u8, u8, u8)                                         |     8 |
| 691A   | CommonControl1A(u8, u8, f16, f16, f16)                                          |    10 |
| 691B   | CommonControl1B(u8, u8, f16, f16)                                               |     8 |
| 691C   | CommonControl1C(u8, u8, u8, u8)                                                 |     6 |
| 691D   | CommonControl1D(u8)                                                             |     3 |
| 691E   | CommonControl1E(u8)                                                             |     3 |
| 691F   | CommonControl1F(u8)                                                             |     3 |
| 6920   | CommonControl20(u8)                                                             |     3 |
| 6921   | CommonControl21(u8)                                                             |     3 |
| 6922   | CommonControl22(u8, u8, u8, u8, u8, u8, u8, u8)                                 |    10 |
| 6924   | CommonControl24()                                                               |     2 |
| 6926   | CommonControl26(u8, u8, f16, f16, f16)                                          |    10 |
| 6928   | CommonControl28(u8, u8)                                                         |     4 |
| 6929   | CommonControl29(u8, u8)                                                         |     4 |
| 692A   | CommonControl2A(u8, u8)                                                         |     4 |
| 692C   | CommonControl2C(u8, u8, u8)                                                     |     5 |
| 692E   | CommonControl2E(u8, u8)                                                         |     4 |
| 692F   | CommonControl2F()                                                               |     2 |
| 6930   | CommonControl30(u8, u8)                                                         |     4 |
| 6931   | CommonControl31(u8, u8)                                                         |     4 |
| 6932   | CommonControl32(u8, u8)                                                         |     4 |
| 6933   | CommonControl33(u8, u8, u8, u8)                                                 |     6 |
| 6934   | CommonControl34(u8, u8)                                                         |     4 |
| 6935   | CommonControl35(u8, u8)                                                         |     4 |
| 6A     | SetEventSkip(u8)                                                                |     2 |
| 6B     | DeleteYakkyou(u8)                                                               |     2 |
| 6C     | SetObjectAlpha(u8, u8, u8, s8, s8, f8, f8)                                      |     8 |
| 6D     | SetCyodan(f32, u8, f16, f16, f16, u8, u8, f16, f16, f16, u8, u8)                |    22 |
| 6E     | SetHEffect(u8, u8, u8, f16, f16, f16, u8, u8, u8, u8, f16, f16, f16, u16)       |    22 |
| 6F     | SetObjectPlayerLink(u8, u8, u8, u8, u8, f16, f16, f16)                          |    12 |
| 70     | EffectPush(u8)                                                                  |     2 |
| 71     | EffectPop(u8)                                                                   |     2 |
| 72     | AreaSearchObject(u8, u8, u8, f16, f16, u8, u8, f16, f16)                        |    14 |
| 73     | SetLightParameterC(u8, f16, f16, f16, f16, f16, f16, f16, f16, f16, f16)        |    22 |
| 74     | StartLightParameter(u8, u8, u8)                                                 |     4 |
| 75     | SetInitMidiSlot(u8, u8, u8)                                                     |     4 |
| 76     | Set3DSoundFlag(u8, u8, u8)                                                      |     4 |
| 77     | SetSoundVolume(u8, u8, u8, u8, u8)                                              |     6 |
| 78     | SetLightParameter(u8, u8, u8, f16, f16, f16, f16, f16)                          |    14 |
| 79     | EnemySEOn(u8, u8, u8, u16)                                                      |     6 |
| 7A     | EnemySEOff(u8)                                                                  |     2 |
| 7B     | SetWallAtari2(u8, u16, u8, u8, u8, u8, u8, u8)                                  |    10 |
| 7C     | SetFloorAtari2(u8, u16, u8, u8, u8, u8, u8, u8)                                 |    10 |
| 7D     | SetEnemyPlayerMotionPos(u8, u8, u8)                                             |     4 |
| 7E     | SetKageSw(u8, u8, u8)                                                           |     4 |
| 7F     | SetSoundPan(u8, u8, u8, u8, u8)                                                 |     6 |
| 80     | SetInitPony(u8, u8, u8)                                                         |     4 |
| 81     | CheckSubMapBusy(u8)                                                             |     2 |
| 82     | SetDebugLoopEx(u8, u16)                                                         |     4 |
| 83     | SoundFadeOut(u8)                                                                |     2 |
| 84     | SetCyoutenHenkeiEx(u8, u8, u8, u8, u8, f16)                                     |     8 |
| 85     | StartCyoutenHenkeiEx(u8, f16)                                                   |     4 |
| 86     | SetEasySE(u8, u8, u8, u8, u8, u8, u8, u8, u8, u8, u8, u8, u8, s16)              |    16 |
| 87     | SetSoundFlagReset(u8)                                                           |     2 |
| 88     | SetEffectUV(u8, f16, f16, f16, f16)                                             |    10 |
| 89     | SetChangePlayer(u8)                                                             |     2 |
| 8A     | CheckPlayerPoison(u8)                                                           |     2 |
| 8B     | AddObjectSE(u8, u8, u8, f16, f16, f16, u16)                                     |    12 |
| 8C     | RandomTest(u8, u16, u16)                                                        |     6 |
| 8D     | SetEventCom(u8)                                                                 |     2 |
| 8E     | CheckZombieUpDeath(u8, u16)                                                     |     4 |
| 8F     | SetFacePause(u8, u8, u8)                                                        |     4 |
| 90     | SetFaceRe(u8)                                                                   |     2 |
| 91     | SetEffectMode(u8, u8, u8)                                                       |     4 |
| 92     | BGSEOff2(u8)                                                                    |     2 |
| 93     | BGMOff2(u8)                                                                     |     2 |
| 94     | BGSEOn2(u8, u16)                                                                |     4 |
| 95     | BGMOn2(u8, u8, u8)                                                              |     4 |
| 96     | SetEffectSensya(u8)                                                             |     2 |
| 97     | SetEffectKokuen(u8, f16, f16, f16)                                              |     8 |
| 98     | SetEffectSand(u8, u8, u8, f16, f16, f16, u8, u8)                                |    12 |
| 99     | EnemyHPUp(u8, u8, u8)                                                           |     4 |
| 9A     | FaceRep(u8, u8, u8)                                                             |     4 |
| 9B     | CheckMovie(u8, u8, u8, u8, u8)                                                  |     6 |
| 9C     | SetItemMotion(u8)                                                               |     2 |
| 9D     | SetObjectAspd(u8, u8, u8)                                                       |     4 |
| 9E     | SetVibrationFlag(u8)                                                            |     2 |
| 9F     | StartVibration(u8)                                                              |     2 |
| A0     | MapSystemOn(u8, u8, u8, u8, u8)                                                 |     6 |
| A1     | SetTrapDamage(u8)                                                               |     2 |
| A2     | SetEventLighterFire(u8, u8, u8)                                                 |     4 |
| A3     | SetPlayerItemLink(u8, u8, u8, u8, u8, f16, f16, f16)                            |    12 |
| A4     | PlayerKaidanMotion(u8, u8, u8)                                                  |     4 |
| A5     | SetEnemyRender(u8, u8, u8)                                                      |     4 |
| A6     | BGMOnEx(u8, u8, u8)                                                             |     4 |
| A7     | BGMOn2Ex(u8, u8, u8)                                                            |     4 |
| A8     | SetFogParameterC(f8, f8, f8, f8, f8, f8, f8, f8, u8)                            |    10 |
| A9     | StartFogParameter(u8)                                                           |     2 |
| AA     | SetEffectUV2(u8, f16, f16, f16, f16)                                            |    10 |
| AB     | SetBGColor(u32, u8)                                                             |     6 |
| AC     | CheckMovieTime(u8, u16, u8, u8)                                                 |     6 |
| AD     | SetEffectType(u8, u8, u8)                                                       |     4 |
| AE     | PlayerPoison2Cr(u8)                                                             |     2 |
| AF     | PlayerHandChange(u8, u8, u8)                                                    |     4 |
| B0     | SetHEffect2(u8, u8, u8, f16, f16, f16, u8, u8, u8, u8, f16, f16, f16, u16)      |    22 |
| B1     | CheckObjectDpos(u8, u8, u8, u8, u8)                                             |     6 |
| B2     | GetItem(u8, u8, u8)                                                             |     4 |
| B3     | SetEtcAtariEnemyPos(u8, u8, u8, s8, u8, u8, u8)                                 |     8 |
| B4     | SetEtcAtariEventPos(u8, u8, u8)                                                 |     4 |
| B5     | LoadWorkEx(u8, u8, u8)                                                          |     4 |
| B6     | RoomSoundCase(u8 case_id)                                                       |     2 |
| B7     | ItemPlayerToStorageBox(u8)                                                      |     2 |
| B8     | ItemStorageBoxToInventoryBox(u8)                                                |     2 |
| B9     | SetGridPos(u8, u8, u8, f16, f16, f16)                                           |    10 |
| BA     | SetGridPosMoveC(u8, f16, f16, f16, u8, u8, f16, f16, f16)                       |    16 |
| BB     | StartGridPosMove(u8)                                                            |     2 |
| BC     | EventKill(u8)                                                                   |     2 |
| BD     | SetReTryPoint(u8)                                                               |     2 |
| BE     | CheckPlayerDPos(u8, u8, s8, u8, u8)                                             |     6 |
| BF     | PlayerItemLostEx(u8, u8, u8)                                                    |     4 |
| C0     | SetCyodanEx(s32, s32, u8, f16, f16, f16, u8, u8, f16, f16, f16, u8, u8, u8, u8) |    28 |
| C1     | SetArmsItem(u8)                                                                 |     2 |
| C2     | GetItemEx(u8, u8, u8, u16)                                                      |     6 |
| C3     | SetMatsumotoEffectSand(u8, u8, u8)                                              |     4 |
| C4     | VoiceWait(u8, u16, u8, u8, u8, u8)                                              |     8 |
| C5     | StartVoice(u8)                                                                  |     2 |
| C6     | SetGameOver(u8)                                                                 |     2 |
| C7     | PlayerItemChangeM(u8 item_id, u8 new_item_id, u8 _)                             |     4 |
| C8     | SetEffectBakuDrm(u8, f16, f16, f16)                                             |     8 |
| C9     | SetPlayerItemTama(u8 item_id, u16 quantity)                                     |     4 |
| CA     | ClearEventEffect(u8)                                                            |     2 |
| CB     | SetEventTimer(u8)                                                               |     2 |
| CC     | SetEnemyLookFlag(u8, u8, u8)                                                    |     4 |
| CD     | ReturnTitleEvent(u8)                                                            |     2 |
| CE     | SetCameraMode(u8 mode)                                                          |     2 |
| CF     | InitGameItemEx(u8)                                                              |     2 |
| D0     | SetEnemyLifeM(u8, u16)                                                          |     4 |
| D1     | SetEffectSSize(u8, u8, u8, f16, f16, f16)                                       |    10 |
| D2     | SetEffectLinkOffset(u8, u8, u8, f16, f16, f16)                                  |    10 |
| D3     | CallRanking(u8)                                                                 |     2 |
| D4     | CallSysSE(u8, u16)                                                              |     4 |
| D5     | nop()                                                                           |     1 |
| D6     | nop()                                                                           |     1 |
| D7     | nop()                                                                           |     1 |
| D8     | nop()                                                                           |     1 |
| D9     | nop()                                                                           |     1 |
| DA     | nop()                                                                           |     1 |
| DB     | nop()                                                                           |     1 |
| DC     | nop()                                                                           |     1 |
| DD     | nop()                                                                           |     1 |
| DE     | nop()                                                                           |     1 |
| DF     | nop()                                                                           |     1 |
| E0     | nop()                                                                           |     1 |
| E1     | nop()                                                                           |     1 |
| E2     | nop()                                                                           |     1 |
| E3     | nop()                                                                           |     1 |
| E4     | nop()                                                                           |     1 |
| E5     | nop()                                                                           |     1 |
| E6     | nop()                                                                           |     1 |
| E7     | nop()                                                                           |     1 |
| E8     | nop()                                                                           |     1 |
| E9     | nop()                                                                           |     1 |
| EA     | nop()                                                                           |     1 |
| EB     | nop()                                                                           |     1 |
| EC     | nop()                                                                           |     1 |
| ED     | nop()                                                                           |     1 |
| EE     | nop()                                                                           |     1 |
| EF     | nop()                                                                           |     1 |
| F0     | nop()                                                                           |     1 |
| F1     | nop()                                                                           |     1 |
| F2     | nop()                                                                           |     1 |
| F3     | EWhile2()                                                                       |     1 |
| F4     | ComNext()                                                                       |     1 |
| F5     | nop()                                                                           |     1 |
| F6     | nop()                                                                           |     1 |
| F7     | nop()                                                                           |     1 |
| F8     | Sleep(u8, u16)                                                                  |     4 |
| F9     | Sleeping(u8, u8)                                                                |     3 |
| FA     | For(u8, u16)                                                                    |     4 |
| FB     | Next(u8)                                                                        |     2 |
| FC     | While(u8)                                                                       |     2 |
| FD     | EWhile()                                                                        |     1 |
| FE     | EventNext()                                                                     |     1 |
| FF     | EventEnd(u8)                                                                    |     2 |

## Links

- [cbnj](https://www.dreamcast-talk.com/forum/memberlist.php?mode=viewprofile&u=16823) - [BH/RECV/X SCD SCRIPTING](https://www.dreamcast-talk.com/forum/viewtopic.php?f=52&t=17129)
- [@IntelOrca](https://github.com/IntelOrca) - [Scripting - Resident Evil Code Veronica](https://github.com/IntelOrca/biohazard-utils/wiki/Scripting-%E2%80%90-Resident-Evil-Code-Veronica)
