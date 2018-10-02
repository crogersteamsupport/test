const path = require('path');

module.exports = {
    entry: {
        //'./app.js',
        ChatInit: './ChatInit.js'
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        //filename: 'bundle.js'
        filename: 'build/[name].js',
        sourceMapFilename: 'build/[name].js.map'
    },
    module: {
        rules: [
            {
                test: /\.m?js$/,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env', '@babel/preset-react']
                    }
                }
            }
        ]
    }
};