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
types:
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
  objectx:
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
  sections:
    pos: 16
    type: s4
    repeat: expr
    repeat-expr: 8
  author:
    pos: 96
    size: 32
    type: str
    encoding: ASCII
  tables:
    pos: sections[0]
    type: s4
    repeat: expr
    repeat-expr: 16
  tables_nums:
    pos: sections[0] + 128
    type: s4
    repeat: expr
    repeat-expr: 16
  cameras:
    pos: tables[0]
    size: 680
    type: camera
    repeat: expr
    repeat-expr: tables_nums[0]
  lights:
    pos: tables[1]
    size: 224
    type: lighting
    repeat: expr
    repeat-expr: tables_nums[1]
  actors:
    pos: tables[2]
    size: 36
    type: actor
    repeat: expr
    repeat-expr: tables_nums[2]
  objects:
    pos: tables[3]
    size: 36
    type: objectx
    repeat: expr
    repeat-expr: tables_nums[3]
  items:
    pos: sections[4]
    size: 36
    type: item
    repeat: expr
    repeat-expr: tables_nums[4]
  effects:
    pos: sections[5]
    size: 68
    type: effect
    repeat: expr
    repeat-expr: tables_nums[5]
  boundaries:
    pos: tables[6]
    size: 36
    type: boundry
    repeat: expr
    repeat-expr: tables_nums[6]
  aots:
    pos: tables[7]
    size: 36
    type: aot
    repeat: expr
    repeat-expr: tables_nums[7]
  triggers:
    pos: tables[8]
    size: 36
    type: trigger
    repeat: expr
    repeat-expr: tables_nums[8]
  player:
    pos: tables[9]
    size: 16
    type: player
    repeat: expr
    repeat-expr: tables_nums[9]
  events:
    pos: tables[10]
    size: 36
    type: event
    repeat: expr
    repeat-expr: tables_nums[10]
  #unknowns_0:
  #  pos: tables[11]
  #  size: 0
  #  type: unknown_0
  #  repeat: expr
  #  repeat-expr: tables_nums[11]
  #unknowns_1:
  #  pos: tables[12]
  #  size: 0
  #  type: unknown_1
  #  repeat: expr
  #  repeat-expr: tables_nums[12]
  #actions:
  #  pos: tables[13]
  #  size: 2056
  #  type: actor
  #  repeat: expr
  #  repeat-expr: tables_nums[13]
  #texts:
  #  pos: tables[14]
  #  type: text
  #sysmes:
  #  pos: tables[15]
  #  type: mes