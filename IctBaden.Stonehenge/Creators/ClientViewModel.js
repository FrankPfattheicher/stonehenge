//ViewModel:_ViewModelType_
var self;
var activation_data;
var loading_ViewModelName_active = false;
var polling_ViewModelName_active = false;
// ReSharper disable InconsistentNaming
// ReSharper disable UseOfImplicitGlobalInFunctionScope
// ReSharper disable UnusedLocals
// ReSharper disable UnusedParameter
function poll_ViewModelName_Events(viewmodel, initial) {
    if (polling_ViewModelName_active)
        return;
    var app = require("durandal/app");
    var router = require("plugins/router");
    var ts = new Date().getTime();
    $.ajaxSetup({timeout: 60000});
    polling_ViewModelName_active = $.getJSON("/events/_ViewModelType_?ts=" + ts + "&stonehenge_id=" + stonehenge_id, function (data) {
        polling_ViewModelName_active = null;
        viewmodel.IsDisconnected(false);
        if (!loading_ViewModelName_active && (data != null)) {
            set_ViewModelName_Data(viewmodel, false, data);
            if (data.stonehenge_eval != null) eval(data.stonehenge_eval);
            if (data.stonehenge_navigate != null) router.navigate(data.stonehenge_navigate);
            if (data.stonehenge_poll != null) initial = true;
        }
        if (initial) {
            setTimeout(function () { poll_ViewModelName_Events(viewmodel, false); }, 200);
        }
// ReSharper disable once PossiblyUnassignedProperty
    }).fail(function (jqXHR, status, error) {
        //alert("poll_ViewModelName_Events getJSON failed");
        if (error !== "abort") { viewmodel.IsDisconnected(true); }
        if (jqXHR.status === 200) { setTimeout(function () { window.location.reload(); }, 1000); }
        polling_ViewModelName_active = null;
        if (!loading_ViewModelName_active) { setTimeout(function () { poll_ViewModelName_Events(self, true); }, 200); }
    });
}
function set_ViewModelName_Data(viewmodel, loading, data) {
    if (loading_ViewModelName_active)
        return;
    _SetData_();
    if ((data != null) && (data.stonehenge_eval != null)) {
	    var app = require('durandal/app');
        eval(data.stonehenge_eval);
    }
    if ((data != null) && (data.stonehenge_navigate != null)) {
        var router = require('plugins/router');
        if (data.stonehenge_navigate === '_stonehenge_back_') {
            router.navigateBack();
        } else {
            router.navigate(data.stonehenge_navigate);
        }
    }
    if (activation_data) {
        data = activation_data;
        activation_data = null;
        set_ViewModelName_Data(viewmodel, loading, data);
    }
    if (loading) {
        if (!polling_ViewModelName_active) {
            setTimeout(function () { poll_ViewModelName_Events(self, true); }, 200);
        }
        viewmodel.InitialLoading(false);
        viewmodel.IsLoading(false);
        viewmodel.IsDirty(false);
        viewmodel.IsDisconnected(false);
    }
}
function post_ViewModelName_Data(viewmodel, sender, method, param) {
    //debugger;
    loading_ViewModelName_active = true;
    if (polling_ViewModelName_active) {
        polling_ViewModelName_active.abort();
    }
    var params = "_stonehenge_CommandSenderName_=" + encodeURIComponent(sender.name) + "&";
    if (param != null) { params += "_stonehenge_CommandParameter_=" + encodeURIComponent(param) + "&"; }
    _GetData_();
    var ts = new Date().getTime();
    viewmodel.IsLoading(true);
    $.post("/viewmodel/_ViewModelType_/" + method + "?ts=" + ts + "&stonehenge_id=" + stonehenge_id, params, function (data) {
        loading_ViewModelName_active = false;
        set_ViewModelName_Data(viewmodel, true, data);
// ReSharper disable once PossiblyUnassignedProperty
    }).fail(function (err) {
        //alert("post_ViewModelName_Data post failed");
        viewmodel.IsDisconnected(true);
        setTimeout(function () { window.location.reload(); }, 1000);
    });
}
define(["durandal/app", "durandal/system", "knockout", "flot"], function(app, system, ko, flot) {
    self = this;

    var ErrorHandlingBindingProvider = function() {
        var original = new ko.bindingProvider();
        //determine if an element has any bindings
        this.nodeHasBindings = original.nodeHasBindings;
        //return the bindings given a node and the bindingContext
        this.getBindings = function(node, bindingContext) {
            var message;
            try {
                return original.getBindings(node, bindingContext);
            } catch (ex1) {
                message = ex1.message;
                if ((typeof(console) != "undefined") && console.log) {
                    console.log("Error in binding: " + message);
                }
            }
            try {
                var params = "Message=" + encodeURIComponent(message);
                params += "&Binding=" + encodeURIComponent(node.dataset.bind);
                var ts = new Date().getTime();
                $.post("/Exception/Binding?ts=" + ts + "&stonehenge_id=" + stonehenge_id, params, function(data) {});
            } catch (ex2) {
                if ((typeof (console) != "undefined") && console.log) {
                    console.log("Error: " + ex2.message);
                }
            }
            return null;
        };
    };
    ko.bindingHandlers.enterkey = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var allBindings = allBindingsAccessor();
            $(element).keypress(function (event) {
                var keyCode = (event.which ? event.which : event.keyCode);
                if (keyCode === 13) {
                    allBindings.enterkey(viewModel, event);
                    return false;
                }
                return true;
            });
        }
    };
    ko.bindingProvider.instance = new ErrorHandlingBindingProvider();

    var InitialLoading = ko.observable(true);
    var IsLoading = ko.observable(true);
    var IsDirty = ko.observable(false);
    var IsDisconnected = ko.observable(false);
    _DeclareData_();
    var viewModel = {
        _ReturnData_: 0,
        InitialLoading: InitialLoading,
        IsLoading: IsLoading,
        IsDirty: IsDirty,
        IsDisconnected: IsDisconnected,
        _ActionMethods_: 0,
        activate: function () {
            self = this;
            system.log("ClientViewModel : activate");
            self.IsLoading(true);
        },
        attached: function (view, parent) {
            self = this;
            system.log("ClientViewModel  : attached");
            var ts = new Date().getTime();
            if (stonehenge_id === "") {
                // ReSharper disable once AssignToImplicitGlobalInFunctionScope
                stonehenge_id = getCookie("stonehenge_id");
            }
            if (stonehenge_id != null)
            {
                loading_ViewModelName_active = true;
                if (polling_ViewModelName_active) {
                    polling_ViewModelName_active.abort();
                }
                $.getJSON("/viewmodel/_ViewModelType_?ts=" + ts + "&stonehenge_ctx=_ViewContext_&stonehenge_id=" + stonehenge_id, function (data) {
                    loading_ViewModelName_active = false;
                    set_ViewModelName_Data(self, true, data);
// ReSharper disable once PossiblyUnassignedProperty
                }).fail(function (err) {
                    //alert("_ViewModelName_ attached getJSON failed");
                    setTimeout(function () { window.location.reload(); }, 1000);
                });
            }
        },
        binding: function () {
            system.log("ClientViewModel : binding");
            return { cacheViews: false }; //cancels view caching for this module, allowing the triggering of the detached callback
        },
        bindingComplete: function () {
            system.log("ClientViewModel : bindingComplete");
            if (typeof (user_bindingComplete) == 'function') {
                try {
                    user_bindingComplete(this);
                } catch (e) { }
            }
        },
        compositionComplete: function () {
            system.log("ClientViewModel : compositionComplete");
            if (typeof (user_compositionComplete) == 'function') {
                try {
                    user_compositionComplete();
                } catch (e) { }
            }
            if (typeof (_ViewModelName_Init) == 'function') {
                try {
                    _ViewModelName_Init();
                } catch (e) { }
            }
            $(".initialfocus").focus();
        }
    };
    return viewModel;
});
