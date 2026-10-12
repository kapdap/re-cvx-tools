(function (g) {
  "use strict";
  const SIZE_OBJ = 52;
  function dv(d) {
    return new DataView(d.buffer, d.byteOffset, d.byteLength);
  }
  function readMlb(data, offset, size) {
    const v = dv(data);
    const mlb = {
      offset: offset,
      size: size,
      magic: "",
      isChunk: false,
      objects: [],
      models: [],
      texNames: [],
      notes: [],
      datTop: 0,
      objBase: 0,
    };
    if (size < 16 || offset + 16 > data.length) {
      throw new Error("MLB too small");
    }
    mlb.magic = String.fromCharCode(
      data[offset],
      data[offset + 1],
      data[offset + 2],
      data[offset + 3],
    );
    const status = data[offset + 3];
    mlb.isChunk = !!(status & 0x80);
    const dtopOff = v.getUint16(offset + 4, true),
      objNum = v.getUint16(offset + 6, true);
    const texOff = v.getInt32(offset + 8, true),
      objOff = v.getInt32(offset + 12, true);
    const datTop = offset + dtopOff;
    mlb.datTop = datTop;
    mlb.objBase = objOff !== -1 ? datTop + objOff : 0;
    mlb.texNames = readTexList(data, datTop, texOff);
    if (!mlb.isChunk) {
      mlb.notes.push("basic model");
      return mlb;
    }
    if (objOff === -1 || objNum <= 0 || objNum > 100000) return mlb;
    const moffs = [];
    for (let i = 0; i < objNum; i++) {
      const oo = datTop + objOff + i * SIZE_OBJ;
      if (oo < 0 || oo + SIZE_OBJ > data.length) {
        mlb.notes.push("object overrun");
        break;
      }
      const o = readObj(data, oo, i);
      moffs.push(o._mo);
      delete o._mo;
      mlb.objects.push(o);
    }
    const uniq = {},
      inv = {};
    for (let k = 0; k < mlb.objects.length; k++) {
      const mo = moffs[k];
      if (mo === -1) continue;
      const mno = datTop + mo;
      if (mno < 0 || mno + 8 > data.length) {
        mlb.notes.push("model offset OOR");
        continue;
      }
      if (!(mno in uniq)) {
        uniq[mno] = mlb.models.length;
        mlb.models.push(null);
      }
      mlb.objects[k].modelIndex = uniq[mno];
    }
    for (const key in uniq) inv[uniq[key]] = +key;
    for (let j = 0; j < mlb.models.length; j++) {
      try {
        mlb.models[j] = g.RDXNJ.readModel(data, datTop, inv[j]);
      } catch {
        mlb.models[j] = null;
        mlb.notes.push("model decode failed");
      }
    }
    return mlb;
  }
  function readObj(data, oo, idx) {
    const v = dv(data);
    const o = {
      index: idx,
      evalflags: v.getInt32(oo, true),
      pos: [
        v.getFloat32(oo + 8, true),
        v.getFloat32(oo + 12, true),
        v.getFloat32(oo + 16, true),
      ],
      ang: [
        v.getInt32(oo + 20, true),
        v.getInt32(oo + 24, true),
        v.getInt32(oo + 28, true),
      ],
      scl: [
        v.getFloat32(oo + 32, true),
        v.getFloat32(oo + 36, true),
        v.getFloat32(oo + 40, true),
      ],
      child: v.getInt32(oo + 44, true),
      sibling: v.getInt32(oo + 48, true),
      modelIndex: -1,
      hidden: false,
      _mo: v.getInt32(oo + 4, true),
    };
    o.hidden = !!(o.evalflags & 0x8);
    return o;
  }
  function readTexList(data, datTop, texOff) {
    const names = [];
    if (texOff === -1) return names;
    const v = dv(data),
      o = datTop + texOff;
    if (o < 0 || o + 8 > data.length) return names;
    const texPtr = v.getInt32(o, true),
      count = v.getInt32(o + 4, true);
    if (texPtr === -1) return names;
    if (count < 0 || count > 4096) return names;
    const arr = datTop + texPtr;
    for (let i = 0; i < count; i++) {
      const eo = arr + i * 12;
      const entry = {
        index: i,
        name: "",
        nameValid: false,
        attribute: 0,
        textureAddress: 0,
      };
      if (eo < 0 || eo + 12 > data.length) {
        names.push(entry);
        continue;
      }
      const fn = v.getInt32(eo, true);
      entry.attribute = v.getInt32(eo + 4, true);
      entry.textureAddress = v.getUint32(eo + 8, true);
      if (fn < 0) {
        names.push(entry);
        continue;
      }
      let q = datTop + fn;
      if (q < 0 || q >= data.length) {
        names.push(entry);
        continue;
      }
      let s = "";
      while (q < data.length && s.length < 256) {
        if (data[q] === 0) break;
        s += String.fromCharCode(data[q++]);
      }
      if (q < data.length && data[q] === 0) {
        entry.name = s;
        entry.nameValid = true;
      }
      names.push(entry);
    }
    return names;
  }
  function iterMlbBlobs(data, base, end) {
    const out = [],
      v = dv(data);
    if (base + 4 > end || base < 0 || end > data.length) return out;
    if (
      end - base >= 3 &&
      data[base] === 0x4d &&
      data[base + 1] === 0x44 &&
      data[base + 2] === 0x4c
    ) {
      out.push([base, end - base]);
      return out;
    }
    let o = base;
    while (o + 4 <= end) {
      const size = v.getInt32(o, true);
      if (size === -1) break;
      if (size === 0) {
        o += 4;
        continue;
      }
      if (size < 0 || o + 4 + size > end) break;
      const blob = o + 4;
      if (blob + 4 > end) break;
      const isMask =
        data[blob] === 0x4d &&
        data[blob + 1] === 0x41 &&
        data[blob + 2] === 0x53 &&
        data[blob + 3] === 0x4b;
      const isSkin =
        data[blob] === 0x53 &&
        data[blob + 1] === 0x4b &&
        data[blob + 2] === 0x49 &&
        data[blob + 3] === 0x4e;
      if (!isMask && !isSkin) out.push([blob, size]);
      o += 4 + size;
    }
    if (
      o + 3 <= end &&
      data[o] === 0x4d &&
      data[o + 1] === 0x44 &&
      data[o + 2] === 0x4c
    ) {
      out.push([o, end - o]);
    }
    return out;
  }
  function objectIndexOf(mlb, offset) {
    if (offset === -1) return -1;
    const rel = mlb.objBase - mlb.datTop,
      d = offset - rel;
    if (d % SIZE_OBJ !== 0) return -1;
    const idx = d / SIZE_OBJ;
    return idx < 0 || idx >= mlb.objects.length ? -1 : idx;
  }
  g.RDXMLB = {
    readMlb: readMlb,
    iterMlbBlobs: iterMlbBlobs,
    objectIndexOf: objectIndexOf,
  };
})(window);
