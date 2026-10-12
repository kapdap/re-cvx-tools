(function (g) {
  "use strict";
  function gotoTable(ti) {
    const S = g.AppState;
    const meshes = S.modelEntries
      .filter(function (e) {
        return e.ti === ti;
      })
      .map(function (e) {
        return e.mesh;
      });
    if (!meshes.length) return;
    const box = new THREE.Box3();
    meshes.forEach(function (m) {
      box.expandByObject(m);
    });
    if (box.isEmpty()) return;
    const dir = S.camera.position.clone().sub(S.controls.target);
    if (dir.lengthSq() < 1) dir.set(1, 1, 1);
    dir.normalize();
    g.AppViewport.frameBox(box, dir);
  }
  function gotoNested(ti, bi, ni) {
    const S = g.AppState;
    const nk = g.AppDetail.nestedKey(ti, bi, ni);
    const meshes = S.modelEntries
      .filter(function (e) {
        return g.AppDetail.nestedKey(e.ti, e.bi, e.ni) === nk;
      })
      .map(function (e) {
        return e.mesh;
      });
    if (!meshes.length) return;
    const box = new THREE.Box3();
    meshes.forEach(function (m) {
      try {
        box.expandByObject(m);
      } catch {
        /* ignore */
      }
    });
    if (box.isEmpty()) return;
    const dir = S.camera.position.clone().sub(S.controls.target);
    if (dir.lengthSq() < 1) dir.set(1, 1, 1);
    dir.normalize();
    g.AppViewport.frameBox(box, dir);
  }
  function gotoCamera(ci, bi) {
    const S = g.AppState;
    const Rr = S.R();
    if (!Rr || !Rr.cameras[ci]) return;
    const b = Rr.cameras[ci].blocks[bi || 0];
    if (!b) return;
    S.camera.position.set(b.pos[0], b.pos[1], b.pos[2]);
    let dir = new THREE.Vector3(0, 0, -1);
    try {
      const Rm = g.RDXScene.ninjaMat(
        b.ang[0] | 0,
        b.ang[1] | 0,
        b.ang[2] | 0,
        false,
      );
      const Mm = new THREE.Matrix4();
      Mm.set(
        Rm[0][0],
        Rm[0][1],
        Rm[0][2],
        0,
        Rm[1][0],
        Rm[1][1],
        Rm[1][2],
        0,
        Rm[2][0],
        Rm[2][1],
        Rm[2][2],
        0,
        0,
        0,
        0,
        1,
      );
      const q = new THREE.Quaternion().setFromRotationMatrix(Mm);
      S.camera.quaternion.copy(q);
      dir.set(0, 0, -1).applyQuaternion(q);
    } catch {
      const box = new THREE.Box3().setFromObject(S.root);
      const center = box.isEmpty()
        ? new THREE.Vector3()
        : box.getCenter(new THREE.Vector3());
      dir = center.clone().sub(S.camera.position);
      if (dir.lengthSq() < 1) dir.set(0, 0, -1);
      dir.normalize();
    }
    if (dir.lengthSq() < 1) dir.set(0, 0, -1);
    dir.normalize();
    S.controls.target.copy(
      S.camera.position.clone().add(dir.multiplyScalar(90)),
    );
    if (S.viewMode === "fps") S.fps.enable();
  }
  function gotoLight(i) {
    const S = g.AppState;
    const Rr = S.R();
    if (!Rr || !Rr.lights[i]) return;
    const p = Rr.lights[i].pos;
    if (!p) return;
    const box = new THREE.Box3(
      new THREE.Vector3(p[0] - 4, p[1] - 4, p[2] - 4),
      new THREE.Vector3(p[0] + 4, p[1] + 4, p[2] + 4),
    );
    const dir = S.camera.position.clone().sub(S.controls.target);
    if (dir.lengthSq() < 1) dir.set(1, 1, 1);
    dir.normalize();
    g.AppViewport.frameBox(box, dir);
  }
  function gotoPlayer(i) {
    const S = g.AppState;
    const Rr = S.R();
    if (!Rr) return;
    const list = Rr.players || Rr.player || [];
    if (!list[i] || !list[i].pos) return;
    const p = list[i].pos;
    const box = new THREE.Box3(
      new THREE.Vector3(p[0] - 4, p[1], p[2] - 4),
      new THREE.Vector3(p[0] + 4, p[1] + 20, p[2] + 4),
    );
    const dir = S.camera.position.clone().sub(S.controls.target);
    if (dir.lengthSq() < 1) dir.set(1, 1, 1);
    dir.normalize();
    g.AppViewport.frameBox(box, dir);
  }
  function gotoZone(kind, i) {
    const S = g.AppState;
    const Rr = S.R();
    if (!Rr) return;
    const list = Rr[kind] || [];
    const b = list[i];
    if (!b || !b.lo || !b.hi) return;
    const box = new THREE.Box3(
      new THREE.Vector3(b.lo[0], b.lo[1], b.lo[2]),
      new THREE.Vector3(b.hi[0], b.hi[1], b.hi[2]),
    );
    if (box.isEmpty()) return;
    const dir = S.camera.position.clone().sub(S.controls.target);
    if (dir.lengthSq() < 1) dir.set(1, 1, 1);
    dir.normalize();
    g.AppViewport.frameBox(box, dir);
  }
  g.AppNavigation = {
    gotoTable: gotoTable,
    gotoNested: gotoNested,
    gotoCamera: gotoCamera,
    gotoLight: gotoLight,
    gotoPlayer: gotoPlayer,
    gotoZone: gotoZone,
  };
})(window);
