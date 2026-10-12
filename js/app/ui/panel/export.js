(function (g) {
  "use strict";
  function register() {
    g.Panels.registerDialog("export", "Export glTF 2.0", function (body) {
      const S = g.AppState;
      const info = g.AppExport.getExportInfo();
      if (!info.hasFile) {
        body.textContent = "No file loaded";
        return;
      }
      const saved = g.AppSetting.loadExportOpts();
      let selInit = saved.selectedOnly;
      if (S.exportOverride && S.exportOverride.selectedOnly !== undefined) {
        selInit = !!S.exportOverride.selectedOnly;
      }
      S.exportOverride = null;
      const infoLine = document.createElement("div");
      infoLine.className = "muted";
      infoLine.textContent =
        info.active +
        " · " +
        info.total +
        " models" +
        (info.selected ? " · " + info.selected + " selected" : "");
      body.appendChild(infoLine);
      const selLab = document.createElement("label");
      selLab.className = "tog";
      const selBox = document.createElement("input");
      selBox.type = "checkbox";
      selBox.checked = selInit;
      selLab.appendChild(selBox);
      selLab.appendChild(
        document.createTextNode(
          " Selected models only" +
            (info.selected ? " (" + info.selected + ")" : ""),
        ),
      );
      body.appendChild(selLab);
      const texLab = document.createElement("label");
      texLab.className = "tog";
      const texBox = document.createElement("input");
      texBox.type = "checkbox";
      texBox.checked = saved.textures;
      texLab.appendChild(texBox);
      texLab.appendChild(document.createTextNode(" Textures"));
      body.appendChild(texLab);
      selBox.onchange = function () {
        g.AppSetting.saveExportOpts({
          selectedOnly: selBox.checked,
          textures: texBox.checked,
        });
      };
      texBox.onchange = function () {
        g.AppSetting.saveExportOpts({
          selectedOnly: selBox.checked,
          textures: texBox.checked,
        });
      };
      const actions = document.createElement("div");
      actions.className = "dlg-actions";
      const btn = document.createElement("button");
      btn.className = "btn btn-primary";
      btn.textContent = "Export";
      const status = document.createElement("div");
      status.className = "muted exp-status";
      btn.onclick = function () {
        g.AppExport.doExportGltf(selBox.checked, texBox.checked, status, btn);
      };
      const close = document.createElement("button");
      close.className = "btn btn-secondary";
      close.textContent = "Close";
      close.onclick = function () {
        g.Panels.closeDialog("export");
      };
      actions.appendChild(btn);
      actions.appendChild(close);
      body.appendChild(actions);
      body.appendChild(status);
    });
  }
  g.AppExportPanel = { register: register };
})(window);
