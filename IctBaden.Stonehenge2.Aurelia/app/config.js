System.config({
    defaultJSExtensions: true,
    transpiler: "babel",
    babelOptions: {
        "optional": [
          "es7.decorators",
          "es7.classProperties"
        ]
    },
    paths: {
        "*": "src/*",
        "github:*": "jspm_packages/github/*",
        "npm:*": "jspm_packages/npm/*"
    },
    map: {
        "aurelia-animator-css": "npm:aurelia-animator-css@1.0.0-beta.1.2.0",
        "aurelia-bootstrapper": "npm:aurelia-bootstrapper@1.0.0-beta.1.2.0",
        "aurelia-framework": "npm:aurelia-framework@1.0.0-beta.1.2.1",
        "aurelia-history-browser": "npm:aurelia-history-browser@1.0.0-beta.1.2.0",
        "aurelia-http-client": "npm:aurelia-http-client@1.0.0-beta.1.2.0",
        "aurelia-loader-default": "npm:aurelia-loader-default@1.0.0-beta.1.2.0",
        "aurelia-logging-console": "npm:aurelia-logging-console@1.0.0-beta.1.2.0",
        "aurelia-pal-browser": "npm:aurelia-pal-browser@1.0.0-beta.1.2.0",
        "aurelia-path": "npm:aurelia-path@1.0.0-beta.1.2.1",
        "aurelia-polyfills": "npm:aurelia-polyfills@1.0.0-beta.1.1.1",
        "aurelia-router": "npm:aurelia-router@1.0.0-beta.1.2.0",
        "aurelia-templating-binding": "npm:aurelia-templating-binding@1.0.0-beta.1.2.1",
        "aurelia-templating-resources": "npm:aurelia-templating-resources@1.0.0-beta.1.2.1",
        "aurelia-templating-router": "npm:aurelia-templating-router@1.0.0-beta.1.2.0",
        "babel": "npm:babel-core@5.8.35",
        "bootstrap": "github:twbs/bootstrap@3.3.6",
        "es6-promise": "npm:es6-promise@3.1.2",
        "font-awesome": "npm:font-awesome@4.5.0",
        "text": "github:systemjs/plugin-text@0.0.4",
        "github:jspm/nodelibs-assert@0.1.0": {
            "assert": "npm:assert@1.3.0"
        },
        "github:jspm/nodelibs-process@0.1.2": {
            "process": "npm:process@0.11.2"
        },
        "github:jspm/nodelibs-util@0.1.0": {
            "util": "npm:util@0.10.3"
        },
        "github:twbs/bootstrap@3.3.6": {
            "jquery": "github:components/jquery@2.2.1"
        },
        "npm:assert@1.3.0": {
            "util": "npm:util@0.10.3"
        },
        "npm:aurelia-animator-css@1.0.0-beta.1.2.0": {
            "aurelia-metadata": "npm:aurelia-metadata@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0",
            "aurelia-templating": "npm:aurelia-templating@1.0.0-beta.1.2.1"
        },
        "npm:aurelia-binding@1.0.0-beta.1.3.1": {
            "aurelia-metadata": "npm:aurelia-metadata@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0",
            "aurelia-task-queue": "npm:aurelia-task-queue@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-bootstrapper@1.0.0-beta.1.2.0": {
            "aurelia-event-aggregator": "npm:aurelia-event-aggregator@1.0.0-beta.1.2.0",
            "aurelia-framework": "npm:aurelia-framework@1.0.0-beta.1.2.1",
            "aurelia-history": "npm:aurelia-history@1.0.0-beta.1.2.0",
            "aurelia-history-browser": "npm:aurelia-history-browser@1.0.0-beta.1.2.0",
            "aurelia-loader-default": "npm:aurelia-loader-default@1.0.0-beta.1.2.0",
            "aurelia-logging-console": "npm:aurelia-logging-console@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0",
            "aurelia-pal-browser": "npm:aurelia-pal-browser@1.0.0-beta.1.2.0",
            "aurelia-polyfills": "npm:aurelia-polyfills@1.0.0-beta.1.1.1",
            "aurelia-router": "npm:aurelia-router@1.0.0-beta.1.2.0",
            "aurelia-templating": "npm:aurelia-templating@1.0.0-beta.1.2.1",
            "aurelia-templating-binding": "npm:aurelia-templating-binding@1.0.0-beta.1.2.1",
            "aurelia-templating-resources": "npm:aurelia-templating-resources@1.0.0-beta.1.2.1",
            "aurelia-templating-router": "npm:aurelia-templating-router@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-dependency-injection@1.0.0-beta.1.2.0": {
            "aurelia-logging": "npm:aurelia-logging@1.0.0-beta.1.2.0",
            "aurelia-metadata": "npm:aurelia-metadata@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-event-aggregator@1.0.0-beta.1.2.0": {
            "aurelia-logging": "npm:aurelia-logging@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-framework@1.0.0-beta.1.2.1": {
            "aurelia-binding": "npm:aurelia-binding@1.0.0-beta.1.3.1",
            "aurelia-dependency-injection": "npm:aurelia-dependency-injection@1.0.0-beta.1.2.0",
            "aurelia-loader": "npm:aurelia-loader@1.0.0-beta.1.2.0",
            "aurelia-logging": "npm:aurelia-logging@1.0.0-beta.1.2.0",
            "aurelia-metadata": "npm:aurelia-metadata@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0",
            "aurelia-path": "npm:aurelia-path@1.0.0-beta.1.2.1",
            "aurelia-task-queue": "npm:aurelia-task-queue@1.0.0-beta.1.2.0",
            "aurelia-templating": "npm:aurelia-templating@1.0.0-beta.1.2.1"
        },
        "npm:aurelia-history-browser@1.0.0-beta.1.2.0": {
            "aurelia-history": "npm:aurelia-history@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-http-client@1.0.0-beta.1.2.0": {
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0",
            "aurelia-path": "npm:aurelia-path@1.0.0-beta.1.2.1"
        },
        "npm:aurelia-loader-default@1.0.0-beta.1.2.0": {
            "aurelia-loader": "npm:aurelia-loader@1.0.0-beta.1.2.0",
            "aurelia-metadata": "npm:aurelia-metadata@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-loader@1.0.0-beta.1.2.0": {
            "aurelia-metadata": "npm:aurelia-metadata@1.0.0-beta.1.2.0",
            "aurelia-path": "npm:aurelia-path@1.0.0-beta.1.2.1"
        },
        "npm:aurelia-logging-console@1.0.0-beta.1.2.0": {
            "aurelia-logging": "npm:aurelia-logging@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-metadata@1.0.0-beta.1.2.0": {
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-pal-browser@1.0.0-beta.1.2.0": {
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-polyfills@1.0.0-beta.1.1.1": {
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-route-recognizer@1.0.0-beta.1.2.0": {
            "aurelia-path": "npm:aurelia-path@1.0.0-beta.1.2.1"
        },
        "npm:aurelia-router@1.0.0-beta.1.2.0": {
            "aurelia-dependency-injection": "npm:aurelia-dependency-injection@1.0.0-beta.1.2.0",
            "aurelia-event-aggregator": "npm:aurelia-event-aggregator@1.0.0-beta.1.2.0",
            "aurelia-history": "npm:aurelia-history@1.0.0-beta.1.2.0",
            "aurelia-logging": "npm:aurelia-logging@1.0.0-beta.1.2.0",
            "aurelia-path": "npm:aurelia-path@1.0.0-beta.1.2.1",
            "aurelia-route-recognizer": "npm:aurelia-route-recognizer@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-task-queue@1.0.0-beta.1.2.0": {
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0"
        },
        "npm:aurelia-templating-binding@1.0.0-beta.1.2.1": {
            "aurelia-binding": "npm:aurelia-binding@1.0.0-beta.1.3.1",
            "aurelia-logging": "npm:aurelia-logging@1.0.0-beta.1.2.0",
            "aurelia-templating": "npm:aurelia-templating@1.0.0-beta.1.2.1"
        },
        "npm:aurelia-templating-resources@1.0.0-beta.1.2.1": {
            "aurelia-binding": "npm:aurelia-binding@1.0.0-beta.1.3.1",
            "aurelia-dependency-injection": "npm:aurelia-dependency-injection@1.0.0-beta.1.2.0",
            "aurelia-loader": "npm:aurelia-loader@1.0.0-beta.1.2.0",
            "aurelia-logging": "npm:aurelia-logging@1.0.0-beta.1.2.0",
            "aurelia-metadata": "npm:aurelia-metadata@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0",
            "aurelia-path": "npm:aurelia-path@1.0.0-beta.1.2.1",
            "aurelia-task-queue": "npm:aurelia-task-queue@1.0.0-beta.1.2.0",
            "aurelia-templating": "npm:aurelia-templating@1.0.0-beta.1.2.1"
        },
        "npm:aurelia-templating-router@1.0.0-beta.1.2.0": {
            "aurelia-dependency-injection": "npm:aurelia-dependency-injection@1.0.0-beta.1.2.0",
            "aurelia-logging": "npm:aurelia-logging@1.0.0-beta.1.2.0",
            "aurelia-metadata": "npm:aurelia-metadata@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0",
            "aurelia-path": "npm:aurelia-path@1.0.0-beta.1.2.1",
            "aurelia-router": "npm:aurelia-router@1.0.0-beta.1.2.0",
            "aurelia-templating": "npm:aurelia-templating@1.0.0-beta.1.2.1"
        },
        "npm:aurelia-templating@1.0.0-beta.1.2.1": {
            "aurelia-binding": "npm:aurelia-binding@1.0.0-beta.1.3.1",
            "aurelia-dependency-injection": "npm:aurelia-dependency-injection@1.0.0-beta.1.2.0",
            "aurelia-loader": "npm:aurelia-loader@1.0.0-beta.1.2.0",
            "aurelia-logging": "npm:aurelia-logging@1.0.0-beta.1.2.0",
            "aurelia-metadata": "npm:aurelia-metadata@1.0.0-beta.1.2.0",
            "aurelia-pal": "npm:aurelia-pal@1.0.0-beta.1.2.0",
            "aurelia-path": "npm:aurelia-path@1.0.0-beta.1.2.1",
            "aurelia-task-queue": "npm:aurelia-task-queue@1.0.0-beta.1.2.0"
        },
        "npm:es6-promise@3.1.2": {
            "process": "github:jspm/nodelibs-process@0.1.2"
        },
        "npm:font-awesome@4.5.0": {
            "css": "github:systemjs/plugin-css@0.1.20"
        },
        "npm:inherits@2.0.1": {
            "util": "github:jspm/nodelibs-util@0.1.0"
        },
        "npm:process@0.11.2": {
            "assert": "github:jspm/nodelibs-assert@0.1.0"
        },
        "npm:util@0.10.3": {
            "inherits": "npm:inherits@2.0.1",
            "process": "github:jspm/nodelibs-process@0.1.2"
        }
    },
    depCache: {},
    bundles: {
        "aurelia-babel.js": [
          "npm:babel-core@5.8.35.js",
          "npm:babel-core@5.8.35/browser.js"
        ],
        "app-build.js": [],
        "aurelia.js": [
          "github:components/jquery@2.2.1.js",
          "github:components/jquery@2.2.1/jquery.js",
          "github:jspm/nodelibs-process@0.1.2.js",
          "github:jspm/nodelibs-process@0.1.2/index.js",
          "github:systemjs/plugin-text@0.0.4.js",
          "github:systemjs/plugin-text@0.0.4/text.js",
          "github:twbs/bootstrap@3.3.6.js",
          "github:twbs/bootstrap@3.3.6/css/bootstrap.css!github:systemjs/plugin-text@0.0.4.js",
          "github:twbs/bootstrap@3.3.6/js/bootstrap.js",
          "npm:aurelia-animator-css@1.0.0-beta.1.2.0.js",
          "npm:aurelia-animator-css@1.0.0-beta.1.2.0/aurelia-animator-css.js",
          "npm:aurelia-binding@1.0.0-beta.1.3.1.js",
          "npm:aurelia-binding@1.0.0-beta.1.3.1/aurelia-binding.js",
          "npm:aurelia-bootstrapper@1.0.0-beta.1.2.0.js",
          "npm:aurelia-bootstrapper@1.0.0-beta.1.2.0/aurelia-bootstrapper.js",
          "npm:aurelia-dependency-injection@1.0.0-beta.1.2.0.js",
          "npm:aurelia-dependency-injection@1.0.0-beta.1.2.0/aurelia-dependency-injection.js",
          "npm:aurelia-event-aggregator@1.0.0-beta.1.2.0.js",
          "npm:aurelia-event-aggregator@1.0.0-beta.1.2.0/aurelia-event-aggregator.js",
          "npm:aurelia-framework@1.0.0-beta.1.2.1.js",
          "npm:aurelia-framework@1.0.0-beta.1.2.1/aurelia-framework.js",
          "npm:aurelia-history-browser@1.0.0-beta.1.2.0.js",
          "npm:aurelia-history-browser@1.0.0-beta.1.2.0/aurelia-history-browser.js",
          "npm:aurelia-history@1.0.0-beta.1.2.0.js",
          "npm:aurelia-history@1.0.0-beta.1.2.0/aurelia-history.js",
          "npm:aurelia-http-client@1.0.0-beta.1.2.0.js",
          "npm:aurelia-http-client@1.0.0-beta.1.2.0/aurelia-http-client.js",
          "npm:aurelia-loader-default@1.0.0-beta.1.2.0.js",
          "npm:aurelia-loader-default@1.0.0-beta.1.2.0/aurelia-loader-default.js",
          "npm:aurelia-loader@1.0.0-beta.1.2.0.js",
          "npm:aurelia-loader@1.0.0-beta.1.2.0/aurelia-loader.js",
          "npm:aurelia-logging-console@1.0.0-beta.1.2.0.js",
          "npm:aurelia-logging-console@1.0.0-beta.1.2.0/aurelia-logging-console.js",
          "npm:aurelia-logging@1.0.0-beta.1.2.0.js",
          "npm:aurelia-logging@1.0.0-beta.1.2.0/aurelia-logging.js",
          "npm:aurelia-metadata@1.0.0-beta.1.2.0.js",
          "npm:aurelia-metadata@1.0.0-beta.1.2.0/aurelia-metadata.js",
          "npm:aurelia-pal-browser@1.0.0-beta.1.2.0.js",
          "npm:aurelia-pal-browser@1.0.0-beta.1.2.0/aurelia-pal-browser.js",
          "npm:aurelia-pal@1.0.0-beta.1.2.0.js",
          "npm:aurelia-pal@1.0.0-beta.1.2.0/aurelia-pal.js",
          "npm:aurelia-path@1.0.0-beta.1.2.1.js",
          "npm:aurelia-path@1.0.0-beta.1.2.1/aurelia-path.js",
          "npm:aurelia-polyfills@1.0.0-beta.1.1.1.js",
          "npm:aurelia-polyfills@1.0.0-beta.1.1.1/aurelia-polyfills.js",
          "npm:aurelia-route-recognizer@1.0.0-beta.1.2.0.js",
          "npm:aurelia-route-recognizer@1.0.0-beta.1.2.0/aurelia-route-recognizer.js",
          "npm:aurelia-router@1.0.0-beta.1.2.0.js",
          "npm:aurelia-router@1.0.0-beta.1.2.0/aurelia-router.js",
          "npm:aurelia-task-queue@1.0.0-beta.1.2.0.js",
          "npm:aurelia-task-queue@1.0.0-beta.1.2.0/aurelia-task-queue.js",
          "npm:aurelia-templating-binding@1.0.0-beta.1.2.1.js",
          "npm:aurelia-templating-binding@1.0.0-beta.1.2.1/aurelia-templating-binding.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/abstract-repeater.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/analyze-view-factory.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/array-repeat-strategy.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/aurelia-templating-resources.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/binding-mode-behaviors.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/binding-signaler.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/compile-spy.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/compose.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/css-resource.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/debounce-binding-behavior.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/dynamic-element.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/focus.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/hide.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/html-resource-plugin.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/html-sanitizer.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/if.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/map-repeat-strategy.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/null-repeat-strategy.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/number-repeat-strategy.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/repeat-strategy-locator.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/repeat-utilities.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/repeat.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/replaceable.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/sanitize-html.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/set-repeat-strategy.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/show.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/signal-binding-behavior.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/throttle-binding-behavior.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/update-trigger-binding-behavior.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/view-spy.js",
          "npm:aurelia-templating-resources@1.0.0-beta.1.2.1/with.js",
          "npm:aurelia-templating-router@1.0.0-beta.1.2.0.js",
          "npm:aurelia-templating-router@1.0.0-beta.1.2.0/aurelia-templating-router.js",
          "npm:aurelia-templating-router@1.0.0-beta.1.2.0/route-href.js",
          "npm:aurelia-templating-router@1.0.0-beta.1.2.0/route-loader.js",
          "npm:aurelia-templating-router@1.0.0-beta.1.2.0/router-view.js",
          "npm:aurelia-templating@1.0.0-beta.1.2.1.js",
          "npm:aurelia-templating@1.0.0-beta.1.2.1/aurelia-templating.js",
          "npm:es6-promise@3.1.2.js",
          "npm:es6-promise@3.1.2/dist/es6-promise.js",
          "npm:process@0.11.2.js",
          "npm:process@0.11.2/browser.js"
        ]
    }
});