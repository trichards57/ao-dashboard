import type { QueryClient } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { Outlet, createRootRouteWithContext } from "@tanstack/react-router";
import React from "react";

import Navbar from "../components/navbar";
import { UserInfo, preloadMe, useMe } from "../queries/user-queries";

const TanStackRouterDevtools =
  process.env.NODE_ENV === "production"
    ? () => null
    : React.lazy(() =>
        // eslint-disable-next-line import/no-extraneous-dependencies
        import("@tanstack/router-devtools").then((res) => ({
          default: res.TanStackRouterDevtools,
        })),
      );

function Root({
  canEditRoles,
  canEditVehicles,
  canViewUsers,
  canViewVor,
}: {
  canEditRoles: boolean;
  canEditVehicles: boolean;
  canViewUsers: boolean;
  canViewVor: boolean;
}) {
  const { data: me } = useMe();

  return (
    <>
      <Navbar
        canEditRoles={canEditRoles}
        canEditVehicles={canEditVehicles}
        canViewUsers={canViewUsers}
        canViewVor={canViewVor}
        loggedIn={me !== null}
        name={me?.realName}
      />
      <main className="container is-fluid">
        <Outlet />
      </main>
      <TanStackRouterDevtools />
      <ReactQueryDevtools />
    </>
  );
}

export interface RootContext extends UserInfo {
  queryClient: QueryClient;
  loggedIn: boolean;
  canEditRoles: boolean;
  canEditVehicles: boolean;
  canViewUsers: boolean;
  canViewVor: boolean;
  canEditUsers: boolean;
  isAdmin: boolean;
}

export const Route = createRootRouteWithContext<RootContext>()({
  loader: ({ context }) => preloadMe(context.queryClient),
  component: function Component() {
    return (
      <Root
        canEditRoles={Route.useRouteContext().canEditRoles}
        canEditVehicles={Route.useRouteContext().canEditVehicles}
        canViewUsers={Route.useRouteContext().canViewUsers}
        canViewVor={Route.useRouteContext().canViewVor}
      />
    );
  },
});
