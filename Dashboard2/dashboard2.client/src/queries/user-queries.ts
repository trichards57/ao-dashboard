import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";

import getOptions from "./get-options";
import useUpdate from "./mutate-query";

export interface UserInfo {
  realName: string;
  userId: string;
  email: string;
  role: string;
  amrUsed: string;
  lastAuthenticated: string;
  otherClaims: Record<string, string>;
}

export interface UserWithRole {
  id: string;
  name: string;
  roleId: string | undefined;
  role: string | undefined;
}

export interface UserRoleUpdate {
  roleId: string;
}

export function useUpdateUser(id: string) {
  return useUpdate("/api/users", ["user"])<UserRoleUpdate>(id);
}

export function userSettings(id: string) {
  return getOptions<UserWithRole>(`/api/users/${id}`, ["user", id]);
}

export function useUser(id: string) {
  return useSuspenseQuery(userSettings(id));
}

export function preloadUser(queryClient: QueryClient, id: string) {
  return queryClient.ensureQueryData(userSettings(id));
}

export const usersOptions = getOptions<UserWithRole[]>("/api/users", ["user"]);

export function useUsers() {
  return useSuspenseQuery(usersOptions);
}

export function preloadUsers(queryClient: QueryClient) {
  return queryClient.ensureQueryData(usersOptions);
}

export const meOptions = queryOptions({
  queryKey: ["me"],
  queryFn: async () => {
    const response = await fetch("/api/users/me", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      return Promise.resolve(null);
    }

    return response.json() as Promise<UserInfo>;
  },
  staleTime: 1000 * 60 * 5,
});

export function useMe() {
  return useSuspenseQuery(meOptions);
}

export function preloadMe(queryClient: QueryClient) {
  return queryClient.ensureQueryData(meOptions);
}
