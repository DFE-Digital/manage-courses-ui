/// <binding AfterBuild='default' />
module.exports = function (grunt) {
    'use strict';

    var sass = require('node-sass');

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        // Sass
        sass: {
            options: {
                implementation: sass,
                sourceMap: true,
                outputStyle: 'compressed',
                  includePaths: [
                    'node_modules/govuk_frontend_toolkit/stylesheets',
                    'node_modules/govuk-elements-sass/public/sass',
                ],
            },
            dist: {
                files: [
                    {
                        expand: true, // Recursive
                        cwd: "Styles", // The startup directory
                        src: ['**/*.scss'], // Source files
                        dest: 'wwwroot/css', // Destination
                        ext: '.css' // File extension
                    }
                ]
            }
        },

        copy: {
            vendor: {
                files: [
                    {
                        expand: true,
                        cwd: 'node_modules/govuk_frontend_toolkit/javascripts',
                        src: ['**/*.js'],
                        dest: 'wwwroot/javascripts'
                    },
                    {
                        expand: true,
                        cwd: 'node_modules/govuk_frontend_toolkit/images',
                        src: ['**/*.png' ],
                        dest: 'wwwroot/images'
                    },
                    {
                        expand: true,
                        cwd: 'node_modules/govuk_template_jinja/assets/stylesheets',
                        src: ['**/*.*'],
                        dest: 'wwwroot/css'
                    },
                    {
                        expand: true,
                        cwd: 'node_modules/govuk_template_jinja/assets/javascripts',
                        src: ['**/*.js'],
                        dest: 'wwwroot/javascripts'
                    },
                    {
                        expand: true,
                        cwd: 'node_modules/govuk_template_jinja/assets/images',
                        src: ['**/*.*'],
                        dest: 'wwwroot/images'
                    },
                ]
            },
            source: {
                files: [
                    {
                        expand: true,
                        cwd: 'javascripts',
                        src: ['**/*.js'],
                        dest: 'wwwroot/javascripts'
                    },
                    {
                        expand: true,
                        cwd: 'images',
                        src: ['**/*.png'],
                        dest: 'wwwroot/images'
                    }
                ]
            }
        }
    });

    // Load the plugin
    grunt.loadNpmTasks('grunt-sass');
    grunt.loadNpmTasks('grunt-contrib-copy');
    // Default task(s).
    grunt.registerTask('default', ['sass', 'copy']);
};