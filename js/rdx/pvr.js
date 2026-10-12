(function (g) {
  "use strict";
  function dv(d) {
    return new DataView(d.buffer, d.byteOffset, d.byteLength);
  }
  function morton(x, y) {
    let o = 0,
      s = 0;
    while (x || y) {
      o |= ((y & 1) << s) | ((x & 1) << (s + 1));
      x >>= 1;
      y >>= 1;
      s += 2;
    }
    return o;
  }
  function d1555(x) {
    return [
      ((((x >> 10) & 31) * 255) / 31) | 0,
      ((((x >> 5) & 31) * 255) / 31) | 0,
      (((x & 31) * 255) / 31) | 0,
      x & 0x8000 ? 255 : 0,
    ];
  }
  function d565(x) {
    return [
      ((((x >> 11) & 31) * 255) / 31) | 0,
      ((((x >> 5) & 63) * 255) / 63) | 0,
      (((x & 31) * 255) / 31) | 0,
      255,
    ];
  }
  function d4444(x) {
    return [
      ((x >> 8) & 15) * 17,
      ((x >> 4) & 15) * 17,
      (x & 15) * 17,
      ((x >> 12) & 15) * 17,
    ];
  }
  const DEC = { 0: d1555, 1: d565, 2: d4444 };
  function decWords(words, fmt) {
    const f = DEC[fmt] || d4444,
      out = new Uint8Array(words.length * 4);
    for (let i = 0; i < words.length; i++) {
      const r = f(words[i]);
      out[i * 4] = r[0];
      out[i * 4 + 1] = r[1];
      out[i * 4 + 2] = r[2];
      out[i * 4 + 3] = r[3];
    }
    return out;
  }
  function detw16(src, w, h) {
    const v = dv(src),
      n = w * h,
      words = new Array(n);
    for (let i = 0; i < n; i++) words[i] = v.getUint16(i * 2, true);
    const dst = new Array(n);
    for (let y = 0; y < h; y++) {
      for (let x = 0; x < w; x++) dst[y * w + x] = words[morton(x, y)];
    }
    return dst;
  }
  function detw8(src, w, h) {
    const dst = new Array(w * h);
    for (let y = 0; y < h; y++) {
      for (let x = 0; x < w; x++) dst[y * w + x] = src[morton(x, y)];
    }
    return dst;
  }
  function detw4(src, w, h) {
    const dst = new Array(w * h);
    for (let y = 0; y < h; y++) {
      for (let x = 0; x < w; x++) {
        const mi = morton(x, y),
          b = src[mi >> 1];
        dst[y * w + x] = mi % 2 ? (b >> 4) & 15 : b & 15;
      }
    }
    return dst;
  }
  function packNib(idxs) {
    const o = new Uint8Array((idxs.length + 1) >> 1);
    for (let i = 0; i < idxs.length; i++) {
      if (i % 2 === 0) o[i >> 1] = idxs[i] & 15;
      else o[i >> 1] |= (idxs[i] & 15) << 4;
    }
    return o;
  }
  function applyPal(idxs, pal, fmt, count) {
    const f = DEC[fmt] || d4444,
      cols = pal
        ? pal.map(function (e) {
            return e[0];
          })
        : null,
      out = new Uint8Array(count * 4);
    for (let i = 0; i < count; i++) {
      const ci = i < idxs.length ? idxs[i] : 0;
      if (cols && ci < cols.length) {
        const c = cols[ci];
        out[i * 4] = c & 255;
        out[i * 4 + 1] = (c >> 8) & 255;
        out[i * 4 + 2] = (c >> 16) & 255;
        out[i * 4 + 3] = (c >>> 24) & 255;
      } else {
        const r = f((ci * 17) & 65535);
        out[i * 4] = r[0];
        out[i * 4 + 1] = r[1];
        out[i * 4 + 2] = r[2];
        out[i * 4 + 3] = r[3];
      }
    }
    return out;
  }
  function decodeVQ(px, w, h, fmt) {
    const v = dv(px),
      ni = (w >> 1) * (h >> 1),
      cbSize = px.length - ni;
    const idx = detw8(px.subarray(cbSize), w >> 1, h >> 1),
      ne = cbSize / 8,
      words = new Array(w * h);
    for (let by = 0; by < h >> 1; by++) {
      for (let bx = 0; bx < w >> 1; bx++) {
        const e = idx[by * (w >> 1) + bx] % ne,
          b = e * 8;
        const t0 = v.getUint16(b, true),
          t1 = v.getUint16(b + 2, true),
          t2 = v.getUint16(b + 4, true),
          t3 = v.getUint16(b + 6, true);
        const dx = bx * 2,
          r0 = by * 2 * w + dx,
          r1 = (by * 2 + 1) * w + dx;
        words[r0] = t0;
        words[r0 + 1] = t2;
        words[r1] = t1;
        words[r1 + 1] = t3;
      }
    }
    return decWords(words, fmt);
  }
  function readPic(data, off, pal) {
    const v = dv(data),
      start = off;
    if (
      data[off] === 0x47 &&
      data[off + 1] === 0x42 &&
      data[off + 2] === 0x49 &&
      data[off + 3] === 0x58
    ) {
      off += 16;
    }
    if (
      !(
        data[off] === 0x50 &&
        data[off + 1] === 0x56 &&
        data[off + 2] === 0x52 &&
        data[off + 3] === 0x54
      )
    ) {
      throw new Error("not PVRT");
    }
    const size = v.getUint32(off + 4, true),
      fmt = data[off + 8],
      dfmt = data[off + 9];
    const w = v.getUint16(off + 12, true),
      h = v.getUint16(off + 14, true);
    const px = data.subarray(off + 16, off + 8 + size),
      end = off + 8 + size,
      pic = {
        width: w,
        height: h,
        pixfmt: fmt,
        datafmt: dfmt,
        pixels: new Uint8Array(0),
        imageType: 1,
        picFormat: 0,
        totalSize: end - start,
      };
    if (dfmt === 0x01) pic.pixels = decWords(detw16(px, w, h), fmt);
    else if (dfmt === 0x0d) {
      const side = Math.min(w, h),
        tiles = Math.max(w, h) / side,
        dst = new Array(w * h);
      for (let t = 0; t < tiles; t++) {
        const ws = new Uint16Array(
          px.buffer,
          px.byteOffset + t * side * side * 2,
          side * side,
        );
        for (let yy = 0; yy < side; yy++) {
          for (let xx = 0; xx < side; xx++) {
            const dx2 = w >= h ? t * side + xx : xx,
              dy2 = w >= h ? yy : t * side + yy;
            dst[dy2 * w + dx2] = ws[morton(xx, yy)];
          }
        }
      }
      pic.pixels = decWords(dst, fmt);
    } else if (dfmt === 0x02) {
      const base = px.subarray(px.length - w * h * 2);
      pic.pixels = decWords(detw16(base, w, h), fmt);
    } else if (dfmt === 0x09) {
      const n = w * h,
        ww = new Array(n);
      for (let i = 0; i < n; i++) ww[i] = v.getUint16(off + 16 + i * 2, true);
      pic.pixels = decWords(ww, fmt);
    } else if (dfmt === 0x03 || dfmt === 0x10) {
      pic.pixels = decodeVQ(px, w, h, fmt);
    } else if (dfmt === 0x05) {
      const ix = detw4(px, w, h);
      pic.imageData = packNib(ix);
      pic.imageType = 4;
      pic.pixels = applyPal(ix, pal, fmt, w * h);
    } else if (dfmt === 0x07) {
      const ix2 = Array.prototype.slice.call(detw8(px, w, h));
      pic.imageData = new Uint8Array(ix2);
      pic.imageType = 5;
      pic.pixels = applyPal(ix2, pal, fmt, w * h);
    } else throw new Error("unsupported PVR fmt " + dfmt);
    return { pic: pic, end: end };
  }
  function decodePvr(data, off, pal) {
    const r = readPic(data, off || 0, pal);
    return { pictures: [r.pic] };
  }
  function parsePvpl(data, off, fmt) {
    fmt = fmt == null ? 2 : fmt;
    if (
      !(
        data[off] === 0x50 &&
        data[off + 1] === 0x56 &&
        data[off + 2] === 0x50 &&
        data[off + 3] === 0x4c
      )
    ) {
      return [0, []];
    }
    const v = dv(data);
    try {
      const size = v.getUint32(off + 4, true),
        nf = v.getUint16(off + 14, true),
        bytes = data.subarray(off + 16, off + 8 + size);
      let n = bytes.length >> 1;
      if (nf >= 1 && nf <= 256) n = Math.min(n, nf);
      if (n <= 0 || n > 256) return [0, []];
      const f = DEC[fmt] || d4444,
        ents = [];
      for (let i = 0; i < n; i++) {
        const w2 = v.getUint16(off + 16 + i * 2, true),
          r = f(w2);
        ents.push([
          (r[0] | (r[1] << 8) | (r[2] << 16) | (r[3] << 24)) >>> 0,
          0,
          0,
          0,
        ]);
      }
      return [n, ents];
    } catch {
      return [0, []];
    }
  }
  g.RDXPVR = { decodePvr: decodePvr, parsePvpl: parsePvpl };
})(window);
