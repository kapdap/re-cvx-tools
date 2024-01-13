meta:
  id: biocv_rdx
  title: 'Biohazard: Code Veronica RDX File Format (RDX)'
  application: 'Biohazard: Code Veronica'
  file-extension: rdx
  xref:
    wikidata: Q1050284
  license: CC0-1.0
  endian: le
seq:
  - id: magic
    type: s4
  - id: dummy_1
    size: 12
  - id: ofs_sections
    type: s4
    repeat: expr
    repeat-expr: 8
  - id: dummy_2
    size: 48
  - id: author
    size: 32
    type: str
    encoding: ASCII
  - id: ofs_tables
    type: s4
    repeat: expr
    repeat-expr: 16
  - id: dummy_3
    size: 64
  - id: num_tables
    type: s4
    repeat: expr
    repeat-expr: 16
  - id: dummy_4
    size: 64
  - id: unknown_1
    type: f4
    repeat: expr
    repeat-expr: 16
  - id: dummy_5
    size: 632
  - id: unknown_2
    size: 1
  - id: unknown_3
    size: 1
  - id: unknown_4
    size: 1
  - id: unknown_5
    size: 1
  - id: unknown_6
    type: f4
    repeat: expr
    repeat-expr: 12
types:
  model:
    params:
      - id: i
        type: s4
    instances:
      ofs_end:
        value: 'i < _parent.num_models - 1 ? _parent.ofs_models[i + 1] : _parent.ofs_sections[2]'
      data:
        pos: _parent.ofs_models[i]
        size: ofs_end - _parent.ofs_models[i]
  motion:
    instances:
      ofs_end:
        value: _parent.ofs_sections[3]
      data:
        pos: _parent.ofs_sections[2]
        size: ofs_end - _parent.ofs_sections[2]
  script:
    instances:
      ofs_end:
        value: _parent.ofs_sections[4]
      data:
        pos: _parent.ofs_sections[3]
        size: ofs_end - _parent.ofs_sections[3]
  texture:
    instances:
      data:
        pos: _parent.ofs_sections[4]
        size-eos: true
  camera:
    seq:
      - id: head
        type: s4
      - id: ofs_unknown
        type: s4
      - id: unknown_0
        type: f4
      - id: unknown_1
        type: f4
      - id: unknown_2
        type: f4
      - id: unknown_3
        type: f4
      - id: unknown_4
        type: f4
      - id: unknown_5
        type: f4
      - id: unknown_6
        type: f4
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
      - id: unknown_10
        type: f4
      - id: unknown_11
        type: f4
      - id: unknown_12
        type: f4
      - id: unknown_13
        type: f4
      - id: unknown_14
        type: f4
      - id: unknown_15
        type: f4
      - id: unknown_16
        type: f4
      - id: unknown_17
        type: f4
      - id: unknown_18
        type: f4
      - id: x_rotation
        type: s4
      - id: y_rotation
        type: s4
      - id: z_rotation
        type: s4
      - id: unknown_22
        type: s4
      - id: unknown_23
        type: s4
      - id: unknown_24
        type: f4
      - id: unknown_25
        type: f4
      - id: unknown_26
        type: s4
      - id: perspective
        type: s4
      - id: unknown_28
        type: f4
      - id: unknown_29
        type: f4
      - id: unknown_30
        type: f4
  camera_unknown:
    seq:
      - id: unknown_0
        size: 1
      - id: unknown_1
        size: 1
      - id: unknown_2
        size: 1
      - id: unknown_3
        size: 1
      - id: unknown_4
        type: s4
      - id: unknown_5
        type: s4
      - id: unknown_6
        type: s4
      - id: unknown_7
        type: f4
      - id: unknown_8
        type: f4
      - id: unknown_9
        type: f4
      - id: unknown_10
        type: f4
  lighting:
    seq:
      - id: type
        type: s2
      - id: unknown_0
        type: s2
      - id: unknown_1
        type: s4
      - id: unknown_2
        type: s4
      - id: unknown_3
        type: s4
      - id: unknown_4
        type: s4
      - id: unknown_5
        type: s4
      - id: unknown_6
        type: s4
    instances:
      points:
        type: f4
        repeat: expr
        repeat-expr: 49
  actor:
    seq:
      - id: head
        type: s4
      - id: type
        type: s2
      - id: unknown_0
        type: s2
      - id: unknown_1
        type: s2
      - id: index
        type: s2
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
      - id: unknown_2
        type: s4
      - id: rotation
        type: s2
      - id: unknown_3
        type: s2
      - id: unknown_4
        type: s4
  obj:
    seq:
      - id: head
        type: s1
      - id: unknown_0
        type: s1
      - id: unknown_1
        type: s1
      - id: unknown_2
        type: s1
      - id: type
        type: s2
      - id: unknown_3
        type: s2
      - id: unknown_4
        type: s4
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
      - id: x_rotation
        type: s2
      - id: y_rotation
        type: s2
      - id: z_rotation
        type: s2
      - id: unknown_5
        type: s2
      - id: unknown_6
        type: s4
  item:
    seq:
      - id: head
        type: s4
      - id: type
        type: s4
      - id: unknown_0
        type: s4
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
      - id: x_rotation
        type: s2
      - id: y_rotation
        type: s2
      - id: z_rotation
        type: s2
      - id: unknown_1
        type: s2
      - id: unknown_2
        type: s4
  effect:
    seq:
      - id: head
        type: s4
      - id: type
        type: s2
      - id: unknown_0
        type: s2
      - id: unknown_1
        type: s4
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
      - id: width
        type: f4
      - id: height
        type: f4
      - id: length
        type: f4
      - id: unknown_2
        type: s4
      - id: unknown_3
        type: s4
      - id: unknown_4
        type: s4
      - id: unknown_5
        type: s4
      - id: unknown_6
        type: s4
      - id: unknown_7
        type: s4
      - id: unknown_8
        type: s4
      - id: unknown_9
        type: s4
  boundry:
    seq:
      - id: head
        type: s4
      - id: type
        type: s2
      - id: unknown_0
        type: s2
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
      - id: width
        type: f4
      - id: height
        type: f4
      - id: length
        type: f4
      - id: unknown_1
        type: s1
      - id: unknown_2
        type: s1
      - id: unknown_3
        type: s1
      - id: unknown_4
        type: s1
  aot:
    seq:
      - id: head
        type: s1
      - id: type
        type: s1
      - id: unknown_0
        type: s1
      - id: unknown_1
        type: s1
      - id: unknown_2
        type: s4
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
      - id: width
        type: f4
      - id: height
        type: f4
      - id: length
        type: f4
      - id: table_index
        type: s1
      - id: unknown_3
        type: s1
      - id: unknown_4
        type: s1
      - id: unknown_5
        type: s1
  trigger:
    seq:
      - id: head
        type: s1
      - id: type
        type: s1
      - id: unknown_0
        type: s1
      - id: unknown_1
        type: s1
      - id: unknown_2
        type: s4
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
      - id: width
        type: f4
      - id: height
        type: f4
      - id: length
        type: f4
      - id: key
        type: s1
      - id: unknown_3
        type: s1
      - id: unknown_4
        type: s1
      - id: unknown_5
        type: s1
  player:
    seq:
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
      - id: rotation
        type: s4
  event:
    seq:
      - id: head
        type: s1
      - id: type
        type: s1
      - id: unknown_0
        type: s1
      - id: unknown_1
        type: s1
      - id: unknown_2
        type: s4
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
      - id: width
        type: f4
      - id: height
        type: f4
      - id: length
        type: f4
      - id: unknown_3
        type: s1
      - id: unknown_4
        type: s1
      - id: unknown_5
        type: s1
      - id: unknown_6
        type: s1
instances:
  ofs_models:
    pos: ofs_sections[1]
    type: s4
    repeat: until
    repeat-until: _ == 0
  num_models:
    value: ofs_models.size - 1
  models:
    pos: ofs_models[0]
    type: model(_index)
    repeat: expr
    repeat-expr: num_models
  motions:
    pos: ofs_sections[2]
    type: motion
  scripts:
    pos: ofs_sections[3]
    type: script
  textures:
    pos: ofs_sections[4]
    type: texture
  cameras:
    pos: ofs_tables[0]
    size: 680
    type: camera
    repeat: expr
    repeat-expr: num_tables[0]
    if: ofs_tables[0] != 0
  cameras_unknowns:
    pos: cameras[0].ofs_unknown
    size: 32
    type: camera_unknown
    repeat: expr
    repeat-expr: cameras.size
  lights:
    pos: ofs_tables[1]
    size: 224
    type: lighting
    repeat: expr
    repeat-expr: num_tables[1]
    if: ofs_tables[1] != 0
  actors:
    pos: ofs_tables[2]
    size: 36
    type: actor
    repeat: expr
    repeat-expr: num_tables[2]
    if: ofs_tables[2] != 0
  objs:
    pos: ofs_tables[3]
    size: 36
    type: obj
    repeat: expr
    repeat-expr: num_tables[3]
    if: ofs_tables[3] != 0
  items:
    pos: ofs_tables[4]
    size: 36
    type: item
    repeat: expr
    repeat-expr: num_tables[4]
    if: ofs_tables[4] != 0
  effects:
    pos: ofs_tables[5]
    size: 68
    type: effect
    repeat: expr
    repeat-expr: num_tables[5]
    if: ofs_tables[5] != 0
  boundaries:
    pos: ofs_tables[6]
    size: 36
    type: boundry
    repeat: expr
    repeat-expr: num_tables[6]
    if: ofs_tables[6] != 0
  aots:
    pos: ofs_tables[7]
    size: 36
    type: aot
    repeat: expr
    repeat-expr: num_tables[7]
    if: ofs_tables[7] != 0
  triggers:
    pos: ofs_tables[8]
    size: 36
    type: trigger
    repeat: expr
    repeat-expr: num_tables[8]
    if: ofs_tables[8] != 0
  players:
    pos: ofs_tables[9]
    size: 16
    type: player
    repeat: expr
    repeat-expr: num_tables[9]
    if: ofs_tables[9] != 0
  events:
    pos: ofs_tables[10]
    size: 36
    type: event
    repeat: expr
    repeat-expr: num_tables[10]
    if: ofs_tables[10] != 0
  #unknowns_0:
  #  pos: ofs_tables[11]
  #  size: 0
  #  type: unknown_type_1
  #  repeat: expr
  #  repeat-expr: num_tables[11]
  #  if: ofs_tables[11] != 0
  #unknowns_1:
  #  pos: ofs_tables[12]
  #  size: 0
  #  type: unknown_type_2
  #  repeat: expr
  #  repeat-expr: num_tables[12]
  #  if: ofs_tables[12] != 0
  actions:
    pos: ofs_tables[13]
    size: 2056
    repeat: expr
    repeat-expr: num_tables[13]
    if: ofs_tables[13] != 0
  texts:
    pos: ofs_tables[14]
    size: (ofs_tables[15] != 0 ? ofs_tables[15] : ofs_sections[1]) - ofs_tables[14]
    if: ofs_tables[14] != 0
  sysmes:
    pos: ofs_tables[15]
    size: ofs_sections[1] - ofs_tables[15]
    if: ofs_tables[15] != 0