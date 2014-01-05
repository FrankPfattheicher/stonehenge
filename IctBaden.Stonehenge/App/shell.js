
define(['plugins/router'], function (router) {
  return {
    router: router,
        activate: function () {
          router.map([
              //%PAGES%
            ]).buildNavigationModel()
            .mapUnknownRoutes('%STARTPAGE%', 'not-found');
          return router.activate('%STARTPAGE%');
        }
    };
});
