(function (g) {
  "use strict";
  function titleFor(ci, bi) {
    return "CAM_" + ("00" + ci).slice(-3) + "_" + bi;
  }
  function openCameraDetail(ci, bi, toggle) {
    const S = g.AppState;
    ci = +ci;
    bi = +bi;
    if (isNaN(ci) || isNaN(bi)) return;
    const Rr = S.R();
    if (!Rr || !Rr.cameras || !Rr.cameras[ci] || !Rr.cameras[ci].blocks[bi]) {
      return;
    }
    const pid = g.AppDetail.xdetailId("camera", ci + "-" + bi);
    const fileName = S.active;
    g.AppDetail.openDetailPanel(
      pid,
      titleFor(ci, bi),
      fileName,
      function (body, fn) {
        renderCameraDetail(body, fn, ci, bi);
      },
      toggle,
    );
  }
  function r1(v) {
    return String(Math.round(v * 100) / 100);
  }
  function renderCameraDetail(body, fileName, ci, bi) {
    const F = g.AppDetailFormat;
    const S = g.AppState;
    const Rr = S.files[fileName];
    if (!Rr) {
      body.textContent = "File closed";
      return;
    }
    F.backgroundNote(body, fileName, S.active);
    const cam = Rr.cameras && Rr.cameras[ci];
    if (!cam || !cam.blocks || !cam.blocks[bi]) {
      F.notFound(body, titleFor(ci, bi));
      return;
    }
    const top = F.top(body);
    F.kv(top, "Camera", titleFor(ci, bi));
    F.kv(
      top,
      "Index",
      "cam #" + ci + " block " + bi + " / " + cam.blocks.length,
    );
    if (cam.flg !== undefined) F.kv(top, "Flag", F.fmtHex(cam.flg));
    if (cam.type !== undefined) F.kv(top, "Type", String(cam.type));
    if (cam.flrNo !== undefined) F.kv(top, "Floor", String(cam.flrNo));
    if (cam.ctabN !== undefined) F.kv(top, "CTabN", String(cam.ctabN));
    if (cam.cuttp) F.kv(top, "CutPtr", "0x" + (cam.cuttp >>> 0).toString(16));
    if (cam.cpos) F.kv(top, "CPos", F.fmtFloat(cam.cpos));
    if (cam.csize) F.kv(top, "CSize", F.fmtFloat(cam.csize));
    F.section(body, "Cut");
    const cut = (Rr.cuts || [])[ci];
    if (cut) {
      const ct = F.top(body);
      F.kv(ct, "Attr", F.fmtHex(cut.attr));
      F.kv(ct, "Floor", String(cut.flrNo));
      F.kv(ct, "AtrTp", String(cut.atrTp));
      F.kv(ct, "H", r1(cut.h));
      F.kv(ct, "MinXZ", "[" + r1(cut.minx) + ", " + r1(cut.minz) + "]");
      F.kv(ct, "MaxXZ", "[" + r1(cut.maxx) + ", " + r1(cut.maxz) + "]");
      F.kv(
        ct,
        "CenterXZ",
        "[" +
          r1((cut.minx + cut.maxx) / 2) +
          ", " +
          r1((cut.minz + cut.maxz) / 2) +
          "]",
      );
    } else {
      const em = document.createElement("div");
      em.className = "muted";
      em.textContent = "No cut data";
      body.appendChild(em);
    }
    cam.blocks.forEach(function (b, j) {
      const tag = j === bi ? ">> " : "";
      const bt = F.collapsible(body, tag + "Block " + j, j === bi);
      if (b.flg !== undefined) F.kv(bt, "Flag", F.fmtHex(b.flg));
      if (b.lgtclip !== undefined || b.spd !== undefined) {
        F.kv(bt, "LgtClip/Spd", b.lgtclip + " / " + b.spd);
      }
      if (b.pos) F.kv(bt, "Pos", F.fmtFloat(b.pos));
      if (b.ang) F.kv(bt, "Ang", F.fmtAng(b.ang));
      if (b.ln !== undefined) F.kv(bt, "Ln", r1(b.ln));
      if (b.size) F.kv(bt, "Size", F.fmtFloat(b.size));
      if (b.y) F.kv(bt, "Y0..Y3", b.y.map(r1).join(" / "));
      if (b.amSpd !== undefined) F.kv(bt, "AmSpd", r1(b.amSpd));
      if (b.lax !== undefined || b.lay !== undefined) {
        F.kv(bt, "LookAt", b.lax + " / " + b.lay);
      }
      if (b.laz) F.kv(bt, "Laz", "[" + b.laz.join(", ") + "]");
      if (b.aaSpd !== undefined) {
        F.kv(bt, "AaSpd/Fil", b.aaSpd + " / " + b.filNo + " / " + b.filRt);
      }
      if (b.pers !== undefined) F.kv(bt, "Pers", String(b.pers));
      if (b.fogCol !== undefined) {
        F.kv(
          bt,
          "Fog",
          "0x" +
            (b.fogCol >>> 0).toString(16) +
            " " +
            r1(b.fogNr) +
            " / " +
            r1(b.fogFr),
        );
      }
      if (b.hidobj) F.collapsedArray(bt, "HidObj", b.hidobj, hex1, true);
      if (b.hidlgt) F.collapsedArray(bt, "HidLgt", b.hidlgt, hex1, true);
    });
  }
  function hex1(v) {
    return "0x" + (v >>> 0).toString(16);
  }
  g.AppCameraDetail = {
    openCameraDetail: openCameraDetail,
  };
})(window);
