import { useQuery } from "@tanstack/react-query";

export interface UserInfo {
  realName: string;
  userId: string;
  email: string;
  role: string;
  amrUsed: string;
  lastAuthenticated: string;
  otherClaims: Record<string, string>;
}

export const userMeOptions = {
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
};

export function useUserMe() {
  return useQuery(userMeOptions);
}
