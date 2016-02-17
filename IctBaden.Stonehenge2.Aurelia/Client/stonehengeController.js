
// ReSharper disable Es6Feature
import {HttpClient} from 'aurelia-fetch-client';
import {jQuery} from 'aurelia-fetch-client';

export class {0} {

constructor(http) {

    this.http = new HttpClient();

    this.StonehengeInitialLoading = true;
    this.StonehengeIsLoading = true;
    this.StonehengeIsDirty = false;
    this.StonehengeIsDisconnected = false;
    this.StonehengePostActive = false;
    this.StonehengePollEventsActive = null;
    this.StonehengePollEvents = function(scope, continuePolling) {
        var ts = new Date().getTime();
        //this.StonehengePollEventsActive = $q.defer();
        scope.http.fetch('/Events/{0}?ts=' + ts, { method: 'get', headers: { 'Accept': 'application/json' } })
            .then(response => response.json())
            .then(data => {
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
        if (scope.StonehengePollEventsActive) {
            var poll = scope.StonehengePollEventsActive;
            scope.StonehengePollEventsActive = null;
            poll.resolve();
        }
        var props = ['propNames'];
        var formData = new Object();
        props.forEach(function(prop) {
            formData[prop] = scope[prop];
        });
        scope.StonehengePostActive = true;
        scope.http.fetch(urlWithParams, { method: 'post', headers: { 'Accept': 'application/json', 'Content-Type': 'application/json' }, body: JSON.stringify(formData) })
            .then(response => response.json())
            .then(data => {
                scope.StonehengeInitialLoading = false;
                scope.StonehengeIsLoading = false;
                if (scope.StonehengePostActive) {
                    for(var propertyName in data) {
                        scope[propertyName] = data[propertyName];
                    }
                    scope.StonehengePostActive = false;
                }
                setTimeout(function() { scope.StonehengePollEvents(scope, true); }, 200);
            })
            .catch(error => {
                scope.StonehengeIsDisconnected = true;
                debugger;
            });
       };

        this.StonehengeGetViewModel = function(scope) {
            scope.http.fetch('ViewModel/{0}', { method: 'get', headers: { 'Accept': 'application/json' } })
                .then(response => {
                    var cookie = response.headers.get("cookie");
                    var match = (/StonehengeSession=([0-9a-fA-F]+)/).exec(cookie);
                    if (match == null) {
                        scope.StonehengeSession = "";
                    }
                    else {
                        scope.StonehengeSession = match[1];
                        document.cookie = "StonehengeSession=" + scope.StonehengeSession;
                    }

                    response.json().then(function(data) {
                        scope.StonehengeInitialLoading = false;
                        scope.StonehengeIsLoading = false;
                        for (var propertyName in data) {
                            scope[propertyName] = data[propertyName];
                        }
                        if (!scope.StonehengePollEventsActive) {
                            setTimeout(function() { scope.StonehengePollEvents(scope, true); }, 200);
                        }
                    });
                })
                .catch(error => {
                    scope.StonehengeIsDisconnected = true;
                    if (console && console.log) console.log(error);
                    setTimeout(function() { window.location.reload(); }, 1000);
                    debugger;
                });
        };

        this.StonehengeGetViewModel(this);
    }

/*commands*/

}
// ReSharper restore Es6Feature
