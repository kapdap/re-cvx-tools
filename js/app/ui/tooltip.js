(function (g) {
  "use strict";
  const DEFAULT_DELAY = 600;
  let tipEl = null;
  let timer = null;
  let pending = null;
  let pendingX = 0;
  let pendingY = 0;
  let bound = false;

  function ensureBound() {
    if (bound) return;
    bound = true;
    try {
      document.addEventListener(
        "scroll",
        function () {
          hide();
        },
        true,
      );
      window.addEventListener("resize", hide);
      document.addEventListener("keydown", function (e) {
        if (!e) return;
        if (e.key === "Escape" || e.keyCode === 27) hide();
      });
      document.addEventListener("contextmenu", function () {
        hide();
      });
    } catch {
      /* ignore */
    }
  }

  function ensureEl() {
    if (tipEl) return tipEl;
    try {
      tipEl = document.createElement("div");
      tipEl.className = "app-tooltip";
      try {
        tipEl.setAttribute("role", "tooltip");
      } catch {
        /* ignore */
      }
      tipEl.style.display = "none";
      document.body.appendChild(tipEl);
    } catch {
      tipEl = null;
    }
    return tipEl;
  }

  function clearTimer() {
    if (timer) {
      try {
        clearTimeout(timer);
      } catch {
        /* ignore */
      }
      timer = null;
    }
    pending = null;
  }

  function hide() {
    clearTimer();
    if (tipEl) {
      try {
        tipEl.style.display = "none";
      } catch {
        /* ignore */
      }
    }
  }

  function isVisible() {
    try {
      return !!(tipEl && tipEl.style.display !== "none");
    } catch {
      return false;
    }
  }

  function place(x, y) {
    const el = ensureEl();
    if (!el) return;
    let w = 0,
      h = 0;
    try {
      el.style.left = "0px";
      el.style.top = "0px";
      const r = el.getBoundingClientRect();
      if (r) {
        w = r.width || 0;
        h = r.height || 0;
      }
    } catch {
      /* ignore */
    }
    let vw = 800,
      vh = 600;
    try {
      vw = window.innerWidth || document.documentElement.clientWidth || vw;
      vh = window.innerHeight || document.documentElement.clientHeight || vh;
    } catch {
      /* ignore */
    }
    let px = (x != null ? x : 0) + 14;
    let py = (y != null ? y : 0) + 16;
    if (px + w > vw - 4) px = Math.max(4, (x != null ? x : vw) - w - 12);
    if (py + h > vh - 4) py = Math.max(4, (y != null ? y : vh) - h - 12);
    if (px < 4) px = 4;
    if (py < 4) py = 4;
    try {
      el.style.left = Math.round(px) + "px";
      el.style.top = Math.round(py) + "px";
    } catch {
      /* ignore */
    }
  }

  function rectCenter(el) {
    try {
      const r = el.getBoundingClientRect();
      if (r) return [r.left + r.width / 2, r.bottom != null ? r.bottom : r.top];
    } catch {
      /* ignore */
    }
    return [0, 0];
  }

  function renderContent(el, tip) {
    try {
      el.textContent = "";
      if (tip == null) return;
      if (typeof tip === "string") {
        const b = document.createElement("div");
        b.className = "app-tooltip-body";
        b.textContent = tip;
        el.appendChild(b);
        return;
      }
      if (tip.title) {
        const t = document.createElement("div");
        t.className = "app-tooltip-title";
        t.textContent = tip.title;
        el.appendChild(t);
      }
      const body = tip.body != null ? tip.body : tip.text;
      if (body) {
        const bb = document.createElement("div");
        bb.className = "app-tooltip-body";
        bb.textContent = body;
        el.appendChild(bb);
      }
    } catch {
      /* ignore */
    }
  }

  function showPending() {
    timer = null;
    const p = pending;
    pending = null;
    if (!p || !p.el) return;
    try {
      if (typeof document !== "undefined" && document.contains) {
        if (!document.contains(p.el)) return;
      }
    } catch {
      /* ignore */
    }
    const el = ensureEl();
    if (!el) return;
    renderContent(el, p.tip);
    try {
      el.style.display = "block";
    } catch {
      /* ignore */
    }
    const xy = p.x != null ? [p.x, p.y] : rectCenter(p.el);
    place(xy[0], xy[1]);
  }

  function delayOf(p) {
    if (p && p.delay != null) {
      const d = +p.delay;
      if (!isNaN(d) && d >= 0) return d;
    }
    return DEFAULT_DELAY;
  }

  function arm(el, tip, delay, x, y) {
    ensureBound();
    ensureEl();
    clearTimer();
    pending = { el: el, tip: tip, delay: delay, x: x, y: y };
    pendingX = x;
    pendingY = y;
    const d = delayOf(pending);
    if (d <= 0) {
      showPending();
      return;
    }
    timer = setTimeout(showPending, d);
  }

  function track(el, x, y) {
    pendingX = x;
    pendingY = y;
    if (pending && pending.el === el) {
      pending.x = x;
      pending.y = y;
    }
    if (isVisible()) {
      try {
        place(x, y);
      } catch {
        /* ignore */
      }
    }
  }

  function normTip(tip, opts) {
    if (opts && (opts.title || opts.body || opts.text)) return opts;
    return tip;
  }

  function attach(el, tip, opts) {
    if (!el) return null;
    ensureBound();
    detach(el);
    let delay = null;
    try {
      if (opts && opts.delay != null) delay = +opts.delay;
      else if (tip && typeof tip === "object" && tip.delay != null) {
        delay = +tip.delay;
      }
    } catch {
      /* ignore */
    }
    const content = normTip(tip, opts);
    function onEnter(e) {
      let x = e && e.clientX != null ? e.clientX : null;
      let y = e && e.clientY != null ? e.clientY : null;
      if (x == null) {
        const c = rectCenter(el);
        x = c[0];
        y = c[1];
      }
      arm(el, content, delay, x, y);
    }
    function onMove(e) {
      if (!e) return;
      try {
        if (
          e.target &&
          e.target !== el &&
          e.target.closest &&
          e.target.closest(".icon-btn")
        ) {
          if (pending && pending.el === el && !isVisible()) clearTimer();
          return;
        }
      } catch {
        /* ignore */
      }
      const x = e.clientX != null ? e.clientX : pendingX;
      const y = e.clientY != null ? e.clientY : pendingY;
      if (pending && pending.el === el) {
        track(el, x, y);
        return;
      }
      if (!isVisible()) arm(el, content, delay, x, y);
      else track(el, x, y);
    }
    function onLeave() {
      hide();
    }
    function onDown() {
      hide();
    }
    try {
      el.addEventListener("mouseenter", onEnter);
      el.addEventListener("mousemove", onMove);
      el.addEventListener("mouseleave", onLeave);
      el.addEventListener("focus", onEnter);
      el.addEventListener("blur", onLeave);
      el.addEventListener("mousedown", onDown);
      el.addEventListener("click", onDown);
    } catch {
      /* ignore */
    }
    try {
      el._appTip = {
        onEnter: onEnter,
        onMove: onMove,
        onLeave: onLeave,
        onDown: onDown,
      };
    } catch {
      /* ignore */
    }
    return el;
  }

  function detach(el) {
    if (!el) return;
    let h = null;
    try {
      h = el._appTip || null;
    } catch {
      /* ignore */
    }
    if (!h) return;
    try {
      el.removeEventListener("mouseenter", h.onEnter);
      el.removeEventListener("mousemove", h.onMove);
      el.removeEventListener("mouseleave", h.onLeave);
      el.removeEventListener("focus", h.onEnter);
      el.removeEventListener("blur", h.onLeave);
      el.removeEventListener("mousedown", h.onDown);
      el.removeEventListener("click", h.onDown);
    } catch {
      /* ignore */
    }
    try {
      el._appTip = null;
    } catch {
      /* ignore */
    }
  }

  g.AppTooltip = {
    attach: attach,
    hide: hide,
  };
})(window);
