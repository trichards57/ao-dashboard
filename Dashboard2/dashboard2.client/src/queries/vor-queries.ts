import { QueryClient, useSuspenseQuery } from "@tanstack/react-query";
import getOptions from "./get-options";

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

export function statisticsOptions(
  region: string,
  district: string,
  hub: string,
) {
  return getOptions<VorStatistics>(
    `/api/vor/statistics?region=${region}&district=${district}&hub=${hub}`,
    ["statistics", region, district, hub],
  );
}

export function useStatistics(region: string, district: string, hub: string) {
  return useSuspenseQuery(statisticsOptions(region, district, hub));
}

export function preloadStatistics(
  queryClient: QueryClient,
  region: string,
  district: string,
  hub: string,
) {
  return queryClient.ensureQueryData(statisticsOptions(region, district, hub));
}

export function statusOptions(
  region: string | undefined,
  district: string | undefined,
  hub: string | undefined,
) {
  return getOptions<VorStatus[]>(
    `/api/vor?region=${region ?? "All"}&district=${district ?? "All"}&hub=${hub ?? "All"}`,
    ["status", region ?? "All", district ?? "All", hub ?? "All"],
  );
}

export function useStatus(
  region: string | undefined,
  district: string | undefined,
  hub: string | undefined,
) {
  return useSuspenseQuery(statusOptions(region, district, hub));
}

export function preloadStatus(
  queryClient: QueryClient,
  region: string | undefined,
  district: string | undefined,
  hub: string | undefined,
) {
  return queryClient.ensureQueryData(statusOptions(region, district, hub));
}
