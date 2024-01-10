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
  header:
    pos: 16
    type: s4
    repeat: expr
    repeat-expr: 8
  author:
    pos: 96
    size: 32
    type: str
    encoding: ASCII
  index:
    pos: header[0]
    type: s4
    repeat: expr
    repeat-expr: 16
  index_count:
    pos: header[0] + 128
    type: s4
    repeat: expr
    repeat-expr: 16
  cameras:
    pos: index[0]
    size: 680
    type: camera
    repeat: expr
    repeat-expr: index_count[0]
  lights:
    pos: index[1]
    size: 224
    type: lighting
    repeat: expr
    repeat-expr: index_count[1]
  actors:
    pos: index[2]
    size: 36
    type: actor
    repeat: expr
    repeat-expr: index_count[2]
  objects:
    pos: index[3]
    size: 36
    type: objectx
    repeat: expr
    repeat-expr: index_count[3]
  items:
    pos: index[4]
    size: 36
    type: item
    repeat: expr
    repeat-expr: index_count[4]
  effects:
    pos: index[5]
    size: 68
    type: effect
    repeat: expr
    repeat-expr: index_count[5]
  boundaries:
    pos: index[6]
    size: 36
    type: boundry
    repeat: expr
    repeat-expr: index_count[6]
  aots:
    pos: index[7]
    size: 36
    type: aot
    repeat: expr
    repeat-expr: index_count[7]
  triggers:
    pos: index[8]
    size: 36
    type: trigger
    repeat: expr
    repeat-expr: index_count[8]
  player:
    pos: index[9]
    size: 16
    type: player
    repeat: expr
    repeat-expr: index_count[9]
  events:
    pos: index[10]
    size: 36
    type: event
    repeat: expr
    repeat-expr: index_count[10]
  #unknowns_0:
  #  pos: index[11]
  #  size: 0
  #  type: unknown_0
  #  repeat: expr
  #  repeat-expr: index_count[11]
  #unknowns_1:
  #  pos: index[12]
  #  size: 0
  #  type: unknown_1
  #  repeat: expr
  #  repeat-expr: index_count[12]
  #actions:
  #  pos: index[13]
  #  size: 2056
  #  type: actor
  #  repeat: expr
  #  repeat-expr: index_count[13]
  #texts:
  #  pos: index[14]
  #  type: text
  #sysmes:
  #  pos: index[15]
  #  type: mes