(function (g) {
  "use strict";
  function register() {
    g.Panels.register(
      "about",
      "About",
      function (body) {
        body.innerHTML =
          '<section class="help-section"><h3>RDX Web Viewer</h3><p>RDX Web Viewer is a browser-based viewer for Resident Evil/BioHazard: Code: Veronica `.rdx` and `.rdt` files.</p><p><a href="https://github.com/kapdap/re-cvx-tools/tree/sloppage" target="_blank" rel="noopener">GitHub repo</a></p></section>';
      },
      { x: 20, y: 20, visible: false, nomenu: true },
    );
  }
  g.AppAboutPanel = { register: register };
})(window);
