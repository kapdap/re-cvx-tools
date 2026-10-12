(function (g) {
  "use strict";
  function getCM() {
    try {
      return window.ContextMenu || null;
    } catch {
      return null;
    }
  }
  const LAYER_KEYS = [
    "room",
    "actor",
    "obj",
    "item",
    "cameras",
    "lights",
    "players",
    "boundary",
    "aot",
    "trigger",
    "route",
  ];
  function setTableVisible(ti, v) {
    const S = g.AppState;
    ti = g.AppSelection.normTi(ti);
    if (ti == null) return;
    S.tableVis[ti] = !!v;
    S.modelEntries.forEach(function (e2) {
      if (e2.ti === ti) {
        e2.mesh.visible =
          !!v &&
          S.detailVis[g.AppDetail.nestedKey(e2.ti, e2.bi, e2.ni)] !== false &&
          !!S.opts[e2.kind];
      }
    });
    g.AppHighlight.updatePickHelpers();
    g.AppDetail.refreshDetailPanels();
    try {
      g.Panels.refresh("models");
    } catch {
      /* ignore */
    }
  }
  function setLightVisible(i, v) {
    const S = g.AppState;
    i = +i;
    if (isNaN(i)) return;
    v = !!v;
    S.lightVis[i] = v;
    S.lightEntries.forEach(function (e) {
      if (e.i === i && e.obj) e.obj.visible = v;
    });
    g.AppHighlight.updatePickHelpers();
    try {
      g.AppDetail.refreshDetailPanels();
    } catch {
      /* ignore */
    }
    try {
      g.Panels.refresh("lights");
    } catch {
      /* ignore */
    }
  }
  function setMarkVisible(key, v) {
    const S = g.AppState;
    if (key == null) return;
    S.markVis[String(key)] = !!v;
    g.AppBuilder.show(S.active, false);
  }
  function setLayerVisible(key, v) {
    const S = g.AppState;
    if (!S.opts || S.opts[key] === undefined) return;
    S.opts[key] = !!v;
    g.AppBuilder.show(S.active, false);
    try {
      g.Panels.refresh("layers");
    } catch {
      /* ignore */
    }
  }
  function setOnlyLayer(key) {
    const S = g.AppState;
    LAYER_KEYS.forEach(function (k) {
      if (S.opts[k] !== undefined) S.opts[k] = k === key;
    });
    g.AppBuilder.show(S.active, false);
    try {
      g.Panels.refresh("layers");
    } catch {
      /* ignore */
    }
  }
  function setAllLayers(v) {
    const S = g.AppState;
    LAYER_KEYS.forEach(function (k) {
      if (S.opts[k] !== undefined) S.opts[k] = !!v;
    });
    g.AppBuilder.show(S.active, false);
    try {
      g.Panels.refresh("layers");
    } catch {
      /* ignore */
    }
  }
  function tableTis(Rr) {
    try {
      return Object.keys(Rr.map || {})
        .map(function (k) {
          return +k;
        })
        .filter(function (t) {
          return !isNaN(t);
        });
    } catch {
      return [];
    }
  }
  function refreshTables() {
    const S = g.AppState;
    S.modelEntries.forEach(function (e2) {
      e2.mesh.visible =
        S.tableVis[e2.ti] !== false &&
        S.detailVis[g.AppDetail.nestedKey(e2.ti, e2.bi, e2.ni)] !== false &&
        !!S.opts[e2.kind];
    });
    g.AppHighlight.updatePickHelpers();
    g.AppDetail.refreshDetailPanels();
    try {
      g.Panels.refresh("models");
    } catch {
      /* ignore */
    }
  }
  function setOnlyTable(ti) {
    const S = g.AppState;
    ti = g.AppSelection.normTi(ti);
    if (ti == null || !S.R() || !S.R().map) return;
    tableTis(S.R()).forEach(function (t) {
      S.tableVis[t] = t === ti;
    });
    refreshTables();
  }
  function setAllTables(v) {
    const S = g.AppState;
    if (!S.R() || !S.R().map) return;
    tableTis(S.R()).forEach(function (t) {
      S.tableVis[t] = !!v;
    });
    refreshTables();
  }
  function refreshLights() {
    const S = g.AppState;
    S.lightEntries.forEach(function (e) {
      if (e.obj) e.obj.visible = S.lightVis[e.i] !== false;
    });
    g.AppHighlight.updatePickHelpers();
    try {
      g.AppDetail.refreshDetailPanels();
    } catch {
      /* ignore */
    }
    try {
      g.Panels.refresh("lights");
    } catch {
      /* ignore */
    }
  }
  function setOnlyLight(i) {
    const S = g.AppState;
    i = +i;
    if (isNaN(i) || !S.R() || !S.R().lights) return;
    S.R().lights.forEach(function (L, idx) {
      S.lightVis[idx] = idx === i;
    });
    refreshLights();
  }
  function setAllLights(v) {
    const S = g.AppState;
    if (!S.R() || !S.R().lights) return;
    S.R().lights.forEach(function (L, idx) {
      S.lightVis[idx] = !!v;
    });
    refreshLights();
  }
  function cameraKeys(Rr) {
    const out = [];
    try {
      (Rr.cameras || []).forEach(function (c, ci) {
        (c.blocks || []).forEach(function (b, bi) {
          out.push("camera:" + ci + ":" + bi);
        });
      });
    } catch {
      /* ignore */
    }
    return out;
  }
  function playerKeys(Rr) {
    const out = [];
    try {
      const list = Rr.players || Rr.player || [];
      list.forEach(function (p, idx) {
        out.push("player:" + idx);
      });
    } catch {
      /* ignore */
    }
    return out;
  }
  function zoneKeys(Rr, kind) {
    const out = [];
    try {
      (Rr[kind] || []).forEach(function (b, idx) {
        out.push(kind + ":" + idx);
      });
    } catch {
      /* ignore */
    }
    return out;
  }
  function setOnlyMark(key, keys) {
    const S = g.AppState;
    if (key == null || !keys) return;
    keys.forEach(function (k) {
      S.markVis[String(k)] = String(k) === String(key);
    });
    g.AppBuilder.show(S.active, false);
  }
  function setAllMarks(keys, v) {
    const S = g.AppState;
    if (!keys) return;
    keys.forEach(function (k) {
      S.markVis[String(k)] = !!v;
    });
    g.AppBuilder.show(S.active, false);
  }
  function nestedKeys(ti) {
    const S = g.AppState,
      seen = {},
      out = [];
    try {
      S.modelEntries.forEach(function (e) {
        if (e.ti === ti) {
          const k = g.AppDetail.nestedKey(e.ti, e.bi, e.ni);
          if (!seen[k]) {
            seen[k] = 1;
            out.push(k);
          }
        }
      });
    } catch {
      /* ignore */
    }
    return out;
  }
  function refreshNested(ti) {
    const S = g.AppState;
    const tiVis = S.tableVis[ti] !== false;
    S.modelEntries.forEach(function (e) {
      if (e.ti === ti) {
        e.mesh.visible =
          S.detailVis[g.AppDetail.nestedKey(e.ti, e.bi, e.ni)] !== false &&
          tiVis &&
          !!S.opts[e.kind];
      }
    });
    g.AppHighlight.updatePickHelpers();
    g.AppDetail.refreshDetailPanels();
  }
  function setOnlyNested(ti, bi, ni) {
    const k = g.AppDetail.nestedKey(ti, bi, ni);
    nestedKeys(ti).forEach(function (kk) {
      g.AppState.detailVis[kk] = kk === k;
    });
    refreshNested(ti);
  }
  function setAllNested(ti, v) {
    nestedKeys(ti).forEach(function (kk) {
      g.AppState.detailVis[kk] = !!v;
    });
    refreshNested(ti);
  }
  function showModelsContextMenu(x, y, ti) {
    const S = g.AppState;
    ti = g.AppSelection.normTi(ti);
    if (ti == null) return;
    if (!S.R()) return;
    let CM = null;
    try {
      CM = window.ContextMenu || null;
    } catch {
      CM = null;
    }
    if (!CM) return;
    const vis = S.tableVis[ti] !== false;
    function toggleVis() {
      setTableVisible(ti, !(S.tableVis[ti] !== false));
    }
    const tis = tableTis(S.R());
    const onlySolo =
      vis &&
      tis.every(function (t) {
        return t === ti ? S.tableVis[t] !== false : S.tableVis[t] === false;
      });
    const allOn = tis.every(function (t) {
      return S.tableVis[t] !== false;
    });
    CM.show(x, y, [
      {
        label: "Center camera",
        onclick: function () {
          g.AppSelection.setSelected(ti);
          g.AppNavigation.gotoTable(ti);
        },
      },
      { label: vis ? "Hide model" : "Show model", onclick: toggleVis },
      {
        label: "Show only this model",
        disabled: !!onlySolo,
        onclick: function () {
          setOnlyTable(ti);
        },
      },
      {
        label: "Show all models",
        disabled: !!allOn,
        onclick: function () {
          setAllTables(true);
        },
      },
      {
        label: "Model details",
        onclick: function () {
          g.AppDetail.openModelDetail(ti);
        },
      },
      { sep: true },
      {
        label: "Export",
        submenu: [
          {
            label: "glTF 2.0 (.glb)",
            title:
              "Export MDL_" +
              g.AppDetail.pad3(ti) +
              " (selected models preselected)",
            onclick: function () {
              g.AppExport.openExportDialog({ selectedOnly: true });
            },
          },
        ],
      },
    ]);
  }
  function showFilesContextMenu(x, y, name) {
    const S = g.AppState;
    if (!name || S.order.indexOf(name) < 0) return;
    const CM = getCM();
    if (!CM) return;
    const isActive = name === S.active;
    CM.show(x, y, [
      {
        label: "Switch to file",
        disabled: isActive,
        onclick: function () {
          g.AppFile.setActive(name);
        },
      },
      {
        label: "Remove file",
        onclick: function () {
          g.AppFile.removeFile(name);
        },
      },
      {
        label: "Close other files",
        disabled: S.order.length < 2,
        onclick: function () {
          S.order.slice().forEach(function (n) {
            if (n !== name) g.AppFile.removeFile(n);
          });
          if (S.active !== name) g.AppFile.setActive(name);
        },
      },
      { sep: true },
      {
        label: "Export...",
        title: isActive
          ? "Open export dialog for the active file"
          : "Switch to this file and open export dialog",
        onclick: function () {
          if (S.active !== name) g.AppFile.setActive(name);
          g.AppExport.openExportDialog(null);
        },
      },
    ]);
  }
  function showLayersContextMenu(x, y, key) {
    const S = g.AppState;
    if (!S.opts || S.opts[key] === undefined) return;
    const CM = getCM();
    if (!CM) return;
    const vis = !!S.opts[key];
    let onlySolo = vis;
    try {
      onlySolo =
        vis &&
        LAYER_KEYS.every(function (k) {
          if (k === key) return !!S.opts[k];
          if (S.opts[k] === undefined) return true;
          return !S.opts[k];
        });
    } catch {
      /* ignore */
    }
    let allOn = true,
      allOff = true;
    try {
      LAYER_KEYS.forEach(function (k) {
        if (S.opts[k] === undefined) return;
        if (S.opts[k]) allOff = false;
        else allOn = false;
      });
    } catch {
      /* ignore */
    }
    CM.show(x, y, [
      {
        label: "Toggle selected layer",
        onclick: function () {
          setLayerVisible(key, !vis);
        },
      },
      {
        label: "Show only this layer",
        disabled: !!onlySolo,
        onclick: function () {
          setOnlyLayer(key);
        },
      },
      { sep: true },
      {
        label: "Show all layers",
        disabled: !!allOn,
        onclick: function () {
          setAllLayers(true);
        },
      },
      {
        label: "Hide all layers",
        disabled: !!allOff,
        onclick: function () {
          setAllLayers(false);
        },
      },
    ]);
  }
  function showCamerasContextMenu(x, y, ci, bi, key) {
    const S = g.AppState;
    if (!S.R()) return;
    const CM = getCM();
    if (!CM) return;
    ci = +ci;
    bi = +bi;
    if (isNaN(ci) || isNaN(bi)) return;
    const Rr = S.R();
    if (!Rr.cameras || !Rr.cameras[ci] || !Rr.cameras[ci].blocks[bi]) return;
    const vis = S.markVis[key] !== false;
    const keys = cameraKeys(Rr);
    const onlySolo =
      vis &&
      keys.every(function (k2) {
        return k2 === key ? S.markVis[k2] !== false : S.markVis[k2] === false;
      });
    const allOn = keys.every(function (k2) {
      return S.markVis[k2] !== false;
    });
    CM.show(x, y, [
      {
        label: "Jump camera here",
        onclick: function () {
          g.AppSelection.setSelectedMark(key);
          g.AppViewport.setViewMode(S.viewMode);
          g.AppNavigation.gotoCamera(ci, bi);
        },
      },
      {
        label: vis ? "Hide camera" : "Show camera",
        onclick: function () {
          setMarkVisible(key, !vis);
        },
      },
      {
        label: "Show only this camera",
        disabled: !!onlySolo,
        onclick: function () {
          setOnlyMark(key, keys);
        },
      },
      {
        label: "Show all cameras",
        disabled: !!allOn,
        onclick: function () {
          setAllMarks(keys, true);
        },
      },
      {
        label: "Camera details",
        onclick: function () {
          g.AppCameraDetail.openCameraDetail(ci, bi);
        },
      },
    ]);
  }
  function showLightsContextMenu(x, y, i, key) {
    const S = g.AppState;
    if (!S.R()) return;
    const CM = getCM();
    if (!CM) return;
    i = +i;
    if (isNaN(i)) return;
    const Rr = S.R();
    if (!Rr.lights || !Rr.lights[i]) return;
    const vis = S.lightVis[i] !== false;
    const idxs = Rr.lights.map(function (L, j) {
      return j;
    });
    const onlySolo =
      vis &&
      idxs.every(function (j) {
        return j === i ? S.lightVis[j] !== false : S.lightVis[j] === false;
      });
    const allOn = idxs.every(function (j) {
      return S.lightVis[j] !== false;
    });
    CM.show(x, y, [
      {
        label: "Center camera",
        onclick: function () {
          g.AppSelection.setSelectedMark(key);
          g.AppNavigation.gotoLight(i);
        },
      },
      {
        label: vis ? "Hide light" : "Show light",
        onclick: function () {
          setLightVisible(i, !vis);
        },
      },
      {
        label: "Show only this light",
        disabled: !!onlySolo,
        onclick: function () {
          setOnlyLight(i);
        },
      },
      {
        label: "Show all lights",
        disabled: !!allOn,
        onclick: function () {
          setAllLights(true);
        },
      },
      {
        label: "Light details",
        onclick: function () {
          g.AppLightDetail.openLightDetail(i);
        },
      },
    ]);
  }
  function showPlayersContextMenu(x, y, i, key) {
    const S = g.AppState;
    if (!S.R()) return;
    const CM = getCM();
    if (!CM) return;
    i = +i;
    if (isNaN(i)) return;
    const Rr = S.R();
    const list = Rr.players || Rr.player || [];
    if (!list[i]) return;
    const vis = S.markVis[key] !== false;
    const keys = playerKeys(Rr);
    const onlySolo =
      vis &&
      keys.every(function (k2) {
        return k2 === key ? S.markVis[k2] !== false : S.markVis[k2] === false;
      });
    const allOn = keys.every(function (k2) {
      return S.markVis[k2] !== false;
    });
    CM.show(x, y, [
      {
        label: "Center camera",
        onclick: function () {
          g.AppSelection.setSelectedMark(key);
          g.AppNavigation.gotoPlayer(i);
        },
      },
      {
        label: vis ? "Hide player" : "Show player",
        onclick: function () {
          setMarkVisible(key, !vis);
        },
      },
      {
        label: "Show only this player",
        disabled: !!onlySolo,
        onclick: function () {
          setOnlyMark(key, keys);
        },
      },
      {
        label: "Show all players",
        disabled: !!allOn,
        onclick: function () {
          setAllMarks(keys, true);
        },
      },
      {
        label: "Player details",
        onclick: function () {
          g.AppPlayerDetail.openPlayerDetail(i);
        },
      },
    ]);
  }
  function showZoneContextMenu(x, y, kind, i, key) {
    const S = g.AppState;
    if (!S.R()) return;
    const CM = getCM();
    if (!CM) return;
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
    const list = Rr[kind] || [];
    if (!list[i]) return;
    const vis = S.markVis[key] !== false;
    const keys = zoneKeys(Rr, kind);
    const onlySolo =
      vis &&
      keys.every(function (k2) {
        return k2 === key ? S.markVis[k2] !== false : S.markVis[k2] === false;
      });
    const allOn = keys.every(function (k2) {
      return S.markVis[k2] !== false;
    });
    CM.show(x, y, [
      {
        label: "Center camera",
        onclick: function () {
          g.AppSelection.setSelectedMark(key);
          g.AppNavigation.gotoZone(kind, i);
        },
      },
      {
        label: vis ? "Hide zone" : "Show zone",
        onclick: function () {
          setMarkVisible(key, !vis);
        },
      },
      {
        label: "Show only this zone",
        disabled: !!onlySolo,
        onclick: function () {
          setOnlyMark(key, keys);
        },
      },
      {
        label: "Show all zones",
        disabled: !!allOn,
        onclick: function () {
          setAllMarks(keys, true);
        },
      },
      {
        label:
          kind === "boundary"
            ? "Boundary details"
            : kind === "aot"
              ? "AOT details"
              : kind === "trigger"
                ? "Trigger details"
                : "Route details",
        onclick: (function (k, idx) {
          return function () {
            g.AppZoneDetail.openZoneDetail(k, idx);
          };
        })(kind, i),
      },
    ]);
  }
  function showNestedContextMenu(x, y, ti, bi, ni, canToggle) {
    const S = g.AppState;
    if (!S.R()) return;
    const CM = getCM();
    if (!CM) return;
    ti = +ti;
    bi = +bi;
    ni = +ni;
    if (isNaN(ti) || isNaN(bi) || isNaN(ni)) return;
    const k = g.AppDetail.nestedKey(ti, bi, ni);
    const vis = S.detailVis[k] !== false;
    const ok = canToggle !== false;
    const keys = nestedKeys(ti);
    const onlySolo =
      vis &&
      keys.every(function (k2) {
        return k2 === k ? S.detailVis[k2] !== false : S.detailVis[k2] === false;
      });
    const allOn = keys.every(function (k2) {
      return S.detailVis[k2] !== false;
    });
    CM.show(x, y, [
      {
        label: "Center camera",
        disabled: !ok,
        onclick: function () {
          g.AppSelection.setSelected(ti);
          g.AppNavigation.gotoNested(ti, bi, ni);
        },
      },
      {
        label: vis ? "Hide nested model" : "Show nested model",
        disabled: !ok,
        onclick: function () {
          g.AppDetail.setNestedVisible(ti, bi, ni, !vis);
          g.AppDetail.refreshDetailPanels();
        },
      },
      {
        label: "Show only this nested model",
        disabled: !ok || !!onlySolo,
        onclick: function () {
          setOnlyNested(ti, bi, ni);
        },
      },
      {
        label: "Show all nested models",
        disabled: !ok || !!allOn,
        onclick: function () {
          setAllNested(ti, true);
        },
      },
    ]);
  }
  function showViewContextMenu(x, y, ti) {
    const S = g.AppState;
    if (!S.R()) return;
    let CM = null;
    try {
      CM = window.ContextMenu || null;
    } catch {
      CM = null;
    }
    if (!CM) return;
    ti = g.AppSelection.normTi(ti);
    const tis = tableTis(S.R());
    const allOn = tis.every(function (t) {
      return S.tableVis[t] !== false;
    });
    const items = [];
    if (ti != null) {
      const vis = S.tableVis[ti] !== false;
      const onlySolo =
        vis &&
        tis.every(function (t) {
          return t === ti ? S.tableVis[t] !== false : S.tableVis[t] === false;
        });
      items.push({
        label: vis ? "Hide this model" : "Show this model",
        onclick: function () {
          setTableVisible(ti, !(S.tableVis[ti] !== false));
        },
      });
      items.push({
        label: "Show only this model",
        disabled: !!onlySolo,
        onclick: function () {
          setOnlyTable(ti);
        },
      });
    }
    items.push({
      label: "Show all models",
      disabled: !!allOn,
      onclick: function () {
        setAllTables(true);
      },
    });
    if (S.selectedTis.length) items.push({ sep: true });
    if (S.selectedTis.length) {
      items.push({
        label: "Selection",
        submenu: [
          {
            label: "Export",
            submenu: [
              {
                label: "glTF 2.0 (.glb)",
                title: "Export selected models with textures",
                disabled: !S.selectedTis.length,
                onclick: function () {
                  g.AppExport.doExportGltf(true, true, null, null);
                },
              },
            ],
          },
        ],
      });
    }
    CM.show(x, y, items);
  }
  g.AppMenu = {
    setTableVisible: setTableVisible,
    setMarkVisible: setMarkVisible,
    setLayerVisible: setLayerVisible,
    showModelsContextMenu: showModelsContextMenu,
    showFilesContextMenu: showFilesContextMenu,
    showLayersContextMenu: showLayersContextMenu,
    showCamerasContextMenu: showCamerasContextMenu,
    showLightsContextMenu: showLightsContextMenu,
    showPlayersContextMenu: showPlayersContextMenu,
    showZoneContextMenu: showZoneContextMenu,
    showNestedContextMenu: showNestedContextMenu,
    showViewContextMenu: showViewContextMenu,
  };
})(window);
