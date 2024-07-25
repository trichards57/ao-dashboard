import {
  QueryClient,
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";
import getOptions from "./get-options";

export interface VehicleSettings {
  id: string;
  registration: string;
  callSign: string;
  hub: string;
  district: string;
  region: string;
  vehicleType: string;
  forDisposal: boolean;
}

export interface UpdateVehicleSettings {
  registration: string;
  hub: string;
  callSign: string;
  district: string;
  region: string;
  vehicleType: string;
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

export function settingsOptions(
  region: string | undefined,
  district: string | undefined,
  hub: string | undefined,
) {
  return getOptions<VehicleSettings[]>(
    `/api/vehicles?region=${region ?? "All"}&district=${district ?? "All"}&hub=${hub ?? "All"}`,
    ["settings", region ?? "All", district ?? "All", hub ?? "All"],
  );
}

export function useAllVehicleSettings(
  region: string | undefined,
  district: string | undefined,
  hub: string | undefined,
) {
  return useSuspenseQuery(settingsOptions(region, district, hub));
}

export function preloadAllVehicleSettings(
  queryClient: QueryClient,
  region: string | undefined,
  district: string | undefined,
  hub: string | undefined,
) {
  return queryClient.ensureQueryData(settingsOptions(region, district, hub));
}
