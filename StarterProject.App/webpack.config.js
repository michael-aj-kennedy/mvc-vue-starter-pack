const path = require("path");
const fs = require("fs");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const TerserPlugin = require("terser-webpack-plugin"); //swapped out for webpack 5 compatibility
const OptimizeCssAssetsPlugin = require("css-minimizer-webpack-plugin"); //swapped out for webpack 5 compatibility
const CopyWebpackPlugin = require("copy-webpack-plugin");

const srcPath = "./app/js/pages";
const entries = {
    main: "./app/js/index.js"
};

fs.readdirSync(srcPath).forEach((name) => {
    var entryName = name.replace(".js", "");
    const indexFile = `${srcPath}/${entryName}.js`;
    if (fs.existsSync(indexFile)) {
        entries[entryName] = indexFile;
    }
});

module.exports = {
    mode: "development", // set to production for release
    devtool: "source-map", // remove, or set to "yes" for prod release

    entry: entries, // Our entry object that contains all entries found
    output: {
        path: path.resolve(__dirname, "./wwwroot/dist"), // output directory
        filename: "js/[name].js", // i.e. <view-name>.js.
        libraryTarget: "module",
        module: true // Output as ESM
    },
    experiments: {
        outputModule: true // Required to enable module output
    },
    optimization: {
        minimizer: [
            `...`,
            new TerserPlugin({ parallel: true }),
            new CssMinimizerPlugin(),
        ],
    },
    plugins: [
        new MiniCssExtractPlugin({ filename: "css/[name].css" }),
        new CopyWebpackPlugin({
            patterns: [{ context: "app/", from: "img/**/*" }]
        }), // copy all images
        new webpack.DefinePlugin({
            __VUE_OPTIONS_API__: 'true',
            __VUE_PROD_DEVTOOLS__: 'false',
            __VUE_PROD_HYDRATION_MISMATCH_DETAILS__: 'false'
        })
    ],
    module: {
        rules: [
            {
                test: /\.html$/,
                type: 'asset/source',
            },
            {
                test: /\.scss$/,
                use: [
                    'style-loader',
                    'css-loader',
                    'sass-loader', // compiles SASS to CSS
                ],
            },
            {
                test: /\.css$/i,
                use: [MiniCssExtractPlugin.loader, 'css-loader'],
            },
            {
                // this will load all js files, transpile to es5
                test: "/\.js$/",
                exclude: /(node_modules)/,
                use: {
                    loader: "babel-loader", options: {
                        presets: ["@babel/preset-env", {
                            loose: true,
                            modules: true }] } }
            }
            //{
            //    test: /\.(jpe?g|png|gif|svg)$/,
            //    loaders: ["file-loader"]
            //},
            //{
            //    test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
            //    use: [{ loader: "file-loader", options: { name: "[name].[ext]", outputPath: "fonts/" } }]
            //}
        ]
    },
    resolve: {
        extensions: ['.js', '.scss', `...` ],
        // this makes webpack loads the development file, not the esm that can't be debugged on Chrome
        alias: {
            vue: "vue/dist/vue.esm-bundler.js"
        }
    }
}