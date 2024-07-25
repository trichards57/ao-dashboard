import { QueryClient, useSuspenseQuery } from "@tanstack/react-query";
import getOptions from "./get-options";
import { Region } from "./place-queries";

export interface VorStatistics {
  totalVehicles: number;
  availableVehicles: number;
  vorVehicles: number;
  pastAvailability: Record<string, number>;
}

export interface VorStatus {
  id: string;
  region: Region;
  hub: string;
  district: string;
  registration: string;
  callSign: string;
  summary: string;
  isVor: boolean;
  dueBack: string;
}

export function statisticsOptions(
  region: Region,
  district: string,
  hub: string,
) {
  return getOptions<VorStatistics>(
    `/api/vor/statistics?region=${region}&district=${district}&hub=${hub}`,
    ["statistics", region, district, hub],
  );
}

export function useStatistics(region: Region, district: string, hub: string) {
  return useSuspenseQuery(statisticsOptions(region, district, hub));
}

export function preloadStatistics(
  queryClient: QueryClient,
  region: Region,
  district: string,
  hub: string,
) {
  return queryClient.ensureQueryData(statisticsOptions(region, district, hub));
}

export function statusOptions(region: Region, district: string, hub: string) {
  return getOptions<VorStatus[]>(
    `/api/vor?region=${region}&district=${district}&hub=${hub}`,
    ["status", region, district, hub],
  );
}

export function useStatus(region: Region, district: string, hub: string) {
  return useSuspenseQuery(statusOptions(region, district, hub));
}

export function preloadStatus(
  queryClient: QueryClient,
  region: Region,
  district: string,
  hub: string,
) {
  return queryClient.ensureQueryData(statusOptions(region, district, hub));
}
