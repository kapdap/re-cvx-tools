(function (g) {
  "use strict";
  function pad3(ti) {
    return ("00" + ti).slice(-3);
  }
  function nestedKey(ti, bi, ni) {
    return ti + ":" + bi + ":" + ni;
  }
  function texEntryName(texNames, texid) {
    const e = texNames ? texNames[texid] : null;
    if (typeof e === "string") return e || null;
    if (e && e.nameValid && e.name) return e.name;
    return null;
  }
  function itemTexName(it, texid) {
    try {
      return texEntryName(it && it.texNames, texid);
    } catch {
      return null;
    }
  }
  function detailTexName(Rr, it, texid) {
    const nm = itemTexName(it, texid);
    if (nm) return nm;
    try {
      if (Rr && g.RDXScene && g.RDXScene.texNameFor) {
        const a = g.RDXScene.texNameFor(Rr, it.texTable, texid);
        if (a) return a;
      }
    } catch {
      /* ignore */
    }
    return null;
  }
  function detailPanelId(ti) {
    return "mdetail-" + ti;
  }
  function xdetailId(kind, key) {
    return (
      "xdetail-" +
      String(kind) +
      "-" +
      String(key).replace(/[^a-zA-Z0-9_-]+/g, "-")
    );
  }
  function listDetailIds() {
    const out = [];
    try {
      const els = document.querySelectorAll(
        '[id^="panel-mdetail-"], [id^="panel-xdetail-"]',
      );
      for (let i = 0; i < els.length; i++) {
        const pid = els[i].id.replace(/^panel-/, "");
        if (g.Panels.exists && g.Panels.exists(pid)) out.push(pid);
      }
    } catch {
      /* ignore */
    }
    return out;
  }
  function closeAllDetails() {
    if (!g.Panels.exists) return;
    listDetailIds().forEach(function (pid) {
      try {
        g.Panels.destroy(pid);
      } catch {
        /* ignore */
      }
    });
  }
  function refreshDetailPanels() {
    if (!g.Panels.exists) return;
    listDetailIds().forEach(function (pid) {
      try {
        g.Panels.refresh(pid);
      } catch {
        /* ignore */
      }
    });
  }
  function setNestedVisible(ti, bi, ni, v) {
    const S = g.AppState;
    ti = +ti;
    bi = +bi;
    ni = +ni;
    const k = nestedKey(ti, bi, ni);
    S.detailVis[k] = !!v;
    const tiVis = S.tableVis[ti] !== false;
    S.modelEntries.forEach(function (e) {
      if (e.ti === ti && e.bi === bi && e.ni === ni) {
        e.mesh.visible = !!v && tiVis && !!S.opts[e.kind];
      }
    });
    g.AppHighlight.updatePickHelpers();
  }
  function openModelDetail(ti, toggle) {
    const S = g.AppState;
    ti = +ti;
    if (isNaN(ti)) return;
    const Rr = S.R();
    if (!Rr || !Rr.map || Rr.map[ti] === undefined) return;
    const pid = detailPanelId(ti);
    if (g.Panels.exists && g.Panels.exists(pid)) {
      if (toggle && g.Panels.isVisible && g.Panels.isVisible(pid)) {
        g.Panels.destroy(pid);
        return;
      }
      g.Panels.setVisible(pid, true);
      g.Panels.refresh(pid);
      return;
    }
    const fileName = S.active,
      title = "MDL_" + pad3(ti);
    openDetailPanel(pid, title, fileName, function (body, fn) {
      renderModelDetail(body, fn, ti);
    });
  }
  function openDetailPanel(pid, title, fileName, renderFn, toggle) {
    if (g.Panels.exists && g.Panels.exists(pid)) {
      if (toggle && g.Panels.isVisible && g.Panels.isVisible(pid)) {
        g.Panels.destroy(pid);
        return;
      }
      g.Panels.setVisible(pid, true);
      g.Panels.refresh(pid);
      return;
    }
    const n = listDetailIds().length;
    const ox = 270 + (n % 4) * 26,
      oy = 46 + (n % 4) * 30;
    g.Panels.create(
      pid,
      title,
      function (body) {
        renderFn(body, fileName);
      },
      { x: ox, y: oy },
    );
  }
  function modelDetailItems(Rr, ti) {
    const out = [];
    try {
      const built = g.RDXScene.buildGeometries(Rr);
      built.items.forEach(function (it) {
        if (it.ti === ti) out.push(it);
      });
    } catch {
      /* ignore */
    }
    out.sort(function (a, b) {
      if (a.bi !== b.bi) return a.bi - b.bi;
      const ai = a.node && a.node.index != null ? a.node.index : 0;
      const bi2 = b.node && b.node.index != null ? b.node.index : 0;
      return ai - bi2;
    });
    return out;
  }
  function findNestedItem(Rr, ti, bi, ni) {
    let found = null;
    try {
      modelDetailItems(Rr, ti).forEach(function (it) {
        const n = it.node && it.node.index != null ? it.node.index : -1;
        if (it.bi === bi && n === ni) found = it;
      });
    } catch {
      /* ignore */
    }
    return found;
  }
  function appendTexStrip(body, Rr, items, isActiveFile) {
    const seenTex = {};
    const usedTex = [];
    items.forEach(function (it) {
      if (it.texTable == null) return;
      const tids = (it.model && it.model.texids) || [];
      tids.forEach(function (tid) {
        if (tid == null || tid < 0) return;
        const nm = detailTexName(Rr, it, tid);
        const key = it.texTable + ":" + tid;
        if (!seenTex[key]) {
          seenTex[key] = true;
          usedTex.push({ ti: it.texTable, texid: tid, name: nm });
        }
      });
    });
    usedTex.sort(function (a, b) {
      if (a.ti !== b.ti) return a.ti - b.ti;
      return a.texid - b.texid;
    });
    if (!usedTex.length) return;
    const strip = document.createElement("div");
    strip.className = "mdetail-texstrip";
    strip._panMoved = false;
    (function (el) {
      let down = false,
        startX = 0,
        startScroll = 0;
      function hideTip() {
        try {
          if (g.AppTooltip) g.AppTooltip.hide();
        } catch {
          /* ignore */
        }
      }
      el.addEventListener("mousedown", function (e) {
        e = e || window.event;
        if (e.button != null && e.button !== 0) return;
        down = true;
        el._panMoved = false;
        startX = e.clientX;
        startScroll = el.scrollLeft;
        el.classList.add("dragging");
        hideTip();
        function mv(e2) {
          e2 = e2 || window.event;
          if (!down) return;
          let dx = e2.clientX - startX;
          if (dx < 0) dx = -dx;
          if (dx > 4) {
            el._panMoved = true;
            hideTip();
          }
          el.scrollLeft = startScroll - (e2.clientX - startX);
        }
        function up() {
          down = false;
          try {
            el.classList.remove("dragging");
          } catch {
            /* ignore */
          }
          if (window.removeEventListener) {
            window.removeEventListener("mousemove", mv);
            window.removeEventListener("mouseup", up);
          }
          setTimeout(function () {
            el._panMoved = false;
          }, 0);
        }
        if (window.addEventListener) {
          window.addEventListener("mousemove", mv);
          window.addEventListener("mouseup", up);
        }
        if (e.preventDefault) {
          try {
            e.preventDefault();
          } catch {
            /* ignore */
          }
        }
      });
    })(strip);
    usedTex.forEach(function (u) {
      let info = null;
      try {
        if (g.AppTexturesPanel && g.AppTexturesPanel.texInfo) {
          info = g.AppTexturesPanel.texInfo(Rr, u.ti, u.texid);
        }
      } catch {
        info = null;
      }
      const cell = document.createElement("div");
      cell.className = "mdetail-texcell";
      let src = null;
      try {
        const tinfo = g.RDXScene.getTexture(Rr, u.ti, u.texid);
        if (tinfo && tinfo.tex && tinfo.tex.image) src = tinfo.tex.image;
      } catch {
        src = null;
      }
      if (src && src.width) {
        const c = document.createElement("canvas");
        const maxDim = 40;
        const sw = src.width || 1,
          sh = src.height || 1;
        let sc = maxDim / Math.max(sw, sh);
        if (!(sc > 0)) sc = 1;
        if (sc > 4) sc = 4;
        c.width = Math.max(1, Math.round(sw * sc));
        c.height = Math.max(1, Math.round(sh * sc));
        c.className = "tex-thumb tex-checker mdetail-texthumb";
        try {
          c.draggable = false;
        } catch {
          /* ignore */
        }
        try {
          const ccx = c.getContext("2d");
          ccx.imageSmoothingEnabled = true;
          ccx.clearRect(0, 0, c.width, c.height);
          ccx.drawImage(src, 0, 0, c.width, c.height);
        } catch {
          /* ignore */
        }
        cell.appendChild(c);
      } else {
        const miss = document.createElement("div");
        miss.className =
          "tex-thumb tex-checker mdetail-texthumb mdetail-texmiss";
        miss.textContent = "—";
        cell.appendChild(miss);
      }
      let tipTitle = u.name || "TEX_" + u.ti + "_" + u.texid;
      try {
        if (info && info.variant != null) tipTitle += " pal" + info.variant;
      } catch {
        /* ignore */
      }
      try {
        if (g.AppTooltip) g.AppTooltip.attach(cell, { title: tipTitle });
      } catch {
        cell.title = tipTitle;
      }
      if (isActiveFile) {
        cell.classList.add("clickable");
        cell.onclick = (function (t2, x2, el) {
          return function () {
            if (el._panMoved) return;
            try {
              if (g.AppTooltip) g.AppTooltip.hide();
            } catch {
              /* ignore */
            }
            try {
              if (g.AppTexturesPanel && g.AppTexturesPanel.openTex) {
                g.AppTexturesPanel.openTex(t2, x2);
              }
            } catch {
              /* ignore */
            }
          };
        })(u.ti, u.texid, strip);
      }
      strip.appendChild(cell);
    });
    body.appendChild(strip);
  }
  function openNestedDetail(ti, bi, ni, toggle) {
    ti = +ti;
    bi = +bi;
    ni = +ni;
    if (isNaN(ti) || isNaN(bi) || isNaN(ni)) return;
    const S = g.AppState;
    const Rr = S.R();
    if (!Rr) return;
    if (!findNestedItem(Rr, ti, bi, ni)) return;
    const pid = xdetailId("nested", ti + "-" + bi + "-" + ni);
    const fileName = S.active;
    const title = "MDL_" + pad3(ti) + " B" + bi + "·N" + ni;
    openDetailPanel(
      pid,
      title,
      fileName,
      function (body, fn) {
        renderNestedDetail(body, fn, ti, bi, ni);
      },
      toggle,
    );
  }
  function renderNestedDetail(body, fileName, ti, bi, ni) {
    const S = g.AppState;
    const F = g.AppDetailFormat;
    const Rr = S.files[fileName];
    if (!Rr) {
      body.textContent = "File closed";
      return;
    }
    F.backgroundNote(body, fileName, S.active);
    const it = findNestedItem(Rr, ti, bi, ni);
    if (!it) {
      F.notFound(body, "MDL_" + pad3(ti) + " B" + bi + "·N" + ni);
      return;
    }
    const isActiveFile = fileName === S.active;
    const top = F.top(body);
    const faces = it.model && it.model.faces ? it.model.faces.length : 0;
    const verts = it.model && it.model.verts ? it.model.verts.length : 0;
    const seen = {};
    const tids = [];
    try {
      ((it.model && it.model.texids) || []).forEach(function (tid) {
        if (tid == null || tid < 0 || seen[tid]) return;
        seen[tid] = true;
        tids.push(tid);
      });
    } catch {
      /* ignore */
    }
    tids.sort(function (a, b) {
      return a - b;
    });
    const tLabel =
      it.texTable == null
        ? "—"
        : !tids.length
          ? "T" + it.texTable
          : tids
              .map(function (tid) {
                return (
                  detailTexName(Rr, it, tid) || "TEX_" + it.texTable + "_" + tid
                );
              })
              .join(", ");
    F.kv(top, "Parent", "MDL_" + pad3(ti));
    F.kv(top, "Nested", "B" + bi + "·N" + ni);
    F.kv(top, "Kind", it.kind || "—");
    F.kv(top, "Tris", String(faces));
    F.kv(top, "Verts", String(verts));
    F.kv(top, "TexTable", it.texTable != null ? "TEX_" + it.texTable : "—");
    F.kv(top, "Textures", tLabel);
    if (it.node) {
      if (it.node.pos) F.kv(top, "Pos", F.fmtFloat(it.node.pos));
      if (it.node.ang) F.kv(top, "Ang", F.fmtAng(it.node.ang));
      if (it.node.scl) F.kv(top, "Scale", F.fmtFloat(it.node.scl));
      F.kv(top, "EvalFlags", F.fmtHex(it.node.evalflags || 0));
    }
    F.kv(top, "Hidden", it.hidden ? "hidden by model data" : "no");
    const k = nestedKey(ti, bi, ni);
    F.kv(
      top,
      "Visible",
      it.hidden
        ? "hidden by model data"
        : S.detailVis[k] !== false &&
            (S.tableVis[ti] !== false || !isActiveFile)
          ? "visible"
          : "hidden",
    );
    appendTexStrip(body, Rr, [it], isActiveFile);
    appendNestedTexTable(body, Rr, it, tids, isActiveFile);
  }
  function appendNestedTexTable(body, Rr, it, tids, isActiveFile) {
    if (it.texTable == null || !tids || !tids.length) return;
    const tbl = document.createElement("table");
    tbl.className = "mdetail-table tex-table";
    const thead = document.createElement("thead");
    const hr = document.createElement("tr");
    ["Texture", "Size", "Format", "Alpha", ""].forEach(function (c) {
      const th = document.createElement("th");
      th.className = "label";
      th.textContent = c;
      hr.appendChild(th);
    });
    thead.appendChild(hr);
    tbl.appendChild(thead);
    const tb = document.createElement("tbody");
    tbl.appendChild(tb);
    tids.forEach(function (tid) {
      let info = null;
      try {
        if (g.AppTexturesPanel && g.AppTexturesPanel.texInfo) {
          info = g.AppTexturesPanel.texInfo(Rr, it.texTable, tid);
        }
      } catch {
        info = null;
      }
      let nm = detailTexName(Rr, it, tid) || "TEX_" + it.texTable + "_" + tid;
      if (info && info.variant != null) nm += " pal" + info.variant;
      const tr = document.createElement("tr");
      const base =
        "TEX table " +
        it.texTable +
        " pic " +
        tid +
        " (block " +
        (info ? info.bi : "?") +
        ")";
      tr.title = nm + " · " + base;
      const tdN = document.createElement("td");
      tdN.textContent = nm;
      tdN.title = tr.title;
      tr.appendChild(tdN);
      const tdS = document.createElement("td");
      tdS.textContent =
        info && info.width ? info.width + "×" + info.height : "—";
      tr.appendChild(tdS);
      const tdF = document.createElement("td");
      tdF.textContent = info ? info.fmt : "—";
      if (info) {
        tdF.title = info.fmt + " · block " + info.blockKind + " #" + info.bi;
      }
      tr.appendChild(tdF);
      const tdA = document.createElement("td");
      tdA.textContent = info ? info.kind : "—";
      tr.appendChild(tdA);
      const tdV = document.createElement("td");
      const b = document.createElement("button");
      b.className = "icon-btn";
      b.textContent = "⤢";
      b.title = "open in Texture Viewer";
      if (!isActiveFile) {
        b.disabled = true;
      } else {
        b.onclick = (function (t2, x2) {
          return function (e) {
            if (e) e.stopPropagation();
            try {
              if (g.AppTexturesPanel && g.AppTexturesPanel.openTex) {
                g.AppTexturesPanel.openTex(t2, x2);
              }
            } catch {
              /* ignore */
            }
          };
        })(it.texTable, tid);
      }
      tdV.appendChild(b);
      tr.appendChild(tdV);
      tb.appendChild(tr);
    });
    body.appendChild(tbl);
  }
  function renderModelDetail(body, fileName, ti) {
    const S = g.AppState;
    const Rr = S.files[fileName];
    if (!Rr) {
      body.textContent = "File closed";
      return;
    }
    if (fileName !== S.active) {
      const note = document.createElement("div");
      note.className = "muted";
      note.textContent =
        "Showing " +
        fileName +
        " (not the active file: scene actions disabled).";
      body.appendChild(note);
    }
    const entry = Rr.map ? Rr.map[ti] : undefined;
    if (entry === undefined) {
      body.textContent = "Model MDL_" + pad3(ti) + " not found";
      return;
    }
    const kind = entry[0],
      tabIdx = entry[1];
    const rec = (Rr.models && Rr.models[ti]) || {};
    const items = modelDetailItems(Rr, ti);
    let meshCount = 0,
      faceCount = 0,
      vertCount = 0;
    items.forEach(function (it) {
      if (it.model) {
        faceCount += (it.model.faces || []).length;
        vertCount += (it.model.verts || []).length;
      }
    });
    if (fileName === S.active) {
      S.modelEntries.forEach(function (e) {
        if (e.ti === ti) meshCount++;
      });
    } else {
      meshCount = items.length
        ? items.reduce(function (a) {
            return a + 1;
          }, 0)
        : 0;
    }
    let place = null;
    try {
      if (kind === "actor" && tabIdx != null && Rr.actor[tabIdx]) {
        place = Rr.actor[tabIdx];
      } else if (kind === "obj" && tabIdx != null && Rr.obj[tabIdx]) {
        place = Rr.obj[tabIdx];
      } else if (kind === "item" && tabIdx != null && Rr.item[tabIdx]) {
        place = Rr.item[tabIdx];
      }
    } catch {
      place = null;
    }
    function fmtN(a) {
      return (
        "[" +
        a
          .map(function (v) {
            return Math.round(v * 10) / 10;
          })
          .join(", ") +
        "]"
      );
    }
    function fmtA(a) {
      return "[" + a.join(", ") + "]";
    }
    const top = document.createElement("div");
    top.className = "mdetail-top";
    function kv(k, v) {
      const r = document.createElement("div");
      r.className = "mdetail-kv";
      const kk = document.createElement("span");
      kk.className = "label";
      kk.textContent = k;
      const vv = document.createElement("span");
      vv.className = "mdetail-v";
      vv.textContent = v;
      vv.title = v;
      r.appendChild(kk);
      r.appendChild(vv);
      top.appendChild(r);
    }
    kv("Model", "MDL_" + pad3(ti));
    kv("Kind", kind + (tabIdx != null ? " #" + tabIdx : ""));
    try {
      let lookupName = null;
      if (g.RDXLookups && place && place.id !== undefined) {
        if (kind === "actor") lookupName = g.RDXLookups.actorName(place.id);
        else if (kind === "item") lookupName = g.RDXLookups.itemName(place.id);
      }
      if (kind === "actor" || kind === "item") {
        kv("Name", lookupName != null ? lookupName : "Unknown");
      }
    } catch {
      /* ignore */
    }
    if (rec.offset != null) {
      kv("Offset", "0x" + (rec.offset >>> 0).toString(16));
    }
    kv("Blobs", String((rec.blobs || []).length));
    const texList = (rec.blobs || [])
      .map(function (b, bi) {
        const a = Rr.assign ? Rr.assign[ti + "," + bi] : undefined;
        return "b" + bi + ":" + (a != null ? "TEX_" + a : "—");
      })
      .join("  ");
    kv("Textures", texList || "—");
    if (place) {
      if (place.pos) kv("Pos", fmtN(place.pos));
      if (place.ang) kv("Ang", fmtA(place.ang));
      if (place.id !== undefined) {
        kv(
          "Id",
          String(place.id) +
            (place.type !== undefined ? " / type " + place.type : ""),
        );
      }
      if (place.flr !== undefined) kv("Floor", String(place.flr));
      if (place.mdlver !== undefined) kv("MdlVer", String(place.mdlver));
      if (place.wrkNo !== undefined || place.prm1 !== undefined) {
        kv("WrkNo/Prm1", place.wrkNo + " / " + place.prm1);
      }
      if (place.aspd !== undefined) kv("Aspd", String(place.aspd));
      if (place.hide) kv("Hide", "[" + place.hide.join(", ") + "]");
    }
    kv("Nodes", String(items.length));
    kv("Faces / Verts", faceCount + " / " + vertCount);
    kv("Meshes", String(meshCount));
    kv(
      "Visible",
      S.tableVis[ti] === false && fileName === S.active ? "hidden" : "visible",
    );
    body.appendChild(top);
    if (!items.length) {
      const em = document.createElement("div");
      em.className = "muted";
      em.textContent = "No nested models";
      body.appendChild(em);
      return;
    }
    const isActiveFile = fileName === S.active;
    appendTexStrip(body, Rr, items, isActiveFile);
    const tbl = document.createElement("table");
    tbl.className = "mdetail-table mdetail-nested";
    const thead = document.createElement("thead");
    const hr = document.createElement("tr");
    ["Model", "Tris", "", "", ""].forEach(function (c) {
      const th = document.createElement("th");
      th.className = "label";
      th.textContent = c;
      hr.appendChild(th);
    });
    thead.appendChild(hr);
    tbl.appendChild(thead);
    const tb = document.createElement("tbody");
    tbl.appendChild(tb);
    items.forEach(function (it) {
      const ni = it.node && it.node.index != null ? it.node.index : -1;
      const k = nestedKey(ti, it.bi, ni);
      const tr = document.createElement("tr");
      if (it.hidden) tr.className = "mdetail-hidden";
      tr.oncontextmenu = (function (t2, b2, n2, ok) {
        return function (e) {
          e = e || window.event;
          if (e.preventDefault) {
            try {
              e.preventDefault();
            } catch {
              /* ignore */
            }
          }
          if (e.stopPropagation) {
            try {
              e.stopPropagation();
            } catch {
              /* ignore */
            }
          }
          const cx = e && e.clientX != null ? e.clientX : 0,
            cy = e && e.clientY != null ? e.clientY : 0;
          g.AppMenu.showNestedContextMenu(cx, cy, t2, b2, n2, ok);
          return false;
        };
      })(ti, it.bi, ni, !it.hidden && isActiveFile);
      const tdN = document.createElement("td");
      tdN.textContent = "B" + it.bi + "·N" + ni;
      tdN.title =
        "blob " +
        it.bi +
        " node " +
        ni +
        (it.hidden ? " (hidden by evalflags)" : "");
      tr.appendChild(tdN);
      const tdF = document.createElement("td");
      tdF.textContent = String(
        it.model && it.model.faces ? it.model.faces.length : 0,
      );
      tr.appendChild(tdF);
      const tdI = document.createElement("td");
      const infoBtn = document.createElement("button");
      infoBtn.className = "icon-btn";
      infoBtn.textContent = "ⓘ";
      infoBtn.title = "open nested model details";
      infoBtn.onclick = (function (t2, b2, n2) {
        return function (e) {
          if (e) e.stopPropagation();
          openNestedDetail(t2, b2, n2);
        };
      })(ti, it.bi, ni);
      tdI.appendChild(infoBtn);
      tr.appendChild(tdI);
      const tdE = document.createElement("td");
      const eye = document.createElement("button");
      eye.className = "icon-btn";
      eye.textContent = "👁";
      eye.title = "toggle nested visibility";
      const nv = S.detailVis[k] !== false;
      if (!nv) eye.classList.add("off");
      if (it.hidden || !isActiveFile) {
        eye.disabled = true;
        eye.classList.add("off");
        eye.title = it.hidden
          ? "hidden by model data"
          : "switch to " + fileName + " to toggle";
      } else {
        eye.onclick = (function (t2, b2, n2, btn) {
          return function (e) {
            if (e) e.stopPropagation();
            setNestedVisible(
              t2,
              b2,
              n2,
              !(S.detailVis[nestedKey(t2, b2, n2)] !== false),
            );
            btn.classList.toggle(
              "off",
              !(S.detailVis[nestedKey(t2, b2, n2)] !== false),
            );
          };
        })(ti, it.bi, ni, eye);
      }
      tdE.appendChild(eye);
      tr.appendChild(tdE);
      const tdG = document.createElement("td");
      const go = document.createElement("button");
      go.className = "icon-btn";
      go.textContent = "◎";
      go.title = "center camera on nested model";
      if (it.hidden || !isActiveFile) {
        go.disabled = true;
        go.title = it.hidden
          ? "hidden by model data"
          : "switch to " + fileName + " to goto";
      } else {
        go.onclick = (function (t2, b2, n2) {
          return function (e) {
            if (e) e.stopPropagation();
            g.AppSelection.setSelected(t2);
            g.AppNavigation.gotoNested(t2, b2, n2);
          };
        })(ti, it.bi, ni);
      }
      tdG.appendChild(go);
      tr.appendChild(tdG);
      tb.appendChild(tr);
    });
    body.appendChild(tbl);
  }
  g.AppDetail = {
    pad3: pad3,
    nestedKey: nestedKey,
    xdetailId: xdetailId,
    openDetailPanel: openDetailPanel,
    closeAllDetails: closeAllDetails,
    refreshDetailPanels: refreshDetailPanels,
    setNestedVisible: setNestedVisible,
    openModelDetail: openModelDetail,
    openNestedDetail: openNestedDetail,
  };
})(window);
