
var stonehengeApp = angular.module('stonehengeApp', ['ngRoute']);

stonehengeApp.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/', {
            templateUrl: 'start.html',
            controller: 'SampleCtrl'
        }).
        otherwise({
            redirectTo: '/'
        });
  }]);


stonehengeApp.controller('SampleCtrl', ['$scope', '$http',
  function ($scope, $http) {
      $http.get('ViewModels/SampleVm.json').
        success(function (data, status, headers, config) {
            $scope.data = data;
        }).
        error(function (data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
              debugger;
          });
      
  }]);

