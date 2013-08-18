//ViewModel:%ViewModelType%
var self;
function poll%ViewModelName%Events(viewmodel) {
  var app = require('durandal/app');
  var ts = new Date().getTime();
  $.getJSON('/events/%ViewModelType%?ts=' + ts, function (data) {
    set%ViewModelName%Data(viewmodel, false, data);
    if (data.eval != null) eval(data.eval);
    setTimeout(function () { poll%ViewModelName%Events(viewmodel); }, 100);
  });
}
function set%ViewModelName%Data(viewmodel, loading, data) {
  %SetData%
  if(loading)
  {
    viewmodel.InitialLoading(false);
    viewmodel.IsLoading(false);
  }
}
function post%ViewModelName%Data(viewmodel, sender, method) {
  //debugger;
  var params = 'Sender=' + encodeURIComponent(sender.innerText) + "&";
  %GetData%
  var ts = new Date().getTime();
  viewmodel.IsLoading(true);
  $.post('/viewmodel/%ViewModelType%/' + method + '?ts=' + ts, params, function (data) { set%ViewModelName%Data(viewmodel, true, data); });
}
define(function (require) {
  self = this;
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
      $.getJSON('/viewmodel/%ViewModelType%?ts=' + ts, function (data) { set%ViewModelName%Data(self, true, data); });
      setTimeout(function () { poll%ViewModelName%Events(self) }, 100);
    }
  };
});
