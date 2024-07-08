import { UseSuspenseQueryOptions } from "@tanstack/react-query";

export interface RolePermissions {
  id: string;
  name: string;
  permissions: string;
  vehicleConfiguration: string;
  vorData: string;
}

export const allRoleOptions: UseSuspenseQueryOptions<RolePermissions[]> = {
  queryKey: ["role"],
  queryFn: async () => {
    const response = await fetch("/api/roles", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error("Failed to fetch roles.");
    }

    return response.json() as Promise<RolePermissions[]>;
  },
};
