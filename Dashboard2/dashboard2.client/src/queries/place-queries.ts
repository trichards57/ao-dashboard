import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";

export type Region =
  | "All"
  | "EastOfEngland"
  | "EastMidlands"
  | "London"
  | "NorthEast"
  | "NorthWest"
  | "SouthEast"
  | "SouthWest"
  | "WestMidlands";

export function districtOptions(region: Region) {
  return queryOptions({
    queryKey: ["districts", region],
    queryFn: async () => {
      if (region == "All") {
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

export function hubOptions(region: Region, district: string) {
  return queryOptions({
    queryKey: ["hubs", region, district],
    queryFn: async () => {
      if (region == "All" || district.toUpperCase() == "ALL") {
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

export function useDistricts(region: Region) {
  return useSuspenseQuery(districtOptions(region));
}

export function useHubs(region: Region, district: string) {
  return useSuspenseQuery(hubOptions(region, district));
}

export function preloadDistricts(queryClient: QueryClient, region: Region) {
  return queryClient.ensureQueryData(districtOptions(region));
}

export function preloadHubs(
  queryClient: QueryClient,
  region: Region,
  district: string,
) {
  return queryClient.ensureQueryData(hubOptions(region, district));
}
