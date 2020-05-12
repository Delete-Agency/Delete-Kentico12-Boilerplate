const merge = require('webpack-merge');
const baseConfig = require('./webpack/webpack.base.js');
const devConfig = require('./webpack/webpack.dev.js');
const prodConfig = require('./webpack/webpack.prod.js');

module.exports = (env, argv) => {
    const isDev = argv.mode === 'development';
    const isAnalyze = env && env.anlz;

    if (isDev) {
        return merge.smart(baseConfig(isDev, isAnalyze), devConfig());
    }

    return merge.smart(baseConfig(isDev, isAnalyze), prodConfig());
};
