(function (g) {
  "use strict";
  function titleFor(i) {
    return "PLY_" + ("00" + i).slice(-3);
  }
  function openPlayerDetail(i, toggle) {
    const S = g.AppState;
    i = +i;
    if (isNaN(i)) return;
    const Rr = S.R();
    if (!Rr) return;
    const list = Rr.players || Rr.player || [];
    if (!list[i]) return;
    const pid = g.AppDetail.xdetailId("player", i);
    const fileName = S.active;
    g.AppDetail.openDetailPanel(
      pid,
      titleFor(i),
      fileName,
      function (body, fn) {
        renderPlayerDetail(body, fn, i);
      },
      toggle,
    );
  }
  function renderPlayerDetail(body, fileName, i) {
    const F = g.AppDetailFormat;
    const S = g.AppState;
    const Rr = S.files[fileName];
    if (!Rr) {
      body.textContent = "File closed";
      return;
    }
    F.backgroundNote(body, fileName, S.active);
    const list = Rr.players || Rr.player || [];
    const p = list[i];
    if (!p) {
      F.notFound(body, titleFor(i));
      return;
    }
    const top = F.top(body);
    F.kv(top, "Player", titleFor(i));
    F.kv(top, "Index", "#" + i);
    if (p.pos) F.kv(top, "Pos", F.fmtFloat(p.pos));
    if (p.ay !== undefined) {
      F.kv(top, "Yaw", p.ay + " (~" + F.ninjaDeg(p.ay) + "°)");
    }
  }
  g.AppPlayerDetail = {
    openPlayerDetail: openPlayerDetail,
  };
})(window);
