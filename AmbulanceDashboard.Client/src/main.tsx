// -----------------------------------------------------------------------
// <copyright file="main.tsx" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { CssBaseline } from "@mui/material";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import React from "react";
import ReactDOM from "react-dom/client";

import App from "./app";

const defaultTheme = createTheme({
  typography: {
    fontFamily: ["Whitney Book", "Arial", "sans-serif"].join(","),
    h1: {
      fontFamily: ["Whitney Black", "Arial", "sans-serif"].join(","),
    },
    h6: {
      fontFamily: ["Whitney SemiBold", "Arial", "sans-serif"].join(","),
    },
    subtitle1: {
      fontFamily: ["Whitney SemiBold", "Arial", "sans-serif"].join(","),
    },
  },
  palette: {
    primary: {
      main: "#007a53",
    },
    success: {
      main: "#009f4d",
    },
  },
});

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <ThemeProvider theme={defaultTheme}>
      <CssBaseline />
      <App />
    </ThemeProvider>
  </React.StrictMode>,
);
