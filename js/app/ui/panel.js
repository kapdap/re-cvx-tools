(function (g) {
  "use strict";
  function registerPanels() {
    g.AppFilePanel.register();
    g.AppLayerPanel.register();
    g.AppModelPanel.register();
    g.AppTexturesPanel.register();
    g.AppCameraPanel.register();
    g.AppLightPanel.register();
    g.AppPlayerPanel.register();
    g.AppBoundaryPanel.register();
    g.AppAotPanel.register();
    g.AppTriggerPanel.register();
    g.AppRoutePanel.register();
    g.AppHelpPanel.register();
    g.AppAboutPanel.register();
    g.AppExportPanel.register();
  }
  g.AppPanel = { registerPanels: registerPanels };
})(window);
