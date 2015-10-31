
stonehengeApp.controller('{0}', ['$scope', '$http', '$q',
  function ($scope, $http, $q) {
      $scope.StonehengeInitialLoading = true;
      $scope.StonehengeIsLoading = true;
      $scope.StonehengeIsDirty = false;
      $scope.StonehengeIsDisconnected = false;
      $scope.StonehengePostActive = false;
      $scope.StonehengePollEventsActive = null;
      /*commands*/
      $scope.StonehengePollEvents = function ($scope, continuePolling) {
          var ts = new Date().getTime();
          $scope.StonehengePollEventsActive = $q.defer();
          $http.get('/Events/{0}?ts=' + ts, { timeout: $scope.StonehengePollEventsActive.promise })
              .success(function (data, status, headers, config) {
                  $scope.StonehengePollEventsActive = null;
                  $scope.StonehengeIsDisconnected = false;
                  angular.extend($scope, data);
                  if (continuePolling || $scope.StonehengeContinuePolling) {
                      setTimeout(function () { $scope.StonehengePollEvents($scope, false); }, 200);
                  }
              })
              .error(function (data, status, headers, config) {
                  if ($scope.StonehengePollEventsActive != null) { $scope.StonehengeIsDisconnected = true; }
                  if (status === 200) { setTimeout(function () { window.location.reload(); }, 1000); }
                  $scope.StonehengePollEventsActive = null;
                  if (!$scope.StonehengePostActive) { setTimeout(function () { $scope.StonehengePollEvents($scope, true); }, 200); }
              });
      }
      $scope.StonehengePost = function ($scope, urlWithParams) {
          if ($scope.StonehengePollEventsActive) {
              var poll = $scope.StonehengePollEventsActive;
              $scope.StonehengePollEventsActive = null;
              poll.resolve();
          }
          var props = ['propNames'];
          var formData = new Object();
          props.forEach(function (prop) {
              formData[prop] = $scope[prop];
          });
          $scope.StonehengePostActive = true;
          $http.post(urlWithParams, formData).
            success(function (data, status, headers, config) {
                $scope.StonehengeInitialLoading = false;
                $scope.StonehengeIsLoading = false;
                if ($scope.StonehengePostActive) {
                    angular.extend($scope, data);
                    $scope.StonehengePostActive = false;
                }
                setTimeout(function () { $scope.StonehengePollEvents($scope, true); }, 200);
            }).
            error(function (data, status, headers, config) {
                $scope.StonehengeIsDisconnected = true;
                debugger;
            });
      }
      $http.get('ViewModel/{0}').
        success(function (data, status, headers, config) {
            $scope.StonehengeInitialLoading = false;
            $scope.StonehengeIsLoading = false;
            var cookie = headers("cookie");
            var match = (/StonehengeSession=([0-9a-fA-F]+)/).exec(cookie);
              if (match == null) {
                  $scope.StonehengeSession = "";
              }
              else {
                  $scope.StonehengeSession = match[1];
                  document.cookie = "StonehengeSession=" + $scope.StonehengeSession;
              }
            angular.extend($scope, data);
            if (!$scope.StonehengePollEventsActive) {
                setTimeout(function () { $scope.StonehengePollEvents($scope, true); }, 200);
            }
        }).
        error(function (data, status, headers, config) {
            $scope.StonehengeIsDisconnected = true;
            setTimeout(function () { window.location.reload(); }, 1000);
            debugger;
        });

  }]);

