// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
  document.getElementsByClassName("navbar-burger")[0]
    .addEventListener("click", function () {
      const target = document.getElementsByClassName("navbar-burger")[0];
      target.classList.toggle("is-active");
      target.ariaExpanded = target.classList.contains("is-active");
      document.getElementsByClassName("navbar-menu")[0].classList.toggle("is-active");
    });
});