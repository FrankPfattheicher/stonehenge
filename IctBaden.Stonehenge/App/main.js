requirejs.config({
    paths: {
        'text': '/lib/require/js/text',
        'durandal':'/App/durandal/js',
        'plugins' : '/App/plugins',
        'transitions' : '/App/transitions',
//        'knockout': '/lib/knockout/js/knockout-3.0.0',
//        'bootstrap': '/lib/bootstrap/js/bootstrap',
//        'jquery': '/lib/jquery/js/jquery-1.10.2'
    },
    shim: {
        'bootstrap': {
            deps: ['jquery'],
            exports: 'jQuery'
        }
    }
});

define(['durandal/system', 'durandal/app', 'durandal/viewLocator'],  function (system, app, viewLocator) {
    //>>excludeStart("build", true);
    system.debug(true);
    //>>excludeEnd("build");

    app.title = '%TITLE%';

    //specify which plugins to install and their configuration
    app.configurePlugins({
        router:true,
        dialog: true,
        widget: {
            kinds: ['expander']
        }
    });

    app.start().then(function () {
        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewLocator.useConvention();

        //Show the app by setting the root view model for our application.
        app.setRoot('shell');
    });
});
