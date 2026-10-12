(function (g) {
  "use strict";
  function titleFor(i) {
    return "LGT_" + ("00" + i).slice(-3);
  }
  function openLightDetail(i, toggle) {
    const S = g.AppState;
    i = +i;
    if (isNaN(i)) return;
    const Rr = S.R();
    if (!Rr || !Rr.lights || !Rr.lights[i]) return;
    const pid = g.AppDetail.xdetailId("light", i);
    const fileName = S.active;
    g.AppDetail.openDetailPanel(
      pid,
      titleFor(i),
      fileName,
      function (body, fn) {
        renderLightDetail(body, fn, i);
      },
      toggle,
    );
  }
  function r1(v) {
    return String(Math.round(v * 100) / 100);
  }
  function renderLightDetail(body, fileName, i) {
    const F = g.AppDetailFormat;
    const S = g.AppState;
    const Rr = S.files[fileName];
    if (!Rr) {
      body.textContent = "File closed";
      return;
    }
    F.backgroundNote(body, fileName, S.active);
    const L = Rr.lights && Rr.lights[i];
    if (!L) {
      F.notFound(body, titleFor(i));
      return;
    }
    const top = F.top(body);
    F.kv(top, "Light", titleFor(i));
    F.kv(top, "Index", "#" + i);
    F.kv(top, "Source", (L.lsrc === 2 ? "SUN" : "POINT") + " (" + L.lsrc + ")");
    F.kv(top, "Flag", F.fmtHex(L.flg));
    F.kv(top, "Type", F.fmtHex(L.type));
    F.kv(top, "Aspd", String(L.aspd));
    F.kv(top, "Lkflg", String(L.lkflg));
    F.kv(top, "Lkno", String(L.lkno));
    F.kv(top, "Lkono", String(L.lkono));
    F.kv(top, "Lsrc", String(L.lsrc));
    F.kv(top, "Pos", F.fmtFloat([L.px, L.py, L.pz]));
    F.kv(top, "Target", F.fmtFloat([L.lx, L.ly, L.lz]));
    F.kv(top, "Dir", F.fmtFloat([L.vx, L.vy, L.vz]));
    F.kv(top, "Spc/Dif/Amb", [L.spc, L.dif, L.amb].map(r1).join(" / "));
    F.kv(top, "Color", F.fmtFloat([L.r, L.g, L.b]));
    F.kv(top, "Near/Far", r1(L.nr) + " / " + r1(L.fr));
    F.kv(top, "In/OutAng", L.iang + " / " + L.oang);
    F.kv(top, "Ang", F.fmtAng([L.ax, L.ay, L.az]));
    const vis = S.lightVis[i] !== false;
    F.kv(top, "Visible", vis ? "visible" : "hidden");
    if (L.params) F.collapsedArray(body, "Params", L.params, r1);
    if (L.iparams) F.collapsedArray(body, "Iparams", L.iparams, String);
  }
  g.AppLightDetail = {
    openLightDetail: openLightDetail,
  };
})(window);
