import {
  useMutation,
  useQueryClient,
  UseSuspenseQueryOptions,
} from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";

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

export const useUpdateUser = (id: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (user: UserRoleUpdate) => {
      const response = await fetch(`/api/users/${id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(user),
      });

      if (!response.ok) {
        if (response.status === 404) {
          throw notFound();
        }
        throw new Error("Failed to update user.");
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["user"] });
    },
  });
};

export const userSettings: (
  id: string,
) => UseSuspenseQueryOptions<UserWithRole> = (id: string) => ({
  queryKey: ["user", id],
  queryFn: async () => {
    const response = await fetch(`/api/users/${id}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      if (response.status === 404) {
        throw notFound();
      }
      throw new Error("Failed to fetch user settings.");
    }

    return response.json() as Promise<UserWithRole>;
  },
  staleTime: 10 * 60 * 1000
});

export const allUserOptions: UseSuspenseQueryOptions<UserWithRole[]> = {
  queryKey: ["user"],
  queryFn: async () => {
    const response = await fetch("/api/users", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error("Failed to fetch users.");
    }

    return response.json() as Promise<UserWithRole[]>;
  },
  staleTime: 10 * 60 * 1000
};

export const userMeOptions: UseSuspenseQueryOptions<UserInfo | null> = {
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
};
