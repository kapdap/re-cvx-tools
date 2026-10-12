(function (g) {
  "use strict";
  const CANDIDATES = [
    { id: "png", label: "PNG (.png)", mime: "image/png", ext: "png" },
    {
      id: "jpeg",
      label: "JPEG (.jpg)",
      mime: "image/jpeg",
      ext: "jpg",
      quality: 0.92,
    },
    {
      id: "webp",
      label: "WebP (.webp)",
      mime: "image/webp",
      ext: "webp",
      quality: 0.92,
    },
    { id: "bmp", label: "BMP (.bmp)", mime: "image/bmp", ext: "bmp" },
    {
      id: "avif",
      label: "AVIF (.avif)",
      mime: "image/avif",
      ext: "avif",
      quality: 0.92,
    },
  ];

  function canvasSupports(mime) {
    try {
      const c = document.createElement("canvas");
      c.width = 1;
      c.height = 1;
      const url = c.toDataURL(mime);
      return typeof url === "string" && url.indexOf("data:" + mime) === 0;
    } catch {
      return false;
    }
  }

  function getSupportedFormats() {
    const out = [];
    CANDIDATES.forEach(function (f) {
      if (f.id === "png") {
        try {
          const c = document.createElement("canvas");
          c.width = 1;
          c.height = 1;
          c.toDataURL("image/png");
        } catch {
          /* ignore */
        }
        out.push(f);
        return;
      }
      if (canvasSupports(f.mime)) out.push(f);
    });
    return out;
  }

  function texName(ti, texid, variant) {
    let n = "TEX_" + ti + "_" + texid;
    if (variant != null) n += " pal" + variant;
    return n;
  }

  function baseName() {
    try {
      if (g.AppExport && g.AppExport.baseName) {
        return g.AppExport.baseName(g.AppState.active);
      }
    } catch {
      /* ignore */
    }
    return "models";
  }

  function listTextureEntries(Rr) {
    const out = [];
    if (!Rr || !Rr.textures || !Rr.textures.length) return out;
    const nt = Rr.textures.length;
    for (let ti = 0; ti < nt; ti++) {
      let pics = [];
      try {
        pics = g.RDX.tablePics(Rr, ti) || [];
      } catch {
        continue;
      }
      for (let texid = 0; texid < pics.length; texid++) {
        let canvas = null;
        try {
          const t = g.RDXScene.getTexture(Rr, ti, texid);
          if (t && t.tex && t.tex.image) canvas = t.tex.image;
        } catch {
          canvas = null;
        }
        if (!canvas || !canvas.width || !canvas.height) continue;
        out.push({
          ti: ti,
          texid: texid,
          variant: pics[texid] ? pics[texid].variant : null,
          canvas: canvas,
        });
      }
    }
    return out;
  }

  function b64ToBytes(b64) {
    try {
      if (typeof atob !== "undefined") {
        const s = atob(b64),
          o = new Uint8Array(s.length);
        let i;
        for (i = 0; i < s.length; i++) o[i] = s.charCodeAt(i) & 255;
        return o;
      }
    } catch {
      /* ignore */
    }
    try {
      if (typeof globalThis.Buffer !== "undefined") {
        const b = globalThis.Buffer.from(b64, "base64");
        return new Uint8Array(b.buffer, b.byteOffset, b.byteLength);
      }
    } catch {
      /* ignore */
    }
    return null;
  }

  function canvasToBytes(canvas, mime, quality, cb) {
    function viaDataUrl() {
      try {
        const url =
          quality != null
            ? canvas.toDataURL(mime, quality)
            : canvas.toDataURL(mime);
        const comma = url.indexOf(",");
        if (comma < 0) {
          cb(null);
          return;
        }
        cb(b64ToBytes(url.slice(comma + 1)));
      } catch {
        cb(null);
      }
    }
    try {
      if (canvas.toBlob) {
        const args = quality != null ? [mime, quality] : [mime];
        canvas.toBlob(
          function (blob) {
            if (!blob) {
              viaDataUrl();
              return;
            }
            try {
              if (blob.arrayBuffer) {
                blob.arrayBuffer().then(
                  function (ab) {
                    cb(new Uint8Array(ab));
                  },
                  function () {
                    viaDataUrl();
                  },
                );
                return;
              }
            } catch {
              /* ignore */
            }
            try {
              const fr = new FileReader();
              fr.onload = function () {
                try {
                  cb(new Uint8Array(fr.result));
                } catch {
                  viaDataUrl();
                }
              };
              fr.onerror = function () {
                viaDataUrl();
              };
              fr.readAsArrayBuffer(blob);
            } catch {
              viaDataUrl();
            }
          },
          args[0],
          args[1],
        );
        return;
      }
    } catch {
      /* ignore */
    }
    viaDataUrl();
  }

  const CRC_TABLE = (function () {
    const t = new Array(256);
    let n, k, c;
    for (n = 0; n < 256; n++) {
      c = n;
      for (k = 0; k < 8; k++) c = c & 1 ? 0xedb88320 ^ (c >>> 1) : c >>> 1;
      t[n] = c >>> 0;
    }
    return t;
  })();
  function crc32Bytes(data) {
    let crc = 0xffffffff,
      i;
    for (i = 0; i < data.length; i++) {
      crc = CRC_TABLE[(crc ^ data[i]) & 0xff] ^ (crc >>> 8);
    }
    return (crc ^ 0xffffffff) >>> 0;
  }
  function strToUtf8(s) {
    try {
      if (typeof TextEncoder !== "undefined") {
        return new TextEncoder().encode(s);
      }
    } catch {
      /* ignore */
    }
    const o = new Uint8Array(s.length);
    let i;
    for (i = 0; i < s.length; i++) o[i] = s.charCodeAt(i) & 255;
    return o;
  }
  function dosTimeDate(d) {
    try {
      d = d || new Date();
      const t =
        ((d.getHours() & 31) << 11) |
        ((d.getMinutes() & 63) << 5) |
        ((Math.floor(d.getSeconds() / 2) & 31) << 0);
      const dt =
        (((d.getFullYear() - 1980) & 127) << 9) |
        (((d.getMonth() + 1) & 15) << 5) |
        ((d.getDate() & 31) << 0);
      return { time: t, date: dt };
    } catch {
      return { time: 0, date: 0 };
    }
  }
  function buildZipStore(files) {
    let i;
    const now = dosTimeDate(new Date());
    const names = [];
    for (i = 0; i < files.length; i++) names.push(strToUtf8(files[i].name));
    let localSize = 0,
      centralSize = 0;
    for (i = 0; i < files.length; i++) {
      localSize += 30 + names[i].length + files[i].data.length;
    }
    for (i = 0; i < files.length; i++) centralSize += 46 + names[i].length;
    const total = localSize + centralSize + 22;
    const out = new Uint8Array(total);
    const dv = new DataView(out.buffer, out.byteOffset, out.byteLength);
    let off = 0;
    const centralOff = localSize;
    let centralBytes = 0;
    const localOffsets = [];
    for (i = 0; i < files.length; i++) {
      const data = files[i].data,
        nb = names[i],
        crc = crc32Bytes(data);
      localOffsets.push(off);
      dv.setUint32(off, 0x04034b50, true);
      dv.setUint16(off + 4, 20, true);
      dv.setUint16(off + 6, 0x0800, true);
      dv.setUint16(off + 8, 0, true);
      dv.setUint16(off + 10, now.time, true);
      dv.setUint16(off + 12, now.date, true);
      dv.setUint32(off + 14, crc, true);
      dv.setUint32(off + 18, data.length, true);
      dv.setUint32(off + 22, data.length, true);
      dv.setUint16(off + 26, nb.length, true);
      dv.setUint16(off + 28, 0, true);
      off += 30;
      out.set(nb, off);
      off += nb.length;
      out.set(data, off);
      off += data.length;
      const co = centralOff + centralBytes;
      dv.setUint32(co, 0x02014b50, true);
      dv.setUint16(co + 4, 20, true);
      dv.setUint16(co + 6, 20, true);
      dv.setUint16(co + 8, 0x0800, true);
      dv.setUint16(co + 10, 0, true);
      dv.setUint16(co + 12, now.time, true);
      dv.setUint16(co + 14, now.date, true);
      dv.setUint32(co + 16, crc, true);
      dv.setUint32(co + 20, data.length, true);
      dv.setUint32(co + 24, data.length, true);
      dv.setUint16(co + 28, nb.length, true);
      dv.setUint16(co + 30, 0, true);
      dv.setUint16(co + 32, 0, true);
      dv.setUint16(co + 34, 0, true);
      dv.setUint16(co + 36, 0, true);
      dv.setUint32(co + 38, 0, true);
      dv.setUint32(co + 42, localOffsets[i], true);
      out.set(nb, co + 46);
      centralBytes += 46 + nb.length;
    }
    const eo = localSize + centralSize;
    dv.setUint32(eo, 0x06054b50, true);
    dv.setUint16(eo + 4, 0, true);
    dv.setUint16(eo + 6, 0, true);
    dv.setUint16(eo + 8, files.length, true);
    dv.setUint16(eo + 10, files.length, true);
    dv.setUint32(eo + 12, centralSize, true);
    dv.setUint32(eo + 16, localSize, true);
    dv.setUint16(eo + 20, 0, true);
    return out;
  }

  function downloadBlob(blob, filename) {
    try {
      if (typeof document === "undefined") return false;
      const objUrl = (g.URL || window.URL).createObjectURL(blob);
      const a = document.createElement("a");
      a.href = objUrl;
      a.download = filename;
      if (document.body) document.body.appendChild(a);
      a.click();
      setTimeout(function () {
        try {
          (g.URL || window.URL).revokeObjectURL(objUrl);
          if (a.remove) a.remove();
        } catch {
          /* ignore */
        }
      }, 1000);
      return true;
    } catch {
      return false;
    }
  }

  function exportTextures(formatId) {
    const S = g.AppState;
    const Rr = S.R();
    if (!Rr) {
      return;
    }
    const formats = getSupportedFormats();
    let fmt = null;
    formats.forEach(function (f) {
      if (f.id === formatId) fmt = f;
    });
    if (!fmt) {
      return;
    }
    const entries = listTextureEntries(Rr);
    if (!entries.length) {
      return;
    }
    const base = baseName();
    const files = [];
    let i = 0;
    function next() {
      if (i >= entries.length) {
        if (!files.length) {
          return;
        }
        let zipBytes;
        try {
          zipBytes = buildZipStore(files);
        } catch {
          return;
        }
        const zipName = base + "_textures_" + fmt.ext + ".zip";
        let blob;
        try {
          blob = new Blob([zipBytes], { type: "application/zip" });
        } catch {
          return;
        }
        downloadBlob(blob, zipName);
        return;
      }
      const e = entries[i++];
      let alias = null;
      try {
        if (g.RDXScene && g.RDXScene.texNameFor) {
          alias = g.RDXScene.texNameFor(Rr, e.ti, e.texid);
        }
      } catch {
        /* ignore */
      }
      const fname =
        base +
        "_" +
        (alias || texName(e.ti, e.texid, e.variant)) +
        (alias && e.variant != null ? " pal" + e.variant : "") +
        "." +
        fmt.ext;
      try {
        canvasToBytes(e.canvas, fmt.mime, fmt.quality, function (bytes) {
          if (bytes && bytes.length) files.push({ name: fname, data: bytes });
          setTimeout(next, 0);
        });
      } catch {
        setTimeout(next, 0);
      }
    }
    next();
  }

  g.AppTextureExport = {
    getSupportedFormats: getSupportedFormats,
    exportTextures: exportTextures,
  };
})(window);
