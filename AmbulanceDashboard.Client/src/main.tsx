// -----------------------------------------------------------------------
// <copyright file="main.tsx" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import {
  AuthenticationResult,
  Configuration,
  EventType,
  PublicClientApplication,
} from "@azure/msal-browser";
import { CssBaseline } from "@mui/material";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import React from "react";
import ReactDOM from "react-dom/client";

import App from "./app";
import { authority, clientId } from "./utils/auth";

const ua = window.navigator.userAgent;
const msie = ua.indexOf("MSIE ");
const msie11 = ua.indexOf("Trident/");
const msedge = ua.indexOf("Edge/");
const firefox = ua.indexOf("Firefox");
const isIE = msie > 0 || msie11 > 0;
const isEdge = msedge > 0;
const isFirefox = firefox > 0;

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

const configuration: Configuration = {
  auth: {
    clientId,
    authority,
    redirectUri: "/",
    postLogoutRedirectUri: "/",
  },
  cache: {
    cacheLocation: "localStorage",
    storeAuthStateInCookie: isIE || isEdge || isFirefox,
  },
  system: {
    allowRedirectInIframe: true,
  },
};

const pca = new PublicClientApplication(configuration);
pca.initialize().then(() => {
  pca.enableAccountStorageEvents();
  pca.handleRedirectPromise();
  pca.addEventCallback((m) => {
    if (m.eventType === EventType.LOGOUT_SUCCESS) {
      localStorage.clear();
    } else if (
      m.eventType === EventType.LOGIN_SUCCESS
      && (m.payload as AuthenticationResult)?.account
    ) {
      const { account } = (m.payload as AuthenticationResult);

      if (account.tenantId === "91d037fb-4714-4fe8-b084-68c083b8193f") {
        pca.setActiveAccount(account);
      }
    }
  });
});

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <ThemeProvider theme={defaultTheme}>
      <CssBaseline />
      <App pca={pca} />
    </ThemeProvider>
  </React.StrictMode>,
);
