
// ReSharper disable Es6Feature
import {HttpClient} from 'aurelia-http-client';
import {Tools} from 'app';

export class stonehengeViewModelName {

    constructor(http) {

        this.http = new HttpClient();
    
        this.StonehengeActive = false;
        this.StonehengeInitialLoading = true;
        this.StonehengeIsLoading = true;
        this.StonehengeIsDirty = false;
        this.StonehengeIsDisconnected = false;
        this.StonehengePostActive = false;
        this.StonehengePollEventsActive = null;

        this.StonehengeCancelRequests = function(scope) {
            for (var rq = 0; rq < scope.http.pendingRequests.length; rq++) {
                scope.http.pendingRequests[rq].abort();
            }
            scope.StonehengePollEventsActive = null;
        };

        this.StonehengePollEvents = function(scope, continuePolling) {
            if (!scope.StonehengeActive || scope.StonehengePostActive || scope.StonehengePollEventsActive != null) return;
            var ts = new Date().getTime();
            scope.StonehengePollEventsActive = scope.http.get('/Events/stonehengeViewModelName?ts=' + ts)
                .then(response => {
                    let data = JSON.parse(response.response);
                    scope.StonehengePollEventsActive = null;
                    scope.StonehengeIsDisconnected = false;
                    for(var propertyName in data) {
                        this[propertyName] = data[propertyName];
                    }
                    if (continuePolling || scope.StonehengeContinuePolling) {
                        setTimeout(function() { scope.StonehengePollEvents(scope, false); }, 200);
                    }
                })
                .catch(error => {
                    if (scope.StonehengePollEventsActive != null) {
                        scope.StonehengeIsDisconnected = true;
                    }
                    if (status === 200) {
                        setTimeout(function() { window.location.reload(); }, 1000);
                    }
                    scope.StonehengePollEventsActive = null;
                    if (!scope.StonehengePostActive) {
                        setTimeout(function() { scope.StonehengePollEvents(scope, true); }, 200);
                    }
                });
        };

        this.StonehengePost = function(scope, urlWithParams) {
            scope.StonehengeCancelRequests(scope);
      
            var props = ['propNames'];
            var formData = new Object();
            props.forEach(function(prop) {
                formData[prop] = scope[prop];
            });
            scope.StonehengePostActive = true;
            scope.http.post(urlWithParams, JSON.stringify(formData) )
                .then(response => {
                    let data = JSON.parse(response.response);
                    scope.StonehengeInitialLoading = false;
                    scope.StonehengeIsLoading = false;
                    if (scope.StonehengePostActive) {
                        for(var propertyName in data) {
                            scope[propertyName] = data[propertyName];
                        }
                        scope.StonehengePostActive = false;
                    }
                    if (scope.StonehengePollEventsActive == null) {
                        setTimeout(function() { scope.StonehengePollEvents(scope, true); }, 200);
                    }
                })
                .catch(error => {
                    if (error.responseType != "abort") {
                        scope.StonehengeIsDisconnected = true;
                        //debugger;
                        window.location.reload();
                    }
                });
        };

        this.StonehengeGetViewModel = function(scope) {
            scope.StonehengeCancelRequests(scope);
            scope.http.get('ViewModel/stonehengeViewModelName')
                .then(response => {
                    var cookie = response.headers.get("cookie");
                    var match = (/stonehenge-id=([0-9a-fA-F]+)/).exec(cookie);
                    if (match == null) {
                        var tools = new Tools();
                        scope.StonehengeSession = tools.getCookie("stonehenge-id");
                    }
                    else {
                        scope.StonehengeSession = match[1];
                    }

                    let data = JSON.parse(response.response);
                    scope.StonehengeInitialLoading = false;
                    scope.StonehengeIsLoading = false;
                    for (var propertyName in data) {
                        scope[propertyName] = data[propertyName];
                    }
                    if (scope.StonehengePollEventsActive == null) {
                        setTimeout(function() { scope.StonehengePollEvents(scope, true); }, 200);
                    }
                    
                })
                .catch(error => {
                    scope.StonehengeIsDisconnected = true;
                    if (console && console.log) console.log(error);
                    setTimeout(function() { window.location.reload(); }, 1000);
                    //debugger;
                    window.location.reload();
                });
        };

        /*commands*/

    }

    activate() {
        this.StonehengeActive = true;
        this.StonehengeGetViewModel(this);
    }

    deactivate() {
        this.StonehengeActive = false;
        this.StonehengeCancelRequests(this);
    }

}
// ReSharper restore Es6Feature
