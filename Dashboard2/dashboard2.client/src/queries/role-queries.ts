import { QueryClient, useSuspenseQuery } from "@tanstack/react-query";
import getOptions from "./get-options";
import { useUpdate } from "./mutate-query";

export interface RolePermissions {
  id: string;
  name: string;
  permissions: string;
  vehicleConfiguration: string;
  vorData: string;
}

export interface RolePermissionsUpdate {
  permissions: string;
  vehicleConfiguration: string;
  vorData: string;
}

export function useUpdateRole(id: string) {
  return useUpdate("/api/roles", ["role"])<RolePermissionsUpdate>(id);
}

export function roleOptions(id: string) {
  return getOptions<RolePermissions>(`/api/roles/${id}`, ["role", id]);
}

export const allRoleOptions = getOptions<RolePermissions[]>("/api/roles", [
  "role",
]);

export function useRole(id: string) {
  return useSuspenseQuery(roleOptions(id));
}

export function useRoles() {
  return useSuspenseQuery(allRoleOptions);
}

export function preloadRole(queryClient: QueryClient, id: string) {
  return queryClient.ensureQueryData(roleOptions(id));
}

export function preloadRoles(queryClient: QueryClient) {
  return queryClient.ensureQueryData(allRoleOptions);
}
