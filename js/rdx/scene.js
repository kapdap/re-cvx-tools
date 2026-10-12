(function (g) {
  "use strict";
  const TAU = Math.PI * 2;
  function ninjaRad(a) {
    return (((a | 0) & 0xffff) * TAU) / 65536;
  }
  function rx(a) {
    const c = Math.cos(a),
      s = Math.sin(a);
    return [
      [1, 0, 0],
      [0, c, -s],
      [0, s, c],
    ];
  }
  function ry(a) {
    const c = Math.cos(a),
      s = Math.sin(a);
    return [
      [c, 0, s],
      [0, 1, 0],
      [-s, 0, c],
    ];
  }
  function rz(a) {
    const c = Math.cos(a),
      s = Math.sin(a);
    return [
      [c, -s, 0],
      [s, c, 0],
      [0, 0, 1],
    ];
  }
  function mm(A, B) {
    const o = [];
    for (let i = 0; i < 3; i++) {
      o.push([]);
      for (let j = 0; j < 3; j++) {
        o[i].push(A[i][0] * B[0][j] + A[i][1] * B[1][j] + A[i][2] * B[2][j]);
      }
    }
    return o;
  }
  function ninjaMat(ax, ay, az, zxy) {
    const rax = ninjaRad(ax),
      ray = ninjaRad(ay),
      raz = ninjaRad(az);
    if (zxy) return mm(ry(ray), mm(rx(rax), rz(raz)));
    return mm(mm(rz(raz), ry(ray)), rx(rax));
  }
  function mMul(A, B) {
    const o = [];
    for (let i = 0; i < 4; i++) {
      o.push([]);
      for (let j = 0; j < 4; j++) {
        o[i].push(
          A[i][0] * B[0][j] +
            A[i][1] * B[1][j] +
            A[i][2] * B[2][j] +
            A[i][3] * B[3][j],
        );
      }
    }
    return o;
  }
  function mTRS(pos, R3, scl) {
    return [
      [R3[0][0] * scl[0], R3[0][1] * scl[1], R3[0][2] * scl[2], pos[0]],
      [R3[1][0] * scl[0], R3[1][1] * scl[1], R3[1][2] * scl[2], pos[1]],
      [R3[2][0] * scl[0], R3[2][1] * scl[1], R3[2][2] * scl[2], pos[2]],
      [0, 0, 0, 1],
    ];
  }
  function xformMatOf(entry, yawOnly) {
    let ax = entry.ang[0];
    const ay = entry.ang[1];
    let az = entry.ang[2];
    if (yawOnly) {
      ax = 0;
      az = 0;
    }
    return mTRS(entry.pos, ninjaMat(ax, ay, az, false), [1, 1, 1]);
  }
  function nodeLocalMat(o, isRoom) {
    const f = o.evalflags || 0;
    let pos = o.pos,
      ax = o.ang[0],
      ay = o.ang[1],
      az = o.ang[2],
      scl = o.scl;
    if (isRoom) {
      if (f & 0x1) pos = [0, 0, 0];
      if (f & 0x2) {
        ax = 0;
        ay = 0;
        az = 0;
      }
      if (f & 0x4) scl = [1, 1, 1];
      return mTRS(pos, ninjaMat(ax, ay, az, !!(f & 0x20)), scl);
    }
    return mTRS(pos, ninjaMat(ax, ay, az, false), [1, 1, 1]);
  }
  function modelPlacements(R) {
    const types = {},
      xforms = {};
    Object.keys(R.map).forEach(function (ti) {
      const kind = R.map[ti][0],
        idx = R.map[ti][1];
      types[ti] = kind;
      if (idx == null) return;
      const tab = kind === "actor" ? R.actor : kind === "obj" ? R.obj : R.item;
      xforms[ti] = xformMatOf(tab[idx], kind === "actor");
    });
    return { types: types, xforms: xforms };
  }
  function bleedTransparentRgb(raw, w, h) {
    if (!raw || !raw.length || w <= 0 || h <= 0 || raw.length < w * h * 4) {
      return raw;
    }
    let i,
      hasOpaque = false;
    for (i = 3; i < raw.length; i += 4) {
      if (raw[i] !== 0) {
        hasOpaque = true;
        break;
      }
    }
    if (!hasOpaque) return raw;
    const n = w * h,
      INF = 1 << 30,
      dist = new Array(n),
      sr = new Array(n),
      sg = new Array(n),
      sb = new Array(n);
    let x, y, idx, o;
    for (idx = 0; idx < n; idx++) {
      o = idx * 4;
      if (raw[o + 3] !== 0) {
        dist[idx] = 0;
        sr[idx] = raw[o];
        sg[idx] = raw[o + 1];
        sb[idx] = raw[o + 2];
      } else dist[idx] = INF;
    }
    let best, bi;
    for (y = 0; y < h; y++) {
      for (x = 0; x < w; x++) {
        idx = y * w + x;
        if (dist[idx] === 0) continue;
        best = dist[idx];
        bi = -1;
        if (x > 0 && dist[idx - 1] + 1 < best) {
          best = dist[idx - 1] + 1;
          bi = idx - 1;
        }
        if (y > 0) {
          if (dist[idx - w] + 1 < best) {
            best = dist[idx - w] + 1;
            bi = idx - w;
          }
          if (x > 0 && dist[idx - w - 1] + 2 < best) {
            best = dist[idx - w - 1] + 2;
            bi = idx - w - 1;
          }
          if (x + 1 < w && dist[idx - w + 1] + 2 < best) {
            best = dist[idx - w + 1] + 2;
            bi = idx - w + 1;
          }
        }
        if (bi >= 0) {
          dist[idx] = best;
          sr[idx] = sr[bi];
          sg[idx] = sg[bi];
          sb[idx] = sb[bi];
        }
      }
    }
    for (y = h - 1; y >= 0; y--) {
      for (x = w - 1; x >= 0; x--) {
        idx = y * w + x;
        if (dist[idx] === 0) continue;
        best = dist[idx];
        bi = -1;
        if (x + 1 < w && dist[idx + 1] + 1 < best) {
          best = dist[idx + 1] + 1;
          bi = idx + 1;
        }
        if (y + 1 < h) {
          if (dist[idx + w] + 1 < best) {
            best = dist[idx + w] + 1;
            bi = idx + w;
          }
          if (x + 1 < w && dist[idx + w + 1] + 2 < best) {
            best = dist[idx + w + 1] + 2;
            bi = idx + w + 1;
          }
          if (x > 0 && dist[idx + w - 1] + 2 < best) {
            best = dist[idx + w - 1] + 2;
            bi = idx + w - 1;
          }
        }
        if (bi >= 0 && best < dist[idx]) {
          dist[idx] = best;
          sr[idx] = sr[bi];
          sg[idx] = sg[bi];
          sb[idx] = sb[bi];
        }
      }
    }
    const out = new Uint8Array(raw);
    for (idx = 0; idx < n; idx++) {
      if (raw[idx * 4 + 3] === 0 && dist[idx] !== INF) {
        out[idx * 4] = sr[idx];
        out[idx * 4 + 1] = sg[idx];
        out[idx * 4 + 2] = sb[idx];
      }
    }
    return out;
  }
  const texCache = {};
  function texKey(ti, entry) {
    return (
      ti +
      ":" +
      entry.bi +
      ":" +
      entry.pi +
      ":" +
      (entry.variant == null ? "n" : entry.variant)
    );
  }
  function getTexture(R, ti, texid) {
    const pics = g.RDX.tablePics(R, ti);
    if (texid < 0 || texid >= pics.length) return null;
    const e = pics[texid],
      key = R._uid + ":" + texKey(ti, e);
    if (texCache[key]) return texCache[key];
    const doc = g.RDX.decodeBlock(R, ti, e.bi),
      pic = doc.pictures[e.pi];
    let px = pic.pixels;
    if (e.variant != null) {
      const st = g.RDX.pliStateAt(R, ti, e.bi);
      const cols = st[1].map(function (r) {
        return r[0];
      });
      px = g.RDXTIM2.applyPalSub(pic.imageType, pic.imageData, px, cols);
    }
    const kind = g.RDXTIM2.classifyAlpha(px);
    let ai;
    if (kind === "OPAQUE") {
      let allZero = true;
      for (ai = 3; ai < px.length; ai += 4) {
        if (px[ai] !== 0) {
          allZero = false;
          break;
        }
      }
      if (allZero) {
        const cp = new Uint8Array(px);
        for (ai = 3; ai < cp.length; ai += 4) cp[ai] = 255;
        px = cp;
      }
    } else px = bleedTransparentRgb(px, pic.width, pic.height);
    const cv = document.createElement("canvas");
    cv.width = pic.width;
    cv.height = pic.height;
    const cx = cv.getContext("2d"),
      img = cx.createImageData(pic.width, pic.height);
    for (let i = 0; i < pic.width * pic.height; i++) {
      img.data[i * 4] = px[i * 4];
      img.data[i * 4 + 1] = px[i * 4 + 1];
      img.data[i * 4 + 2] = px[i * 4 + 2];
      img.data[i * 4 + 3] = px[i * 4 + 3];
    }
    cx.putImageData(img, 0, 0);
    const t = new THREE.CanvasTexture(cv);
    t.flipY = true;
    t.magFilter = THREE.LinearFilter;
    t.minFilter = THREE.LinearMipmapLinearFilter;
    t.generateMipmaps = true;
    try {
      t.anisotropy = 4;
    } catch {
      /* ignore */
    }
    if (THREE.sRGBEncoding !== undefined) t.encoding = THREE.sRGBEncoding;
    texCache[key] = { tex: t, kind: kind };
    return texCache[key];
  }
  const texNamesByTableCache = {};
  function validTexNameCount(arr) {
    let n = 0;
    for (let i = 0; i < arr.length; i++) {
      const e = arr[i];
      if (typeof e === "string") {
        if (e) n++;
      } else if (e && e.nameValid && e.name) n++;
    }
    return n;
  }
  function texNamesByTable(R) {
    const uid = (R && R._uid) || 0;
    if (texNamesByTableCache[uid]) return texNamesByTableCache[uid];
    const map = {};
    try {
      const built = buildGeometries(R);
      built.items.forEach(function (it) {
        if (it.texTable == null) return;
        if (!it.texNames || !it.texNames.length) return;
        const cur = map[it.texTable];
        if (
          cur === undefined ||
          validTexNameCount(it.texNames) > validTexNameCount(cur)
        ) {
          map[it.texTable] = it.texNames;
        }
      });
    } catch {
      /* ignore */
    }
    texNamesByTableCache[uid] = map;
    return map;
  }
  function texNameFor(R, ti, texid) {
    try {
      const arr = texNamesByTable(R)[ti];
      if (!arr) return null;
      const e = arr[texid];
      if (typeof e === "string") return e || null;
      if (e && e.nameValid && e.name) return e.name;
    } catch {
      /* ignore */
    }
    return null;
  }
  function buildGeometries(R) {
    const out = [],
      place = modelPlacements(R);
    R.models.forEach(function (tab, ti) {
      const kind = place.types[ti] || "room",
        xf = place.xforms[ti] || null,
        isRoom = kind === "room";
      tab.blobs.forEach(function (bb, bi) {
        let mlb;
        try {
          mlb = g.RDXMLB.readMlb(R.data, bb[0], bb[1]);
        } catch {
          return;
        }
        const parents = {};
        let i, o;
        for (i = 0; i < mlb.objects.length; i++) {
          o = mlb.objects[i];
          const seen = {};
          seen[o.index] = 1;
          let cur = g.RDXMLB.objectIndexOf(mlb, o.child);
          while (cur !== -1 && !seen[cur]) {
            seen[cur] = 1;
            parents[cur] = o.index;
            cur = g.RDXMLB.objectIndexOf(mlb, mlb.objects[cur].sibling);
          }
        }
        const locals = {};
        for (i = 0; i < mlb.objects.length; i++) {
          o = mlb.objects[i];
          locals[o.index] = nodeLocalMat(o, isRoom);
        }
        const worlds = {};
        function worldOf(idx) {
          if (worlds[idx]) return worlds[idx];
          let w;
          if (parents[idx] == null) {
            w = xf ? mMul(xf, locals[idx]) : locals[idx];
          } else w = mMul(worldOf(parents[idx]), locals[idx]);
          worlds[idx] = w;
          return w;
        }
        for (i = 0; i < mlb.objects.length; i++) {
          o = mlb.objects[i];
          if (o.modelIndex < 0 || o.modelIndex >= mlb.models.length) continue;
          const m = mlb.models[o.modelIndex];
          if (!m || !m.faces.length) continue;
          const texTable =
            R.assign[ti + "," + bi] != null
              ? R.assign[ti + "," + bi]
              : R.assign[ti + ",0"] != null
                ? R.assign[ti + ",0"]
                : null;
          out.push({
            ti: ti,
            bi: bi,
            kind: kind,
            node: o,
            model: m,
            texTable: texTable,
            texNames: mlb.texNames || [],
            world: worldOf(o.index),
            hidden: !!o.hidden,
          });
        }
      });
    });
    return { items: out, placements: place };
  }
  g.RDXScene = {
    ninjaMat: ninjaMat,
    getTexture: getTexture,
    buildGeometries: buildGeometries,
    texNameFor: texNameFor,
  };
})(window);
