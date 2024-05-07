module.exports = {
  plugins: [
    require('@fullhuman/postcss-purgecss')({
      content: ['./Components/**/*.razor', '../Dashboard.Client/Components/**/*.razor', '../Dashboard.Client/Pages/**/*.razor']
    }),
    require('autoprefixer'),
    require('cssnano')({
      preset: 'default',
    }),
  ]
};
