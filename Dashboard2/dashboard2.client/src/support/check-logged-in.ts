import { redirect } from "@tanstack/react-router";
import { RootContext } from "../routes/__root";
import { Region } from "../queries/place-queries";

export function redirectIfLoggedIn(context: RootContext) {
  if (context.loggedIn) {
    throw redirect({
      to: "/home",
      search: { region: "All", district: "All", hub: "All" },
    });
  }
}

export function redirectIfLoggedOut(context: RootContext) {
  if (!context.loggedIn) {
    throw redirect({
      to: "/",
    });
  }
}

export function redirectIfAdmin(context: RootContext) {
  if (context.isAdmin) {
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
    throw redirect({
      to: "/home",
      search,
    });
  }
}
