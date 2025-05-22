# Sony PlayStation 2 TIM2 Binary Format Specification

This table describes the binary layout for TIM2 files, commonly used for textures in PlayStation 2 games.

Reference: [openkh.dev/common/tm2.html](https://openkh.dev/common/tm2.html)

---

## File Header

| Offset | Size | Name           | Type  | Description                                |
| -----: | ---: | -------------- | ----- | ------------------------------------------ |
|   0x00 |    4 | file_id        | ASCII | File signature: `"TIM2"` or `"CLT2"`       |
|   0x04 |    1 | format_version | u1    | Format version                             |
|   0x05 |    1 | format_id      | u1    | Alignment: `0` = 16 bytes, `1` = 128 bytes |
|   0x06 |    2 | pictures       | u2    | Number of picture entries in the file      |
|   0x08 |    4 | reserved_1     | u4    | Reserved                                   |
|   0x0C |    4 | reserved_2     | u4    | Reserved                                   |
|   0x10 |  112 | padding        | bytes | If format_id = 1                           |

---

## Picture Entry Header (repeats `pictures` times)

| Offset |       Size | Name            | Type  | Description                                                                                 |
| -----: | ---------: | --------------- | ----- | ------------------------------------------------------------------------------------------- |
|   0x00 |          4 | total_size      | u4    | Total size of this picture block                                                            |
|   0x04 |          4 | clut_size       | u4    | Size in bytes of CLUT (palette) data                                                        |
|   0x08 |          4 | image_size      | u4    | Size in bytes of image data                                                                 |
|   0x0C |          2 | header_size     | u2    | Size in bytes of the picture header                                                         |
|   0x0E |          2 | clut_colors     | u2    | Number of colors in the CLUT (palette)                                                      |
|   0x10 |          1 | pict_format     | u1    | Picture format flags                                                                        |
|   0x11 |          1 | mipmap_textures | u1    | Number of mipmaps (1 = none)                                                                |
|   0x12 |          1 | clut_type       | u1    | Bit 7: [CSM](#csm-clut-storage-mode), Bit 6: compound, Bits 5..0: [color_type](#color_type) |
|   0x13 |          1 | image_type      | u1    | Image pixel format (see: [color_type](#color_type))                                         |
|   0x14 |          2 | image_width     | u2    | Width in pixels                                                                             |
|   0x16 |          2 | image_height    | u2    | Height in pixels                                                                            |
|   0x18 |          8 | gs_tex_0        | u8    | GS TEX0 register (see: [GSTex Register](#gstex-register))                                   |
|   0x20 |          8 | gs_tex_1        | u8    | GS TEX1 register (see: [GSTex Register](#gstex-register))                                   |
|   0x28 |          4 | gs_tex_flags    | u4    | GS TEXA/FBA/PABE flags                                                                      |
|   0x2C |          4 | gs_tex_clut     | u4    | GS TEXCLUT register                                                                         |
|    ... |        ... | mipmap_header   | ...   | If mipmap_textures > 1 (see: [Mipmap Header](#mipmap-header-optional))                      |
|    ... |        ... | ex_header       | ...   | Optional extended header (see: [Extended Header](#extended-header-optional))                |
|    ... |  clut_size | clut_data       | bytes | CLUT (palette) data                                                                         |
|    ... | image_size | image_data      | bytes | Image data                                                                                  |

---

## GSTex Register

|  Bits | Name | Type | Description                                                  |
| ----: | ---- | ---- | ------------------------------------------------------------ |
|  0–13 | tbp0 | u14  | Texture buffer pointer, tbp0 \* 0x100 = VRAM address         |
| 14–19 | tbw  | u6   | Texture buffer width                                         |
| 20–25 | psm  | u6   | Pixel storage format (see: [psf](#psf-pixel-storage-format)) |
| 26–29 | tw   | u4   | Texture width exponent (pixels = 2^tw)                       |
| 30–33 | th   | u4   | Texture height exponent (pixels = 2^th)                      |
|    34 | tcc  | u1   | Texture color component control (1 = RGBA, 0 = RGB)          |
| 35–36 | tfx  | u2   | Texture function (see: [tfx](#tfx-texture-function))         |
| 37–50 | cbp  | u14  | CLUT buffer pointer, cbp \* 0x100 = VRAM address             |
| 51–54 | cpsm | u4   | CLUT storage format (see: [csf](#csf-clut-storage-format))   |
|    55 | csm  | u1   | CLUT storage mode (see: [csm](#csm-clut-storage-mode))       |
| 56–60 | csa  | u5   | CLUT entry offset                                            |
| 61–63 | cld  | u3   | CLUT load control                                            |

---

## Mipmap Header (optional)

| Offset | Size | Name           | Type  | Description                      |
| -----: | ---: | -------------- | ----- | -------------------------------- |
|   0x00 |    8 | gs_miptbp1     | u8    | GS MIPTBP1 register              |
|   0x08 |    8 | gs_miptbp2     | u8    | GS MIPTBP2 register              |
|   0x10 | 4\*N | mm_image_sizes | u4[N] | Byte size of each mipmap texture |

---

## Extended Header (optional)

|              Offset |           Size | Name            | Type  | Description                                     |
| ------------------: | -------------: | --------------- | ----- | ----------------------------------------------- |
|                0x00 |              4 | ex_header_id    | ASCII | Extended header signature "eXt\0"               |
|                0x04 |              4 | user_space_size | u4    | Total size of the user space (including header) |
|                0x08 |              4 | user_data_size  | u4    | Size of the user data section                   |
|                0x0C |              4 | reserved        | u4    | Reserved                                        |
|                0x10 | user_data_size | user_data       | bytes | Optional user data section                      |
| 0x10+user_data_size |            ... | comment         | ASCII | Optional null-terminated comment string         |

---

## CLUT/Palette Data

| Offset |      Size | Name      | Type  | Description             |
| -----: | --------: | --------- | ----- | ----------------------- |
|   0x00 | clut_size | clut_data | bytes | CLUT/palette data block |

---

## Image Data

| Offset |       Size | Name       | Type  | Description      |
| -----: | ---------: | ---------- | ----- | ---------------- |
|   0x00 | image_size | image_data | bytes | Image data block |

---

## Enums

### format_id

| Value | Name     | Description        |
| ----: | -------- | ------------------ |
|     0 | align16  | 16-byte alignment  |
|     1 | align128 | 128-byte alignment |

### color_type

| Value | Name   | Description                        |
| ----: | ------ | ---------------------------------- |
|     0 | none   | No color (CLUT-only)               |
|     1 | rgb16  | 16-bit direct color (5:5:5:1 RGBA) |
|     2 | rgb24  | 24-bit direct color (8:8:8 RGB)    |
|     3 | rgb32  | 32-bit direct color (8:8:8:8 RGBA) |
|     4 | idtex4 | 4-bit indexed color (palette)      |
|     5 | idtex8 | 8-bit indexed color (palette)      |

### psf (Pixel Storage Format)

| Value | Name     | Description                        |
| ----: | -------- | ---------------------------------- |
|     0 | psmct32  | 32-bit RGBA                        |
|     1 | psmct24  | 24-bit RGB                         |
|     2 | psmct16  | 16-bit RGBA (5:5:5:1)              |
|    10 | psmct16s | 16-bit RGBA (swizzled)             |
|    19 | psmt8    | 8-bit indexed (palette)            |
|    20 | psmt4    | 4-bit indexed (palette)            |
|    26 | psmt4hl  | 4-bit indexed, high/low (palette)  |
|    27 | psmt8h   | 8-bit indexed, high (palette)      |
|    44 | psmt4hh  | 4-bit indexed, high/high (palette) |
|    48 | psmz32   | 32-bit Z-buffer                    |
|    49 | psmz24   | 24-bit Z-buffer                    |
|    50 | psmz16   | 16-bit Z-buffer                    |
|    58 | psmz16s  | 16-bit Z-buffer (swizzled)         |

### csf (CLUT Storage Format)

| Value | Name     | Description            |
| ----: | -------- | ---------------------- |
|     0 | psmct32  | 32-bit RGBA            |
|     1 | psmct24  | 24-bit RGB             |
|     2 | psmct16  | 16-bit RGBA (5:5:5:1)  |
|    10 | psmct16s | 16-bit RGBA (swizzled) |

### csm (CLUT Storage Mode)

| Value | Name | Description           |
| ----: | ---- | --------------------- |
|     0 | csm1 | Swizzled (0x20 bytes) |
|     1 | csm2 | Sequential (linear)   |

### tfx (Texture Function)

| Value | Name      | Description        |
| ----: | --------- | ------------------ |
|     0 | modulate  | Modulate (default) |
|     1 | decal     | Decal              |
|     2 | hilight   | Highlight          |
|     3 | hilight_2 | Highlight 2        |
