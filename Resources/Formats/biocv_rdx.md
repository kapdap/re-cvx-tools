# RDX/RDT Specification

> [!NOTE]
>
> This document is a work in progress (WIP) and may contain errors.

RDX file format is used in Biohazard/Resident Evil: Code Veronica to store geometry, model, texture, event, message and other common data for rooms.

RDX files are compressed with PRS. Use [prsutil](https://github.com/essen/prsutil/tree/master/bin) to extract the RDT content.

[RDXplorer](https://github.com/nickworonekin/puyotools/tree/master/tools/RDXplorer) can open compressed and uncompressed files.

---

## Table of Contents

- [File Structure](#file-structure)
  - [Header](#header)
  - [Section Offsets](#section-offsets)
  - [Table Offsets](#table-offsets)
- [Table Structures](#table-structures)
  - [model](#model)
  - [motion](#motion)
  - [script](#script)
  - [texture](#texture)
  - [camera](#camera)
  - [camera_info](#camera_info)
  - [camera_unknown](#camera_unknown)
  - [lighting](#lighting)
  - [actor](#actor)
  - [obj](#obj)
  - [item](#item)
  - [effect](#effect)
  - [boundry](#boundry)
  - [aot](#aot)
  - [trigger](#trigger)
  - [player](#player)
  - [event](#event)
- [Instance Table Mapping](#instance-table-mapping)
- [Notes](#notes)
- [References](#references)

---

## File Structure

### Header

The header is a fixed-size block that contains offsets and counts needed to locate and read each data section in the file.

| Offset | Name                             | Type   | Size | Description |
| ------ | -------------------------------- | ------ | ---: | ----------- |
| 0x000  | magic                            | s4     |    4 | Magic bytes |
| 0x004  | dummy_0                          | u1     |   12 | Dummy       |
| 0x010  | [ofs_sections](#section-offsets) | s4[8]  |   32 |             |
| 0x030  | dummy_1                          | u1     |   48 | Dummy       |
| 0x060  | author                           | ASCII  |   32 | Room author |
| 0x080  | [ofs_tables](#table-offsets)     | s4[16] |   64 |             |
| 0x0C0  | dummy_2                          | u1     |   64 | Dummy       |
| 0x100  | [num_tables](#table-offsets)     | s4[16] |   64 |             |
| 0x140  | dummy_3                          | u1     |   64 | Dummy       |
| 0x180  | unknown_0                        | f4[16] |   64 |             |
| 0x1C0  | dummy_7                          | u1     |  632 | Dummy       |
| 0x438  | unknown_1                        | u1     |    1 |             |
| 0x439  | unknown_2                        | u1     |    1 |             |
| 0x432  | unknown_3                        | u1     |    1 |             |
| 0x433  | unknown_4                        | u1     |    1 |             |
| 0x434  | unknown_5                        | f4[12] |   48 |             |

---

### Section Offsets

The `ofs_sections` array starts at offset `0x10` and contains pointers to the start of the main data sections.

| Index | Name                     | Type | Size | Description                                       |
| ----- | ------------------------ | ---- | ---: | ------------------------------------------------- |
| 0     | [tables](#table-offsets) | s4   |    4 | Array of pointers to the start of each data table |
| 1     | [models](#model)         | s4   |    4 | 3D model data                                     |
| 2     | [motions](#motion)       | s4   |    4 | Animation data                                    |
| 3     | [scripts](#script)       | s4   |    4 | Room scripts                                      |
| 4     | [textures](#texture)     | s4   |    4 | Textures for room models                          |
| 5     | unused_0                 | s4   |    4 | Unused                                            |
| 6     | unused_1                 | s4   |    4 | Unused                                            |
| 7     | unused_2                 | s4   |    4 | Unused                                            |

---

### Tables Offsets

The `ofs_tables` and `num_tables` arrays each contain 16 entries.

- `ofs_tables[i]` points to the start of the table.
- `num_tables[i]` is the number of entries in that table.
- `Entry Size` is the size of each entry in the table.

| Index | Name                   | Type     | Entry Size | Description     |
| ----- | ---------------------- | -------- | ---------: | --------------- |
| 0     | [cameras](#camera)     | camera   |        680 | Cameras         |
| 1     | [lights](#lighting)    | lighting |        224 | Lighting        |
| 2     | [actors](#actor)       | actor    |         36 | Actors          |
| 3     | [objs](#obj)           | obj      |         36 | Objects         |
| 4     | [items](#item)         | item     |         36 | Items           |
| 5     | [effects](#effect)     | effect   |         68 | Effects         |
| 6     | [boundaries](#boundry) | boundry  |         36 | Room geometry   |
| 7     | [aots](#aot)           | aot      |         36 | AOTs            |
| s     | [triggers](#trigger)   | trigger  |         36 | Triggers        |
| 9     | [players](#player)     | player   |         16 | Player spawns   |
| 10    | [events](#event)       | event    |         36 | Events          |
| 11    | unknown_0              | ?        |          ? | Unknown         |
| 12    | unknown_1              | ?        |          ? | Unknown         |
| 13    | actions                | bytes    |       2056 | Actions data    |
| 14    | texts                  | bytes    |   variable | Text messages   |
| 15    | sysmes                 | bytes    |   variable | System messages |

---

## Table Structures

### model

| Offset | Name   | Type  | Size | Description                           |
| ------ | ------ | ----- | ---: | ------------------------------------- |
| 0x00   | head_0 | s4    |    4 | Size or type; see notes below         |
| 0x04   | head_1 | s4    |    4 | Only if `has_size`; model type        |
| 0x08   | data   | bytes |    ? | Model data, size determined by head_0 |

**Notes:**  
If `head_0 << 8 != 0x4C444D00` and `head_0 != 0x4E494B53`, then `head_1` is present and head_0 is the size. Otherwise, head_0 is the type and `head_1` is omitted.

---

### motion

| Offset | Name | Type  | Size | Description |
| ------ | ---- | ----- | ---: | ----------- |
| 0x00   | data | bytes |    ? | Motion data |

---

### script

| Offset | Name | Type  | Size | Description |
| ------ | ---- | ----- | ---: | ----------- |
| 0x00   | data | bytes |    ? | Script data |

---

### texture

| Offset | Name | Type  | Size | Description  |
| ------ | ---- | ----- | ---: | ------------ |
| 0x00   | data | bytes |    ? | Texture data |

---

### camera

| Offset | Name        | Type                        |  Size | Description                                         |
| ------ | ----------- | --------------------------- | ----: | --------------------------------------------------- |
| 0x00   | flag_0      | u1                          |     1 |                                                     |
| 0x01   | flag_1      | u1                          |     1 |                                                     |
| 0x02   | flag_2      | u1                          |     1 |                                                     |
| 0x03   | flag_3      | u1                          |     1 |                                                     |
| 0x04   | ofs_unknown | s4                          |     4 | Offset to [unknown camera section](#camera_unknown) |
| 0x08   | info        | [camera_info](#camera_info) | 196×3 |                                                     |

---

### camera_info

| Offset | Name        | Type | Size | Description       |
| ------ | ----------- | ---- | ---: | ----------------- |
| 0x00   | unknown_0   | f4   |    4 |                   |
| 0x04   | unknown_1   | f4   |    4 |                   |
| 0x08   | unknown_2   | f4   |    4 |                   |
| 0x0C   | unknown_3   | f4   |    4 |                   |
| 0x10   | unknown_4   | f4   |    4 |                   |
| 0x14   | unknown_5   | f4   |    4 |                   |
| 0x18   | unknown_6   | f4   |    4 |                   |
| 0x1C   | x           | f4   |    4 | Camera X position |
| 0x20   | y           | f4   |    4 | Camera Y position |
| 0x24   | z           | f4   |    4 | Camera Z position |
| 0x28   | unknown_10  | f4   |    4 |                   |
| 0x2C   | unknown_11  | f4   |    4 |                   |
| 0x30   | unknown_12  | f4   |    4 |                   |
| 0x34   | unknown_13  | f4   |    4 |                   |
| 0x38   | unknown_14  | f4   |    4 |                   |
| 0x3C   | unknown_15  | f4   |    4 |                   |
| 0x40   | unknown_16  | f4   |    4 |                   |
| 0x44   | unknown_17  | f4   |    4 |                   |
| 0x48   | unknown_18  | f4   |    4 |                   |
| 0x4C   | unknown_19  | f4   |    4 |                   |
| 0x50   | unknown_20  | f4   |    4 |                   |
| 0x54   | unknown_21  | f4   |    4 |                   |
| 0x58   | unknown_22  | f4   |    4 |                   |
| 0x5C   | unknown_23  | f4   |    4 |                   |
| 0x60   | unknown_24  | f4   |    4 |                   |
| 0x64   | unknown_25  | f4   |    4 |                   |
| 0x68   | unknown_26  | s4   |    4 |                   |
| 0x6C   | perspective | s4   |    4 |                   |

---

### camera_unknown

| Offset | Name       | Type | Size | Description |
| ------ | ---------- | ---- | ---: | ----------- |
| 0x00   | unknown_0  | u1   |    1 |             |
| 0x01   | unknown_1  | u1   |    1 |             |
| 0x02   | unknown_2  | u1   |    1 |             |
| 0x03   | unknown_3  | u1   |    1 |             |
| 0x04   | unknown_4  | s4   |    4 |             |
| 0x08   | unknown_5  | s4   |    4 |             |
| 0x0C   | unknown_6  | s4   |    4 |             |
| 0x10   | unknown_7  | f4   |    4 |             |
| 0x14   | unknown_8  | f4   |    4 |             |
| 0x18   | unknown_9  | f4   |    4 |             |
| 0x1C   | unknown_10 | f4   |    4 |             |

---

### lighting

| Offset | Name      | Type | Size | Description |
| ------ | --------- | ---- | ---: | ----------- |
| 0x00   | type      | s2   |    2 |             |
| 0x02   | unknown_0 | s2   |    2 |             |
| 0x04   | unknown_1 | s4   |    4 |             |
| 0x08   | unknown_2 | s4   |    4 |             |
| 0x0C   | unknown_3 | s4   |    4 |             |
| 0x10   | unknown_4 | s4   |    4 |             |
| 0x14   | unknown_5 | s4   |    4 |             |
| 0x18   | unknown_6 | s4   |    4 |             |
| 0x1C   | points    | f4   |  196 |             |

---

### actor

| Offset | Name      | Type | Size | Description |
| ------ | --------- | ---- | ---: | ----------- |
| 0x00   | head      | s4   |    4 |             |
| 0x04   | type      | s2   |    2 |             |
| 0x06   | unknown_0 | s2   |    2 |             |
| 0x08   | unknown_1 | s2   |    2 |             |
| 0x0A   | index     | s2   |    2 |             |
| 0x0C   | x         | f4   |    4 | Position X  |
| 0x10   | y         | f4   |    4 | Position Y  |
| 0x14   | z         | f4   |    4 | Position Z  |
| 0x18   | unknown_2 | s4   |    4 |             |
| 0x1C   | rotation  | s2   |    2 |             |
| 0x1E   | unknown_3 | s2   |    2 |             |
| 0x20   | unknown_4 | s4   |    4 |             |

---

### obj

| Offset | Name       | Type | Size | Description |
| ------ | ---------- | ---- | ---: | ----------- |
| 0x00   | head       | s1   |    1 |             |
| 0x01   | unknown_0  | s1   |    1 |             |
| 0x02   | unknown_1  | s1   |    1 |             |
| 0x03   | unknown_2  | s1   |    1 |             |
| 0x04   | type       | s2   |    2 |             |
| 0x06   | unknown_3  | s2   |    2 |             |
| 0x08   | unknown_4  | s4   |    4 |             |
| 0x0C   | x          | f4   |    4 | Position X  |
| 0x10   | y          | f4   |    4 | Position Y  |
| 0x14   | z          | f4   |    4 | Position Z  |
| 0x18   | x_rotation | s2   |    2 |             |
| 0x1A   | y_rotation | s2   |    2 |             |
| 0x1C   | z_rotation | s2   |    2 |             |
| 0x1E   | unknown_5  | s2   |    2 |             |
| 0x20   | unknown_6  | s4   |    4 |             |

---

### item

| Offset | Name       | Type | Size | Description |
| ------ | ---------- | ---- | ---: | ----------- |
| 0x00   | head       | s4   |    4 |             |
| 0x04   | type       | s4   |    4 |             |
| 0x08   | unknown_0  | s4   |    4 |             |
| 0x0C   | x          | f4   |    4 | Position X  |
| 0x10   | y          | f4   |    4 | Position Y  |
| 0x14   | z          | f4   |    4 | Position Z  |
| 0x18   | x_rotation | s2   |    2 |             |
| 0x1A   | y_rotation | s2   |    2 |             |
| 0x1C   | z_rotation | s2   |    2 |             |
| 0x1E   | unknown_1  | s2   |    2 |             |
| 0x20   | unknown_2  | s4   |    4 |             |

---

### effect

| Offset | Name      | Type | Size | Description |
| ------ | --------- | ---- | ---: | ----------- |
| 0x00   | head      | s4   |    4 |             |
| 0x04   | type      | s2   |    2 |             |
| 0x06   | unknown_0 | s2   |    2 |             |
| 0x08   | unknown_1 | s4   |    4 |             |
| 0x0C   | x         | f4   |    4 | Position X  |
| 0x10   | y         | f4   |    4 | Position Y  |
| 0x14   | z         | f4   |    4 | Position Z  |
| 0x18   | width     | f4   |    4 |             |
| 0x1C   | height    | f4   |    4 |             |
| 0x20   | length    | f4   |    4 |             |
| 0x24   | unknown_2 | s4   |    4 |             |
| 0x28   | unknown_3 | s4   |    4 |             |
| 0x2C   | unknown_4 | s4   |    4 |             |
| 0x30   | unknown_5 | s4   |    4 |             |
| 0x34   | unknown_6 | s4   |    4 |             |
| 0x38   | unknown_7 | s4   |    4 |             |
| 0x3C   | unknown_8 | s4   |    4 |             |
| 0x40   | unknown_9 | s4   |    4 |             |

---

### boundry

| Offset | Name      | Type | Size | Description |
| ------ | --------- | ---- | ---: | ----------- |
| 0x00   | flag1     | u1   |    1 |             |
| 0x01   | flag2     | u1   |    1 |             |
| 0x02   | flag3     | u1   |    1 |             |
| 0x03   | flag4     | u1   |    1 |             |
| 0x04   | type      | s2   |    2 |             |
| 0x06   | unknown_0 | s2   |    2 |             |
| 0x08   | x         | f4   |    4 | Position X  |
| 0x0C   | y         | f4   |    4 | Position Y  |
| 0x10   | z         | f4   |    4 | Position Z  |
| 0x14   | width     | f4   |    4 |             |
| 0x18   | height    | f4   |    4 |             |
| 0x1C   | length    | f4   |    4 |             |
| 0x20   | unknown_1 | u1   |    1 |             |
| 0x21   | unknown_2 | u1   |    1 |             |
| 0x22   | unknown_3 | u1   |    1 |             |
| 0x23   | unknown_4 | u1   |    1 |             |

---

### aot

| Offset | Name        | Type | Size | Description |
| ------ | ----------- | ---- | ---: | ----------- |
| 0x00   | head        | u1   |    1 |             |
| 0x01   | type        | u1   |    1 |             |
| 0x02   | unknown_0   | u1   |    1 |             |
| 0x03   | unknown_1   | u1   |    1 |             |
| 0x04   | unknown_2   | s4   |    4 |             |
| 0x08   | x           | f4   |    4 | Position X  |
| 0x0C   | y           | f4   |    4 | Position Y  |
| 0x10   | z           | f4   |    4 | Position Z  |
| 0x14   | width       | f4   |    4 |             |
| 0x18   | height      | f4   |    4 |             |
| 0x1C   | length      | f4   |    4 |             |
| 0x20   | table_index | u1   |    1 |             |
| 0x21   | unknown_3   | u1   |    1 |             |
| 0x22   | unknown_4   | u1   |    1 |             |
| 0x23   | unknown_5   | u1   |    1 |             |

---

### trigger

| Offset | Name      | Type | Size | Description |
| ------ | --------- | ---- | ---: | ----------- |
| 0x00   | head      | u1   |    1 |             |
| 0x01   | type      | u1   |    1 |             |
| 0x02   | unknown_0 | u1   |    1 |             |
| 0x03   | unknown_1 | u1   |    1 |             |
| 0x04   | unknown_2 | s4   |    4 |             |
| 0x08   | x         | f4   |    4 | Position X  |
| 0x0C   | y         | f4   |    4 | Position Y  |
| 0x10   | z         | f4   |    4 | Position Z  |
| 0x14   | width     | f4   |    4 |             |
| 0x18   | height    | f4   |    4 |             |
| 0x1C   | length    | f4   |    4 |             |
| 0x20   | key       | u1   |    1 |             |
| 0x21   | unknown_3 | u1   |    1 |             |
| 0x22   | unknown_4 | u1   |    1 |             |
| 0x23   | unknown_5 | u1   |    1 |             |

---

### player

| Offset | Name     | Type | Size | Description |
| ------ | -------- | ---- | ---: | ----------- |
| 0x00   | x        | f4   |    4 | Position X  |
| 0x04   | y        | f4   |    4 | Position Y  |
| 0x08   | z        | f4   |    4 | Position Z  |
| 0x0C   | rotation | s4   |    4 |             |

---

### event

| Offset | Name      | Type | Size | Description |
| ------ | --------- | ---- | ---: | ----------- |
| 0x00   | head      | u1   |    1 |             |
| 0x01   | type      | u1   |    1 |             |
| 0x02   | unknown_0 | u1   |    1 |             |
| 0x03   | unknown_1 | u1   |    1 |             |
| 0x04   | unknown_2 | s4   |    4 |             |
| 0x08   | x         | f4   |    4 | Position X  |
| 0x0C   | y         | f4   |    4 | Position Y  |
| 0x10   | z         | f4   |    4 | Position Z  |
| 0x14   | width     | f4   |    4 |             |
| 0x18   | height    | f4   |    4 |             |
| 0x1C   | length    | f4   |    4 |             |
| 0x20   | unknown_3 | u1   |    1 |             |
| 0x21   | unknown_4 | u1   |    1 |             |
| 0x22   | unknown_5 | u1   |    1 |             |
| 0x23   | unknown_6 | u1   |    1 |             |

---

## References

- [biocv_rdx.ksy](https://github.com/kapdap/re-cvx-tools/blob/master/Resources/Formats/biocv_rdx.ksy)
- [RDXplorer](https://github.com/nickworonekin/puyotools/tree/master/tools/RDXplorer)
- [prsutil](https://github.com/essen/prsutil/tree/master/bin)

