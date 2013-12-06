//ViewModel:_ViewModelType_
var self;
var activation_data;
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
  if (activation_data) {
    data = activation_data;
    activation_data = null;
    set_ViewModelName_Data(viewmodel, loading, data);
  }
  if(loading)
  {
    viewmodel.InitialLoading(false);
    viewmodel.IsLoading(false);
    viewmodel.IsDirty(false);
  }
}
function post_ViewModelName_Data(viewmodel, sender, method, param) {
  //debugger;
  var params = '_stonehenge_CommandSenderName_=' + encodeURIComponent(sender.name) + '&';
  if (param != null) { params += '_stonehenge_CommandParameter_=' + encodeURIComponent(param) + '&'; }
  _GetData_();
  var ts = new Date().getTime();
  viewmodel.IsLoading(true);
  $.post('/viewmodel/_ViewModelType_/' + method + '?ts=' + ts, params, function (data) { set_ViewModelName_Data(viewmodel, true, data); });
}
define(function (require) {
  self = this;
  var InitialLoading = ko.observable(true);
  var IsLoading = ko.observable(true);
  var IsDirty = ko.observable(false);
   _DeclareData_();
  var viewModel = {
    _ReturnData_: 0,
    InitialLoading: InitialLoading,
    IsLoading: IsLoading,
    IsDirty: IsDirty,
    _ActionMethods_: 0,
    activate: function(data) {
      self = this;
      if (activation_data == null) activation_data = data;
      self.IsLoading(true);
    },
    viewAttached: function(view) {
      self = this;
      var ts = new Date().getTime();
      var startPolling = function() { setTimeout(function() { poll_ViewModelName_Events(self, true); }, 100); };
      $.getJSON('/viewmodel/_ViewModelType_?ts=' + ts, function(data) {
        set_ViewModelName_Data(self, true, data);
        startPolling();
      });
    }
  };
  return viewModel;
});
