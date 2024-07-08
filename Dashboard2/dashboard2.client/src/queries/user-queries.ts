import { UseSuspenseQueryOptions } from "@tanstack/react-query";

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
