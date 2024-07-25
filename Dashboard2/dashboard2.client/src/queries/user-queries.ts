import { useUpdate } from "./mutate-query";
import getOptions from "./get-options";
import { QueryClient, useSuspenseQuery } from "@tanstack/react-query";

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
  roleId: string;
  role: string;
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

export const meOptions = getOptions<UserInfo>("/api/users/me", ["user", "me"]);

export function useMe() {
  return useSuspenseQuery(meOptions);
}

export function preloadMe(queryClient: QueryClient) {
  return queryClient.ensureQueryData(meOptions);
}
