(function (g) {
  "use strict";
  function register() {
    g.Panels.register(
      "files",
      "Files",
      function (body) {
        const S = g.AppState;
        if (!S.order.length) {
          body.textContent = "No files. File > Open or drop .rdx/.rdt";
          return;
        }
        S.order.forEach(function (n) {
          const row = document.createElement("div");
          row.className = "filerow";
          row.oncontextmenu = (function (name) {
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
              g.AppMenu.showFilesContextMenu(cx, cy, name);
              return false;
            };
          })(n);
          const s = document.createElement("span");
          s.className = "grow truncate";
          s.textContent = n;
          if (n === S.active) s.style.fontWeight = "bold";
          row.onclick = function () {
            g.AppFile.setActive(n);
          };
          const x = document.createElement("button");
          x.className = "icon-btn";
          x.textContent = "✕";
          x.title = "remove file";
          x.onclick = function (e) {
            e.stopPropagation();
            g.AppFile.removeFile(n);
          };
          row.appendChild(s);
          row.appendChild(x);
          body.appendChild(row);
        });
      },
      { x: 10, y: 10, visible: false },
    );
  }
  g.AppFilePanel = { register: register };
})(window);
