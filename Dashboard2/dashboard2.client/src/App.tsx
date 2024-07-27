import { QueryClient } from "@tanstack/react-query";
import { RouterProvider } from "@tanstack/react-router";

import { useMe } from "./queries/user-queries";
import { RootContext } from "./routes/__root";

export default function InnerApp({
  queryClient,
  router,
}: {
  queryClient: QueryClient;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  router: any;
}) {
  const { data: me } = useMe();

  const isAdmin = me?.role === "Administrator";
  const canEditRoles = me?.role === "Administrator";
  const canEditVehicles = me?.otherClaims.VehicleConfiguration === "Edit";
  const canViewVor =
    me?.otherClaims.VORData === "Read" || me?.otherClaims.VORData === "Edit";
  const canViewUsers =
    me?.otherClaims.Permissions === "Read" ||
    me?.otherClaims.Permissions === "Edit";
  const canEditUsers = me?.otherClaims.Permissions === "Edit";

  const context: RootContext = {
    queryClient,
    realName: me?.realName ?? "",
    userId: me?.userId ?? "",
    email: me?.email ?? "",
    role: me?.role ?? "",
    amrUsed: me?.amrUsed ?? "",
    lastAuthenticated: me?.lastAuthenticated ?? "",
    otherClaims: me?.otherClaims ?? {},
    loggedIn: me !== null,
    canEditRoles,
    canEditVehicles,
    canViewVor,
    canViewUsers,
    canEditUsers,
    isAdmin,
  };

  return <RouterProvider router={router} context={context} />;
}
