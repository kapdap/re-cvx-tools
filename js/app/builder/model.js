(function (g) {
  "use strict";
  function build(root, Rr, built, matCache) {
    const S = g.AppState;
    built.items.forEach(function (it) {
      if (it.hidden) return;
      const ni = it.node && it.node.index != null ? it.node.index : -1;
      const nkey = it.ti + ":" + it.bi + ":" + ni;
      const itemVis =
        !!S.opts[it.kind] &&
        S.tableVis[it.ti] !== false &&
        S.detailVis[nkey] !== false;
      const m = it.model;
      const groups = {};
      for (let f = 0; f < m.faces.length; f++) {
        const tid = m.texids[f];
        const st =
          g.RDXNJ && g.RDXNJ.faceState ? g.RDXNJ.faceState(m, f) : null;
        let blend, alpha, diffuse, colorCalc, side, matAttr;
        if (st) {
          blend = st.blend;
          alpha = st.blend ? st.alpha : 1;
          diffuse = st.diffuse;
          colorCalc = st.colorCalc;
          side = st.side;
          matAttr = st.matAttr;
        } else {
          blend = m.faceBlend[f];
          const mat = m.mats[f];
          alpha = 1;
          if (blend && mat >= 0 && m.materials[mat]) {
            alpha = m.materials[mat].alpha == null ? 1 : m.materials[mat].alpha;
          }
          diffuse =
            mat >= 0 && m.materials[mat] ? m.materials[mat].diffuse : null;
          colorCalc = null;
          side = "double";
          matAttr = mat >= 0 && m.materials[mat] ? m.materials[mat].attr : 0;
        }
        if (!blend) alpha = 1;
        function fmtC(c) {
          if (!c) return "n";
          return (
            c[0].toFixed(3) + "," + c[1].toFixed(3) + "," + c[2].toFixed(3)
          );
        }
        const key =
          tid +
          "|" +
          (blend ? 1 : 0) +
          "|" +
          (blend ? alpha.toFixed(3) : "1") +
          "|" +
          fmtC(diffuse) +
          "|" +
          (colorCalc != null ? colorCalc : "n") +
          "|" +
          side +
          "|" +
          (matAttr != null ? matAttr : "n");
        (groups[key] = groups[key] || {
          tid: tid,
          blend: blend,
          alpha: alpha,
          diffuse: diffuse,
          ambient: null,
          colorCalc: colorCalc,
          side: side,
          matAttr: matAttr,
          faces: [],
        }).faces.push(f);
      }
      Object.keys(groups).forEach(function (k) {
        const gr = groups[k],
          pp = [],
          nn = [],
          uv = [];
        gr.faces.forEach(function (fi) {
          const fc = m.faces[fi],
            uvs = m.uvs[fi];
          const flat = m.faceFlat ? !!m.faceFlat[fi] : false;
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
          let fnx = 0,
            fny = 0,
            fnz = 0;
          if (
            isFinite(cx) &&
            isFinite(cy) &&
            isFinite(cz) &&
            isFinite(cl) &&
            cl > 1e-12
          ) {
            fnx = cx / cl;
            fny = cy / cl;
            fnz = cz / cl;
          }
          for (let c = 0; c < 3; c++) {
            const vi = fc[c];
            const vv = m.verts[vi],
              no = m.normals[vi];
            let nx = fnx,
              ny = fny,
              nz = fnz;
            if (
              !flat &&
              no &&
              isFinite(no[0]) &&
              isFinite(no[1]) &&
              isFinite(no[2])
            ) {
              const nl = Math.sqrt(
                no[0] * no[0] + no[1] * no[1] + no[2] * no[2],
              );
              if (isFinite(nl) && nl > 1e-12) {
                nx = no[0] / nl;
                ny = no[1] / nl;
                nz = no[2] / nl;
              }
            }
            pp.push(vv[0], vv[1], vv[2]);
            nn.push(nx, ny, nz);
            uv.push(uvs[c][0], 1 - uvs[c][1]);
          }
        });
        const geo = new THREE.BufferGeometry();
        geo.setAttribute("position", new THREE.Float32BufferAttribute(pp, 3));
        geo.setAttribute("normal", new THREE.Float32BufferAttribute(nn, 3));
        geo.setAttribute("uv", new THREE.Float32BufferAttribute(uv, 2));
        const mk = g.AppMaterial.key(
          it.texTable,
          gr.tid,
          gr.blend,
          gr.alpha,
          gr,
        );
        let material = matCache[mk];
        if (!material) {
          material = g.AppMaterial.make(
            Rr,
            it.texTable,
            gr.tid,
            gr.blend,
            gr.alpha,
            gr,
          );
          matCache[mk] = material;
        }
        const mesh = new THREE.Mesh(geo, material);
        mesh.matrixAutoUpdate = false;
        mesh.matrix.set(
          it.world[0][0],
          it.world[0][1],
          it.world[0][2],
          it.world[0][3],
          it.world[1][0],
          it.world[1][1],
          it.world[1][2],
          it.world[1][3],
          it.world[2][0],
          it.world[2][1],
          it.world[2][2],
          it.world[2][3],
          0,
          0,
          0,
          1,
        );
        mesh.matrixWorldNeedsUpdate = true;
        mesh.userData.entry = { kind: it.kind, ti: it.ti, bi: it.bi, ni: ni };
        if (!itemVis) mesh.visible = false;
        root.add(mesh);
        S.modelEntries.push({
          mesh: mesh,
          kind: it.kind,
          ti: it.ti,
          bi: it.bi,
          ni: ni,
        });
      });
    });
  }
  g.AppModelBuilder = { build: build };
})(window);
