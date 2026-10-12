(function (g) {
  "use strict";
  const TEXKEY = "rdxweb.textures";
  const MODES = ["details", "list"];
  const MODE_LABEL = { details: "Details", list: "List" };
  const selByFile = {};
  let viewerRO = null;

  function loadMode() {
    try {
      const s = JSON.parse(localStorage.getItem(TEXKEY) || "{}");
      if (s && MODES.indexOf(s.mode) >= 0) return s.mode;
    } catch {
      /* ignore */
    }
    return "list";
  }
  function saveMode(m) {
    try {
      localStorage.setItem(TEXKEY, JSON.stringify({ mode: m }));
    } catch {
      /* ignore */
    }
  }
  let mode = loadMode();

  function imgTypeName(t) {
    return (
      { 1: "RGBA16", 2: "RGB32", 3: "RGBA32", 4: "IDX4", 5: "IDX8" }[t] ||
      "img" + t
    );
  }
  function fmtLabel(blockKind, pic) {
    if (!pic) return blockKind || "?";
    if (blockKind === "TIM2") return "TIM2 " + imgTypeName(pic.imageType);
    if (blockKind === "PVR") return fmtPvrLabel(pic);
    return blockKind || "?";
  }
  function fmtPvrLabel(pic) {
    const pf =
      pic.pixfmt != null
        ? pic.pixfmt
        : "? (0x" + (+pic.datafmt).toString(16) + ")";
    return "PVR f" + pf;
  }
  function texName(e, Rr) {
    const base = "TEX_" + e.ti + "_" + e.texid;
    let alias = null;
    try {
      if (Rr && g.RDXScene && g.RDXScene.texNameFor) {
        alias = g.RDXScene.texNameFor(Rr, e.ti, e.texid);
      }
    } catch {
      /* ignore */
    }
    let n = alias || base;
    if (e.variant != null) n += " pal" + e.variant;
    return n;
  }

  function resolveBlockKind(Rr, ti, bi) {
    try {
      const tab = Rr.textures[ti];
      if (tab && tab.blocks[bi]) return tab.blocks[bi].kind;
    } catch {
      /* ignore */
    }
    return "?";
  }
  function applyDecodedSize(entry, Rr, ti, bi, pi) {
    try {
      const doc = g.RDX.decodeBlock(Rr, ti, bi);
      const pic = doc && doc.pictures[pi];
      if (!pic) return;
      entry.width = pic.width || 0;
      entry.height = pic.height || 0;
      entry.fmt = fmtLabel(entry.blockKind, pic);
    } catch {
      /* ignore */
    }
  }
  function applySceneTexture(entry, Rr, ti, texid) {
    try {
      const t = g.RDXScene.getTexture(Rr, ti, texid);
      if (!t) return;
      if (t.kind) entry.kind = t.kind;
      if (!entry.width && t.tex && t.tex.image) {
        entry.width = t.tex.image.width || 0;
        entry.height = t.tex.image.height || 0;
      }
    } catch {
      /* ignore */
    }
  }
  function buildEntry(Rr, ti, texid, e) {
    const entry = {
      ti: ti,
      texid: texid,
      bi: e.bi,
      pi: e.pi,
      variant: e.variant,
      width: 0,
      height: 0,
      blockKind: "?",
      fmt: "?",
      kind: "?",
    };
    entry.blockKind = resolveBlockKind(Rr, ti, e.bi);
    applyDecodedSize(entry, Rr, ti, e.bi, e.pi);
    applySceneTexture(entry, Rr, ti, texid);
    return entry;
  }

  function tablePicsSafe(Rr, ti) {
    try {
      return g.RDX.tablePics(Rr, ti) || [];
    } catch {
      return null;
    }
  }
  function listEntries(Rr) {
    const out = [];
    const nt = (Rr.textures || []).length;
    for (let ti = 0; ti < nt; ti++) {
      const pics = tablePicsSafe(Rr, ti);
      if (pics == null) continue;
      for (let texid = 0; texid < pics.length; texid++) {
        out.push(buildEntry(Rr, ti, texid, pics[texid]));
      }
    }
    return out;
  }

  function texSource(Rr, entry) {
    try {
      const t = g.RDXScene.getTexture(Rr, entry.ti, entry.texid);
      if (t && t.tex && t.tex.image) {
        return { canvas: t.tex.image, kind: t.kind };
      }
    } catch {
      /* ignore */
    }
    return null;
  }
  function collectAssignGroups(Rr) {
    const m = {};
    try {
      Object.keys(Rr.assign || {}).forEach(function (k) {
        const mdl = +String(k).split(",")[0];
        const texTable = Rr.assign[k];
        if (texTable == null) return;
        if (!m[texTable]) m[texTable] = {};
        m[texTable][mdl] = true;
      });
    } catch {
      /* ignore */
    }
    return m;
  }
  function toSortedModelLists(m) {
    const out = {};
    Object.keys(m).forEach(function (t) {
      out[t] = Object.keys(m[t])
        .map(function (x) {
          return +x;
        })
        .sort(function (a, b) {
          return a - b;
        });
    });
    return out;
  }
  function usedBy(Rr) {
    return toSortedModelLists(collectAssignGroups(Rr));
  }
  function mdlName(ti) {
    return "MDL_" + ("00" + ti).slice(-3);
  }

  function thumbCanvas(src, maxDim, cls) {
    const c = document.createElement("canvas");
    const w = src.width || 1,
      h = src.height || 1;
    let s = maxDim / Math.max(w, h);
    if (!(s > 0)) s = 1;
    if (s > 4) s = 4;
    c.width = Math.max(1, Math.round(w * s));
    c.height = Math.max(1, Math.round(h * s));
    if (cls) c.className = cls;
    try {
      const cx = c.getContext("2d");
      cx.imageSmoothingEnabled = true;
      cx.clearRect(0, 0, c.width, c.height);
      cx.drawImage(src, 0, 0, c.width, c.height);
    } catch {
      /* ignore */
    }
    return c;
  }
  function fullCanvas(src, cls) {
    const c = document.createElement("canvas");
    c.width = src.width || 1;
    c.height = src.height || 1;
    if (cls) c.className = cls;
    try {
      c.getContext("2d").drawImage(src, 0, 0);
    } catch {
      /* ignore */
    }
    return c;
  }
  function missingThumb(maxDim, cls, label) {
    const c = document.createElement("canvas");
    c.width = maxDim;
    c.height = maxDim;
    if (cls) c.className = cls;
    try {
      paintMissing(c);
    } catch {
      /* ignore */
    }
    c.title = label || "undecodable";
    return c;
  }
  function paintMissing(c) {
    const cx = c.getContext("2d");
    cx.fillStyle = "#1a1a1a";
    cx.fillRect(0, 0, c.width, c.height);
    cx.strokeStyle = "#555";
    cx.beginPath();
    cx.moveTo(4, 4);
    cx.lineTo(c.width - 4, c.height - 4);
    cx.moveTo(c.width - 4, 4);
    cx.lineTo(4, c.height - 4);
    cx.stroke();
  }

  function getSel(file, count) {
    let s = selByFile[file];
    if (s == null || s < 0 || (count != null && s >= count)) s = 0;
    return s;
  }
  function refreshBoth() {
    try {
      g.Panels.refresh("textures");
    } catch {
      /* ignore */
    }
    try {
      if (g.Panels.exists && g.Panels.exists("texviewer")) {
        g.Panels.refresh("texviewer");
      }
    } catch {
      /* ignore */
    }
  }
  function setMode(m) {
    if (MODES.indexOf(m) < 0) return;
    mode = m;
    saveMode(m);
    try {
      g.Panels.refresh("textures");
    } catch {
      /* ignore */
    }
  }
  function setSel(file, i) {
    if (selByFile[file] === i) return;
    selByFile[file] = i;
    refreshBoth();
  }
  function openViewer(i) {
    const S = g.AppState;
    if (S.active) selByFile[S.active] = i;
    try {
      g.Panels.setVisible("texviewer", true);
    } catch {
      /* ignore */
    }
    refreshBoth();
  }
  function findEntryIndex(entries, ti, texid) {
    for (let i = 0; i < entries.length; i++) {
      if (entries[i].ti === ti && entries[i].texid === texid) return i;
    }
    return -1;
  }
  function openTex(ti, texid) {
    const S = g.AppState;
    const Rr = S.R();
    if (!Rr) return false;
    try {
      const idx = findEntryIndex(listEntries(Rr), +ti, +texid);
      if (idx < 0) return false;
      openViewer(idx);
      return true;
    } catch {
      return false;
    }
  }
  function texInfo(Rr, ti, texid) {
    if (!Rr) return null;
    ti = +ti;
    texid = +texid;
    try {
      const pics = tablePicsSafe(Rr, ti);
      if (pics == null) return null;
      if (texid < 0 || texid >= pics.length) return null;
      if (!pics[texid]) return null;
      return buildEntry(Rr, ti, texid, pics[texid]);
    } catch {
      /* ignore */
    }
    return null;
  }

  function renderToolbar(body, total) {
    const bar = document.createElement("div");
    bar.className = "tex-toolbar";
    MODES.forEach(function (m) {
      bar.appendChild(modeButton(m));
    });
    const c = document.createElement("span");
    c.className = "muted";
    c.textContent = total ? total + " pics" : "empty";
    bar.appendChild(c);
    body.appendChild(bar);
  }
  function modeButton(m) {
    const b = document.createElement("button");
    b.className = "btn " + (m === mode ? "btn-primary" : "btn-secondary");
    b.textContent = MODE_LABEL[m];
    b.title = MODE_LABEL[m] + " view";
    b.onclick = function () {
      setMode(m);
    };
    return b;
  }

  function viewButton(i) {
    const b = document.createElement("button");
    b.className = "icon-btn";
    b.textContent = "⤢";
    b.title = "open in Texture Viewer";
    b.onclick = function (e) {
      stopEvent(e);
      openViewer(i);
    };
    return b;
  }
  function stopEvent(e) {
    if (!e || !e.stopPropagation) return;
    try {
      e.stopPropagation();
    } catch {
      /* ignore */
    }
  }

  function entryTitle(e, Rr) {
    const base =
      "TEX table " + e.ti + " pic " + e.texid + " (block " + e.bi + ")";
    let alias = null;
    try {
      if (Rr && g.RDXScene && g.RDXScene.texNameFor) {
        alias = g.RDXScene.texNameFor(Rr, e.ti, e.texid);
      }
    } catch {
      /* ignore */
    }
    return alias ? alias + " · " + base : base;
  }
  function fmtSize(e) {
    return e.width ? e.width + "×" + e.height : "—";
  }
  function renderDetailsHead(tbl) {
    const thead = document.createElement("thead");
    const hr = document.createElement("tr");
    ["Texture", "Size", "Format", "Alpha", ""].forEach(function (t) {
      const th = document.createElement("th");
      th.className = "label";
      th.textContent = t;
      hr.appendChild(th);
    });
    thead.appendChild(hr);
    tbl.appendChild(thead);
  }
  function appendDetailsCell(tr, text, title) {
    const td = document.createElement("td");
    td.textContent = text;
    if (title != null) td.title = title;
    tr.appendChild(td);
    return td;
  }
  function appendDetailsRow(tb, S, Rr, e, i, sel) {
    const tr = document.createElement("tr");
    if (i === sel) tr.className = "tex-selected";
    tr.title = entryTitle(e, Rr);
    tr.onclick = function () {
      setSel(S.active, i);
    };
    tr.ondblclick = makeOpenViewer(i);
    appendDetailsCell(tr, texName(e, Rr), tr.title);
    appendDetailsCell(tr, fmtSize(e));
    appendDetailsCell(
      tr,
      e.fmt,
      e.fmt + " · block " + e.blockKind + " #" + e.bi,
    );
    appendDetailsCell(tr, e.kind);
    const tdV = document.createElement("td");
    tdV.appendChild(viewButton(i));
    tr.appendChild(tdV);
    tb.appendChild(tr);
  }
  function makeOpenViewer(idx) {
    return function () {
      openViewer(idx);
    };
  }
  function renderDetails(body, S, Rr, entries) {
    const tbl = document.createElement("table");
    tbl.className = "mdetail-table tex-table";
    renderDetailsHead(tbl);
    const tb = document.createElement("tbody");
    tbl.appendChild(tb);
    const sel = getSel(S.active, entries.length);
    entries.forEach(function (e, i) {
      appendDetailsRow(tb, S, Rr, e, i, sel);
    });
    body.appendChild(tbl);
  }

  function appendListRow(wrap, S, Rr, e, i, sel) {
    const row = document.createElement("div");
    row.className = "filerow tex-list-row" + (i === sel ? " selected" : "");
    row.onclick = function () {
      setSel(S.active, i);
    };
    row.ondblclick = makeOpenViewer(i);
    row.appendChild(listThumb(Rr, e));
    row.appendChild(listInfo(e, Rr));
    row.appendChild(viewButton(i));
    wrap.appendChild(row);
  }
  function listThumb(Rr, e) {
    const src = texSource(Rr, e);
    if (src) return thumbCanvas(src.canvas, 48, "tex-thumb tex-checker");
    return missingThumb(48, "tex-thumb tex-checker", texName(e, Rr));
  }
  function listInfo(e, Rr) {
    const info = document.createElement("div");
    info.className = "grow tex-list-info";
    const n = document.createElement("div");
    n.className = "truncate";
    n.textContent = texName(e, Rr);
    n.title = entryTitle(e, Rr);
    const d = document.createElement("div");
    d.className = "muted truncate";
    d.textContent = fmtSize(e) + " · " + e.fmt + " · " + e.kind;
    info.appendChild(n);
    info.appendChild(d);
    return info;
  }
  function renderList(body, S, Rr, entries) {
    const sel = getSel(S.active, entries.length);
    const wrap = document.createElement("div");
    wrap.className = "tex-list";
    entries.forEach(function (e, i) {
      appendListRow(wrap, S, Rr, e, i, sel);
    });
    body.appendChild(wrap);
  }

  function viewerResource(body) {
    const S = g.AppState;
    const Rr = S.R();
    if (!Rr) {
      body.textContent = "No file loaded";
      return null;
    }
    if (!Rr.textures || !Rr.textures.length) {
      body.textContent = "No textures";
      return null;
    }
    return { S: S, Rr: Rr };
  }
  function readViewerEntries(body, Rr) {
    let entries = null;
    try {
      entries = listEntries(Rr);
    } catch (e) {
      body.textContent = "texture read error: " + e.message;
      return null;
    }
    if (!entries.length) {
      body.textContent = "No decodable textures";
      return null;
    }
    return entries;
  }
  function resolveViewerState(body) {
    const res = viewerResource(body);
    if (!res) return null;
    const entries = readViewerEntries(body, res.Rr);
    if (!entries) return null;
    const sel = getSel(res.S.active, entries.length);
    selByFile[res.S.active] = sel;
    return {
      S: res.S,
      Rr: res.Rr,
      entries: entries,
      sel: sel,
      entry: entries[sel],
      byTex: usedBy(res.Rr),
    };
  }

  function navStepButton(text, title, disabled, onStep) {
    const b = document.createElement("button");
    b.className = "icon-btn";
    b.textContent = text;
    b.title = title;
    b.disabled = disabled;
    b.onclick = onStep;
    return b;
  }
  function renderViewerNav(body, S, entries, sel, entry) {
    const nav = document.createElement("div");
    nav.className = "tex-viewer-nav";
    nav.appendChild(
      navStepButton("◀", "previous texture", sel <= 0, function () {
        if (sel > 0) setSel(S.active, sel - 1);
      }),
    );
    const lab = document.createElement("span");
    lab.className = "grow truncate tex-viewer-title";
    lab.textContent =
      sel + 1 + " / " + entries.length + " · " + texName(entry, S.R());
    lab.title = lab.textContent;
    nav.appendChild(lab);
    nav.appendChild(
      navStepButton(
        "▶",
        "next texture",
        sel >= entries.length - 1,
        function () {
          if (sel < entries.length - 1) setSel(S.active, sel + 1);
        },
      ),
    );
    body.appendChild(nav);
  }

  function resetViewerObserver() {
    try {
      if (viewerRO) viewerRO.disconnect();
      viewerRO = null;
    } catch {
      /* ignore */
    }
  }
  function resolveViewerCanvas(Rr, entry) {
    const src = texSource(Rr, entry);
    if (src) {
      const big = fullCanvas(src.canvas, "tex-viewer-img");
      big.title = texName(entry, Rr) + " " + entry.width + "×" + entry.height;
      return big;
    }
    return missingThumb(128, "tex-viewer-img", "undecodable");
  }
  function renderViewerMain(body, Rr, entry) {
    const main = document.createElement("div");
    main.className = "tex-viewer-main tex-checker";
    resetViewerObserver();
    const big = resolveViewerCanvas(Rr, entry);
    main.appendChild(big);
    body.appendChild(main);
    return { main: main, big: big };
  }

  function newViewerZoom() {
    return { zoom: 1, panX: 0, panY: 0, lastW: null, lastH: null };
  }
  function applyViewerZoom(big, st) {
    big.style.transform =
      st.zoom === 1 && st.panX === 0 && st.panY === 0
        ? ""
        : "translate(" +
          st.panX +
          "px," +
          st.panY +
          "px) scale(" +
          st.zoom +
          ")";
    big.style.transformOrigin = "center center";
    try {
      big.style.imageRendering = st.zoom > 2 ? "pixelated" : "auto";
    } catch {
      /* ignore */
    }
  }
  function clampViewerZoom(nz) {
    if (nz < 0.1) return 0.1;
    if (nz > 16) return 16;
    return nz;
  }
  function zoomViewerAt(big, st, cx, cy, f) {
    const nz = clampViewerZoom(st.zoom * f);
    if (nz === st.zoom) return;
    const r = big.getBoundingClientRect();
    const fx = r.width ? (cx - r.left) / r.width : 0.5;
    const fy = r.height ? (cy - r.top) / r.height : 0.5;
    const nw = r.width * (nz / st.zoom);
    const nh = r.height * (nz / st.zoom);
    const ccx = r.left + r.width / 2;
    const ccy = r.top + r.height / 2;
    st.zoom = nz;
    st.panX += cx - fx * nw - (ccx - nw / 2);
    st.panY += cy - fy * nh - (ccy - nh / 2);
    applyViewerZoom(big, st);
  }
  function viewerPadding(main) {
    let padL = 8;
    let padT = 8;
    try {
      if (!window.getComputedStyle) return { padL: padL, padT: padT };
      const cs = window.getComputedStyle(main);
      if (!cs) return { padL: padL, padT: padT };
      const pl = parseFloat(cs.paddingLeft);
      const pt = parseFloat(cs.paddingTop);
      if (pl === pl && pl >= 0) padL = pl;
      if (pt === pt && pt >= 0) padT = pt;
    } catch {
      /* ignore */
    }
    return { padL: padL, padT: padT };
  }
  function centerViewerImage(main, big, st) {
    try {
      const mw = main.clientWidth;
      const mh = main.clientHeight;
      const iw = big.offsetWidth;
      const ih = big.offsetHeight;
      if (!(mw > 0 && mh > 0 && iw > 0 && ih > 0)) return;
      const pad = viewerPadding(main);
      st.panX = (mw - iw) / 2 - pad.padL;
      st.panY = (mh - ih) / 2 - pad.padT;
      st.lastW = mw;
      st.lastH = mh;
      applyViewerZoom(big, st);
    } catch {
      /* ignore */
    }
  }
  function shiftViewerOnResize(main, big, st) {
    try {
      const mw = main.clientWidth;
      const mh = main.clientHeight;
      if (!(mw > 0 && mh > 0)) return;
      if (st.lastW == null || st.lastH == null) {
        centerViewerImage(main, big, st);
        return;
      }
      const dw = mw - st.lastW;
      const dh = mh - st.lastH;
      st.lastW = mw;
      st.lastH = mh;
      if (!dw && !dh) return;
      st.panX += dw / 2;
      st.panY += dh / 2;
      applyViewerZoom(big, st);
    } catch {
      /* ignore */
    }
  }
  function bindViewerWheel(main, big, st) {
    try {
      main.addEventListener(
        "wheel",
        function (ev) {
          wheelZoom(ev, big, st);
        },
        { passive: false },
      );
    } catch {
      /* ignore */
    }
  }
  function wheelZoom(ev, big, st) {
    ev = ev || window.event;
    if (ev.preventDefault) {
      try {
        ev.preventDefault();
      } catch {
        /* ignore */
      }
    }
    const d = ev.deltaY || 0;
    zoomViewerAt(big, st, ev.clientX, ev.clientY, d < 0 ? 1.25 : 1 / 1.25);
  }
  function bindViewerDrag(main, big, st) {
    try {
      main.addEventListener("mousedown", function (ev) {
        startViewerDrag(ev, main, big, st);
      });
      main.addEventListener("dblclick", function () {
        st.zoom = 1;
        centerViewerImage(main, big, st);
      });
    } catch {
      /* ignore */
    }
  }
  function startViewerDrag(ev, main, big, st) {
    ev = ev || window.event;
    if (ev.button != null && ev.button !== 0) return;
    const sx = ev.clientX;
    const sy = ev.clientY;
    const sl = st.panX;
    const sTop = st.panY;
    setGrabbing(main);
    function mv(e2) {
      e2 = e2 || window.event;
      st.panX = sl + (e2.clientX - sx);
      st.panY = sTop + (e2.clientY - sy);
      applyViewerZoom(big, st);
    }
    function up() {
      setGrab(main);
      if (window.removeEventListener) {
        window.removeEventListener("mousemove", mv);
        window.removeEventListener("mouseup", up);
      }
    }
    if (window.addEventListener) {
      window.addEventListener("mousemove", mv);
      window.addEventListener("mouseup", up);
    }
    if (ev.preventDefault) {
      try {
        ev.preventDefault();
      } catch {
        /* ignore */
      }
    }
  }
  function setGrabbing(main) {
    try {
      main.style.cursor = "grabbing";
    } catch {
      /* ignore */
    }
  }
  function setGrab(main) {
    try {
      main.style.cursor = "grab";
    } catch {
      /* ignore */
    }
  }
  function watchViewerResize(main, big, st) {
    try {
      if (!window.ResizeObserver) return;
      const ro = new window.ResizeObserver(function () {
        if (isDetached(main, ro)) return;
        shiftViewerOnResize(main, big, st);
      });
      viewerRO = ro;
      ro.observe(main);
    } catch {
      /* ignore */
    }
  }
  function isDetached(main, ro) {
    try {
      if (!document.contains(main)) {
        try {
          ro.disconnect();
        } catch {
          /* ignore */
        }
        return true;
      }
    } catch {
      /* ignore */
    }
    return false;
  }
  function initViewerZoom(main, big) {
    if (!big) return;
    const st = newViewerZoom();
    try {
      main.style.overflow = "hidden";
    } catch {
      /* ignore */
    }
    setGrab(main);
    bindViewerWheel(main, big, st);
    bindViewerDrag(main, big, st);
    try {
      centerViewerImage(main, big, st);
    } catch {
      /* ignore */
    }
    try {
      if (window.requestAnimationFrame) {
        window.requestAnimationFrame(function () {
          centerViewerImage(main, big, st);
        });
      }
    } catch {
      /* ignore */
    }
    watchViewerResize(main, big, st);
  }

  function saveTexturePng(Rr, entry) {
    try {
      const src = texSource(Rr, entry);
      if (!src) return;
      const url = src.canvas.toDataURL("image/png");
      const a = document.createElement("a");
      a.href = url;
      a.download = texName(entry, Rr) + ".png";
      if (document.body) document.body.appendChild(a);
      a.click();
      try {
        if (a.remove) a.remove();
      } catch {
        /* ignore */
      }
    } catch {
      /* ignore */
    }
  }
  function renderViewerTools(body, Rr, entry) {
    const tools = document.createElement("div");
    tools.className = "tex-viewer-tools";
    const save = document.createElement("button");
    save.className = "btn btn-secondary";
    save.textContent = "Save PNG";
    save.title = "save texture as PNG";
    save.onclick = function () {
      saveTexturePng(Rr, entry);
    };
    tools.appendChild(save);
    body.appendChild(tools);
  }

  function kv(meta, k, v) {
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
    meta.appendChild(r);
  }
  function appendViewerMeta(body, Rr, entry) {
    const meta = document.createElement("div");
    meta.className = "mdetail-top tex-viewer-meta";
    let alias = null;
    try {
      if (g.RDXScene && g.RDXScene.texNameFor) {
        alias = g.RDXScene.texNameFor(Rr, entry.ti, entry.texid);
      }
    } catch {
      /* ignore */
    }
    kv(meta, "Name", alias || "—");
    kv(meta, "Size", entry.width ? entry.width + " × " + entry.height : "—");
    kv(meta, "Format", entry.fmt);
    kv(meta, "Alpha", entry.kind);
    kv(
      meta,
      "Variant",
      entry.variant != null ? "palette " + entry.variant : "—",
    );
    body.appendChild(meta);
  }
  function appendUsedByRow(utb, mti) {
    const ur = document.createElement("tr");
    const name = document.createElement("td");
    name.textContent = mdlName(mti);
    ur.appendChild(name);
    const goCell = document.createElement("td");
    goCell.appendChild(gotoModelButton(mti));
    ur.appendChild(goCell);
    utb.appendChild(ur);
  }
  function gotoModelButton(mti) {
    const go = document.createElement("button");
    go.className = "icon-btn";
    go.textContent = "◎";
    go.title = "center camera on model";
    go.onclick = (function (t2) {
      return function (ev) {
        if (ev) ev.stopPropagation();
        g.AppSelection.setSelected(t2);
        g.AppNavigation.gotoTable(t2);
      };
    })(mti);
    return go;
  }
  function renderViewerUsedBy(body, byTex, entry) {
    const users = byTex[entry.ti] || [];
    if (!users.length) return;
    const ut = document.createElement("table");
    ut.className = "mdetail-table tex-table tex-usedby";
    const uth = document.createElement("thead");
    const uhr = document.createElement("tr");
    const umh = document.createElement("th");
    umh.className = "label";
    umh.textContent = "Model";
    uhr.appendChild(umh);
    uhr.appendChild(document.createElement("th")).className = "label";
    uth.appendChild(uhr);
    ut.appendChild(uth);
    const utb = document.createElement("tbody");
    ut.appendChild(utb);
    users.forEach(function (mti) {
      appendUsedByRow(utb, mti);
    });
    body.appendChild(ut);
  }

  function renderViewer(body) {
    const st = resolveViewerState(body);
    if (!st) return;
    renderViewerNav(body, st.S, st.entries, st.sel, st.entry);
    const view = renderViewerMain(body, st.Rr, st.entry);
    initViewerZoom(view.main, view.big);
    renderViewerTools(body, st.Rr, st.entry);
    appendViewerMeta(body, st.Rr, st.entry);
    renderViewerUsedBy(body, st.byTex, st.entry);
  }

  function renderTexturesPanel(body) {
    const res = viewerResource(body);
    if (!res) return;
    const entries = readViewerEntries(body, res.Rr);
    if (!entries) return;
    selByFile[res.S.active] = getSel(res.S.active, entries.length);
    renderToolbar(body, entries.length);
    if (mode === "details") renderDetails(body, res.S, res.Rr, entries);
    else renderList(body, res.S, res.Rr, entries);
  }
  function register() {
    g.Panels.register("textures", "Textures", renderTexturesPanel, {
      x: 10,
      y: 10,
      visible: false,
    });
    g.Panels.register("texviewer", "Texture Viewer", renderViewer, {
      x: 360,
      y: 10,
      visible: false,
      resizable: true,
    });
  }
  g.AppTexturesPanel = {
    register: register,
    openTex: openTex,
    texInfo: texInfo,
  };
})(window);
