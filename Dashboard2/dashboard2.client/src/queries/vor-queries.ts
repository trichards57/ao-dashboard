import { notFound } from "@tanstack/react-router";

export interface VorStatistics {
  totalVehicles: number;
  availableVehicles: number;
  vorVehicles: number;
  pastAvailability: Record<string, number>;
}

export interface VorStatus {
  id: string;
  region: string;
  hub: string;
  district: string;
  registration: string;
  callSign: string;
  summary: string;
  isVor: boolean;
  dueBack: string;
}

export const statisticsOptions = (
  region: string | undefined,
  district: string | undefined,
  hub: string | undefined,
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
      if (response.status === 404) {
        throw notFound();
      }
      throw new Error("Failed to fetch hubs.");
    }

    return response.json() as Promise<VorStatistics>;
  },
});

export const statusOptions = (
  region: string | undefined,
  district: string | undefined,
  hub: string | undefined,
) => ({
  queryKey: ["status", region ?? "All", district ?? "All", hub ?? "All"],
  queryFn: async () => {
    const response = await fetch(
      `/api/vor?region=${region ?? "All"}&district=${district ?? "All"}&hub=${hub ?? "All"}`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (!response.ok) {
      if (response.status === 404) {
        throw notFound();
      }
      throw new Error("Failed to fetch hubs.");
    }

    return response.json() as Promise<VorStatus[]>;
  },
});
