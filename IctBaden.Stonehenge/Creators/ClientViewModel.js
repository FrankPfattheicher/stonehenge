//ViewModel:%ViewModelType%
function poll%ViewModelName%Events(self) {
  var app = require('durandal/app');
  var ts = new Date().getTime();
  $.getJSON('/events/%ViewModelType%?ts=' + ts, function (data) {
    set%ViewModelName%Data(self, data);
    if (data.eval != null) eval(data.eval);
    setTimeout(function () { poll%ViewModelName%Events(self); }, 100);
  });
}
function set%ViewModelName%Data(self, data) {
  %SetData%
  self.InitialLoading(false);
  self.IsLoading(false);
}
function post%ViewModelName%Data(self, method) {
  var params = '';
  %GetData%
  var ts = new Date().getTime();
  self.IsLoading(true);
  $.post('/viewmodel/%ViewModelType%/' + method + '?ts=' + ts, params, function (data) { set%ViewModelName%Data(self, data); });
}
define(function (require) {
  var InitialLoading = ko.observable(true);
  var IsLoading = ko.observable(true);
  %DeclareData%
  return {
    %ReturnData%
    InitialLoading: InitialLoading,
    IsLoading: IsLoading,
    %ActionMethods% // OnXxxxx: function () { post%ViewModelName%Data(this); },
    activate: function (view) {
      self = this;
      self.IsLoading(true);
    },
    viewAttached: function (view) {
      self = this;
      var ts = new Date().getTime();
      $.getJSON('/viewmodel/%ViewModelType%?ts=' + ts, function (data) { set%ViewModelName%Data(self, data); });
      setTimeout(function () { poll%ViewModelName%Events(self) }, 100);
    }
  };
});
