(function (g) {
  "use strict";
  function openExportDialog(pre) {
    g.AppState.exportOverride = pre || null;
    g.Panels.openDialog("export");
  }
  function getExportItems(selectedOnly) {
    const S = g.AppState;
    const Rr = S.R();
    if (!Rr) return [];
    const sel = {};
    if (selectedOnly) {
      S.selectedTis.forEach(function (t) {
        sel[t] = 1;
      });
    }
    const built = g.RDXScene.buildGeometries(Rr);
    return built.items.filter(function (it) {
      if (it.hidden) return false;
      if (!S.opts[it.kind]) return false;
      if (S.tableVis[it.ti] === false) return false;
      const ni2 = it.node && it.node.index != null ? it.node.index : -1;
      if (S.detailVis[it.ti + ":" + it.bi + ":" + ni2] === false) return false;
      if (selectedOnly && !sel[it.ti]) return false;
      const m = it.model;
      if (!m || !m.faces || !m.faces.length) return false;
      return true;
    });
  }
  function getExportInfo() {
    const S = g.AppState;
    const Rr = S.R();
    let total = 0;
    if (Rr) {
      try {
        total = Rr.map ? Object.keys(Rr.map).length : 0;
      } catch {
        total = 0;
      }
    }
    return {
      active: S.active,
      hasFile: !!Rr,
      total: total,
      selected: S.selectedTis.length,
    };
  }
  function refreshExportPanel() {
    try {
      if (g.Panels.isVisible("export")) g.Panels.refresh("export");
    } catch {
      /* ignore */
    }
  }
  function baseName(n) {
    if (!n) return "models";
    let b = String(n).split("/").pop().split("\\").pop();
    const dot = b.lastIndexOf(".");
    if (dot > 0) b = b.slice(0, dot);
    return b || "models";
  }
  function doExportGltf(selOnly, withTex, statusEl, btnEl) {
    const S = g.AppState;
    function status(m) {
      if (statusEl) statusEl.textContent = m;
    }
    const Rr = S.R();
    if (!Rr) {
      status("No file loaded.");
      return;
    }
    if (selOnly && !S.selectedTis.length) {
      status(
        "No models selected. Select models first or uncheck “Selected models”.",
      );
      return;
    }
    let items;
    try {
      items = getExportItems(selOnly);
    } catch (err) {
      status("Export failed: " + err.message);
      return;
    }
    if (!items.length) {
      status("Nothing to export (all models hidden or filtered out).");
      return;
    }
    if (btnEl) btnEl.disabled = true;
    status("Building .glb…");
    setTimeout(function () {
      try {
        const out = g.RDXExportGLTF.buildGlb(Rr, items, {
          textures: withTex,
          name: baseName(S.active),
        });
        const suffix = selOnly ? "_selected" : "";
        const fname = baseName(S.active) + suffix + ".glb";
        if (!g.RDXExportGLTF.downloadGlb(out.bytes, fname)) {
          status(
            "Export built (" + out.stats.bytes + " bytes) but download failed.",
          );
        } else {
          status(
            "Exported " +
              out.stats.nodes +
              " models, " +
              out.stats.triangles +
              " tris, " +
              out.stats.images +
              " textures → " +
              fname +
              " (" +
              out.stats.bytes +
              " bytes).",
          );
        }
      } catch (err) {
        status("Export failed: " + (err && err.message ? err.message : err));
      }
      if (btnEl) btnEl.disabled = false;
      try {
        g.Panels.refresh("models");
      } catch {
        /* ignore */
      }
    }, 30);
  }
  g.AppExport = {
    openExportDialog: openExportDialog,
    getExportItems: getExportItems,
    getExportInfo: getExportInfo,
    refreshExportPanel: refreshExportPanel,
    baseName: baseName,
    doExportGltf: doExportGltf,
  };
})(window);
