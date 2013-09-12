//ViewModel:_ViewModelType_
var self;
// ReSharper disable InconsistentNaming
// ReSharper disable UseOfImplicitGlobalInFunctionScope
// ReSharper disable UnusedLocals
// ReSharper disable UnusedParameter
function poll_ViewModelName_Events(viewmodel, initial) {
  var app = require('durandal/app');
  var ts = new Date().getTime();
  $.getJSON('/events/_ViewModelType_?ts=' + ts, function (data) {
    if (data == null) return;
    set_ViewModelName_Data(viewmodel, false, data);
    if (data.stonehenge_eval != null) eval(data.stonehenge_eval);
    if (initial || (data.stonehenge_poll != null)) setTimeout(function () { poll_ViewModelName_Events(viewmodel, false); }, 100);
  });
}
function set_ViewModelName_Data(viewmodel, loading, data) {
  _SetData_();
  if(loading)
  {
    viewmodel.InitialLoading(false);
    viewmodel.IsLoading(false);
  }
}
function post_ViewModelName_Data(viewmodel, sender, method) {
  //debugger;
  var params = '_Command_Sender_Text_=' + encodeURIComponent(sender.innerText) + "&";
  _GetData_();
  var ts = new Date().getTime();
  viewmodel.IsLoading(true);
  $.post('/viewmodel/_ViewModelType_/' + method + '?ts=' + ts, params, function (data) { set_ViewModelName_Data(viewmodel, true, data); });
}
define(function (require) {
  self = this;
  var InitialLoading = ko.observable(true);
  var IsLoading = ko.observable(true);
  _DeclareData_();
  return {
    _ReturnData_ : 0,
    InitialLoading: InitialLoading,
    IsLoading: IsLoading,
    _ActionMethods_ : 0,
    activate: function (view) {
      self = this;
      self.IsLoading(true);
    },
    viewAttached: function (view) {
      self = this;
      var ts = new Date().getTime();
      $.getJSON('/viewmodel/_ViewModelType_?ts=' + ts, function (data) { set_ViewModelName_Data(self, true, data); });
      setTimeout(function () { poll_ViewModelName_Events(self, true); }, 100);
    }
  };
});
