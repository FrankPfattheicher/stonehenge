//ViewModel:_ViewModelType_
var self;
var activation_data;
// ReSharper disable InconsistentNaming
// ReSharper disable UseOfImplicitGlobalInFunctionScope
// ReSharper disable UnusedLocals
// ReSharper disable UnusedParameter
function poll_ViewModelName_Events(viewmodel, initial) {
    var app = require('durandal/app');
    var router = require('plugins/router');
    var ts = new Date().getTime();
    $.getJSON('/events/_ViewModelType_?ts=' + ts + '&stonehenge_id=' + stonehenge_id, function (data) {
        if (data == null) return;
        set_ViewModelName_Data(viewmodel, false, data);
        if (data.stonehenge_eval != null) eval(data.stonehenge_eval);
        if (data.stonehenge_navigate != null) router.navigate(data.stonehenge_navigate);
        if (initial || (data.stonehenge_poll != null)) setTimeout(function () { poll_ViewModelName_Events(viewmodel, false); }, 100);
    }).fail(function () {
        //alert("poll_ViewModelName_Events getJSON failed");
    });
}
function set_ViewModelName_Data(viewmodel, loading, data) {
    _SetData_();
    if (activation_data) {
        data = activation_data;
        activation_data = null;
        set_ViewModelName_Data(viewmodel, loading, data);
    }
    if (loading) {
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
    $.post('/viewmodel/_ViewModelType_/' + method + '?ts=' + ts + '&stonehenge_id=' + stonehenge_id, params, function (data) {
        set_ViewModelName_Data(viewmodel, true, data);
    }).fail(function () {
        //alert("post_ViewModelName_Data post failed");
        window.location.reload();
    });
}
define(['durandal/app', 'durandal/system', 'knockout', 'flot'], function (app, system, ko, flot) {
    self = this;

    var ErrorHandlingBindingProvider = function () {
        var original = new ko.bindingProvider();
        //determine if an element has any bindings
        this.nodeHasBindings = original.nodeHasBindings;
        //return the bindings given a node and the bindingContext
        this.getBindings = function (node, bindingContext) {
            var message = '';
            try { return original.getBindings(node, bindingContext); }
            catch (e) {
                message = e.message;
                if (console && console.log) { console.log("Error in binding: " + message); }
            }
            try {
                var params = 'Message=' + encodeURIComponent(message);
                params += '&Binding=' + encodeURIComponent(node.dataset.bind);
                var ts = new Date().getTime();
                $.post('/Exception/Binding?ts=' + ts + '&stonehenge_id=' + stonehenge_id, params, function (data) { });
            }
            catch (e) { if (console && console.log) { console.log("Error: " + e.message); } }
            return null;
        };
    };
    ko.bindingProvider.instance = new ErrorHandlingBindingProvider();

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
        activate: function () {
            self = this;
            system.log('ClientViewModel : activate');
            self.IsLoading(true);
        },
        attached: function (view, parent) {
            self = this;
            system.log('ClientViewModel  : attached');
            var ts = new Date().getTime();
            if (stonehenge_id != null)
            {
                var startPolling = function () { setTimeout(function () { poll_ViewModelName_Events(self, true); }, 100); };
                $.getJSON('/viewmodel/_ViewModelType_?ts=' + ts + '&stonehenge_id=' + stonehenge_id, function (data) {
                    set_ViewModelName_Data(self, true, data);
                    startPolling();
                }).fail(function () {
                    //alert("_ViewModelName_ attached getJSON failed");
                    window.location.reload();
                });
            }
        },
        binding: function () {
            system.log('ClientViewModel : binding');
            return { cacheViews: false }; //cancels view caching for this module, allowing the triggering of the detached callback
        },
        bindingComplete: function () {
            system.log('ClientViewModel : bindingComplete');
        },
        compositionComplete: function () {
            system.log('ClientViewModel : compositionComplete');
            if (typeof (user_compositionComplete) == 'function') {
                try {
                    user_compositionComplete();
                } catch (e) { }
            }
        },
    };
    return viewModel;
});
