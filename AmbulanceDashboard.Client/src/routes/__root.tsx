// -----------------------------------------------------------------------
// <copyright file="__root.tsx" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { Outlet, createRootRoute } from "@tanstack/react-router";
import { Suspense, lazy } from "react";

import Layout from "../layout";

const TanStackRouterDevtools =
  process.env.NODE_ENV === "production"
    ? () => null // Render nothing in production
    : lazy(() =>
        import("@tanstack/router-devtools").then((res) => ({
          default: res.TanStackRouterDevtools,
        })),
      );

// eslint-disable-next-line import/prefer-default-export
export const Route = createRootRoute({
  component: () => (
    <Layout>
      <Outlet />
      <Suspense>
        <TanStackRouterDevtools />
      </Suspense>
    </Layout>
  ),
});
