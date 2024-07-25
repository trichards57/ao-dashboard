import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";

export function districtOptions(region: string) {
  return queryOptions({
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
        if (response.status === 404) {
          throw notFound();
        }
        throw new Error("Failed to fetch districts.");
      }

      return response.json() as Promise<string[]>;
    },
    staleTime: 10 * 60 * 1000,
  });
}

export function hubOptions(region: string, district: string) {
  return queryOptions({
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
        if (response.status === 404) {
          throw notFound();
        }
        throw new Error("Failed to fetch hubs.");
      }

      return response.json() as Promise<string[]>;
    },
    staleTime: 10 * 60 * 1000,
  });
}

export function useDistricts(region: string) {
  return useSuspenseQuery(districtOptions(region));
}

export function useHubs(region: string, district: string) {
  return useSuspenseQuery(hubOptions(region, district));
}

export function preloadDistricts(queryClient: QueryClient, region: string) {
  return queryClient.ensureQueryData(districtOptions(region));
}

export function preloadHubs(
  queryClient: QueryClient,
  region: string,
  district: string,
) {
  return queryClient.ensureQueryData(hubOptions(region, district));
}
