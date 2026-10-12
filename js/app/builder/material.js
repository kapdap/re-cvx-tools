(function (g) {
  "use strict";
  function fmtCol(c) {
    if (!c) return "n";
    return c[0].toFixed(3) + "," + c[1].toFixed(3) + "," + c[2].toFixed(3);
  }
  function matKey(ti, tid, blend, alpha, state) {
    const s = state || {};
    return (
      (ti == null ? "flat" : ti + ":" + tid) +
      ":" +
      (blend ? "t" : "o") +
      alpha +
      ":" +
      fmtCol(s.diffuse) +
      ":" +
      (s.colorCalc != null ? s.colorCalc : "n") +
      ":" +
      (s.side || "double") +
      ":" +
      (s.matAttr != null ? s.matAttr : "n")
    );
  }
  function makeMaterial(Rr, texTable, tid, blend, alpha, state) {
    const S = g.AppState;
    const st = state || {};
    let map = null,
      kind = "OPAQUE";
    if (S.opts.textured && texTable != null) {
      const tinfo = g.RDXScene.getTexture(Rr, texTable, tid);
      if (tinfo) {
        map = tinfo.tex;
        kind = tinfo.kind || "OPAQUE";
      }
    }
    const isTransparent = blend && (kind !== "OPAQUE" || alpha < 0.999);
    const isCutout = isTransparent && kind === "CUTOUT" && alpha >= 0.999;
    let color = map ? 0xffffff : 0x999999;
    if (st.diffuse) {
      try {
        color = new THREE.Color(st.diffuse[0], st.diffuse[1], st.diffuse[2]);
      } catch {
        color = map ? 0xffffff : 0x999999;
      }
    }
    const emissive = 0x000000;
    let blending = THREE.NormalBlending;
    if (
      blend &&
      st.matAttr != null &&
      g.RDXNJ &&
      g.RDXNJ.materialBlendIndices
    ) {
      const bi = g.RDXNJ.materialBlendIndices(st.matAttr);
      if (st.matAttr === 0x25 || (bi.srcIndex === 4 && bi.dstIndex === 5)) {
        blending = THREE.NormalBlending;
      }
    }
    return new THREE.MeshLambertMaterial({
      map: map || null,
      color: color,
      emissive: emissive,
      transparent: isTransparent && !isCutout,
      opacity: isTransparent && !isCutout ? alpha : 1,
      alphaTest: isCutout ? 0.5 : 0,
      side: st.side === "front" ? THREE.FrontSide : THREE.DoubleSide,
      blending: blending,
      wireframe: S.opts.wireframe,
    });
  }
  g.AppMaterial = { key: matKey, make: makeMaterial };
})(window);
