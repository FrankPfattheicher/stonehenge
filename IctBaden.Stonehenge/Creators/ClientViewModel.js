//ViewModel:%ViewModelType%
function poll%ViewModelName%Events(self) {
  var app = require('durandal/app');
  var ts = new Date().getTime();
  $.getJSON('/events/CoreWeb.UserMgrViewModel?ts=' + ts, function (data) {
    set%ViewModelName%Data(self, data);
    if (data.eval != null) eval(data.eval);
    setTimeout(function () { pollUserMgrViewModelEvents(self) }, 100);
  });
}
function set%ViewModelName%Data(self, data) {
  InitialLoading(false);
  %SetData% // if (data.Ddddd != null) Ddddd(data.Ddddd);
}
function post%ViewModelName%Data(self) {
  var params = '';
  %GetData% // if (Ddddd() != null) params += 'Ddddd=' + encodeURIComponent(JSON.stringify(Ddddd())) + '&';
    var ts = new Date().getTime();
    $.post('/viewmodel/CoreWeb.UserMgrViewModel/OnSelectedAccountChanged?ts=' + ts, params, function (data) {
      set%ViewModelName%Data(self, data);
  });
}
define(function (require) {
  var InitialLoading = ko.observable(true);
  %DeclareData%
  return {
    %ReturnData%
    InitialLoading: InitialLoading,
    %ActionMethods% // OnXxxxx: function () { post%ViewModelName%Data(this); },
    viewAttached: function (view) {
      var self = this;
      var ts = new Date().getTime();
      $.getJSON('/viewmodel/CoreWeb.UserMgrViewModel?ts=' + ts, function (data) {
        set%ViewModelName%Data(self, data);
      });
      setTimeout(function () { pollUserMgrViewModelEvents(self) }, 100);
    }
  };
});
