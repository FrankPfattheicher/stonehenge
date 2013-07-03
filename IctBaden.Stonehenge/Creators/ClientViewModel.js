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
  self.InitialLoading(false);
  %SetData%
}
function post%ViewModelName%Data(self, method) {
  var params = '';
  %GetData%
  var ts = new Date().getTime();
  $.post('/viewmodel/%ViewModelType%/' + method + '?ts=' + ts, params, function (data) { set%ViewModelName%Data(self, data); });
}
define(function (require) {
  var InitialLoading = ko.observable(true);
  %DeclareData%
  return {
    %ReturnData%
    InitialLoading: InitialLoading,
    %ActionMethods% // OnXxxxx: function () { post%ViewModelName%Data(this); },
    viewAttached: function (view) {
      self = this;
      var ts = new Date().getTime();
      $.getJSON('/viewmodel/%ViewModelType%?ts=' + ts, function (data) { set%ViewModelName%Data(self, data); });
      setTimeout(function () { poll%ViewModelName%Events(self) }, 100);
    }
  };
});
