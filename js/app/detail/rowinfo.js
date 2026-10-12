(function (g) {
  "use strict";
  function addInfoButton(row, title, openFn) {
    const b = document.createElement("button");
    b.className = "icon-btn";
    b.textContent = "ⓘ";
    b.title = title;
    b.onclick = function (e) {
      if (e) e.stopPropagation();
      try {
        openFn();
      } catch {
        /* ignore */
      }
    };
    row.appendChild(b);
    return b;
  }
  function addMarkEye(row, key) {
    const b = document.createElement("button");
    b.className = "icon-btn";
    b.textContent = "👁";
    b.title = "toggle visibility";
    try {
      if (g.AppState.markVis[key] === false) b.classList.add("off");
    } catch {
      /* ignore */
    }
    b.onclick = (function (k, btn) {
      return function (e) {
        if (e) e.stopPropagation();
        try {
          const v = !(g.AppState.markVis[k] !== false);
          g.AppMenu.setMarkVisible(k, v);
          btn.classList.toggle("off", !v);
        } catch {
          /* ignore */
        }
      };
    })(key, b);
    row.appendChild(b);
    return b;
  }
  g.AppRowInfo = { addInfoButton: addInfoButton, addMarkEye: addMarkEye };
})(window);
