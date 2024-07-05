import { useQuery } from "@tanstack/react-query";

export function useDistricts(region: string) {
  return useQuery({
    queryKey: ["districts", region],
    queryFn: async () => {
      if (!region || region == "All") {
        return [];
      }

      const response = await fetch(`/api/places/${region}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (!response.ok) {
        throw new Error("Failed to fetch districts.");
      }

      return response.json() as Promise<string[]>;
    },
  });
}

export function useHubs(region: string, district: string) {
  return useQuery({
    queryKey: ["hubs", region, district],
    queryFn: async () => {
      if (
        !region ||
        region.toUpperCase() == "ALL" ||
        !district ||
        district.toUpperCase() == "ALL"
      ) {
        return [];
      }

      const response = await fetch(`/api/places/${region}/${district}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (!response.ok) {
        throw new Error("Failed to fetch hubs.");
      }

      return response.json() as Promise<string[]>;
    },
  });
}
