(function (g) {
  "use strict";
  function closeMenus() {
    document.querySelectorAll(".menu.open").forEach(function (m) {
      m.classList.remove("open");
    });
  }
  function initToolbar() {
    const S = g.AppState;
    document.querySelectorAll(".menubtn").forEach(function (b) {
      b.onclick = function (e) {
        e.stopPropagation();
        const m = b.parentElement;
        const was = m.classList.contains("open");
        closeMenus();
        if (!was) m.classList.add("open");
      };
    });
    document.addEventListener("click", closeMenus);
    document.getElementById("menu-open").onclick = function (e) {
      e.preventDefault();
      document.getElementById("pick").click();
      closeMenus();
    };
    document.getElementById("menu-close").onclick = function (e) {
      e.preventDefault();
      if (S.active) g.AppFile.removeFile(S.active);
      closeMenus();
    };
    document.getElementById("menu-closeall").onclick = function (e) {
      e.preventDefault();
      S.order.slice().forEach(g.AppFile.removeFile);
      closeMenus();
    };
    document.querySelectorAll(".hassub > .menu-item").forEach(function (a) {
      a.addEventListener("click", function (e) {
        e.preventDefault();
        e.stopPropagation();
      });
    });
    document.getElementById("menu-export-gltf").onclick = function (e) {
      e.preventDefault();
      g.Panels.openDialog("export");
      closeMenus();
    };
    function buildTextureExportMenu() {
      const sub = document.getElementById("menu-export-textures-sub");
      if (!sub) return;
      sub.innerHTML = "";
      let formats = [];
      try {
        if (g.AppTextureExport) {
          formats = g.AppTextureExport.getSupportedFormats();
        }
      } catch {
        formats = [];
      }
      if (!formats.length) {
        const none = document.createElement("span");
        none.className = "menu-item";
        none.textContent = "No supported formats";
        sub.appendChild(none);
        return;
      }
      formats.forEach(function (f) {
        const a = document.createElement("a");
        a.className = "menu-item";
        a.href = "#";
        a.id = "menu-export-tex-" + f.id;
        a.textContent = f.label;
        a.onclick = (function (id) {
          return function (e) {
            e.preventDefault();
            try {
              g.AppTextureExport.exportTextures(id);
            } catch {
              /* ignore */
            }
            closeMenus();
          };
        })(f.id);
        sub.appendChild(a);
      });
    }
    buildTextureExportMenu();
    function buildOpenRecentMenu() {
      const sub = document.getElementById("menu-open-recent-sub");
      if (!sub) return;
      sub.innerHTML = "";
      const order = (S.order || []).slice();
      if (!order.length) {
        const none = document.createElement("span");
        none.className = "menu-item";
        none.textContent = "No files open";
        sub.appendChild(none);
        return;
      }
      order.forEach(function (n) {
        const a = document.createElement("a");
        a.className = "menu-item";
        a.href = "#";
        a.textContent = (n === S.active ? "✓ " : "") + n;
        a.title = n;
        a.onclick = (function (name) {
          return function (e) {
            e.preventDefault();
            try {
              g.AppFile.setActive(name);
            } catch {
              /* ignore */
            }
            closeMenus();
          };
        })(n);
        sub.appendChild(a);
      });
    }
    buildOpenRecentMenu();
    const recentParent = document.getElementById("menu-open-recent");
    if (recentParent) {
      recentParent.addEventListener("mouseenter", buildOpenRecentMenu);
    }
    document.getElementById("pick").addEventListener("change", function (e) {
      g.AppFile.addFiles(e.target.files);
      e.target.value = "";
    });
    document.getElementById("menu-orbit").onclick = function (e) {
      e.preventDefault();
      g.AppViewport.setViewMode("orbit");
      closeMenus();
    };
    document.getElementById("menu-fps").onclick = function (e) {
      e.preventDefault();
      g.AppViewport.setViewMode("fps");
      closeMenus();
    };
    document.getElementById("menu-reset").onclick = function (e) {
      e.preventDefault();
      g.AppViewport.refit();
      closeMenus();
    };
    function renderLabels() {
      document.getElementById("menu-textured").textContent =
        (S.opts.textured ? "✓ " : "○ ") + "Textured";
      document.getElementById("menu-wireframe").textContent =
        (S.opts.wireframe ? "✓ " : "○ ") + "Wireframe";
      document.getElementById("menu-lighting").textContent =
        (S.opts.gameLights ? "✓ " : "○ ") + "Lighting";
    }
    document.getElementById("menu-textured").onclick = function (e) {
      e.preventDefault();
      S.opts.textured = !S.opts.textured;
      renderLabels();
      g.AppBuilder.show(S.active, false);
    };
    document.getElementById("menu-wireframe").onclick = function (e) {
      e.preventDefault();
      S.opts.wireframe = !S.opts.wireframe;
      renderLabels();
      g.AppBuilder.show(S.active, false);
    };
    document.getElementById("menu-lighting").onclick = function (e) {
      e.preventDefault();
      S.opts.gameLights = !S.opts.gameLights;
      renderLabels();
      g.AppBuilder.show(S.active, false);
    };
    renderLabels();
    document.getElementById("menu-help").onclick = function (e) {
      e.preventDefault();
      g.Panels.setVisible("help", true);
      closeMenus();
    };
    document.getElementById("menu-about").onclick = function (e) {
      e.preventDefault();
      g.Panels.setVisible("about", true);
      closeMenus();
    };
  }
  g.AppToolbar = { initToolbar: initToolbar };
})(window);
