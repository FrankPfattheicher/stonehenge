﻿requirejs.config({
  paths: {
    'text':         '/app/00000000-0000-0000-0000-000000000000/lib/require/js/text',
    'durandal':     '/app/00000000-0000-0000-0000-000000000000/durandal/js',
    'plugins':      '/app/00000000-0000-0000-0000-000000000000/plugins',
    'transitions':  '/app/00000000-0000-0000-0000-000000000000/transitions',
    'knockout':     '/app/00000000-0000-0000-0000-000000000000/lib/knockout/js/knockout-3.0.0',
    'bootstrap':    '/app/00000000-0000-0000-0000-000000000000/lib/bootstrap/js/bootstrap',
    'jquery':       '/app/00000000-0000-0000-0000-000000000000/lib/jquery/js/jquery-1.10.2',
    'throttle':     '/app/00000000-0000-0000-0000-000000000000/lib/jquery.ba-throttle-debounce/jquery.ba-throttle-debounce',
    'flot':         '/app/00000000-0000-0000-0000-000000000000/lib/flot/js/jquery.flot',
    'flot_resize':  '/app/00000000-0000-0000-0000-000000000000/lib/flot/jquery.flot.resize',
    'flot_time':    '/app/00000000-0000-0000-0000-000000000000/lib/flot/jquery.flot.time',
  },
  shim: {
    'bootstrap': {
      deps: ['jquery'],
      exports: 'jQuery'
    }
  },
  waitSeconds: 15
});

define(['durandal/system', 'durandal/app', 'durandal/viewLocator'], function (system, app, viewLocator) {
  //>>excludeStart("build", true);
  system.debug(true);
  //>>excludeEnd("build");

  app.title = '%TITLE%';

  require(['bootstrap']);
  require(['throttle']);
  require(['flot'], function(_) {
    require(['flot_resize']);
    require(['flot_time']);
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

    var dialog = require('plugins/dialog');
    dialog.MessageBox.defaultViewMarkup = [
        '<div data-view="plugins/messageBox" class="messageBox">',
            '<div class="modal-header">',
                '<h3 data-bind="text: title"></h3>',
            '</div>',
            '<div class="modal-body">',
                '<p class="message" data-bind="html: message"></p>',
            '</div>',
            '<div class="modal-footer" data-bind="foreach: options">',
                '<button class="btn" data-bind="click: function () { $parent.selectOption($data); }, text: $data, css: { \'btn-primary\': $index() == 0, autofocus: $index() == 0 }"></button>',
            '</div>',
        '</div>'
    ].join('\n');

    //Show the app by setting the root view model for our application.
    app.setRoot('shell');
  });
});
