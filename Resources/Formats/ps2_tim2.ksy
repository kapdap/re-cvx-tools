meta:
  id: ps2_tim2
  title: 'Sony PlayStation 2 TIM2'
  application: 'Sony PlayStation 2 TIM2'
  file-extension: tm2
  license: CC0-1.0
  endian: le

doc-ref:
  - https://openkh.dev/common/tm2.html

seq:
  - id: header
    type: file_header
  - id: pictures
    type: picture
    repeat: expr
    repeat-expr: header.pictures

enums:
  # Alignment Format
  format_id:
    0: align16 # 16-byte alignment for all headers
    1: align128 # 128-byte alignment for file header, 16 for others

  # Pixel Color Type
  color_type:
    0: none # No color (used for CLUT-only data)
    1: rgb16 # 16-bit direct color (5:5:5:1 RGBA)
    2: rgb24 # 24-bit direct color (8:8:8 RGB)
    3: rgb32 # 32-bit direct color (8:8:8:8 RGBA)
    4: idtex4 # 4-bit indexed color (palette)
    5: idtex8 # 8-bit indexed color (palette)

  # Pixel Storage Format
  psf:
    0: psmct32
    1: psmct24
    2: psmct16
    10: psmct16s
    19: psmt8
    20: psmt4
    26: psmt4hl
    27: psmt8h
    44: psmt4hh
    48: psmz32
    49: psmz24
    50: psmz16
    58: psmz16s

  # CLUT Storage Format
  csf:
    0: psmct32
    1: psmct24
    2: psmct16
    10: psmct16s

  # CLUT Storage Mode
  csm:
    0: csm1 # Swizzled (0x20 bytes)
    1: csm2 # Sequential

  # Texture Function
  tfx:
    0: modulate
    1: decal
    2: hilight
    3: hilight_2

types:
  file_header:
    seq:
      - id: file_id
        type: str
        size: 4
        encoding: ASCII
        doc: 'File signature ("TIM2" or "CLT2")'
      - id: format_version
        type: u1
        doc: 'Format version, usually 4'
      - id: format_id
        type: u1
        enum: format_id
        doc: '0 = 16-byte alignment, 1 = 128-byte alignment'
      - id: pictures
        type: u2
        doc: 'Number of picture entries in the file'
      - id: reserved_1
        type: u4
      - id: reserved_2
        type: u4
      - id: ext_data
        size: 0x70
        if: format_id == format_id::align128
        doc: 'Padding/extended data for 128-byte alignment'

  picture:
    seq:
      - id: total_size
        type: u4
        doc: 'Total size of the picture block'
      - id: clut_size
        type: u4
        doc: 'Size of the CLUT data'
      - id: image_size
        type: u4
        doc: 'Size of the image data'
      - id: header_size
        type: u2
        doc: 'Size of the picture header'
      - id: clut_colors
        type: u2
        doc: 'Number of colors in the CLUT'
      - id: pict_format
        type: u1
        doc: 'Picture format flags'
      - id: mipmap_textures
        type: u1
        doc: 'Number of mipmaps'
      - id: clut_type
        type: u1
        doc: 'CLUT type: bit 7 = storage mode (CSM), bit 6 = compound, bits 5..0 = color_type'
      - id: image_type
        type: u1
        enum: color_type
        doc: 'Image type (pixel format for image data)'
      - id: image_width
        type: u2
        doc: 'Width of the picture'
      - id: image_height
        type: u2
        doc: 'Height of the picture'
      - id: gs_tex_0
        type: gs_tex
        doc: 'GS register TEX0'
      - id: gs_tex_1
        type: gs_tex
        doc: 'GS register TEX1'
      - id: gs_tex_flags
        type: u4
        doc: 'TEXA/FBA/PABE flags'
      - id: gs_tex_clut
        type: u4
        doc: 'GS register TEXCLUT'
      - id: mipmap_header
        type: mipmap_header
        if: mipmap_textures > 1
        doc: 'Mipmap header, only present if mipmap_textures > 1'
      - id: ex_header
        type: ex_header
        if: _io.pos < (_io.size - image_size - clut_size)
        doc: 'Optional extended header'
      - id: clut_data
        size: clut_size
        if: clut_size > 0
        doc: 'CLUT (palette) data'
      - id: image_data
        size: image_size
        doc: 'Main image data'

    instances:
      clut_type_csm:
        value: (clut_type & 0x80) >> 7
        enum: csm
        doc: '0: CSM1 (swizzled), 1: CSM2 (sequential)'
      clut_type_compound:
        value: (clut_type & 0x40) >> 6
        doc: 'Compound flag (valid only for CSM1, 16 colors)'
      clut_type_pixel:
        value: clut_type & 0x3f
        enum: color_type
        doc: 'Palette pixel format'

  gs_tex:
    seq:
      - id: raw
        type: u8
        doc: 'Raw 64-bit GS register value'
    instances:
      tbp0:
        value: raw & 0x3fff
        doc: 'Texture buffer pointer (TBP0): start address in GS VRAM (in 256-byte units)'
      tbp0_addr:
        value: tbp0 * 0x100
        doc: 'VRAM address in bytes for texture base'
      tbw:
        value: (raw >> 14) & 0x3f
        doc: 'Texture buffer width (TBW): width in 64-pixel units'
      psm:
        value: (raw >> 20) & 0x3f
        enum: psf
        doc: 'Pixel storage mode'
      tw:
        value: (raw >> 26) & 0xf
        doc: 'Texture width exponent (TW): 2^TW pixels'
      th:
        value: (raw >> 30) & 0xf
        doc: 'Texture height exponent (TH): 2^TH pixels'
      tcc:
        value: (raw >> 34) & 0x1
        doc: 'Texture color component control (TCC): 1 = RGBA, 0 = RGB'
      tfx:
        value: (raw >> 35) & 0x3
        enum: tfx
        doc: 'Texture function (TFX): blending mode'
      cbp:
        value: (raw >> 37) & 0x3fff
        doc: 'CLUT buffer pointer (CBP): base address for palette in VRAM'
      cbp_addr:
        value: cbp * 0x100
        doc: 'VRAM address in bytes for CLUT base'
      cpsm:
        value: (raw >> 51) & 0xf
        enum: csf
        doc: 'Pixel storage mode for CLUT data'
      csm:
        value: (raw >> 55) & 0x1
        enum: csm
        doc: 'CLUT storage mode: 0 = CSM1 (swizzled), 1 = CSM2 (sequential)'
      csa:
        value: (raw >> 56) & 0x1f
        doc: 'CLUT entry offset (CSA): offset within palette'
      cld:
        value: (raw >> 61) & 0x7
        doc: 'CLUT load control (CLD): controls loading of CLUT to GS'

  mipmap_header:
    seq:
      - id: gs_miptbp1
        type: u8
        doc: 'GS register MIPTBP1'
      - id: gs_miptbp2
        type: u8
        doc: 'GS register MIPTBP2'
      - id: mm_image_sizes
        type: u4
        repeat: expr
        repeat-expr: _parent.mipmap_textures
        doc: 'Byte size of each mipmap texture'

  ex_header:
    seq:
      - id: ex_header_id
        type: str
        size: 4
        encoding: ASCII
        doc: 'Extended header signature ("eXt\\x00")'
      - id: user_space_size
        type: u4
      - id: user_data_size
        type: u4
      - id: reserved
        type: u4
      - id: ext_data
        size: 64
        doc: 'Extended data (typically comment string or tool-specific data)'
