# Scripting

## Opcodes

- First byte of an opcode is an index into the Scenario Jump Table
- Scenario Jump Table entries are 4 byte pointers to functions
- Some functions appear in the jump table mutiple times
- Scenario Jump Table has 256 entries

USA PS2 release, the Scenario Jump Table is found at 0x00304FA0.<br />
Opcodes are read by the Scenario Check function located at 0x00171750.

TODO: Determine function arguments<br />
TODO: Update function descriptions

|Description|Function|Arg 0|Arg 1|Arg 2|Arg 3|Arg 4|Arg 5|
|-|-|-|-|-|-|-|-|
|End|00| | | | | | |
|Else If|01| | | | | | |
|Else|02| | | | | | |
|End If|03| | | | | | |
|Check|04| | | | | | |
|Set|05| | | | | | |
|Set|06| | | | | | |
|Compare Word|07| | | | | | |
|Sv|08| | | | | | |
|Sv Word|09| | | | | | |
|Wall Atari Set|0A| | | | | | |
|Etc Atari Set|0B| | | | | | |
|Floor Atari Set|0C| | | | | | |
|Death Check|0D| | | | | | |
|Item Check|0E| | | | | | |
|Use Item Clear|0F| | | | | | |
|Use Item Check|10| | | | | | |
|Player Item Check|11| | | | | | |
|Cinematic Set|12| | | | | | |
|Camera Set|13| | | | | | |
|Event On|14| | | | | | |
|BGM On|15| | | | | | |
|BGM On|16| | | | | | |
|SE On|17| | | | | | |
|SE Off|18| | | | | | |
|Voice On|19| | | | | | |
|Voice Off|1A| | | | | | |
|ADX Check|1B| | | | | | |
|BG SE On|1C| | | | | | |
|BG SE Off|1D| | | | | | |
|ADX Time Check|1E| | | | | | |
|Message Set|1F| | | | | | |
|Set Display Object|20| | | | | | |
|Death Event Check|21| | | | | | |
|Enemy Set Check|22| | | | | | |
|Item Set Check|23| | | | | | |
|Init Model Set|24| | | | | | |
|Etc Atari Set 2|25| | | | | | |
|Use Item Check|26| | | | | | |
|Arms Item Change|27| | | | | | |
|Sub Status|28| | | | | | |
|Camera Set 2|29| | | | | | |
|Camera Set 2|2A| | | | | | |
|Motion Pause Set|2B| | | | | | |
|Effect Set|2C| | | | | | |
|Init Motion Pause|2D| | | | | | |
|Motion Pause Set Ply|2E| | | | | | |
|Init Set Kage|2F| | | | | | |
|Init Motion Pause Ex|30| | | | | | |
|Player Item Lost|31| | | | | | |
|Object Link Set|32| | | | | | |
|Set Disp Object|33| | | | | | |
|Object Link Set Ply|34| | | | | | |
|Light Set|35| | | | | | |
|Fade Set|36| | | | | | |
|Room Case No|37| | | | | | |
|Frame Check|38| | | | | | |
|Camera Info Set|39| | | | | | |
|Muteki Set Pl|3A| | | | | | |
|Def Model Set|3B| | | | | | |
|Mask Set|3C| | | | | | |
|Lip Set|3D| | | | | | |
|Mask Start|3E| | | | | | |
|Lip Start|3F| | | | | | |
|Look G Set Player Start|40| | | | | | |
|Look G Set Player Stop|41| | | | | | |
|Item Aspd Set|42| | | | | | |
|Item Aspd Set|43| | | | | | |
|Effect Amb Set|44| | | | | | |
|Delete Object SE|45| | | | | | |
|Set Next Room BGM|46| | | | | | |
|Set Next Room BG SE|47| | | | | | |
|Foot SE Call|48| | | | | | |
|Light Set|49| | | | | | |
|Yakkyou Set|4A| | | | | | |
|Light Type Set|4B| | | | | | |
|Fog Color Set|4C| | | | | | |
|Player Item Block Check|4D| | | | | | |
|Effect Blood Set|4E| | | | | | |
|Cyouten Henkei Set|4F| | | | | | |
|Set Object Motion|50| | | | | | |
|Object Link Set Object Enemy|51| | | | | | |
|Object Link Set Object Item|52| | | | | | |
|Object Link Set Enemy Item|53| | | | | | |
|Object Link Set Enemy Enemy|54| | | | | | |
|Cyouten Henkei Start|55| | | | | | |
|Effect Blood Pool Set|56| | | | | | |
|Fix Event Camera Ply|57| | | | | | |
|Effect Blood Pool Set 2|58| | | | | | |
|Object Link Set Object Object|59| | | | | | |
|Camera Yure Set|5A| | | | | | |
|Init Camera Set|5B| | | | | | |
|Message Display End Set|5C| | | | | | |
|Pad Check|5D| | | | | | |
|Movie Start|5E| | | | | | |
|Movie Stop|5F| | | | | | |
|T Frame Check|60| | | | | | |
|Event Timer Clear|61| | | | | | |
|Camera Check|62| | | | | | |
|Random Set|63| | | | | | |
|Player Ctr|64| | | | | | |
|Load Work|65| | | | | | |
|Object Ctr|66| | | | | | |
|Sub Ctr|67| | | | | | |
|Load Work 2|68| | | | | | |
|Common Ctr|69| | | | | | |
|Event Skip Set|6A| | | | | | |
|Delete Yakkyou|6B| | | | | | |
|Object Alpha Set|6C| | | | | | |
|Cyodan Set|6D| | | | | | |
|H Effect Set|6E| | | | | | |
|Object Link Set Object Ply|6F| | | | | | |
|Effect Push|70| | | | | | |
|Effect Pop|71| | | | | | |
|Area Search Object|72| | | | | | |
|Light Parameter C Set|73| | | | | | |
|Light Parameter Start|74| | | | | | |
|Init Midi Slot Set|75| | | | | | |
|D Sound Flag Set|76| | | | | | |
|Sound Volume Set|77| | | | | | |
|Light Parameter Set|78| | | | | | |
|Enemy SE On|79| | | | | | |
|Enemy SE Off|7A| | | | | | |
|Wal Atari Set 2|7B| | | | | | |
|Flr Atari Set2|7C| | | | | | |
|Motion Pos Set Enemy Ply|7D| | | | | | |
|Kage Sw Set|7E| | | | | | |
|Sound Pan Set|7F| | | | | | |
|Init Pony Set|80| | | | | | |
|Sub Map Busy Check|81| | | | | | |
|Set Debug Loop Ex|82| | | | | | |
|Sound Fade Out|83| | | | | | |
|Cyouten Henkei Set Ex|84| | | | | | |
|Cyouten Henkei Start Ex|85| | | | | | |
|Easy S E Set|86| | | | | | |
|Sound Flag Re Set|87| | | | | | |
|Effect UV Set|88| | | | | | |
|Player Change Set|89| | | | | | |
|Player Poison Check|8A| | | | | | |
|Add Object SE|8B| | | | | | |
|Rand Test|8C| | | | | | |
|Event Com Set|8D| | | | | | |
|Zombie Up Death Check|8E| | | | | | |
|Face Pause Set|8F| | | | | | |
|Face Re Set|90| | | | | | |
|Effect Mode Set|91| | | | | | |
|BG SE Off 2|92| | | | | | |
|BGM Off 2|93| | | | | | |
|BG SE On 2|94| | | | | | |
|BGM On 2|95| | | | | | |
|Effect Sensya Set|96| | | | | | |
|Effect Kokuen Set|97| | | | | | |
|Effect Sand Set|98| | | | | | |
|Enemy HP Up|99| | | | | | |
|Face Rep|9A| | | | | | |
|Movie Check|9B| | | | | | |
|Set Item Motion|9C| | | | | | |
|Object Aspd Set|9D| | | | | | |
|Puru Puru Flag Set|9E| | | | | | |
|Puru Puru Start|9F| | | | | | |
|Map System On|A0| | | | | | |
|Map System On|A1| | | | | | |
|Event Lighter Fire Set|A2| | | | | | |
|Object Link Set Ply Item|A3| | | | | | |
|Player Kaidan Motion|A4| | | | | | |
|Enemy Render Set|A5| | | | | | |
|BGM On Ex|A6| | | | | | |
|BGM On 2 Ex|A7| | | | | | |
|Fog Parameter C Set|A8| | | | | | |
|Fog Parameter Start|A9| | | | | | |
|Effect UV Set 2|AA| | | | | | |
|BG Color Set|AB| | | | | | |
|Movie Time Check|AC| | | | | | |
|Effect Type Set|AD| | | | | | |
|Player Poison 2 Cr|AE| | | | | | |
|Ply Hand Change|AF| | | | | | |
|H Effect Set 2|B0| | | | | | |
|Object Dpos Check|B1| | | | | | |
|Item Get Get|B2| | | | | | |
|Etc Atari Enemy Pos Set|B3| | | | | | |
|Etc Atari Event Pos Set|B4| | | | | | |
|Load Work Ex|B5| | | | | | |
|Room Sound Case|B6| | | | | | |
|Room Sound Case|B7| | | | | | |
|Item S Box To I Box|B8| | | | | | |
|Grd Pos Set|B9| | | | | | |
|Grd Pos Move C Set|BA| | | | | | |
|Grd Pos Move Start|BB| | | | | | |
|Event Kill|BC| | | | | | |
|Re Try Point Set|BD| | | | | | |
|Re Try Point Set|BE| | | | | | |
|Ply Dpos Check|BF| | | | | | |
|Cyodan Set Ex|C0| | | | | | |
|Arms Item Set|C1| | | | | | |
|Item Get Get Ex|C2| | | | | | |
|Effect Sand Set Matsumoto|C3| | | | | | |
|Voice Wait|C4| | | | | | |
|Voice Start|C5| | | | | | |
|Game Over Set|C6| | | | | | |
|Player Item Change M|C7| | | | | | |
|Effect Baku Drm Set|C8| | | | | | |
|Effect Baku Drm Set|C9| | | | | | |
|Effect Clear Event|CA| | | | | | |
|Event Timer Set|CB| | | | | | |
|Enemy Look Flag Set|CC| | | | | | |
|Return Title Event|CD| | | | | | |
|Syukan Mode Set|CE| | | | | | |
|Ex Game Item Init|CF| | | | | | |
|Ex Game Item Init|D0| | | | | | |
|Effect S Size Set|D1| | | | | | |
|Effect S Size Set|D2| | | | | | |
|Ranking Call|D3| | | | | | |
|Call Sys SE|D4| | | | | | |
|Nothing|D5| | | | | | |
|Nothing|D6| | | | | | |
|Nothing|D7| | | | | | |
|Nothing|D8| | | | | | |
|Nothing|D9| | | | | | |
|Nothing|DA| | | | | | |
|Nothing|DB| | | | | | |
|Nothing|DC| | | | | | |
|Nothing|DD| | | | | | |
|Nothing|DE| | | | | | |
|Nothing|DF| | | | | | |
|Nothing|E0| | | | | | |
|Nothing|E1| | | | | | |
|Nothing|E2| | | | | | |
|Nothing|E3| | | | | | |
|Nothing|E4| | | | | | |
|Nothing|E5| | | | | | |
|Nothing|E6| | | | | | |
|Nothing|E7| | | | | | |
|Nothing|E8| | | | | | |
|Nothing|E9| | | | | | |
|Nothing|EA| | | | | | |
|Nothing|EB| | | | | | |
|Nothing|EC| | | | | | |
|Nothing|ED| | | | | | |
|Nothing|EE| | | | | | |
|Nothing|EF| | | | | | |
|Nothing|F0| | | | | | |
|Nothing|F1| | | | | | |
|Nothing|F2| | | | | | |
|E While 2|F3| | | | | | |
|Com Next|F4| | | | | | |
|Nothing|F5| | | | | | |
|Nothing|F6| | | | | | |
|Nothing|F7| | | | | | |
|Sleep|F8| | | | | | |
|Sleeping|F9| | | | | | |
|For|FA| | | | | | |
|For|FB| | | | | | |
|While|FC| | | | | | |
|E While|FD| | | | | | |
|Event Next|FE| | | | | | |
|Event End|FF| | | | | | |

## Links

- [cbnj](https://www.dreamcast-talk.com/forum/memberlist.php?mode=viewprofile&u=16823) - [BH/RECV/X SCD SCRIPTING](https://www.dreamcast-talk.com/forum/viewtopic.php?f=52&t=17129)
- [@IntelOrca](https://github.com/IntelOrca) - [Scripting - Resident Evil Code Veronica](https://github.com/IntelOrca/biohazard-utils/wiki/Scripting-%E2%80%90-Resident-Evil-Code-Veronica)
