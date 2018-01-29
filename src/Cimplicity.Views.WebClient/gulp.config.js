module.exports = function () {

    var base = {
        webroot: "./wwwroot/",
        node_modules: "./node_modules/"
    };

    var config = {
        /**
         * Files paths
         */
        angular: base.node_modules + "@angular/**/*.js",
        app: "App/**/*.*",
        appDest: base.webroot + "app",
        js: base.webroot + "js/*.js",
        jsDest: base.webroot + 'js',
        css: base.webroot + "css/*.css",
        cssDest: base.webroot + 'css',
        lib: base.webroot + "lib/",
        node_modules: base.node_modules,
        angularWebApi: base.node_modules + "angular2-in-memory-web-api/*.js",
        corejs: base.node_modules + "core-js/client/shim*.js",
        zonejs: base.node_modules + "zone.js/dist/zone*.js",
        reflectjs: base.node_modules + "reflect-metadata/Reflect*.js",
        systemjs: base.node_modules + "systemjs/dist/*.js",
        rxjs: base.node_modules + "rxjs/**/*.js",
        jasminejs: base.node_modules + "jasmine-core/lib/jasmine-core/*.*",
        shim_es6: base.node_modules + "es6-shim/es6*.*",
        signalr: base.node_modules + "@aspnet/signalr-client/dist/*/*.*",
        plugin_babel: base.node_modules + "systemjs-plugin-babel/*.js",
        index:"Views/Home/Index.cshtml"
    };

    return config;
};