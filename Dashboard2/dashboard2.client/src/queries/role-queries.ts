import {
  useMutation,
  useQueryClient,
  UseSuspenseQueryOptions,
} from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";

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

export const useUpdateRole = (id: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (role: RolePermissionsUpdate) => {
      const response = await fetch(`/api/roles/${id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(role),
      });

      if (!response.ok) {
        if (response.status === 404) {
          throw notFound();
        }
        throw new Error("Failed to update role.");
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["role"] });
    },
  });
};

export const roleOptions: (
  id: string,
) => UseSuspenseQueryOptions<RolePermissions> = (id: string) => ({
  queryKey: ["role", id],
  queryFn: async () => {
    const response = await fetch(`/api/roles/${id}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      if (response.status === 404) {
        throw notFound();
      }
      throw new Error("Failed to fetch role settings.");
    }

    return response.json() as Promise<RolePermissions>;
  },
  staleTime: 10 * 60 * 1000,
});

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
      if (response.status === 404) {
        throw notFound();
      }
      throw new Error("Failed to fetch roles.");
    }

    return response.json() as Promise<RolePermissions[]>;
  },
  staleTime: 10 * 60 * 1000,
};
