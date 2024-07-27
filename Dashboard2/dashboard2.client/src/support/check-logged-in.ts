import { redirect } from "@tanstack/react-router";

import { Region } from "../queries/place-queries";
import { RootContext } from "../routes/__root";

export function redirectIfLoggedIn(context: RootContext) {
  if (context.loggedIn) {
    // eslint-disable-next-line @typescript-eslint/no-throw-literal
    throw redirect({
      to: "/home",
      search: { region: "All", district: "All", hub: "All" },
    });
  }
}

export function redirectIfLoggedOut(context: RootContext) {
  if (!context.loggedIn) {
    // eslint-disable-next-line @typescript-eslint/no-throw-literal
    throw redirect({
      to: "/",
    });
  }
}

export function redirectIfAdmin(context: RootContext) {
  if (context.isAdmin) {
    // eslint-disable-next-line @typescript-eslint/no-throw-literal
    throw redirect({
      to: "/users",
    });
  }
}

export function redirectIfNoPermission(
  permission: boolean,
  search: { region: Region; district: string; hub: string } = {
    region: "All",
    district: "All",
    hub: "All",
  },
) {
  if (!permission) {
    // eslint-disable-next-line @typescript-eslint/no-throw-literal
    throw redirect({
      to: "/home",
      search,
    });
  }
}
