
define(['plugins/router'], function (router) {
  return {
    router: router,
        activate: function () {
          router.map([
              //{ route: ['', '%STARTPAGE%'], moduleId: '%STARTPAGE%', title: '%STARTPAGE%', nav: 1 },
              //%PAGES%
            ]).buildNavigationModel()
            .mapUnknownRoutes('%STARTPAGE%', 'not-found');
          return router.activate('%STARTPAGE%');
        }
    };
});
