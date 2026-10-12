(function (g) {
  "use strict";
  const CN = 0x00,
    CB_BA = 0x01,
    CB_DA = 0x02,
    CB_EXP = 0x03,
    TID = 0x08;
  const CM_D = 0x11,
    CM_A = 0x12,
    CM_DA = 0x13,
    CM_S = 0x14,
    CM_DS = 0x15,
    CM_AS = 0x16,
    CM_DAS = 0x17;
  const CV = 0x22,
    CV_VN = 0x29,
    CV_VN_PS2 = 0x33,
    CO_P3 = 0x38,
    CO_P4 = 0x39,
    CO_ST = 0x3a;
  const CS = 0x40,
    CS_UVN = 0x41,
    CS_UVH = 0x42,
    END = 0xff;
  const UVH_SCALE = 0.0009775171,
    UVN_SCALE = 0.003921569;
  function dv(d) {
    return new DataView(d.buffer, d.byteOffset, d.byteLength);
  }
  function readModel(data, datTop, mno) {
    const v = dv(data);
    const model = {
      verts: [],
      normals: [],
      faces: [],
      uvs: [],
      texids: [],
      mats: [],
      materials: [],
      faceBlend: [],
      faceFlat: [],
      faceAttr: [],
      vtype: -1,
      notes: [],
    };
    if (mno + 24 > data.length) throw new Error("model node out of range");
    const vlistOff = v.getInt32(mno, true),
      plistOff = v.getInt32(mno + 4, true);
    if (vlistOff === -1 || vlistOff === 0) {
      model.notes.push("no vlist");
      return model;
    }
    const vo = datTop + vlistOff;
    const vt = vo >= 0 && vo < data.length ? data[vo] : -1;
    if (vt === CV_VN_PS2) readVlistPS2(data, vo, model);
    else if (vt === CV_VN) readVlistCVVN(data, vo, model);
    else if (vt === CV) readVlistCV(data, vo, model);
    else {
      model.notes.push("unsupported vertex type " + vt);
      return model;
    }
    model.vtype = vt;
    if (plistOff === -1) {
      model.notes.push("no plist");
      return model;
    }
    if (plistOff === 0) {
      const t0 = datTop >= 0 && datTop < data.length ? data[datTop] : END;
      if (t0 === END) return model;
      if (t0 >= 76) {
        model.notes.push("no plist");
        return model;
      }
    }
    readPlist(data, datTop + plistOff, model);
    return model;
  }
  function ensureVerts(model, need) {
    while (model.verts.length < need) {
      model.verts.push([0, 0, 0]);
      model.normals.push([0, 0, 0]);
    }
  }
  function readVlistPS2(data, o, model) {
    const v = dv(data);
    const usOfs = v.getUint16(o + 4, true),
      usMax = v.getUint16(o + 6, true);
    const base = o + 64;
    ensureVerts(model, usOfs + usMax);
    for (let i = 0; i < usMax; i++) {
      const bo = base + i * 32;
      if (bo + 28 > data.length) {
        model.notes.push("vertex overrun");
        break;
      }
      model.verts[usOfs + i] = [
        v.getFloat32(bo, true),
        v.getFloat32(bo + 4, true),
        v.getFloat32(bo + 8, true),
      ];
      model.normals[usOfs + i] = [
        v.getFloat32(bo + 16, true),
        v.getFloat32(bo + 20, true),
        v.getFloat32(bo + 24, true),
      ];
    }
  }
  function readVlistCV(data, o, model) {
    const v = dv(data);
    const usOfs = v.getUint16(o + 4, true),
      usMax = v.getUint16(o + 6, true);
    const base = o + 8;
    ensureVerts(model, usOfs + usMax);
    for (let i = 0; i < usMax; i++) {
      const bo = base + i * 12;
      if (bo + 12 > data.length) break;
      model.verts[usOfs + i] = [
        v.getFloat32(bo, true),
        v.getFloat32(bo + 4, true),
        v.getFloat32(bo + 8, true),
      ];
      model.normals[usOfs + i] = [0, 0, 0];
    }
  }
  function readVlistCVVN(data, o, model) {
    const v = dv(data);
    const usOfs = v.getUint16(o + 4, true),
      usMax = v.getUint16(o + 6, true);
    const base = o + 8;
    ensureVerts(model, usOfs + usMax);
    for (let i = 0; i < usMax; i++) {
      const bo = base + i * 24;
      if (bo + 24 > data.length) break;
      model.verts[usOfs + i] = [
        v.getFloat32(bo, true),
        v.getFloat32(bo + 4, true),
        v.getFloat32(bo + 8, true),
      ];
      model.normals[usOfs + i] = [
        v.getFloat32(bo + 12, true),
        v.getFloat32(bo + 16, true),
        v.getFloat32(bo + 20, true),
      ];
    }
  }
  function readMaterial(data, o, t, model) {
    const need = {};
    need[CM_D] = 4;
    need[CM_A] = 4;
    need[CM_DA] = 8;
    need[CM_S] = 4;
    need[CM_DS] = 8;
    need[CM_AS] = 8;
    need[CM_DAS] = 12;
    if (o + 4 + need[t] > data.length) return -1;
    let p = o + 4;
    const e = {
      type: t,
      attr: data[o + 1] || 0,
      diffuse: null,
      ambient: null,
      specular: null,
      exponent: null,
      alpha: 1,
    };
    function col(q) {
      return [data[q + 2] / 255, data[q + 1] / 255, data[q] / 255];
    }
    if (t === CM_D || t === CM_DA || t === CM_DS || t === CM_DAS) {
      e.diffuse = col(p);
      e.alpha = data[p + 3] / 255;
      p += 4;
    }
    if (t === CM_A || t === CM_DA || t === CM_AS || t === CM_DAS) {
      e.ambient = col(p);
      p += 4;
    }
    if (t === CM_S || t === CM_DS || t === CM_AS || t === CM_DAS) {
      e.specular = col(p);
      e.exponent = data[p + 3];
      p += 4;
    }
    model.materials.push(e);
    return o + 4 + need[t];
  }
  function readStrips(data, o, t, texid, mat, model, attr) {
    const scale = t === CS ? null : t === CS_UVN ? UVN_SCALE : UVH_SCALE;
    const blend = !!(attr & 0x08),
      flat = !!(attr & 0x20);
    const v = dv(data);
    if (o + 6 > data.length) return -1;
    const first = v.getUint16(o + 4, true);
    const usNext = t === CS ? (first >> 14) & 3 : 0;
    const nstrips = first & 0x3fff;
    let p = o + 6;
    for (let s = 0; s < nstrips; s++) {
      if (p + 2 > data.length) break;
      const h = v.getInt16(p, true);
      p += 2;
      const rev = h < 0;
      const n = rev ? -h : h;
      const est = scale === null ? 2 : 6,
        els = [];
      let ok = true;
      for (let k = 0; k < n; k++) {
        if (p + est > data.length) {
          ok = false;
          break;
        }
        const idx = v.getUint16(p, true);
        let u = 0,
          w = 0;
        if (scale !== null) {
          u = v.getUint16(p + 2, true) * scale;
          w = v.getUint16(p + 4, true) * scale;
          p += 6;
        } else {
          p += 2;
        }
        els.push([idx, u, w]);
        if (scale === null && k >= 2 && usNext) {
          if (p + usNext * 2 > data.length) {
            ok = false;
            break;
          }
          p += usNext * 2;
        }
      }
      if (!ok) {
        model.notes.push("strip overrun");
        break;
      }
      const nv = model.verts.length;
      let bad = false;
      for (let q = 0; q < els.length; q++) {
        if (els[q][0] < 0 || els[q][0] >= nv) {
          bad = true;
          break;
        }
      }
      if (bad) {
        model.notes.push("strip index out of range");
        continue;
      }
      for (let f = 0; f + 2 < els.length; f++) {
        let a = els[f],
          b = els[f + 1];
        const c = els[f + 2];
        if (a[0] === b[0] || b[0] === c[0] || a[0] === c[0]) continue;
        if ((f % 2 === 1) !== rev) {
          const tmp = a;
          a = b;
          b = tmp;
        }
        model.faces.push([a[0], b[0], c[0]]);
        model.uvs.push([
          [a[1], a[2]],
          [b[1], b[2]],
          [c[1], c[2]],
        ]);
        model.texids.push(texid);
        model.mats.push(mat);
        model.faceBlend.push(blend);
        model.faceFlat.push(flat);
        model.faceAttr.push(attr);
      }
    }
    return p;
  }
  function skipCVN(data, o, t) {
    const v = dv(data);
    if (t === CV_VN_PS2) {
      if (o + 64 > data.length) return -1;
      const m = v.getUint16(o + 6, true),
        end = o + 64 + m * 32;
      return end > data.length ? -1 : end;
    }
    if (o + 8 > data.length) return -1;
    const c = v.getUint16(o + 6, true);
    const e2 = t === CV ? o + 8 + c * 12 : o + 8 + c * 24;
    return e2 > data.length ? -1 : e2;
  }
  function readPlist(data, o, model) {
    const v = dv(data);
    let texid = 0,
      mat = -1,
      guard = 0;
    for (;;) {
      if (++guard > 100000 || o + 1 > data.length || o < 0) {
        model.notes.push("plist overrun");
        return;
      }
      const t = data[o];
      if (t === END) return;
      if (t >= 76) {
        model.notes.push("unknown chunk " + t);
        return;
      }
      if (t === CN) {
        o += 2;
        continue;
      }
      if (t === CB_BA || t === CB_DA || t === CB_EXP) {
        if (o + 4 > data.length) return;
        o += 4;
        continue;
      }
      if (t >= TID && t <= 0x0f) {
        if (o + 4 > data.length) return;
        if (t === TID) texid = v.getUint16(o + 2, true) & 0xfff;
        o += 4;
        continue;
      }
      if (o + 4 > data.length) return;
      const sizew = v.getUint16(o + 2, true);
      if (
        t === CM_D ||
        t === CM_A ||
        t === CM_DA ||
        t === CM_S ||
        t === CM_DS ||
        t === CM_AS ||
        t === CM_DAS
      ) {
        o = readMaterial(data, o, t, model);
        if (o < 0) return;
        mat = model.materials.length - 1;
        continue;
      }
      if (t === CS || t === CS_UVN || t === CS_UVH) {
        const attr = data[o + 1] || 0,
          no = readStrips(data, o, t, texid, mat, model, attr);
        if (no < 0) return;
        o = no;
        continue;
      }
      if (t === CO_P3) {
        if (o + 6 > data.length) return;
        const word = v.getUint16(o + 4, true),
          un = (word >> 14) & 3,
          up = word & 0x3fff,
          e3 = o + 6 + up * (3 + un) * 2;
        if (e3 > data.length) return;
        o = e3;
        continue;
      }
      if (t === CO_P4 || t === CO_ST) {
        const e4 = o + 4 + sizew * 2;
        if (e4 > data.length) return;
        o = e4;
        continue;
      }
      if (t === CV_VN_PS2 || t === CV_VN || t === CV) {
        const nx = skipCVN(data, o, t);
        if (nx < 0) return;
        o = nx;
        continue;
      }
      let end;
      if (
        (t >= 32 && t <= 40) ||
        (t >= 42 && t <= 50) ||
        (t >= 52 && t <= 55)
      ) {
        end = o + 4 + sizew * 4;
      } else end = o + 4 + sizew * 2;
      if (end > data.length || end < o) return;
      o = end;
    }
  }
  const DRAW_MODE_DEFAULT = "easy";
  function colorCalcEngine(sel) {
    if (sel === 2) return 4;
    return sel;
  }
  function materialBlendIndices(matAttr) {
    const a = matAttr || 0;
    return { srcIndex: (a >> 3) & 7, dstIndex: a & 7 };
  }
  function resolveFaceState(attr, mat, vtype, drawMode) {
    const a = attr || 0;
    const dm = drawMode || DRAW_MODE_DEFAULT;
    const colorSel = a & 0x07;
    const matAttr = mat && mat.attr != null ? mat.attr : 0;
    const bi = materialBlendIndices(matAttr);
    return {
      attr: a,
      blend: !!(a & 0x08),
      flat: !!(a & 0x20),
      colorSel: colorSel,
      colorCalc: colorCalcEngine(colorSel),
      side: "double",
      diffuse: mat && mat.diffuse ? mat.diffuse : null,
      ambient: mat && mat.ambient ? mat.ambient : null,
      alpha: mat && mat.alpha != null ? mat.alpha : 1,
      matAttr: matAttr,
      srcIndex: bi.srcIndex,
      dstIndex: bi.dstIndex,
      vtype: vtype == null ? -1 : vtype,
      drawMode: dm,
    };
  }
  function faceState(model, fi) {
    let attr;
    if (model.faceAttr && model.faceAttr[fi] != null) attr = model.faceAttr[fi];
    else {
      attr = 0x02;
      if (model.faceBlend && model.faceBlend[fi]) attr |= 0x08;
      if (model.faceFlat && model.faceFlat[fi]) attr |= 0x20;
    }
    const mi = model.mats ? model.mats[fi] : -1;
    const mat =
      mi >= 0 && model.materials && model.materials[mi]
        ? model.materials[mi]
        : null;
    return resolveFaceState(attr, mat, model.vtype, DRAW_MODE_DEFAULT);
  }
  g.RDXNJ = {
    readModel: readModel,
    resolveFaceState: resolveFaceState,
    faceState: faceState,
    materialBlendIndices: materialBlendIndices,
    drawModeDefault: DRAW_MODE_DEFAULT,
  };
})(window);
