
var stonehengeApp = angular.module('stonehengeApp', ['ngRoute']);

stonehengeApp.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        //stonehengeAppRoutes
        otherwise({
            redirectTo: '/'
        });
  }]);

//stonehengeControllers

