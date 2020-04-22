module.exports = {
    plugins: {
        autoprefixer: {},
        'postcss-preset-env': {
            stage: 0
        },
        'postcss-flexbugs-fixes': {},
        'postcss-object-fit-images': {},
        'postcss-inline-svg': {},
        'postcss-pxtorem': {
            propList: ['font', 'font-size', 'letter-spacing']
        }
    }
};
