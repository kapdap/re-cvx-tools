(function (g) {
  "use strict";
  const panels = {};
  let order = [],
    container = null,
    menuEl = null,
    topZ = 10;
  const LSKEY = "rdxweb.panels";
  function loadPos() {
    try {
      return JSON.parse(localStorage.getItem(LSKEY) || "{}");
    } catch {
      return {};
    }
  }
  function savePos(p) {
    if (p.dialog || p.transient) return;
    try {
      const all = loadPos();
      all[p.id] = {
        x: parseInt(p.el.style.left, 10) || 0,
        y: parseInt(p.el.style.top, 10) || 0,
        visible: p.visible,
        w:
          p.resizable && p.el.style.width
            ? parseInt(p.el.style.width, 10) || null
            : null,
        h:
          p.resizable && p.el.style.height
            ? parseInt(p.el.style.height, 10) || null
            : null,
      };
      localStorage.setItem(LSKEY, JSON.stringify(all));
    } catch {
      /* ignore */
    }
  }
  function init(containerId, panelsMenuId) {
    container = document.getElementById(containerId);
    menuEl = document.getElementById(panelsMenuId);
  }
  function register(id, title, render, opts) {
    panels[id] = {
      id: id,
      title: title,
      render: render,
      dialog: false,
      visible: !(opts && opts.visible === false),
      defaultVisible: !(opts && opts.visible === false),
      el: null,
      wrap: null,
      body: null,
      x: (opts && opts.x) != null ? opts.x : null,
      y: (opts && opts.y) != null ? opts.y : null,
      w: (opts && opts.w) != null ? opts.w : null,
      h: (opts && opts.h) != null ? opts.h : null,
      resizable: !!(opts && opts.resizable),
      nomenu: !!(opts && opts.nomenu),
      transient: !!(opts && opts.transient),
      layerKey: opts && opts.layerKey != null ? opts.layerKey : null,
    };
    order.push(id);
  }
  function layerOn(key) {
    try {
      return !!(g.AppState && g.AppState.opts && g.AppState.opts[key]);
    } catch {
      return true;
    }
  }
  function toggleLayerKey(key) {
    const S = g.AppState;
    if (!S || !S.opts || S.opts[key] === undefined) return;
    S.opts[key] = !S.opts[key];
    try {
      g.AppBuilder.show(S.active, false);
    } catch {
      /* ignore */
    }
    try {
      refresh("layers");
    } catch {
      /* ignore */
    }
    syncLayerEyes();
  }
  function syncLayerEyes() {
    order.forEach(function (id) {
      const p = panels[id];
      if (!p || !p.layerKey || !p.el) return;
      try {
        const eye = p.el.querySelector(".layer-eye");
        if (eye) eye.classList.toggle("off", !layerOn(p.layerKey));
      } catch {
        /* ignore */
      }
    });
  }
  function registerDialog(id, title, render) {
    register(id, title, render, null);
    panels[id].dialog = true;
    panels[id].visible = false;
  }
  function buildPanel(p, i) {
    const saved = loadPos();
    const id = p.id;
    const el = document.createElement("div");
    el.className = "panel" + (p.resizable ? " resizable" : "");
    el.id = "panel-" + id;
    const sx =
      !p.transient && saved[id] && saved[id].x != null ? saved[id].x : null;
    const sy =
      !p.transient && saved[id] && saved[id].y != null ? saved[id].y : null;
    el.style.left =
      (sx != null
        ? sx
        : p.x !== null && p.x !== undefined
          ? p.x
          : 10 + (i % 4) * 10) + "px";
    el.style.top =
      (sy != null
        ? sy
        : p.y !== null && p.y !== undefined
          ? p.y
          : 10 + (i % 4) * 28) + "px";
    if (!p.transient && saved[id] && saved[id].visible !== undefined) {
      p.visible = !!saved[id].visible;
    }
    try {
      el.style.zIndex = ++topZ;
    } catch {
      /* ignore */
    }
    const head = document.createElement("div");
    head.className = "panel-head";
    const t = document.createElement("span");
    t.textContent = p.title;
    head.appendChild(t);
    if (p.layerKey) {
      const eye = document.createElement("button");
      eye.className = "icon-btn icon-btn-sm layer-eye";
      eye.textContent = "👁";
      eye.title = "toggle " + p.title + " layer";
      if (!layerOn(p.layerKey)) eye.classList.add("off");
      eye.onclick = (function (key) {
        return function (e) {
          if (e && e.stopPropagation) {
            try {
              e.stopPropagation();
            } catch {
              /* ignore */
            }
          }
          toggleLayerKey(key);
        };
      })(p.layerKey);
      head.appendChild(eye);
    }
    const x = document.createElement("button");
    x.className = "icon-btn icon-btn-sm";
    x.textContent = "✕";
    if (p.transient) {
      x.title = "close panel";
      x.onclick = function () {
        destroy(id);
      };
    } else {
      x.onclick = function () {
        setVisible(id, false);
      };
    }
    head.appendChild(x);
    el.appendChild(head);
    makeDraggable(p, el, head);
    const body = document.createElement("div");
    body.className = "panel-body";
    el.appendChild(body);
    p.el = el;
    p.body = body;
    if (p.resizable) {
      const sw =
        !p.transient && saved[id] && saved[id].w != null ? saved[id].w : p.w;
      const sh =
        !p.transient && saved[id] && saved[id].h != null ? saved[id].h : p.h;
      if (sw != null) el.style.width = sw + "px";
      if (sh != null) el.style.height = sh + "px";
      if (sh != null) {
        try {
          el.classList.add("sized");
        } catch {
          /* ignore */
        }
      }
      makeResizable(p, el);
    }
    try {
      if (container) container.appendChild(el);
    } catch {
      /* ignore */
    }
    if (!p.visible) el.style.display = "none";
    try {
      p.render(body);
    } catch (e) {
      body.textContent = "render error: " + e.message;
    }
  }
  function build() {
    order.forEach(function (id, i) {
      const p = panels[id];
      if (p.dialog) {
        buildDialog(p);
        return;
      }
      buildPanel(p, i);
    });
    rebuildMenu();
    bindDialogKeys();
  }
  function create(id, title, render, opts) {
    const ex = panels[id];
    if (ex) {
      if (ex.dialog) {
        setVisible(id, true);
        return ex;
      }
      try {
        ex.title = title;
      } catch {
        /* ignore */
      }
      try {
        const ht = ex.el ? ex.el.querySelector(".panel-head span") : null;
        if (ht) ht.textContent = title;
      } catch {
        /* ignore */
      }
      setVisible(id, true);
      refresh(id);
      focusPanel(ex);
      return ex;
    }
    const p = {
      id: id,
      title: title,
      render: render,
      dialog: false,
      visible: true,
      el: null,
      wrap: null,
      body: null,
      x: opts && opts.x != null ? opts.x : null,
      y: opts && opts.y != null ? opts.y : null,
      w: opts && opts.w != null ? opts.w : null,
      h: opts && opts.h != null ? opts.h : null,
      resizable: !!(opts && opts.resizable),
      nomenu: true,
      transient: true,
    };
    if (opts && opts.nomenu === false) p.nomenu = false;
    if (opts && opts.transient === false) p.transient = false;
    if (opts && opts.menu) p.nomenu = false;
    panels[id] = p;
    order.push(id);
    if (container) {
      buildPanel(p, order.length - 1);
      rebuildMenu();
    }
    return p;
  }
  function destroy(id) {
    const p = panels[id];
    if (!p) return;
    if (p.dialog) {
      setVisible(id, false);
      return;
    }
    if (!p.transient && !p.nomenu) {
      setVisible(id, false);
      return;
    }
    try {
      if (p.el && p.el.parentNode) p.el.parentNode.removeChild(p.el);
    } catch {
      /* ignore */
    }
    try {
      if (p.wrap && p.wrap.parentNode) p.wrap.parentNode.removeChild(p.wrap);
    } catch {
      /* ignore */
    }
    delete panels[id];
    order = order.filter(function (k) {
      return k !== id;
    });
    rebuildMenu();
  }
  function exists(id) {
    return !!panels[id];
  }
  function focusPanel(p) {
    try {
      if (p && p.el) p.el.style.zIndex = ++topZ;
    } catch {
      /* ignore */
    }
  }
  function buildDialog(p) {
    const wrap = document.createElement("div");
    wrap.className = "dlg-backdrop";
    wrap.id = "panel-" + p.id + "-wrap";
    wrap.style.display = "none";
    const el = document.createElement("div");
    el.className = "panel dlg";
    el.id = "panel-" + p.id;
    try {
      el.setAttribute("role", "dialog");
      el.setAttribute("aria-modal", "true");
      el.setAttribute("aria-label", p.title);
    } catch {
      /* ignore */
    }
    try {
      el.tabIndex = -1;
    } catch {
      /* ignore */
    }
    const head = document.createElement("div");
    head.className = "panel-head";
    const t = document.createElement("span");
    t.textContent = p.title;
    head.appendChild(t);
    const x = document.createElement("button");
    x.className = "icon-btn icon-btn-sm";
    x.textContent = "✕";
    x.onclick = function () {
      setVisible(p.id, false);
    };
    head.appendChild(x);
    el.appendChild(head);
    const body = document.createElement("div");
    body.className = "panel-body";
    el.appendChild(body);
    p.el = el;
    p.body = body;
    p.wrap = wrap;
    wrap.appendChild(el);
    container.appendChild(wrap);
    wrap.addEventListener("click", function (e) {
      if (e && e.target === wrap) setVisible(p.id, false);
    });
  }
  function makeDraggable(p, el, head) {
    p.el = el;
    head.style.cursor = "move";
    head.onmousedown = function (e) {
      if (e.target.classList && e.target.classList.contains("icon-btn")) return;
      try {
        el.style.zIndex = ++topZ;
      } catch {
        /* ignore */
      }
      e.preventDefault();
      const sx = e.clientX - el.offsetLeft,
        sy = e.clientY - el.offsetTop;
      function mv(ev) {
        const host = container.getBoundingClientRect();
        let nx = ev.clientX - sx,
          ny = ev.clientY - sy;
        if (nx < 0) nx = 0;
        if (ny < 0) ny = 0;
        if (nx > host.width - 40) nx = host.width - 40;
        if (ny > host.height - 20) ny = host.height - 20;
        el.style.left = nx + "px";
        el.style.top = ny + "px";
      }
      function up() {
        document.removeEventListener("mousemove", mv);
        document.removeEventListener("mouseup", up);
        savePos(p);
      }
      document.addEventListener("mousemove", mv);
      document.addEventListener("mouseup", up);
    };
  }
  function makeResizable(p, el) {
    const grip = document.createElement("div");
    grip.className = "panel-resize-handle";
    grip.title = "resize panel";
    el.appendChild(grip);
    grip.addEventListener("mousedown", function (e) {
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
      try {
        el.style.zIndex = ++topZ;
      } catch {
        /* ignore */
      }
      const startX = e.clientX,
        startY = e.clientY;
      const r = el.getBoundingClientRect();
      const startW = r.width,
        startH = r.height;
      function mv(ev) {
        let nw = Math.round(startW + (ev.clientX - startX));
        let nh = Math.round(startH + (ev.clientY - startY));
        if (nw < 180) nw = 180;
        if (nh < 120) nh = 120;
        try {
          const host = container.getBoundingClientRect();
          if (nw > host.width - 20) {
            nw = Math.max(180, Math.floor(host.width - 20));
          }
          if (nh > host.height - 20) {
            nh = Math.max(120, Math.floor(host.height - 20));
          }
        } catch {
          /* ignore */
        }
        el.style.width = nw + "px";
        el.style.height = nh + "px";
      }
      function up() {
        document.removeEventListener("mousemove", mv);
        document.removeEventListener("mouseup", up);
        try {
          if (el.style.height) el.classList.add("sized");
        } catch {
          /* ignore */
        }
        savePos(p);
      }
      document.addEventListener("mousemove", mv);
      document.addEventListener("mouseup", up);
    });
  }
  function refresh(id) {
    if (id) {
      const p = panels[id];
      if (p && p.body) {
        p.body.innerHTML = "";
        try {
          p.render(p.body);
        } catch {
          /* ignore */
        }
      }
      syncLayerEyes();
      return;
    }
    order.forEach(function (k) {
      refresh(k);
    });
    syncLayerEyes();
  }
  function setVisible(id, v) {
    const p = panels[id];
    if (!p) return;
    p.visible = !!v;
    if (p.dialog) {
      if (p.wrap) {
        if (p.visible) {
          p.body.innerHTML = "";
          try {
            p.render(p.body);
          } catch (e) {
            p.body.textContent = "render error: " + e.message;
          }
        }
        p.wrap.style.display = p.visible ? "flex" : "none";
        if (p.visible) {
          try {
            p.el.focus();
          } catch {
            /* ignore */
          }
        }
      }
      return;
    }
    if (p.el) p.el.style.display = v ? "flex" : "none";
    if (v) focusPanel(p);
    savePos(p);
    rebuildMenu();
  }
  function openDialog(id) {
    setVisible(id, true);
  }
  function closeDialog(id) {
    setVisible(id, false);
  }
  function anyDialogOpen() {
    for (let i = 0; i < order.length; i++) {
      if (panels[order[i]].dialog && panels[order[i]].visible) return true;
    }
    return false;
  }
  function closeTopDialog() {
    for (let i = order.length - 1; i >= 0; i--) {
      const p = panels[order[i]];
      if (p.dialog && p.visible) {
        setVisible(p.id, false);
        return true;
      }
    }
    return false;
  }
  let keysBound = false;
  function bindDialogKeys() {
    if (keysBound) return;
    keysBound = true;
    try {
      document.addEventListener("keydown", function (e) {
        if (!e) return;
        if (e.key !== "Escape" && e.keyCode !== 27) return;
        closeTopDialog();
      });
    } catch {
      /* ignore */
    }
  }
  function isVisible(id) {
    return !!(panels[id] && panels[id].visible);
  }
  function rebuildMenu() {
    if (!menuEl) return;
    menuEl.innerHTML = "";
    order.forEach(function (id) {
      const p = panels[id];
      if (p.dialog || p.nomenu) return;
      const a = document.createElement("a");
      a.href = "#";
      a.className = "menu-item";
      const eye = document.createElement("span");
      eye.className = "icon-btn" + (p.visible ? "" : " off");
      eye.textContent = "👁";
      a.appendChild(eye);
      a.appendChild(document.createTextNode(" " + p.title));
      a.onclick = function (e) {
        e.preventDefault();
        setVisible(id, !p.visible);
      };
      menuEl.appendChild(a);
    });
    const sep = document.createElement("div");
    sep.className = "sep";
    menuEl.appendChild(sep);
    const r = document.createElement("a");
    r.href = "#";
    r.className = "menu-item";
    r.textContent = "Reset layout";
    r.onclick = function (e) {
      e.preventDefault();
      resetLayout();
    };
    menuEl.appendChild(r);
  }
  function resetLayout() {
    try {
      localStorage.removeItem(LSKEY);
    } catch {
      /* ignore */
    }
    order.forEach(function (id, i) {
      const p = panels[id];
      if (p.dialog || p.transient) return;
      p.visible = p.defaultVisible !== false;
      if (p.el) {
        p.el.style.display = p.visible ? "flex" : "none";
        p.el.style.left =
          (p.x !== null && p.x !== undefined ? p.x : 10 + (i % 4) * 10) + "px";
        p.el.style.top =
          (p.y !== null && p.y !== undefined ? p.y : 10 + (i % 4) * 28) + "px";
        if (p.resizable) {
          p.el.style.width = p.w != null ? p.w + "px" : "";
          p.el.style.height = p.h != null ? p.h + "px" : "";
          try {
            p.el.classList.remove("sized");
          } catch {
            /* ignore */
          }
        }
      }
    });
    rebuildMenu();
  }
  g.Panels = {
    init: init,
    register: register,
    registerDialog: registerDialog,
    build: build,
    refresh: refresh,
    setVisible: setVisible,
    openDialog: openDialog,
    closeDialog: closeDialog,
    anyDialogOpen: anyDialogOpen,
    isVisible: isVisible,
    create: create,
    destroy: destroy,
    exists: exists,
  };
})(window);
