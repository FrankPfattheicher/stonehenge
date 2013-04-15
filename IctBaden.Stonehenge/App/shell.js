
define(function (require) {
  var router = require('durandal/plugins/router');

  return {
    router: router,
    activate: function () {
      return router.activate('about');
    }
  };
});
