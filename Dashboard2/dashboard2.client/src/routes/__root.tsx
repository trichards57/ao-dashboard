import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";
import React from "react";
import Navbar from "../components/navbar";
import { UserInfo, userMeOptions } from "../queries/user-queries";
import { QueryClient, useSuspenseQuery } from "@tanstack/react-query";

const TanStackRouterDevtools =
  process.env.NODE_ENV === "production"
    ? () => null
    : React.lazy(() =>
        import("@tanstack/router-devtools").then((res) => ({
          default: res.TanStackRouterDevtools,
        })),
      );

const Root = () => {
  const meQuery = useSuspenseQuery(userMeOptions);
  const me = meQuery.data;

  const canEditRoles = me?.role == "Administrator";
  const canEditVehicles = me?.otherClaims["VehicleConfiguration"] == "Edit";
  const canViewVor =
    me?.otherClaims["VORData"] == "Read" ||
    me?.otherClaims["VORData"] == "Edit";
  const canViewUsers =
    me?.otherClaims["Permissions"] == "Read" ||
    me?.otherClaims["Permissions"] == "Edit";

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
};

export interface RootContext extends UserInfo {
  queryClient: QueryClient;
  loggedIn: boolean;
}

export const Route = createRootRouteWithContext<RootContext>()({
  loader: (o) => {
    return o.context.queryClient.ensureQueryData(userMeOptions);
  },
  component: Root,
});
