const webpack = require('webpack');
const path = require('path');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const ManifestPlugin = require('webpack-manifest-plugin');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;
const config = require('../assets/config');
const jqueryPath = path.resolve(__dirname, '../assets/general/js/jquery.js');

module.exports = (isDev, isAnlz) => ({
    devtool: 'source-map',
    entry: {
        app: './assets',
        // todo remove if not needed
        // explicitly put jquery into vendors chunk
        vendors: [jqueryPath],
    },
    output: {
        path: path.resolve(__dirname, '../dist'),
        publicPath: '/dist/',
        filename: '[name].[chunkhash].js',
        jsonpFunction: 'webpackJsonpDelete',
    },
    resolve: {
        extensions: ['.js', '.jsx'],
        modules: ['node_modules', path.resolve(__dirname, '../assets/')],
        alias: {
            // use our custom version of jquery
            // check the top of this file to understand what was excluded
            // see also https://github.com/jquery/jquery#modules
            jquery: jqueryPath,
        },
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: [
                    /node_modules/,
                    // When jquery is transpiled by babel it starts to load differently
                    // and breaks libraries which depend on it
                    // In order to use local version of jQuery
                    // and avoid that issue we should apply this exclusion
                    jqueryPath,
                ],
                loader: 'babel-loader',
            },
            {
                test: /\.hbs$/,
                loader: 'handlebars-loader',
            },
            {
                test: /\.(png|svg|jpe?g|gif)$/,
                exclude: /svg[\/\\]/,
                loader: 'file-loader',
                options: {
                    name: 'images/[name].[ext]',
                },
            },
            {
                test: /\.(woff2?|eot|ttf)$/,
                loader: 'file-loader',
                options: {
                    name: 'fonts/[name].[ext]',
                },
            },
            {
                test: /\.svg$/,
                include: /svg[\/\\]/,
                use: [
                    {
                        loader: 'svg-sprite-loader',
                        options: {
                            symbolId: 'icon-[name]',
                        },
                    },
                    {
                        loader: 'svgo-loader',
                        options: {
                            plugins: [
                                { removeNonInheritableGroupAttrs: true },
                                { collapseGroups: true },
                                { removeAttrs: { attrs: '(fill|stroke)' } },
                            ],
                        },
                    },
                ],
            },
            {
                test: /\.css$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: 'css-loader',
                        options: {
                            sourceMap: true,
                            importLoaders: 1,
                        },
                    },
                    {
                        loader: 'postcss-loader',
                        options: {
                            sourceMap: true,
                        },
                    },
                ],
            },
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: 'css-loader',
                        options: {
                            sourceMap: true,
                            importLoaders: 2,
                        },
                    },
                    {
                        loader: 'postcss-loader',
                        options: {
                            sourceMap: true,
                        },
                    },
                    {
                        loader: 'sass-loader',
                        options: {
                            sourceMap: true,
                        },
                    },
                    // Reads Sass vars from files or inlined in the options property
                    {
                        loader: '@epegzz/sass-vars-loader',
                        options: {
                            syntax: 'scss',
                            vars: config,
                        },
                    },
                ],
            },
        ],
    },
    optimization: {
        splitChunks: {
            chunks: 'all',
            automaticNameDelimiter: '.',
            maxInitialRequests: Infinity,
            minSize: 0,
            cacheGroups: {
                corejs: {
                    test: /[\\/]node_modules[\\/](core-js)[\\/]/,
                    name: 'corejs',
                    reuseExistingChunk: true,
                    priority: 20,
                },
                commons: {
                    chunks: 'initial',
                    name: 'commons',
                    minChunks: 2,
                    // set minSize to 0 in order to create this chunk even total size of the common modules is small
                    minSize: 0,
                    priority: 10,
                },
            },
        },
        runtimeChunk: 'single',
    },
    plugins: [
        // set correct path to your /dist folder
        new CleanWebpackPlugin(['dist'], {
            root: path.resolve(__dirname, '..'),
        }),

        new CopyWebpackPlugin([{ from: './assets/favicon', to: '../dist/favicon' }]),

        // uncomment if need jquery
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery',
            'window.jQuery': 'jquery',
        }),

        new ManifestPlugin({
            filter: ({ name }) => !name.endsWith('.map'),
            // https://delete.atlassian.net/browse/MSW-589
            // https://github.com/danethurber/webpack-manifest-plugin/issues/144#issuecomment-382779618
            seed: {},
        }),
        new MiniCssExtractPlugin({
            filename: '[name].[contenthash].css',
        }),
    ].concat(
        isAnlz
            ? new BundleAnalyzerPlugin({
                  analyzerPort: 8889,
              })
            : []
    ),
    stats: {
        children: false,
    },
});
