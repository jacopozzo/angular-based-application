/// <binding Clean='clean' AfterBuild='copy:app'/>
"use strict";

var gulp = require('gulp');
var config = require('./gulp.config')();
var uglify = require('gulp-uglify');
var concat = require('gulp-concat');
var cleanCSS = require('gulp-clean-css');
var clean = require('gulp-clean');
var rename = require('gulp-rename');
var pump = require('pump');
var $ = require('gulp-load-plugins')({ lazy: true });

gulp.task("clean:js", function (cb) {
    return gulp.src('wwwroot/js/*.min.js', { read: false }).pipe(clean());
});

gulp.task("clean:css", function (cb) {
    return gulp.src('wwwroot/css/*.min.css', { read: false }).pipe(clean());
});

gulp.task('publish-app-wwwroot:js', function (www) {
    pump([
        gulp.src("app/**/*.js"),
        uglify(),
        rename({ suffix: '.min' }),
        gulp.dest(config.appDest)
    ],
        www)
});

//Valutare, eventualmente, di concatenare tutti i file js in uno solo e minimizzarlo
//gulp.task('all-in-one', function (cb) {
//    pump([
//        gulp.src("app/**/*.js"),
//        concat("all-in-one.js"),
//        uglify(),
//        rename({ suffix: '.min' }),
//        gulp.dest(config.appDest + '/dist')
//    ],
//        cb)
//});

gulp.task('minify:css', function () {
    return gulp.src(config.css)
        .pipe(cleanCSS())
        .pipe(rename({
            suffix: '.min'
        }))
        .pipe(gulp.dest(config.cssDest));
});

gulp.task("clean", ["clean:js", "clean:css"]);
gulp.task('minify', ['minify:css']);

gulp.task("copy:angular", function () {

    return gulp.src(config.angular,
        { base: config.node_modules + "@angular/" })
        .pipe(gulp.dest(config.lib + "@angular/"));
});

gulp.task("copy:angularWebApi", function () {
    return gulp.src(config.angularWebApi,
        { base: config.node_modules })
        .pipe(gulp.dest(config.lib));
});

gulp.task("copy:corejs", function () {
    return gulp.src(config.corejs,
        { base: config.node_modules })
        .pipe(gulp.dest(config.lib));
});

gulp.task("copy:zonejs", function () {
    return gulp.src(config.zonejs,
        { base: config.node_modules })
        .pipe(gulp.dest(config.lib));
});

gulp.task("copy:reflectjs", function () {
    return gulp.src(config.reflectjs,
        { base: config.node_modules })
        .pipe(gulp.dest(config.lib));
});

gulp.task("copy:systemjs", function () {
    return gulp.src(config.systemjs,
        { base: config.node_modules })
        .pipe(gulp.dest(config.lib));
});

gulp.task("copy:rxjs", function () {
    return gulp.src(config.rxjs,
        { base: config.node_modules })
        .pipe(gulp.dest(config.lib));
});

gulp.task("copy:app", ["publish-app-wwwroot:js"], function () {
    return gulp.src(config.appOther)
        .pipe(gulp.dest(config.appDest));
});

gulp.task("copy:index", function () {
    return gulp.src(config.index)
        .pipe(gulp.dest(config.indexDest));
});

gulp.task("copy:jasmine", function () {
    return gulp.src(config.jasminejs,
        { base: config.node_modules + "jasmine-core/lib" })
        .pipe(gulp.dest(config.lib));
});

gulp.task("copy:es6-shim", function () {
    return gulp.src(config.shim_es6,
        { base: config.node_modules + "es6-shim" })
        .pipe(gulp.dest(config.lib + "es6-shim"));
});

gulp.task("copy:es5-shim", function () {
    return gulp.src(config.shim_es5,
        { base: config.node_modules + "es5-shim" })
        .pipe(gulp.dest(config.lib + "es5-shim"));
});

gulp.task("copy:signalr", function () {
    return gulp.src(config.signalr,
        { base: config.node_modules + "@aspnet" })
        .pipe(gulp.dest(config.lib));
});

gulp.task("copy:plugin_babel", function () {
    return gulp.src(config.plugin_babel,
        { base: config.node_modules + "systemjs-plugin-babel" })
        .pipe(gulp.dest(config.lib + "systemjs-plugin-babel"));
});


gulp.task("dependencies", [
    "copy:angular",
    "copy:angularWebApi",
    "copy:corejs",
    "copy:zonejs",
    "copy:reflectjs",
    "copy:systemjs",
    "copy:rxjs",
    "copy:jasmine",
    "copy:app",
    "copy:es6-shim",
    "copy:es5-shim",
    "copy:index",
    "copy:signalr",
    "copy:plugin_babel"
]);

gulp.task("watch", function () {
    return $.watch(config.app)
        .pipe(gulp.dest(config.appDest));
});

gulp.task("default", ["clean", 'minify', "dependencies","copy:app"]);