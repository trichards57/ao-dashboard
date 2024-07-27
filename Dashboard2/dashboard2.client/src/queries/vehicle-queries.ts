import {
  QueryClient,
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";

import getOptions from "./get-options";
import { Region } from "./place-queries";

export type VehicleType =
  | "Other"
  | "FrontLineAmbulance"
  | "AllWheelDrive"
  | "OffRoadAmbulance";

export interface VehicleSettings {
  id: string;
  registration: string;
  callSign: string;
  hub: string;
  district: string;
  region: Region;
  vehicleType: VehicleType;
  forDisposal: boolean;
}

export interface UpdateVehicleSettings {
  registration: string;
  hub: string;
  callSign: string;
  district: string;
  region: Region;
  vehicleType: VehicleType;
  forDisposal: boolean;
}

export const useUpdateVehicle = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (vehicle: UpdateVehicleSettings) => {
      const response = await fetch("/api/vehicles", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(vehicle),
      });

      if (!response.ok) {
        if (response.status === 404) {
          // eslint-disable-next-line @typescript-eslint/no-throw-literal
          throw notFound();
        }
        throw new Error("Failed to update vehicle.");
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["vehicle"] });
      queryClient.invalidateQueries({ queryKey: ["districts"] });
      queryClient.invalidateQueries({ queryKey: ["hubs"] });
      queryClient.invalidateQueries({ queryKey: ["status"] });
      queryClient.invalidateQueries({ queryKey: ["statistics"] });
      queryClient.invalidateQueries({ queryKey: ["settings"] });
    },
  });
};

export function vehicleSettings(id: string) {
  return getOptions<VehicleSettings>(`/api/vehicles/${id}`, ["vehicle", id]);
}

export function useVehicleSettings(id: string) {
  return useSuspenseQuery(vehicleSettings(id));
}

export function preloadVehicleSettings(queryClient: QueryClient, id: string) {
  return queryClient.ensureQueryData(vehicleSettings(id));
}

export function settingsOptions(region: Region, district: string, hub: string) {
  return getOptions<VehicleSettings[]>(
    `/api/vehicles?region=${region}&district=${district}&hub=${hub}`,
    ["settings", region, district, hub],
  );
}

export function useAllVehicleSettings(
  region: Region,
  district: string,
  hub: string,
) {
  return useSuspenseQuery(settingsOptions(region, district, hub));
}

export function preloadAllVehicleSettings(
  queryClient: QueryClient,
  region: Region,
  district: string,
  hub: string,
) {
  return queryClient.ensureQueryData(settingsOptions(region, district, hub));
}
