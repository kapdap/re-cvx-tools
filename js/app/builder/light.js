(function (g) {
  "use strict";
  function quatFromAng(ang) {
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
  function gameLightRGB(L) {
    let r = 0,
      gg = 0,
      b = 0;
    if (L) {
      if (L.r != null && L.g != null && L.b != null) {
        r = +L.r;
        gg = +L.g;
        b = +L.b;
      } else if (L.color) {
        r = +L.color[0] || 0;
        gg = +L.color[1] || 0;
        b = +L.color[2] || 0;
      } else if (L.params) {
        r = +L.params[15] || 0;
        gg = +L.params[16] || 0;
        b = +L.params[17] || 0;
      }
    }
    if (!(r > 0 || gg > 0 || b > 0)) {
      if (r < 0) r = 0;
      if (gg < 0) gg = 0;
      if (b < 0) b = 0;
    }
    return [r, gg, b];
  }
  function gameLightIntensity(L, rgb) {
    if (!L) return 0;
    if (L.flg != null && !(L.flg & 1)) return 0;
    const m = Math.max(rgb[0], rgb[1], rgb[2]);
    return m > 0 && isFinite(m) ? m : 0;
  }
  function gameLightDir(L) {
    const vx = +(L.vx || (L.dir && L.dir[0]) || 0),
      vy = +(L.vy || (L.dir && L.dir[1]) || 0),
      vz = +(L.vz || (L.dir && L.dir[2]) || 0);
    if (vx * vx + vy * vy + vz * vz > 1e-12) return [vx, vy, vz];
    try {
      const R = g.RDXScene.ninjaMat(L.ax | 0, L.ay | 0, L.az | 0, false);
      return [-R[0][2], -R[1][2], -R[2][2]];
    } catch {
      return [0, -1, 0];
    }
  }
  function gameLightColorObj(rgb, intensity) {
    const c = new THREE.Color(0xffffff);
    if (intensity > 0) {
      c.setRGB(rgb[0] / intensity, rgb[1] / intensity, rgb[2] / intensity);
    }
    return c;
  }
  function lgtSphereGeo(len) {
    const r = len * 0.55,
      seg = 32,
      pts = [];
    let i, a0, a1, c0, s0, c1, s1;
    for (i = 0; i < seg; i++) {
      a0 = (i / seg) * Math.PI * 2;
      a1 = ((i + 1) / seg) * Math.PI * 2;
      c0 = Math.cos(a0) * r;
      s0 = Math.sin(a0) * r;
      c1 = Math.cos(a1) * r;
      s1 = Math.sin(a1) * r;
      pts.push(c0, s0, 0, c1, s1, 0);
      pts.push(c0, 0, s0, c1, 0, s1);
      pts.push(0, c0, s0, 0, c1, s1);
    }
    const geo = new THREE.BufferGeometry();
    geo.setAttribute("position", new THREE.Float32BufferAttribute(pts, 3));
    return geo;
  }
  function build(root, Rr) {
    const S = g.AppState;
    if (!Rr.lights || !Rr.lights.length) return;
    const lgtGeo = lgtSphereGeo(1);
    Rr.lights.forEach(function (L, i) {
      if (!S.opts.lights && !S.opts.gameLights) return;
      const rgb = gameLightRGB(L),
        intensity = gameLightIntensity(L, rgb);
      const colObj = gameLightColorObj(rgb, intensity);
      const holder = new THREE.Group();
      holder.name = "LGT_" + i;
      holder.userData.lgtmark = true;
      if (S.opts.lights) {
        const marker = new THREE.Group();
        marker.position.set(L.pos[0], L.pos[1], L.pos[2]);
        try {
          marker.quaternion.copy(quatFromAng(L.ang || [0, 0, 0]));
        } catch {
          /* ignore */
        }
        const dispC = colObj.clone();
        try {
          if (intensity > 0) {
            const mx = Math.max(rgb[0], rgb[1], rgb[2], 1e-6);
            dispC.setRGB(
              Math.min(1, Math.max(0, rgb[0] / Math.max(1, mx))),
              Math.min(1, Math.max(0, rgb[1] / Math.max(1, mx))),
              Math.min(1, Math.max(0, rgb[2] / Math.max(1, mx))),
            );
            if (mx <= 1) dispC.copy(colObj);
          } else dispC.setHex(0x334455);
        } catch {
          /* ignore */
        }
        const wire = new THREE.LineSegments(
          lgtGeo,
          new THREE.LineBasicMaterial({ color: dispC }),
        );
        wire.userData.lgtmark = true;
        marker.add(wire);
        marker.userData.lgtmark = true;
        marker.userData.pick = { kind: "light", a: i };
        holder.add(marker);
      }
      let real = null;
      if (S.opts.gameLights) {
        if (L.lsrc === 2) {
          real = new THREE.DirectionalLight(colObj, intensity);
          let dir = gameLightDir(L),
            dl = Math.sqrt(dir[0] * dir[0] + dir[1] * dir[1] + dir[2] * dir[2]);
          if (!(dl > 1e-9)) {
            dir = [0, -1, 0];
            dl = 1;
          }
          real.position.set(
            (-dir[0] / dl) * 10000,
            (-dir[1] / dl) * 10000,
            (-dir[2] / dl) * 10000,
          );
          real.target.position.set(0, 0, 0);
          holder.add(real);
          holder.add(real.target);
        } else {
          const fr = +(L.fr != null ? L.fr : L.params ? L.params[19] : 0) || 0;
          real = new THREE.PointLight(colObj, intensity, fr > 0 ? fr : 0, 1);
          real.position.set(L.pos[0], L.pos[1], L.pos[2]);
          holder.add(real);
        }
      }
      holder.visible = S.lightVis[i] !== false;
      root.add(holder);
      S.lightEntries.push({ obj: holder, light: real, i: i });
    });
  }
  g.AppLightBuilder = { build: build, quatFromAng: quatFromAng };
})(window);
