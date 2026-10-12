(function (g) {
  "use strict";
  const KIND_PREFIX = {
    boundary: "BOUNDARY",
    aot: "AOT",
    trigger: "TRIGGER",
    route: "ROUTE",
  };
  function titleFor(kind, i) {
    const p = KIND_PREFIX[kind] || String(kind).toUpperCase();
    return p + "_" + ("00" + i).slice(-3);
  }
  function openZoneDetail(kind, i, toggle) {
    const S = g.AppState;
    i = +i;
    if (isNaN(i)) return;
    if (
      kind !== "boundary" &&
      kind !== "aot" &&
      kind !== "trigger" &&
      kind !== "route"
    ) {
      return;
    }
    const Rr = S.R();
    if (!Rr) return;
    const list = Rr[kind] || [];
    if (!list[i]) return;
    const pid = g.AppDetail.xdetailId(kind, i);
    const fileName = S.active;
    g.AppDetail.openDetailPanel(
      pid,
      titleFor(kind, i),
      fileName,
      function (body, fn) {
        renderZoneDetail(body, fn, kind, i);
      },
      toggle,
    );
  }
  function renderZoneDetail(body, fileName, kind, i) {
    const F = g.AppDetailFormat;
    const S = g.AppState;
    const Rr = S.files[fileName];
    if (!Rr) {
      body.textContent = "File closed";
      return;
    }
    F.backgroundNote(body, fileName, S.active);
    const list = Rr[kind] || [];
    const b = list[i];
    if (!b) {
      F.notFound(body, titleFor(kind, i));
      return;
    }
    const top = F.top(body);
    F.kv(top, "Zone", titleFor(kind, i));
    F.kv(top, "Kind", kind + " #" + i);
    try {
      let zoneName = null;
      if (g.RDXLookups && b.type !== undefined) {
        if (kind === "aot") zoneName = g.RDXLookups.aotTypeName(b.type);
        else if (kind === "boundary") {
          zoneName = g.RDXLookups.boundaryTypeName(b.type);
        } else if (kind === "trigger") {
          zoneName = g.RDXLookups.triggerTypeName(b.type);
        }
      }
      if (kind === "aot" || kind === "boundary" || kind === "trigger") {
        F.kv(top, "Name", zoneName != null ? zoneName : "Unknown");
      }
    } catch {
      /* ignore */
    }
    F.kv(top, "Flag", F.fmtHex(b.flg));
    F.kv(top, "Type", F.fmtHex(b.type));
    if (b.id !== undefined) F.kv(top, "Id", String(b.id));
    if (b.flrNo !== undefined) F.kv(top, "Floor", String(b.flrNo));
    if (b.attr !== undefined) F.kv(top, "Attr", F.fmtHex(b.attr));
    if (b.pos) F.kv(top, "Pos", F.fmtFloat(b.pos));
    if (b.size) F.kv(top, "Size", F.fmtFloat(b.size));
    if (b.prm) F.kv(top, "Prm", "[" + b.prm.join(", ") + "]");
    if (b.lo) F.kv(top, "Min", F.fmtFloat(b.lo));
    if (b.hi) F.kv(top, "Max", F.fmtFloat(b.hi));
    try {
      const cx = (b.lo[0] + b.hi[0]) / 2,
        cy = (b.lo[1] + b.hi[1]) / 2,
        cz = (b.lo[2] + b.hi[2]) / 2;
      F.kv(top, "Center", F.fmtFloat([cx, cy, cz]));
      const ex = Math.abs(b.hi[0] - b.lo[0]),
        ey = Math.abs(b.hi[1] - b.lo[1]),
        ez = Math.abs(b.hi[2] - b.lo[2]);
      F.kv(top, "Extents", F.fmtFloat([ex, ey, ez]));
      F.kv(top, "Volume", String(Math.round(ex * ey * ez * 10) / 10));
    } catch {
      /* ignore */
    }
  }
  g.AppZoneDetail = {
    openZoneDetail: openZoneDetail,
  };
})(window);
