
define(['plugins/router'], function (router) {
  return {
    router: router,
        activate: function () {
          router.map([
            { route: ['', '%STARTPAGE%'], moduleId: '%STARTPAGE%', title: '%STARTPAGE%', nav: 1 },
            { route: ['', 'login'], moduleId: 'login', title: 'login', nav: 1 },
          //                { route: 'view-composition',            moduleId: 'viewComposition/index',  title: 'View Composition',  nav: true },
//                { route: 'modal',                       moduleId: 'modal/index',            title: 'Modal Dialogs',     nav: 3 },
//                { route: 'event-aggregator',            moduleId: 'eventAggregator/index',  title: 'Events',            nav: 2 },
//                { route: 'widgets',                     moduleId: 'widgets/index',          title: 'Widgets',           nav: true },
//                { route: 'master-detail',               moduleId: 'masterDetail/index',     title: 'Master Detail',     nav: true },
            //{ route: '%STARTPAGE%', moduleId: '%STARTPAGE%', title: '%STARTPAGE%', nav: true }
          ]).buildNavigationModel();
          //.mapUnknownRoutes('%STARTPAGE%', 'not-found')
          return router.activate();
        }
    };
});
