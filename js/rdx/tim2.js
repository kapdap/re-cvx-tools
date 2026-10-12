(function (g) {
  "use strict";
  function dv(d) {
    return new DataView(d.buffer, d.byteOffset, d.byteLength);
  }
  const CT = { UNDEF: 0, RGBA16: 1, RGB32: 2, RGBA32: 3, IDX4: 4, IDX8: 5 };
  function isVqPath(f) {
    return !(f >= 5 && f < 9);
  }
  function classifyAlpha(px) {
    if (!px || !px.length) return "OPAQUE";
    let z = false,
      f = false;
    for (let i = 3; i < px.length; i += 4) {
      const a = px[i];
      if (a === 0) z = true;
      else if (a === 255) f = true;
      else return "BLEND";
    }
    return z && f ? "CUTOUT" : "OPAQUE";
  }
  function fixPalettePS2(pal, clutType, imgType) {
    if (!pal) return pal;
    if (imgType !== CT.IDX4 && imgType !== CT.IDX8) return pal;
    if (clutType & 0x80) return pal;
    const entry = { 1: 2, 2: 3, 3: 4 }[clutType & 0x3f];
    if (!entry || pal.length % entry !== 0) return pal;
    const grp = 32 * entry;
    if (pal.length < grp) return pal;
    const out = new Uint8Array(pal);
    const q = grp / 4;
    for (let b = 0; b + q * 3 <= pal.length; b += grp) {
      const t1 = pal.slice(b + q, b + 2 * q),
        t2 = pal.slice(b + 2 * q, b + 3 * q);
      out.set(t2, b + q);
      out.set(t1, b + 2 * q);
    }
    return out;
  }
  function decodeImage(imgType, imgData, clutData, clutType, w, h, texa) {
    const n = w * h,
      out = new Uint8Array(n * 4),
      clut = clutData ? fixPalettePS2(clutData, clutType, imgType) : null;
    let i, s, val2, r2, gg, b2, a2;
    if (imgType === CT.RGBA32) {
      out.set(imgData.subarray(0, n * 4));
    } else if (imgType === CT.RGB32) {
      for (i = 0; i < n; i++) {
        s = i * 4;
        out[s] = imgData[s];
        out[s + 1] = imgData[s + 1];
        out[s + 2] = imgData[s + 2];
        out[s + 3] = 255;
      }
    } else if (imgType === CT.RGBA16) {
      const aem = (texa >> 15) & 1,
        ta0 = texa & 0xff,
        ta1 = (texa >> 16) & 0xff;
      for (i = 0; i < n; i++) {
        s = i * 2;
        val2 = imgData[s] | (imgData[s + 1] << 8);
        r2 = (((val2 & 0x1f) * 255) / 31) | 0;
        gg = ((((val2 >> 5) & 0x1f) * 255) / 31) | 0;
        b2 = ((((val2 >> 10) & 0x1f) * 255) / 31) | 0;
        if (val2 & 0x8000) a2 = !aem ? 255 : Math.min(255, ta1 * 2);
        else a2 = !aem ? 0 : Math.min(255, ta0 * 2);
        out[i * 4] = r2;
        out[i * 4 + 1] = gg;
        out[i * 4 + 2] = b2;
        out[i * 4 + 3] = a2;
      }
    } else if (imgType === CT.IDX8) {
      for (i = 0; i < n && i < imgData.length; i++) {
        const ci = imgData[i] * 4,
          d = i * 4;
        if (ci + 3 < clut.length) {
          out[d] = clut[ci];
          out[d + 1] = clut[ci + 1];
          out[d + 2] = clut[ci + 2];
          out[d + 3] = clut[ci + 3];
        }
      }
    } else if (imgType === CT.IDX4) {
      for (i = 0; i < n; i++) {
        const bi = i >> 1;
        if (bi >= imgData.length) break;
        const sh = i % 2 ? 4 : 0,
          cj = ((imgData[bi] >> sh) & 0x0f) * 4,
          d2 = i * 4;
        if (cj + 3 < clut.length) {
          out[d2] = clut[cj];
          out[d2 + 1] = clut[cj + 1];
          out[d2 + 2] = clut[cj + 2];
          out[d2 + 3] = clut[cj + 3];
        }
      }
    }
    return out;
  }
  function normalizeAlpha(px, imgType, orgColor, pictFmt) {
    if ((imgType !== CT.IDX4 && imgType !== CT.IDX8) || !px || !px.length) {
      return px;
    }
    if (orgColor !== 0 && orgColor !== 1 && orgColor !== 2 && orgColor !== 6) {
      return px;
    }
    const out = new Uint8Array(px),
      vq = isVqPath(pictFmt == null ? -1 : pictFmt);
    let i;
    function a000() {
      for (i = 0; i < out.length; i += 4) {
        if (
          out[i + 3] !== 0 &&
          out[i] === 0 &&
          out[i + 1] === 0 &&
          out[i + 2] === 0
        ) {
          out[i + 3] = 0;
        }
      }
    }
    function a48() {
      for (i = 3; i < out.length; i += 4) {
        const x = out[i];
        out[i] = x | (x >> 4);
      }
    }
    function half() {
      for (i = 3; i < out.length; i += 4) out[i] = (out[i] + 1) >> 1;
    }
    if (vq) {
      if (orgColor === 2) {
        a48();
        half();
      } else if (orgColor === 6) half();
    } else {
      if (orgColor === 0 || orgColor === 1) a000();
      else if (orgColor === 2) {
        a000();
        a48();
        half();
      } else if (orgColor === 6) {
        a000();
        half();
      }
    }
    for (i = 3; i < out.length; i += 4) {
      const dd = out[i] * 2;
      out[i] = dd > 255 ? 255 : dd;
    }
    return out;
  }
  function decodeTim2(data, offset) {
    offset = offset || 0;
    const v = dv(data);
    if (
      data[offset] !== 0x54 ||
      data[offset + 1] !== 0x49 ||
      data[offset + 2] !== 0x4d ||
      data[offset + 3] !== 0x32
    ) {
      throw new Error("not TIM2");
    }
    const fmt = data[offset + 5],
      npics = v.getInt16(offset + 6, true);
    const orgC = data[offset + 12];
    let o = offset + 16;
    if (fmt === 1) o += 112;
    const doc = { pictures: [], orgColor: orgC };
    for (let k = 0; k < npics; k++) {
      const start = o;
      const total = v.getInt32(o, true),
        clutS = v.getInt32(o + 4, true),
        imgS = v.getInt32(o + 8, true),
        hdrS = v.getUint16(o + 12, true);
      const picFmt = data[o + 16],
        mips = data[o + 17],
        clutT = data[o + 18],
        imgT = data[o + 19];
      const w = v.getUint16(o + 20, true),
        h = v.getUint16(o + 22, true);
      o += 24;
      o += 24;
      let texa = 0;
      if (start + 44 <= data.length) texa = v.getUint32(start + 40, true);
      if (mips > 1 && o - start < hdrS) o += 48;
      if (o - start < hdrS) o += hdrS - (o - start);
      const imgData = data.subarray(o, o + imgS);
      o += imgS;
      const clutData = clutS > 0 ? data.subarray(o, o + clutS) : null;
      o += clutS;
      o = start + total;
      let px =
        w > 0 && h > 0
          ? decodeImage(imgT, imgData, clutData, clutT, w, h, texa)
          : new Uint8Array(0);
      px = normalizeAlpha(px, imgT, orgC, picFmt);
      doc.pictures.push({
        width: w,
        height: h,
        imageType: imgT,
        picFormat: picFmt,
        pixels: px,
        imageData: imgData.subarray(0),
        texa: texa,
      });
    }
    return doc;
  }
  function parsePli(data, off) {
    const v = dv(data);
    let pal = 0;
    try {
      pal = v.getInt32(off, true);
    } catch {
      return [0, []];
    }
    if (pal <= 0 || pal > 64) return [0, []];
    const ents = [];
    for (let i = 0; i < pal; i++) {
      try {
        const c = v.getUint32(off + 4 + i * 8 + 4, true);
        ents.push([c, c & 255, (c >> 8) & 255, (c >> 16) & 65535]);
      } catch {
        return [0, []];
      }
    }
    return [pal, ents];
  }
  function applyPalSub(imgType, imgData, px, palColors) {
    if (
      !palColors ||
      !palColors.length ||
      !px ||
      !px.length ||
      !imgData ||
      !imgData.length
    ) {
      return px;
    }
    const out = new Uint8Array(px);
    let i, d, c;
    if (imgType === CT.IDX8) {
      const n = imgData.length;
      for (i = 0; i < n; i++) {
        const ci = imgData[i];
        if (ci < palColors.length) {
          d = i * 4;
          if (d + 4 > out.length) break;
          c = palColors[ci];
          out[d] = c & 255;
          out[d + 1] = (c >> 8) & 255;
          out[d + 2] = (c >> 16) & 255;
          out[d + 3] = (c >>> 24) & 255;
        }
      }
      return out;
    }
    if (imgType === CT.IDX4) {
      const m = imgData.length * 2;
      for (i = 0; i < m; i++) {
        const bi = i >> 1;
        if (bi >= imgData.length) break;
        const b = imgData[bi],
          c2 = i % 2 ? (b >> 4) & 15 : b & 15;
        if (c2 < palColors.length) {
          d = i * 4;
          if (d + 4 > out.length) break;
          c = palColors[c2];
          out[d] = c & 255;
          out[d + 1] = (c >> 8) & 255;
          out[d + 2] = (c >> 16) & 255;
          out[d + 3] = (c >>> 24) & 255;
        }
      }
      return out;
    }
    return px;
  }
  g.RDXTIM2 = {
    decodeTim2: decodeTim2,
    parsePli: parsePli,
    applyPalSub: applyPalSub,
    classifyAlpha: classifyAlpha,
    isVqPath: isVqPath,
  };
})(window);
