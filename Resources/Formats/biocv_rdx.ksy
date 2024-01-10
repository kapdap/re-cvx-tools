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
types:
  model:
    params:
      - id: i
        type: s4
    instances:
      ofs_end:
        value: 'i < _parent.ofs_models.size - 2 ? _parent.ofs_models[i + 1] : _parent.ofs_sections[2]'
      data:
        pos: _parent.ofs_models[i]
        size: ofs_end - _parent.ofs_models[i]
  camera:
    seq:
      - id: type
        type: s2
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
  ofs_tables:
    pos: ofs_sections[0]
    type: s4
    repeat: expr
    repeat-expr: 16
  num_tables:
    pos: ofs_sections[0] + 128
    type: s4
    repeat: expr
    repeat-expr: 16
  ofs_models:
    pos: ofs_sections[1]
    type: s4
    repeat: until
    repeat-until: _ == 0
  models:
    pos: ofs_models[0]
    type: model(_index)
    repeat: expr
    repeat-expr: ofs_models.size - 1
  cameras:
    pos: ofs_tables[0]
    size: 680
    type: camera
    repeat: expr
    repeat-expr: num_tables[0]
  lights:
    pos: ofs_tables[1]
    size: 224
    type: lighting
    repeat: expr
    repeat-expr: num_tables[1]
  actors:
    pos: ofs_tables[2]
    size: 36
    type: actor
    repeat: expr
    repeat-expr: num_tables[2]
  objs:
    pos: ofs_tables[3]
    size: 36
    type: obj
    repeat: expr
    repeat-expr: num_tables[3]
  items:
    pos: ofs_tables[4]
    size: 36
    type: item
    repeat: expr
    repeat-expr: num_tables[4]
  effects:
    pos: ofs_tables[5]
    size: 68
    type: effect
    repeat: expr
    repeat-expr: num_tables[5]
  boundaries:
    pos: ofs_tables[6]
    size: 36
    type: boundry
    repeat: expr
    repeat-expr: num_tables[6]
  aots:
    pos: ofs_tables[7]
    size: 36
    type: aot
    repeat: expr
    repeat-expr: num_tables[7]
  triggers:
    pos: ofs_tables[8]
    size: 36
    type: trigger
    repeat: expr
    repeat-expr: num_tables[8]
  player:
    pos: ofs_tables[9]
    size: 16
    type: player
    repeat: expr
    repeat-expr: num_tables[9]
  events:
    pos: ofs_tables[10]
    size: 36
    type: event
    repeat: expr
    repeat-expr: num_tables[10]
  #unknowns_0:
  #  pos: ofs_tables[11]
  #  size: 0
  #  type: unknown_0
  #  repeat: expr
  #  repeat-expr: num_tables[11]
  #unknowns_1:
  #  pos: ofs_tables[12]
  #  size: 0
  #  type: unknown_1
  #  repeat: expr
  #  repeat-expr: num_tables[12]
  #actions:
  #  pos: ofs_tables[13]
  #  size: 2056
  #  type: actor
  #  repeat: expr
  #  repeat-expr: num_tables[13]
  #texts:
  #  pos: ofs_tables[14]
  #  type: text
  #sysmes:
  #  pos: ofs_tables[15]
  #  type: mes