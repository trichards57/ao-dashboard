// -----------------------------------------------------------------------
// <copyright file="app.tsx" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import type { PublicClientApplication } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { RouterProvider, createRouter } from "@tanstack/react-router";

import ErrorDisplay from "./components/error";
import { routeTree } from "./routeTree.gen";

const queryClient = new QueryClient();

// Create a new router instance
const router = createRouter({ routeTree, defaultErrorComponent: ErrorDisplay });

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router
  }
}

function InnerApp() {
  return <RouterProvider router={router} />;
}

function App({ pca }: Readonly<{ pca: PublicClientApplication }>) {
  return (
    <MsalProvider instance={pca}>
      <QueryClientProvider client={queryClient}>
        <InnerApp />
        <ReactQueryDevtools />
      </QueryClientProvider>
    </MsalProvider>
  );
}

export default App;
