(function (g) {
  "use strict";
  function dv(d) {
    return new DataView(d.buffer, d.byteOffset, d.byteLength);
  }
  function ascii(d, o, s) {
    let e = d.subarray(o, o + s);
    const i = e.indexOf(0);
    if (i >= 0) e = e.subarray(0, i);
    try {
      return String.fromCharCode.apply(null, e);
    } catch {
      return "";
    }
  }
  const S_ETTY = 36,
    S_EFF = 68,
    S_ATR = 36,
    S_PLY = 16,
    S_CAM = 680,
    S_BLK = 196,
    S_CUT = 32,
    S_LGT = 224;
  function parseRdx(rawInput) {
    const raw =
      rawInput instanceof Uint8Array ? rawInput : new Uint8Array(rawInput);
    const data = g.RDXPRS.tryDecompress(raw),
      v = dv(data);
    const magic = v.getUint32(0, true) >>> 0;
    const ok =
      magic === 0x41200000 >>> 0 ||
      magic === 0x40051eb8 >>> 0 ||
      magic === 0x40028f5c >>> 0;
    if (!ok) throw new Error("not RDX (magic " + magic.toString(16) + ")");
    const secs = [];
    let i;
    for (i = 0; i < 8; i++) secs.push(v.getInt32(0x10 + i * 4, true));
    let tp = v.getInt32(0x10, true);
    if (tp < 0 || tp + 64 > data.length) tp = 0x80;
    const tables = [],
      counts = [];
    for (i = 0; i < 16; i++) tables.push(v.getInt32(tp + i * 4, true));
    for (i = 0; i < 16; i++) counts.push(v.getInt32(0x100 + i * 4, true));
    const author = ascii(data, 0x60, 32);
    const R = {
      data: data,
      author: author,
      sections: secs,
      tables: tables,
      counts: counts,
      actor: [],
      obj: [],
      item: [],
      effect: [],
      boundary: [],
      aot: [],
      trigger: [],
      route: [],
      player: [],
      cameras: [],
      cuts: [],
      lights: [],
      models: [],
      textures: [],
    };
    function entries(cls, ti) {
      const out = [],
        base = tables[ti],
        n = counts[ti];
      if (!base || n <= 0) return out;
      for (let k = 0; k < n; k++) {
        const o = base + k * cls;
        if (o + cls > data.length) break;
        out.push(o);
      }
      return out;
    }
    R.actor = entries(S_ETTY, 2).map(function (o) {
      return readEtty(data, o);
    });
    R.obj = entries(S_ETTY, 3).map(function (o) {
      return readEtty(data, o);
    });
    R.item = entries(S_ETTY, 4).map(function (o) {
      return readEtty(data, o);
    });
    R.effect = entries(S_EFF, 5).map(function (o) {
      return readEffect(data, o);
    });
    R.boundary = entries(S_ATR, 6).map(function (o) {
      return readAtr(data, o);
    });
    R.aot = entries(S_ATR, 7).map(function (o) {
      return readAtr(data, o);
    });
    R.trigger = entries(S_ATR, 8).map(function (o) {
      return readAtr(data, o);
    });
    R.route = entries(S_ATR, 10).map(function (o) {
      return readAtr(data, o);
    });
    R.player = entries(S_PLY, 9).map(function (o) {
      return readPlayer(data, o);
    });
    R.players = R.player;
    R.routeTable = [];
    try {
      const rtBase = tables[11],
        rn = R.route.length;
      if (rtBase && rn > 0 && rtBase + rn * rn <= data.length) {
        for (let ri = 0; ri < rn * rn; ri++) {
          R.routeTable.push(data[rtBase + ri]);
        }
      }
    } catch {
      /* ignore */
    }
    R.cameras = entries(S_CAM, 0).map(function (o) {
      return readCamera(data, o);
    });
    R.cuts = [];
    try {
      const cutBase = R.cameras.length ? R.cameras[0].cuttp : 0;
      if (cutBase > 0 && cutBase + R.cameras.length * S_CUT <= data.length) {
        for (let cci = 0; cci < R.cameras.length; cci++) {
          R.cuts.push(readCut(data, cutBase + cci * S_CUT));
        }
      }
    } catch {
      /* ignore */
    }
    R.lights = entries(S_LGT, 1).map(function (o) {
      return readLight(data, o);
    });
    readModels(R);
    readTextures(R);
    R.map = modelTableMap(R);
    R.assign = textureAssignment(R);
    R.picIndex = {};
    return R;
  }
  function readEtty(d, o) {
    const v = dv(d);
    return {
      flg: v.getUint32(o, true),
      id: v.getUint16(o + 4, true),
      type: v.getUint16(o + 6, true),
      flr: v.getInt8(o + 8),
      mdlver: d[o + 9],
      wrkNo: d[o + 10],
      prm1: v.getInt8(o + 11),
      pos: [
        v.getFloat32(o + 12, true),
        v.getFloat32(o + 16, true),
        v.getFloat32(o + 20, true),
      ],
      ang: [
        v.getInt16(o + 24, true),
        v.getInt16(o + 28, true),
        v.getInt16(o + 26, true),
      ],
      aspd: v.getInt16(o + 30, true),
      hide: [d[o + 32], d[o + 33], d[o + 34], d[o + 35]],
    };
  }
  function readEffect(d, o) {
    const v = dv(d);
    return {
      flg: v.getInt32(o, true),
      id: v.getInt16(o + 4, true),
      pos: [
        v.getFloat32(o + 12, true),
        v.getFloat32(o + 16, true),
        v.getFloat32(o + 20, true),
      ],
      scale: [
        v.getFloat32(o + 24, true),
        v.getFloat32(o + 28, true),
        v.getFloat32(o + 32, true),
      ],
      ax: v.getInt16(o + 38, true),
      ay: v.getInt16(o + 36, true),
    };
  }
  function readAtr(d, o) {
    const v = dv(d);
    const px = v.getFloat32(o + 8, true),
      py = v.getFloat32(o + 12, true),
      pz = v.getFloat32(o + 16, true);
    const w = v.getFloat32(o + 20, true),
      h = v.getFloat32(o + 24, true),
      dd = v.getFloat32(o + 28, true);
    return {
      flg: d[o],
      type: d[o + 1],
      id: d[o + 2],
      flrNo: v.getInt8(o + 3),
      attr: v.getUint32(o + 4, true),
      pos: [px, py, pz],
      size: [w, h, dd],
      prm: [d[o + 32], d[o + 33], d[o + 34], d[o + 35]],
      lo: [Math.min(px, px + w), Math.min(py, py + h), Math.min(pz, pz + dd)],
      hi: [Math.max(px, px + w), Math.max(py, py + h), Math.max(pz, pz + dd)],
    };
  }
  function readPlayer(d, o) {
    const v = dv(d);
    return {
      pos: [
        v.getFloat32(o, true),
        v.getFloat32(o + 4, true),
        v.getFloat32(o + 8, true),
      ],
      ay: v.getInt32(o + 12, true),
    };
  }
  function readCamera(d, o) {
    const v = dv(d),
      blocks = [];
    for (let i = 0; i < 2; i++) {
      blocks.push(readCameraBlock(d, o + 32 + i * S_BLK));
    }
    return {
      flg: d[o],
      type: d[o + 1],
      flrNo: v.getInt8(o + 2),
      ctabN: d[o + 3],
      cuttp: v.getUint32(o + 4, true),
      cpos: [
        v.getFloat32(o + 8, true),
        v.getFloat32(o + 12, true),
        v.getFloat32(o + 16, true),
      ],
      csize: [
        v.getFloat32(o + 20, true),
        v.getFloat32(o + 24, true),
        v.getFloat32(o + 28, true),
      ],
      blocks: blocks,
    };
  }
  function readCameraBlock(d, b) {
    const v = dv(d);
    function f32(o) {
      return v.getFloat32(o, true);
    }
    function s32(o) {
      return v.getInt32(o, true);
    }
    const hidobj = [],
      hidlgt = [];
    let i;
    for (i = 0; i < 16; i++) hidobj.push(v.getUint32(b + 88 + i * 4, true));
    for (i = 0; i < 8; i++) hidlgt.push(v.getUint32(b + 152 + i * 4, true));
    const px = f32(b + 4),
      py = f32(b + 8),
      pz = f32(b + 12);
    const ax = s32(b + 52),
      ay = s32(b + 56),
      az = s32(b + 60);
    return {
      flg: v.getUint16(b, true),
      lgtclip: d[b + 2],
      spd: v.getInt8(b + 3),
      pos: [px, py, pz],
      ln: f32(b + 16),
      size: [f32(b + 20), f32(b + 24), f32(b + 28)],
      y: [f32(b + 32), f32(b + 36), f32(b + 40), f32(b + 44)],
      amSpd: f32(b + 48),
      ang: [ax, ay, az],
      lax: s32(b + 64),
      lay: s32(b + 68),
      laz: [
        v.getInt16(b + 72, true),
        v.getInt16(b + 74, true),
        v.getInt16(b + 76, true),
        v.getInt16(b + 78, true),
      ],
      aaSpd: v.getInt8(b + 80),
      filNo: v.getInt8(b + 81),
      filRt: v.getInt8(b + 82),
      pers: s32(b + 84),
      hidobj: hidobj,
      hidlgt: hidlgt,
      fogCol: v.getUint32(b + 184, true),
      fogNr: f32(b + 188),
      fogFr: f32(b + 192),
    };
  }
  function readCut(d, o) {
    const v = dv(d);
    return {
      attr: v.getUint32(o, true),
      flrNo: v.getInt8(o + 4),
      atrTp: v.getInt8(o + 7),
      h: v.getFloat32(o + 8, true),
      minx: v.getFloat32(o + 16, true),
      minz: v.getFloat32(o + 20, true),
      maxx: v.getFloat32(o + 24, true),
      maxz: v.getFloat32(o + 28, true),
    };
  }
  function readLight(d, o) {
    const v = dv(d);
    const flg = v.getUint32(o, true),
      type = v.getUint32(o + 4, true);
    const aspd = v.getInt32(o + 8, true),
      lkflg = v.getInt32(o + 12, true),
      lkno = v.getInt32(o + 16, true),
      lkono = v.getInt32(o + 20, true),
      lsrc = v.getInt32(o + 24, true);
    const px = v.getFloat32(o + 28, true),
      py = v.getFloat32(o + 32, true),
      pz = v.getFloat32(o + 36, true);
    const lx = v.getFloat32(o + 40, true),
      ly = v.getFloat32(o + 44, true),
      lz = v.getFloat32(o + 48, true);
    const vx = v.getFloat32(o + 52, true),
      vy = v.getFloat32(o + 56, true),
      vz = v.getFloat32(o + 60, true);
    const spc = v.getFloat32(o + 64, true),
      dif = v.getFloat32(o + 68, true),
      amb = v.getFloat32(o + 72, true);
    const rr = v.getFloat32(o + 76, true),
      gg = v.getFloat32(o + 80, true),
      bb = v.getFloat32(o + 84, true);
    const nr = v.getFloat32(o + 88, true),
      fr = v.getFloat32(o + 92, true);
    const iang = v.getInt32(o + 96, true),
      oang = v.getInt32(o + 100, true);
    const ax = v.getInt32(o + 104, true),
      ay = v.getInt32(o + 108, true),
      az = v.getInt32(o + 112, true);
    const p = [];
    for (let i = 0; i < 49; i++) p.push(v.getFloat32(o + 28 + i * 4, true));
    const ip = [];
    for (let j = 0; j < 49; j++) ip.push(v.getInt32(o + 28 + j * 4, true));
    return {
      flg: flg,
      type: type,
      aspd: aspd,
      lkflg: lkflg,
      lkno: lkno,
      lkono: lkono,
      lsrc: lsrc,
      px: px,
      py: py,
      pz: pz,
      lx: lx,
      ly: ly,
      lz: lz,
      vx: vx,
      vy: vy,
      vz: vz,
      spc: spc,
      dif: dif,
      amb: amb,
      r: rr,
      g: gg,
      b: bb,
      nr: nr,
      fr: fr,
      iang: iang,
      oang: oang,
      ax: ax,
      ay: ay,
      az: az,
      params: p,
      iparams: ip,
      pos: [px, py, pz],
      dir: [vx, vy, vz],
      color: [rr, gg, bb],
      ang: [ax, ay, az],
    };
  }
  function readModels(R) {
    const v = dv(R.data),
      base = R.sections[1];
    if (!base) return;
    const ptrs = [];
    let o = base,
      guard = 0;
    while (ptrs.length < 512 && o + 4 <= R.data.length && guard++ < 600) {
      const slot = ascii(R.data, o, 4),
        val = v.getInt32(o, true);
      o += 4;
      if (val === 0 || slot === "SKIN" || slot.indexOf("MDL") === 0) break;
      if (val < 0 || val >= R.data.length) break;
      ptrs.push(val);
    }
    const motion = R.sections[2] || R.data.length;
    for (let i = 0; i < ptrs.length; i++) {
      const tp = ptrs[i];
      let size = (i + 1 < ptrs.length ? ptrs[i + 1] : motion) - tp;
      if (size < 0) size = 0;
      const end = Math.min(tp + size, R.data.length);
      const blobs = g.RDXMLB.iterMlbBlobs(R.data, tp, end),
        has = [];
      for (let b = 0; b < blobs.length; b++) {
        const bo = blobs[b][0];
        let hh = false;
        if (bo + 16 <= R.data.length) {
          try {
            hh = v.getInt32(bo + 8, true) !== -1;
          } catch {
            /* ignore */
          }
        }
        has.push(hh);
      }
      R.models.push({ offset: tp, size: size, blobs: blobs, blobTex: has });
    }
  }
  function readTextures(R) {
    const v = dv(R.data),
      base = R.sections[4];
    if (!base || base + 4 > R.data.length) return;
    const count = v.getUint32(base, true);
    if (count > 4096) return;
    const ptrs = [];
    for (let i = 0; i < count; i++) {
      const o = base + 4 + i * 4;
      if (o + 4 > R.data.length) break;
      const x = v.getInt32(o, true);
      if (x === 0) break;
      ptrs.push(x);
    }
    for (let k = 0; k < ptrs.length; k++) {
      const tp = ptrs[k],
        size = (k + 1 < ptrs.length ? ptrs[k + 1] : R.data.length) - tp;
      const tab = { offset: tp, size: size, blocks: [] };
      let p = tp;
      const end = tp + size;
      while (p + 8 <= end && p + 8 <= R.data.length) {
        const code = v.getUint32(p, true);
        if (code === 0xffffffff >>> 0) break;
        const sz = v.getInt32(p + 4, true);
        if (sz <= 0) {
          p += 32;
          continue;
        }
        if (p + 32 + sz > R.data.length) break;
        tab.blocks.push({
          code: code,
          size: sz,
          offset: p,
          dataOffset: p + 32,
          kind: blkKind(code),
        });
        p += 32 + sz;
      }
      R.textures.push(tab);
    }
  }
  function blkKind(c) {
    c = c >>> 0;
    if (c === 0x324d4954) return "TIM2";
    if (c === 0x00494c50) return "PLI";
    if (c === 0x52565050 || c === 0x52565054) return "PVR";
    if (c === 0x50565050) return "PVP";
    if (c === 0xffffffff >>> 0) return "END";
    return "UNK";
  }
  function modelTableMap(R) {
    const c = [["room", null]];
    R.actor.forEach(function (e, i) {
      if (e.flg & 1) c.push(["actor", i]);
    });
    for (let n = 0; n < R.obj.length - 4; n++) c.push(["obj", n + 4]);
    for (let m = 0; m < R.item.length; m++) c.push(["item", m]);
    const out = {};
    for (let k = 0; k < c.length && k < R.models.length; k++) out[k] = c[k];
    return out;
  }
  function tableTex(R, ti) {
    return (
      ti >= 0 &&
      ti < R.models.length &&
      R.models[ti].blobTex.length &&
      R.models[ti].blobTex[0]
    );
  }
  function textureAssignment(R) {
    const a = {},
      nt = R.textures.length;
    let t = 0,
      b = 0;
    if (b < R.models.length) {
      if (tableTex(R, 0) && t < nt) {
        a[b + ",0"] = t++;
      }
      b++;
    }
    const seen = {};
    R.actor.forEach(function (e) {
      if (!(e.flg & 1)) return;
      if (b >= R.models.length) return;
      let local = -1;
      const bb = b;
      b++;
      R.models[bb].blobTex.forEach(function (has, bi) {
        if (has) {
          local++;
          const key = e.id + "/" + e.mdlver + "/" + local;
          if (!(key in seen) && t < nt) seen[key] = t++;
          if (key in seen) a[bb + "," + bi] = seen[key];
        } else if (local >= 0) {
          const k2 = e.id + "/" + e.mdlver + "/" + local;
          if (k2 in seen) a[bb + "," + bi] = seen[k2];
        }
      });
    });
    const so = {};
    R.obj.slice(4).forEach(function (e) {
      if (b >= R.models.length) return;
      const bb = b;
      b++;
      if (tableTex(R, bb)) {
        if (!(e.id in so) && t < nt) so[e.id] = t++;
        if (e.id in so) a[bb + ",0"] = so[e.id];
      }
    });
    const si = {};
    R.item.forEach(function (e) {
      if (b >= R.models.length) return;
      const bb = b;
      b++;
      if (tableTex(R, bb)) {
        if (!(e.id in si) && t < nt) si[e.id] = t++;
        if (e.id in si) a[bb + ",0"] = si[e.id];
      }
    });
    return a;
  }
  function pliStateAt(R, ti, bi) {
    let pal = 0,
      ents = [];
    if (ti < 0 || ti >= R.textures.length) return [0, []];
    const blks = R.textures[ti].blocks.slice(0, bi + 1);
    for (let i = 0; i < blks.length; i++) {
      const blk = blks[i];
      let pn = 0,
        pe = [];
      if (blk.kind === "PLI") {
        if (!blk.size) continue;
        const r = g.RDXTIM2.parsePli(R.data, blk.dataOffset);
        pn = r[0];
        pe = r[1];
      } else if (blk.kind === "PVP") {
        if (!blk.size) continue;
        const r2 = g.RDXPVR.parsePvpl(R.data, blk.dataOffset);
        pn = r2[0];
        pe = r2[1];
      } else continue;
      if (pn >= 1) {
        pal = pn;
        ents = pe;
      }
    }
    return [pal, ents];
  }
  function decodeBlock(R, ti, bi) {
    const blk = R.textures[ti].blocks[bi];
    if (blk.kind === "PVR") {
      const st = pliStateAt(R, ti, bi);
      const pal = st[0] > 0 ? st[1] : null;
      return g.RDXPVR.decodePvr(R.data, blk.dataOffset, pal);
    }
    return g.RDXTIM2.decodeTim2(R.data, blk.dataOffset);
  }
  function tablePics(R, ti) {
    if (R.picIndex[ti]) return R.picIndex[ti];
    const pics = [],
      tab = R.textures[ti];
    for (let bi = 0; bi < tab.blocks.length; bi++) {
      const blk = tab.blocks[bi];
      if (blk.kind !== "TIM2" && blk.kind !== "PVR") continue;
      let doc;
      try {
        doc = decodeBlock(R, ti, bi);
      } catch {
        continue;
      }
      const st = pliStateAt(R, ti, bi),
        pn = st[0],
        pe = st[1];
      for (let pi = 0; pi < doc.pictures.length; pi++) {
        const pic = doc.pictures[pi];
        if (!g.RDXTIM2.isVqPath(pic.picFormat) && pn > 0) {
          const vars = [];
          for (let l = 0; l < pe.length; l++) if (pe[l][1] === 0) vars.push(l);
          if (vars.length) {
            vars.forEach(function (lop) {
              pics.push({ bi: bi, pi: pi, variant: lop });
            });
            continue;
          }
        }
        pics.push({ bi: bi, pi: pi, variant: null });
      }
    }
    R.picIndex[ti] = pics;
    return pics;
  }
  g.RDX = {
    parseRdx: parseRdx,
    tablePics: tablePics,
    decodeBlock: decodeBlock,
    pliStateAt: pliStateAt,
  };
})(window);
