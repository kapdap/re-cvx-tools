(function (g) {
  "use strict";
  let camMat = null;
  function quatFromAng(ang) {
    if (g.AppLightBuilder && g.AppLightBuilder.quatFromAng) {
      return g.AppLightBuilder.quatFromAng(ang);
    }
    const R = g.RDXScene.ninjaMat(ang[0] | 0, ang[1] | 0, ang[2] | 0, false);
    const m = new THREE.Matrix4();
    m.set(
      R[0][0],
      R[0][1],
      R[0][2],
      0,
      R[1][0],
      R[1][1],
      R[1][2],
      0,
      R[2][0],
      R[2][1],
      R[2][2],
      0,
      0,
      0,
      0,
      1,
    );
    return new THREE.Quaternion().setFromRotationMatrix(m);
  }
  function camFrustumGeo(len) {
    const n = len * 0.15,
      f = len;
    const nw = len * 0.11,
      nh = len * 0.08,
      fw = len * 0.55,
      fh = len * 0.4;
    const n0 = [-nw, -nh, -n],
      n1 = [nw, -nh, -n],
      n2 = [nw, nh, -n],
      n3 = [-nw, nh, -n];
    const f0 = [-fw, -fh, -f],
      f1 = [fw, -fh, -f],
      f2 = [fw, fh, -f],
      f3 = [-fw, fh, -f];
    const pts = []
      .concat(n0, n1, n1, n2, n2, n3, n3, n0)
      .concat(f0, f1, f1, f2, f2, f3, f3, f0)
      .concat(n0, f0, n1, f1, n2, f2, n3, f3);
    const geo = new THREE.BufferGeometry();
    geo.setAttribute("position", new THREE.Float32BufferAttribute(pts, 3));
    return geo;
  }
  function buildCamera(root, Rr) {
    const S = g.AppState;
    if (!S.opts.cameras) return;
    if (!camMat) camMat = new THREE.LineBasicMaterial({ color: 0xffc040 });
    const camGeo = camFrustumGeo(1);
    Rr.cameras.forEach(function (c, ci) {
      c.blocks.forEach(function (b, bi) {
        if (S.markVis["camera:" + ci + ":" + bi] === false) return;
        const mk = new THREE.LineSegments(camGeo, camMat);
        mk.position.set(b.pos[0], b.pos[1], b.pos[2]);
        try {
          mk.quaternion.copy(quatFromAng(b.ang));
        } catch {
          /* ignore */
        }
        mk.userData.cammark = true;
        mk.userData.pick = { kind: "camera", a: ci, b: bi };
        root.add(mk);
      });
    });
  }
  function buildPlayer(root, Rr) {
    const S = g.AppState;
    if (!S.opts.players) return;
    const plyGeo = new THREE.BoxGeometry(8, 20, 8);
    const plyMat = new THREE.MeshBasicMaterial({
      color: 0x00ff66,
      wireframe: true,
    });
    const plyDirGeo = (function () {
      const geo = new THREE.BufferGeometry();
      geo.setAttribute(
        "position",
        new THREE.Float32BufferAttribute(
          [0, -9, 3, 0, -9, -3, 0, -9, -3, -2, -9, -1, 0, -9, -3, 2, -9, -1],
          3,
        ),
      );
      return geo;
    })();
    const plyDirMat = new THREE.LineBasicMaterial({ color: 0x00ff66 });
    (Rr.players || Rr.player || []).forEach(function (p, pi) {
      if (S.markVis["player:" + pi] === false) return;
      const mk = new THREE.Mesh(plyGeo, plyMat);
      mk.position.set(p.pos[0], p.pos[1] + 10, p.pos[2]);
      try {
        const Rq = g.RDXScene.ninjaMat(0, p.ay | 0, 0, false);
        const Mq = new THREE.Matrix4();
        Mq.set(
          Rq[0][0],
          Rq[0][1],
          Rq[0][2],
          0,
          Rq[1][0],
          Rq[1][1],
          Rq[1][2],
          0,
          Rq[2][0],
          Rq[2][1],
          Rq[2][2],
          0,
          0,
          0,
          0,
          1,
        );
        mk.quaternion.setFromRotationMatrix(Mq);
      } catch {
        /* ignore */
      }
      mk.add(new THREE.LineSegments(plyDirGeo, plyDirMat));
      mk.userData.playermark = true;
      mk.userData.pick = { kind: "player", a: pi };
      root.add(mk);
    });
  }
  function buildZone(root, Rr) {
    const S = g.AppState;
    [
      ["boundary", 0xff8080],
      ["aot", 0xffff00],
      ["trigger", 0xff0000],
      ["route", 0x00ffff],
    ].forEach(function (pair) {
      const k = pair[0];
      if (!S.opts[k]) return;
      (Rr[k] || []).forEach(function (b, zi) {
        if (S.markVis[k + ":" + zi] === false) return;
        const geo = new THREE.BoxGeometry(
          Math.max(1, Math.abs(b.hi[0] - b.lo[0])),
          Math.max(1, Math.abs(b.hi[1] - b.lo[1])),
          Math.max(1, Math.abs(b.hi[2] - b.lo[2])),
        );
        const wmesh = new THREE.Mesh(
          geo,
          new THREE.MeshBasicMaterial({ color: pair[1], wireframe: true }),
        );
        wmesh.position.set(
          (b.lo[0] + b.hi[0]) / 2,
          (b.lo[1] + b.hi[1]) / 2,
          (b.lo[2] + b.hi[2]) / 2,
        );
        wmesh.userData.zonemark = k;
        wmesh.userData.pick = { kind: k, a: zi };
        root.add(wmesh);
      });
    });
    buildRouteLinks(root, Rr);
  }
  let routeLinkMat = null;
  function routeCenter(b) {
    return [
      (b.lo[0] + b.hi[0]) / 2,
      (b.lo[1] + b.hi[1]) / 2,
      (b.lo[2] + b.hi[2]) / 2,
    ];
  }
  function buildRouteLinks(root, Rr) {
    const S = g.AppState;
    if (!S.opts.route || !S.opts.routeLinks) return;
    const routes = Rr.route || [],
      n = routes.length,
      tab = Rr.routeTable || [];
    if (!n || tab.length !== n * n) return;
    const pts = [];
    for (let i = 0; i < n; i++) {
      if (S.markVis["route:" + i] === false) continue;
      const a = routes[i];
      if (!a || !a.lo || !a.hi) continue;
      const ca = routeCenter(a),
        seen = {};
      for (let j = 0; j < n; j++) {
        if (j === i) continue;
        const via = tab[n * j + i];
        if (via == null || via >= n || via === i || seen[via]) continue;
        seen[via] = 1;
        if (S.markVis["route:" + via] === false) continue;
        const b = routes[via];
        if (!b || !b.lo || !b.hi) continue;
        const cb = routeCenter(b);
        pts.push(ca[0], ca[1], ca[2], cb[0], cb[1], cb[2]);
      }
    }
    if (!pts.length) return;
    if (!routeLinkMat) {
      routeLinkMat = new THREE.LineBasicMaterial({ color: 0xff00ff });
    }
    const geo = new THREE.BufferGeometry();
    geo.setAttribute("position", new THREE.Float32BufferAttribute(pts, 3));
    const lines = new THREE.LineSegments(geo, routeLinkMat);
    lines.userData.routelink = true;
    root.add(lines);
  }
  g.AppMarkerBuilder = {
    buildCamera: buildCamera,
    buildPlayer: buildPlayer,
    buildZone: buildZone,
  };
})(window);
