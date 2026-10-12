(function (g) {
  "use strict";
  function pad3(ti) {
    return ("00" + ti).slice(-3);
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
  function canvasPngBytes(canvas) {
    try {
      if (!canvas || !canvas.toDataURL) return null;
      const url = canvas.toDataURL("image/png");
      const i = url.indexOf(",");
      if (i < 0) return null;
      return b64ToBytes(url.slice(i + 1));
    } catch {
      return null;
    }
  }
  function xformPos(w, p) {
    return [
      w[0][0] * p[0] + w[0][1] * p[1] + w[0][2] * p[2] + w[0][3],
      w[1][0] * p[0] + w[1][1] * p[1] + w[1][2] * p[2] + w[1][3],
      w[2][0] * p[0] + w[2][1] * p[1] + w[2][2] * p[2] + w[2][3],
    ];
  }
  function xformNormal(w, n) {
    const a = w[0][0],
      b = w[0][1],
      c = w[0][2],
      d = w[1][0],
      e = w[1][1],
      f = w[1][2],
      hh = w[2][0],
      i = w[2][1],
      j = w[2][2];
    const det =
      a * (e * j - f * i) - b * (d * j - f * hh) + c * (d * i - e * hh);
    let x, y, z;
    if (det !== 0 && Math.abs(det) > 1e-12) {
      const id = 1 / det;
      const i00 = (e * j - f * i) * id,
        i01 = (c * i - b * j) * id,
        i02 = (b * f - c * e) * id;
      const i10 = (f * hh - d * j) * id,
        i11 = (a * j - c * hh) * id,
        i12 = (c * d - a * f) * id;
      const i20 = (d * i - e * hh) * id,
        i21 = (b * hh - a * i) * id,
        i22 = (a * e - b * d) * id;
      x = i00 * n[0] + i10 * n[1] + i20 * n[2];
      y = i01 * n[0] + i11 * n[1] + i21 * n[2];
      z = i02 * n[0] + i12 * n[1] + i22 * n[2];
    } else {
      x = a * n[0] + b * n[1] + c * n[2];
      y = d * n[0] + e * n[1] + f * n[2];
      z = hh * n[0] + i * n[1] + j * n[2];
    }
    const l = Math.sqrt(x * x + y * y + z * z);
    if (!(l > 1e-12)) return [0, 1, 0];
    return [x / l, y / l, z / l];
  }
  function fmtCol(c) {
    if (!c) return "n";
    return c[0].toFixed(3) + "," + c[1].toFixed(3) + "," + c[2].toFixed(3);
  }
  function primKey(texTable, tid, blend, alpha, state) {
    const s = state || {};
    return (
      (texTable == null ? "x" : texTable) +
      "|" +
      tid +
      "|" +
      (blend ? 1 : 0) +
      "|" +
      (blend ? alpha.toFixed(3) : "1") +
      "|" +
      fmtCol(s.diffuse) +
      "|" +
      (s.colorCalc != null ? s.colorCalc : "n") +
      "|" +
      (s.side || "double") +
      "|" +
      (s.matAttr != null ? s.matAttr : "n")
    );
  }
  function getTexInfo(R, texTable, tid) {
    try {
      if (!R || texTable == null) return null;
      const scene = g.RDXScene;
      if (!scene || !scene.getTexture) return null;
      return scene.getTexture(R, texTable, tid) || null;
    } catch {
      return null;
    }
  }
  function buildGlb(R, items, opts) {
    opts = opts || {};
    const wantTex = opts.textures !== false;
    const sceneName = opts.name || "rdx";
    const tis = {},
      tiList = [];
    (items || []).forEach(function (it) {
      if (!it || !it.model || !it.model.faces || !it.model.faces.length) return;
      if (it.hidden) return;
      const ti = it.ti;
      if (tis[ti] === undefined) {
        tis[ti] = { kind: it.kind || "room", prims: {} };
        tiList.push(ti);
      }
    });
    tiList.sort(function (a, b) {
      return a - b;
    });
    const byTi = {};
    tiList.forEach(function (ti) {
      byTi[ti] = [];
    });
    (items || []).forEach(function (it) {
      if (!it || byTi[it.ti] === undefined) return;
      if (it.hidden) return;
      if (!it.model || !it.model.faces || !it.model.faces.length) return;
      byTi[it.ti].push(it);
    });
    const acc = {};
    tiList.forEach(function (ti) {
      acc[ti] = {};
    });
    let triCount = 0;
    tiList.forEach(function (ti) {
      const list = byTi[ti];
      list.forEach(function (it) {
        const m = it.model;
        let f;
        const groups = {};
        for (f = 0; f < m.faces.length; f++) {
          const st =
            g.RDXNJ && g.RDXNJ.faceState ? g.RDXNJ.faceState(m, f) : null;
          let blend, matIdx, alpha, diffuse, colorCalc, side, matAttr;
          const tid = m.texids[f];
          if (st) {
            blend = st.blend;
            matIdx = m.mats[f];
            alpha = st.blend ? st.alpha : 1;
            diffuse = st.diffuse;
            colorCalc = st.colorCalc;
            side = st.side;
            matAttr = st.matAttr;
          } else {
            blend = !!m.faceBlend[f];
            matIdx = m.mats[f];
            alpha = 1;
            if (blend && matIdx >= 0 && m.materials && m.materials[matIdx]) {
              alpha =
                m.materials[matIdx].alpha == null
                  ? 1
                  : m.materials[matIdx].alpha;
            }
            if (!blend) alpha = 1;
            diffuse =
              matIdx >= 0 && m.materials && m.materials[matIdx]
                ? m.materials[matIdx].diffuse
                : null;
            colorCalc = null;
            side = "double";
            matAttr =
              matIdx >= 0 && m.materials && m.materials[matIdx]
                ? m.materials[matIdx].attr
                : 0;
          }
          if (!blend) alpha = 1;
          const stKey = {
            diffuse: diffuse,
            colorCalc: colorCalc,
            side: side,
            matAttr: matAttr,
          };
          const key = primKey(it.texTable, tid, blend, alpha, stKey);
          let gr = groups[key];
          if (!gr) {
            gr = groups[key] = {
              texTable: it.texTable,
              tid: tid,
              blend: blend,
              alpha: alpha,
              diffuse: diffuse,
              ambient: null,
              colorCalc: colorCalc,
              side: side,
              matAttr: matAttr,
              faces: [],
            };
          }
          gr.faces.push(f);
        }
        Object.keys(groups).forEach(function (key) {
          const gr2 = groups[key];
          let slot = acc[ti][key];
          if (!slot) {
            slot = acc[ti][key] = {
              texTable: gr2.texTable,
              tid: gr2.tid,
              blend: gr2.blend,
              alpha: gr2.alpha,
              diffuse: gr2.diffuse,
              ambient: null,
              colorCalc: gr2.colorCalc,
              side: gr2.side,
              matAttr: gr2.matAttr,
              pos: [],
              nrm: [],
              uv: [],
            };
          }
          gr2.faces.forEach(function (fi) {
            const fc = m.faces[fi],
              uvs = m.uvs[fi];
            let c;
            triCount++;
            const isFlat =
              g.RDXNJ && g.RDXNJ.faceState
                ? !!g.RDXNJ.faceState(m, fi).flat
                : m.faceFlat
                  ? !!m.faceFlat[fi]
                  : false;
            let fn = null;
            if (isFlat) {
              const p0 = m.verts[fc[0]],
                p1 = m.verts[fc[1]],
                p2 = m.verts[fc[2]];
              const e1x = p1[0] - p0[0],
                e1y = p1[1] - p0[1],
                e1z = p1[2] - p0[2];
              const e2x = p2[0] - p0[0],
                e2y = p2[1] - p0[1],
                e2z = p2[2] - p0[2];
              const cx = e1y * e2z - e1z * e2y,
                cy = e1z * e2x - e1x * e2z,
                cz = e1x * e2y - e1y * e2x;
              const cl = Math.sqrt(cx * cx + cy * cy + cz * cz);
              if (
                isFinite(cx) &&
                isFinite(cy) &&
                isFinite(cz) &&
                isFinite(cl) &&
                cl > 1e-12
              ) {
                fn = [cx / cl, cy / cl, cz / cl];
              } else fn = [0, 0, 0];
              fn = xformNormal(it.world, fn);
            }
            for (c = 0; c < 3; c++) {
              const vi = fc[c],
                vv = m.verts[vi],
                no = (m.normals && m.normals[vi]) || [0, 1, 0];
              const p = xformPos(it.world, vv);
              const n = fn ? fn : xformNormal(it.world, no);
              slot.pos.push(p[0], p[1], p[2]);
              slot.nrm.push(n[0], n[1], n[2]);
              const uv = (uvs && uvs[c]) || [0, 0];
              slot.uv.push(uv[0], uv[1]);
            }
          });
        });
      });
    });
    const chunks = [];
    let byteOffset = 0;
    const bufferViews = [],
      accessors = [];
    function pushData(u8) {
      const pad = (4 - (byteOffset % 4)) % 4;
      if (pad) {
        const zp = new Uint8Array(pad);
        chunks.push(zp);
        byteOffset += pad;
      }
      const idx = bufferViews.length;
      bufferViews.push({
        buffer: 0,
        byteOffset: byteOffset,
        byteLength: u8.length,
      });
      chunks.push(u8);
      byteOffset += u8.length;
      return idx;
    }
    function pushFloat(arr) {
      const f = new Float32Array(arr);
      return pushData(new Uint8Array(f.buffer, f.byteOffset, f.byteLength));
    }
    const nodes = [],
      meshes = [],
      materials = [],
      textures = [],
      images = [],
      samplers = [];
    const matCache = {},
      imgCache = {};
    const texInfoCache = {};
    function texInfoFor(texTable, tid) {
      const ck = texTable + ":" + tid;
      if (texInfoCache[ck] !== undefined) return texInfoCache[ck];
      const ti2 = getTexInfo(R, texTable, tid);
      texInfoCache[ck] = ti2;
      return ti2;
    }
    function imageFor(texTable, tid, label) {
      const ck = texTable + ":" + tid;
      if (imgCache[ck] !== undefined) return imgCache[ck];
      let idx = -1;
      try {
        const ti2 = texInfoFor(texTable, tid);
        let canvas = ti2 && (ti2.tex ? ti2.tex.image : ti2.image);
        if (!canvas && ti2 && ti2.canvas) canvas = ti2.canvas;
        const png = canvasPngBytes(canvas);
        if (png && png.length) {
          const vi = pushData(png);
          idx = images.length;
          images.push({
            bufferView: vi,
            mimeType: "image/png",
            name: label || "TEX_" + texTable + "_" + tid,
          });
          if (samplers.length === 0) {
            samplers.push({
              magFilter: 9729,
              minFilter: 9987,
              wrapS: 10497,
              wrapT: 10497,
            });
          }
        }
      } catch {
        idx = -1;
      }
      imgCache[ck] = idx;
      return idx;
    }
    function materialFor(ti, slot) {
      const ck =
        ti +
        "|" +
        primKey(slot.texTable, slot.tid, slot.blend, slot.alpha, slot);
      if (matCache[ck] !== undefined) {
        return {
          index: matCache[ck],
          textured: materials[matCache[ck]]._textured === true,
        };
      }
      let textured = false,
        alphaMode = "OPAQUE",
        base = [0.6, 0.6, 0.6, 1];
      const emissive = [0, 0, 0];
      let img = -1,
        texKind = null;
      if (slot.texTable != null) {
        const ti2 = texInfoFor(slot.texTable, slot.tid);
        if (ti2) {
          texKind = ti2.kind || null;
          if (wantTex) {
            img = imageFor(
              slot.texTable,
              slot.tid,
              "TEX_" + slot.texTable + "_" + slot.tid,
            );
            if (img >= 0) textured = true;
          }
        }
      }
      if (slot.diffuse) {
        base = [
          slot.diffuse[0],
          slot.diffuse[1],
          slot.diffuse[2],
          slot.blend ? slot.alpha : 1,
        ];
      } else if (textured) {
        base = [1, 1, 1, slot.blend ? slot.alpha : 1];
      } else {
        base = [0.6, 0.6, 0.6, slot.blend ? slot.alpha : 1];
      }
      if (textured) {
        const transparent =
          slot.blend && (texKind !== "OPAQUE" || slot.alpha < 0.999);
        if (transparent && texKind === "CUTOUT" && slot.alpha >= 0.999) {
          alphaMode = "MASK";
        } else if (transparent) alphaMode = "BLEND";
        else if (texKind === "CUTOUT") alphaMode = "MASK";
      } else {
        if (slot.blend && slot.alpha < 0.999) alphaMode = "BLEND";
        else if (texKind === "CUTOUT") alphaMode = "MASK";
      }
      const mat = {
        name: "MDL_" + pad3(ti) + (textured ? "_TEX_" + slot.tid : "_flat"),
        pbrMetallicRoughness: {
          baseColorFactor: base,
          metallicFactor: 0,
          roughnessFactor: 1,
        },
        emissiveFactor: emissive,
        alphaMode: alphaMode,
        doubleSided: slot.side !== "front",
      };
      if (alphaMode === "MASK") mat.alphaCutoff = 0.5;
      if (textured) {
        mat.pbrMetallicRoughness.baseColorTexture = {
          index: textures.length,
          texCoord: 0,
        };
      }
      mat._textured = textured;
      const mi = materials.length;
      materials.push(mat);
      if (textured) {
        textures.push({
          source: img,
          sampler: 0,
          name: "TEX_" + slot.texTable + "_" + slot.tid,
        });
      }
      matCache[ck] = mi;
      return { index: mi, textured: textured };
    }
    tiList.forEach(function (ti) {
      const keys = Object.keys(acc[ti]);
      if (!keys.length) return;
      const prims = [],
        tiName = "MDL_" + pad3(ti) + "_" + (tis[ti].kind || "room");
      keys.forEach(function (key) {
        const slot = acc[ti][key];
        const vc = slot.pos.length / 3;
        if (!(vc > 0)) return;
        const mr = materialFor(ti, slot);
        const posView = pushFloat(slot.pos);
        const nrmView = pushFloat(slot.nrm);
        const mn = [slot.pos[0], slot.pos[1], slot.pos[2]],
          mx = [slot.pos[0], slot.pos[1], slot.pos[2]];
        let i;
        for (i = 1; i < vc; i++) {
          const x = slot.pos[i * 3],
            y = slot.pos[i * 3 + 1],
            z = slot.pos[i * 3 + 2];
          if (x < mn[0]) mn[0] = x;
          if (y < mn[1]) mn[1] = y;
          if (z < mn[2]) mn[2] = z;
          if (x > mx[0]) mx[0] = x;
          if (y > mx[1]) mx[1] = y;
          if (z > mx[2]) mx[2] = z;
        }
        const posAcc = accessors.length;
        accessors.push({
          bufferView: posView,
          componentType: 5126,
          count: vc,
          type: "VEC3",
          min: mn,
          max: mx,
        });
        const nrmAcc = accessors.length;
        accessors.push({
          bufferView: nrmView,
          componentType: 5126,
          count: vc,
          type: "VEC3",
        });
        const attrs = { POSITION: posAcc, NORMAL: nrmAcc };
        if (mr.textured) {
          const uvView = pushFloat(slot.uv);
          const uvAcc = accessors.length;
          accessors.push({
            bufferView: uvView,
            componentType: 5126,
            count: vc,
            type: "VEC2",
          });
          attrs.TEXCOORD_0 = uvAcc;
        }
        prims.push({ attributes: attrs, material: mr.index });
      });
      if (!prims.length) return;
      const meshIdx = meshes.length;
      meshes.push({ name: tiName, primitives: prims });
      nodes.push({ name: tiName, mesh: meshIdx });
    });
    materials.forEach(function (m) {
      delete m._textured;
    });
    let totalLen = byteOffset;
    const padEnd = (4 - (totalLen % 4)) % 4;
    if (padEnd) {
      chunks.push(new Uint8Array(padEnd));
      totalLen += padEnd;
    }
    const json = {
      asset: { version: "2.0", generator: "RDX Web Viewer" },
      scene: 0,
      scenes: [
        {
          name: sceneName,
          nodes: nodes.map(function (_, i) {
            return i;
          }),
        },
      ],
      nodes: nodes,
      meshes: meshes,
      materials: materials,
      accessors: accessors,
      bufferViews: bufferViews,
      buffers: [{ byteLength: totalLen }],
    };
    if (images.length) {
      json.images = images;
      json.textures = textures;
      json.samplers = samplers;
    }
    const jsonStr = JSON.stringify(json);
    let jsonBytes;
    try {
      if (typeof TextEncoder !== "undefined") {
        jsonBytes = new TextEncoder().encode(jsonStr);
      } else if (typeof globalThis.Buffer !== "undefined") {
        jsonBytes = new Uint8Array(globalThis.Buffer.from(jsonStr, "utf8"));
      } else throw new Error("no UTF-8 encoder available");
    } catch (e) {
      if (typeof globalThis.Buffer !== "undefined") {
        jsonBytes = new Uint8Array(globalThis.Buffer.from(jsonStr, "utf8"));
      } else throw e;
    }
    const jsonPad = (4 - (jsonBytes.length % 4)) % 4;
    const binLen = totalLen;
    const total =
      12 + 8 + jsonBytes.length + jsonPad + (binLen ? 8 + binLen : 0);
    const out = new Uint8Array(total);
    const dv = new DataView(out.buffer, out.byteOffset, out.byteLength);
    dv.setUint32(0, 0x46546c67, true);
    dv.setUint32(4, 2, true);
    dv.setUint32(8, total, true);
    dv.setUint32(12, jsonBytes.length + jsonPad, true);
    dv.setUint32(16, 0x4e4f534a, true);
    out.set(jsonBytes, 20);
    for (let s = 0; s < jsonPad; s++) out[20 + jsonBytes.length + s] = 0x20;
    const bo = 20 + jsonBytes.length + jsonPad;
    if (binLen) {
      dv.setUint32(bo, binLen, true);
      dv.setUint32(bo + 4, 0x004e4942, true);
      let off = bo + 8;
      chunks.forEach(function (c) {
        out.set(c, off);
        off += c.length;
      });
    }
    const stats = {
      nodes: nodes.length,
      meshes: meshes.length,
      primitives: meshes.reduce(function (a, m) {
        return a + m.primitives.length;
      }, 0),
      materials: materials.length,
      images: images.length,
      triangles: triCount,
      bytes: total,
    };
    return { bytes: out, json: json, stats: stats };
  }
  function downloadGlb(bytes, filename) {
    try {
      if (typeof document === "undefined") return false;
      let blob;
      try {
        blob = new Blob([bytes], { type: "model/gltf-binary" });
      } catch {
        return false;
      }
      const a = document.createElement("a");
      a.href = (g.URL || window.URL).createObjectURL(blob);
      a.download = filename || "models.glb";
      if (document.body) document.body.appendChild(a);
      a.click();
      setTimeout(function () {
        try {
          (g.URL || window.URL).revokeObjectURL(a.href);
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
  g.RDXExportGLTF = { buildGlb: buildGlb, downloadGlb: downloadGlb };
})(window);
