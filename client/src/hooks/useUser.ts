import { useQuery } from "@tanstack/react-query";

interface IClientPrincipal {
  identityProvider: string;
  userId: string;
  userDetails: string;
  userRoles: string[];
  claims: { typ: string; val: string }[];
}

export default function useUser() {
  return useQuery({
    queryKey: ["user"],
    queryFn: async () => {
      const response = await fetch("/.auth/me");
      if (!response.ok) {
        throw new Error("Could not access user details.");
      }

      const payload = await response.json();
      const { clientPrincipal } = payload as {
        clientPrincipal: IClientPrincipal | null;
      };

      return clientPrincipal;
    },
  });
}
