requirejs.config({
  paths: {
    'text':         '/app/lib/require/js/text',
    'durandal':     '/app/durandal/js',
    'plugins':      '/app/plugins',
    'transitions':  '/app/transitions',
    'knockout':     '/app/lib/knockout/js/knockout-3.3.0',
    'bootstrap': '/app/lib/bootstrap/js/bootstrap',
    'datepicker': '/app/lib/bootstrap/js/bootstrap-datepicker',
    'jquery': '/app/lib/jquery/js/jquery-2.1.3',
    'throttle':     '/app/lib/jquery.ba-throttle-debounce/jquery.ba-throttle-debounce',
    'flot':         '/app/lib/flot/js/jquery.flot',
    'flot_resize':  '/app/lib/flot/jquery.flot.resize',
    'flot_time':    '/app/lib/flot/jquery.flot.time',
    'flot_labels':  '/app/lib/flot/jquery.flot.axislabels'
  },
  shim: {
    'bootstrap': {
      deps: ['jquery'],
      exports: 'jQuery'
    }
  },
  waitSeconds: 30
});

define(['durandal/system', 'durandal/app', 'durandal/viewLocator'], function (system, app, viewLocator) {
  //>>excludeStart("build", true);
  system.debug(true);
  //>>excludeEnd("build");

  app.title = '%TITLE%';

  require(['bootstrap']);
  require(['datepicker']);
  require(['throttle']);
  require(['flot'], function(_) {
    require(['flot_resize']);
    require(['flot_time']);
    require(['flot_labels']);
  });

  //specify which plugins to install and their configuration
  app.configurePlugins({
    router: true,
    dialog: true,
    widget: true
  });


  app.start().then(function () {
    //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
    //Look for partial views in a 'views' folder in the root.
    // (modulesPath, viewsPath, areasPath)
    viewLocator.useConvention('', '', '');

    viewLocator.convertModuleIdToViewId = function (moduleId) {
        return moduleId;// + '?stonehenge_id=' + stonehenge_id;
    };

    //MessageBox=HTML

    //Show the app by setting the root view model for our application.
    app.setRoot('shell');
  });
});
