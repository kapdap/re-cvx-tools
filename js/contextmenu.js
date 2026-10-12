(function (g) {
  "use strict";
  let rootEl = null,
    bound = false;
  function ensureBound() {
    if (bound) return;
    bound = true;
    try {
      document.addEventListener(
        "mousedown",
        function (e) {
          if (rootEl && !rootEl.contains(e.target)) hide();
        },
        true,
      );
      document.addEventListener(
        "contextmenu",
        function (e) {
          if (rootEl && !rootEl.contains(e.target)) hide();
        },
        true,
      );
      document.addEventListener(
        "keydown",
        function (e) {
          if (!e) return;
          if (e.key === "Escape" || e.keyCode === 27) hide();
        },
        true,
      );
      window.addEventListener("resize", hide);
      window.addEventListener("blur", hide);
    } catch {
      /* ignore */
    }
  }
  function hide() {
    try {
      if (rootEl && rootEl.parentNode) rootEl.parentNode.removeChild(rootEl);
    } catch {
      /* ignore */
    }
    rootEl = null;
  }
  function clampXY(x, y, w, h) {
    let vw = 800,
      vh = 600;
    try {
      vw = window.innerWidth || document.documentElement.clientWidth || vw;
      vh = window.innerHeight || document.documentElement.clientHeight || vh;
    } catch {
      /* ignore */
    }
    if (x + w > vw - 4) x = Math.max(4, vw - w - 4);
    if (y + h > vh - 4) y = Math.max(4, vh - h - 4);
    if (x < 4) x = 4;
    if (y < 4) y = 4;
    return [x, y];
  }
  function makeMenuEl(cls) {
    const el = document.createElement("div");
    el.className = cls;
    return el;
  }
  function fitSub(sub) {
    if (!sub) return;
    try {
      sub.classList.remove("opens-left");
      sub.style.top = "";
      let vw = 800,
        vh = 600;
      try {
        vw = window.innerWidth || document.documentElement.clientWidth || vw;
        vh = window.innerHeight || document.documentElement.clientHeight || vh;
      } catch {
        /* ignore */
      }
      let r = sub.getBoundingClientRect();
      if (r && r.right > vw - 4) sub.classList.add("opens-left");
      r = sub.getBoundingClientRect();
      if (r && r.bottom > vh - 4) {
        sub.style.top = Math.floor(vh - 4 - r.bottom) + "px";
      }
    } catch {
      /* ignore */
    }
  }
  function makeItem(def) {
    def = def || {};
    if (def.sep) {
      const s = document.createElement("div");
      s.className = "sep";
      return s;
    }
    const hasSub = !!(def.submenu && def.submenu.length);
    const it = document.createElement("div");
    it.className =
      "ctxitem menu-item" +
      (def.disabled ? " disabled" : "") +
      (hasSub ? " hassub" : "");
    if (def.title) it.title = def.title;
    const lab = document.createElement("span");
    lab.className = "grow truncate";
    lab.textContent = def.label || "";
    it.appendChild(lab);
    if (hasSub) {
      const ar = document.createElement("span");
      ar.className = "ctxarrow";
      ar.textContent = "›";
      it.appendChild(ar);
      const sub = makeMenuEl("ctxmenu ctxsub menu-surface");
      def.submenu.forEach(function (d) {
        sub.appendChild(makeItem(d));
      });
      it.appendChild(sub);
      try {
        it.addEventListener("mouseenter", function () {
          if (!def.disabled) fitSub(sub);
        });
        it.addEventListener("click", function (e) {
          if (e) e.stopPropagation();
          if (!def.disabled) fitSub(sub);
        });
      } catch {
        /* ignore */
      }
    } else {
      try {
        it.addEventListener("click", function (e) {
          if (e) e.stopPropagation();
          if (def.disabled) return;
          const fn = def.onclick;
          hide();
          try {
            if (fn) fn(e);
          } catch {
            /* ignore */
          }
        });
      } catch {
        /* ignore */
      }
    }
    return it;
  }
  function show(x, y, items) {
    ensureBound();
    hide();
    rootEl = makeMenuEl("ctxmenu menu-surface");
    (items || []).forEach(function (d) {
      rootEl.appendChild(makeItem(d));
    });
    try {
      document.body.appendChild(rootEl);
    } catch {
      rootEl = null;
      return null;
    }
    let w = 180,
      h = 40;
    try {
      const r = rootEl.getBoundingClientRect();
      if (r && r.width) w = r.width;
      if (r && r.height) h = r.height;
    } catch {
      /* ignore */
    }
    const p = clampXY(x, y, w, h);
    rootEl.style.left = p[0] + "px";
    rootEl.style.top = p[1] + "px";
    return rootEl;
  }
  g.ContextMenu = { show: show, hide: hide };
})(window);
