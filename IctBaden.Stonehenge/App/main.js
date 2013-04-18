require.config({
  paths: {
    'text': 'durandal/amd/text'
  }
});

define(function (require) {
  var app = require('durandal/app'),
        system = require('durandal/system'),
        router = require('durandal/plugins/router');

  system.debug(true);

  app.start().then(function () {

    router.useConvention();
    //%PAGES%

    app.title = '%TITLE%';
    app.adaptToDevice();
    app.setRoot('shell');
  });
});