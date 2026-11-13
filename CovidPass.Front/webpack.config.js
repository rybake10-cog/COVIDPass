// © 2020 Partners HealthCare Technology & Innovation Department
/* eslint-disable no-undef */
/* eslint-disable @typescript-eslint/no-var-requires */
const path = require('path');
const TerserJSPlugin = require('terser-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const OptimizeCSSAssetsPlugin = require('optimize-css-assets-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const { BundleAnalyzerPlugin } = require('webpack-bundle-analyzer');
const CopyPlugin = require('copy-webpack-plugin');

const isDev = process.env.NODE_ENV !== 'production';
const analyzePlugins = process.env.analyze === 'yes' ? [new BundleAnalyzerPlugin()] : [];

module.exports = {
  mode: isDev ? 'development' : 'production',
  entry: {
    app: ['./src/index.tsx']
  },
  output: {
    path: path.resolve(__dirname, '../CovidPass.Back/wwwroot'),
    publicPath: '/employee/',
    filename: '[name].js?[hash]'
  },
  optimization: {
    minimizer: [
      new TerserJSPlugin({
        cache: true,
        parallel: true,
        sourceMap: false // set to true if you want JS source maps
      }),
      new OptimizeCSSAssetsPlugin({})
    ]
  },
  module: {
    rules: [
      {
        enforce: 'pre',
        test: /\.tsx?$/,
        exclude: /node_modules/,
        loader: 'eslint-loader'
      },
      {
        test: /\.(j|t)sx?$/,
        use: 'ts-loader',
        exclude: /node_modules[\\,/](?!(query-string|strict-uri-encode|split-on-first)[\\,/]).*/
      },
      {
        test: /\.s?css$/,
        use: [
          MiniCssExtractPlugin.loader,
          'css-loader',
          {
            loader: 'postcss-loader',
            options: {
              ident: 'postcss',
              plugins: [require('autoprefixer')()]
            }
          },
          'sass-loader'
        ]
      },
      {
        test: /\.(ico|png|jpg|gif|svg|eot|ttf|woff2?)$/,
        loader: 'file-loader',
        options: {
          name: '[name].[ext]?[hash]'
        }
      }
    ]
  },
  plugins: [
    new CopyPlugin([{ from: 'public/locales', to: 'locales' }]),
    new MiniCssExtractPlugin({
      filename: '[name].css?[hash]',
      chunkFilename: '[id].css?[hash]'
    }),
    new HtmlWebpackPlugin({
      template: './public/index.html',
      favicon: './public/favicon.ico'
    }),
    {
      apply(compiler) {
        compiler.hooks.done.tap('CleanUpStatsPlugin', stats => {
          const children = stats.compilation.children;
          if (Array.isArray(children))
            stats.compilation.children = children.filter(child => child.name.indexOf('mini-css-extract-plugin') !== 0);
        });
      }
    },
    ...analyzePlugins
  ],
  resolve: {
    extensions: ['.tsx', '.ts', '.js'],
    alias: {
      root: path.resolve(__dirname, './src'),
      app: 'root/app',
      shared: 'root/shared',
      core: 'root/core',
      enums: 'root/enums',
      utils: 'root/utils',
      data: 'root/data'
    }
  },
  devtool: isDev ? '#eval-source-map' : 'none',
  performance: {
    maxEntrypointSize: 750000,
    maxAssetSize: 750000
  }
};
