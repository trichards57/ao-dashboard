import { useQuery } from "@tanstack/react-query";

export interface VorStatistics {
  totalVehicles: number;
  availableVehicles: number;
  vorVehicles: number;
  pastAvailability: Record<string, number>;
}

export const statisticsOptions = (
  region: string,
  district: string,
  hub: string,
) => ({
  queryKey: ["statistics", region ?? "All", district ?? "All", hub ?? "All"],
  queryFn: async () => {
    const response = await fetch(
      `/api/vor/statistics?region=${region ?? "All"}&district=${district ?? "All"}&hub=${hub ?? "All"}`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (!response.ok) {
      throw new Error("Failed to fetch hubs.");
    }

    return response.json() as Promise<VorStatistics>;
  },
});

export function useStatistics(region: string, district: string, hub: string) {
  return useQuery(statisticsOptions(region, district, hub));
}
