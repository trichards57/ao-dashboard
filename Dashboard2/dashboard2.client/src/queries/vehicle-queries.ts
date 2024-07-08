import { useMutation, useQueryClient } from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";

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

export const vehicleSettings = (id: string) => ({
  queryKey: ["vehicle", id],
  queryFn: async () => {
    const response = await fetch(`/api/vehicles/${id}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      if (response.status === 404) {
        throw notFound();
      }
      throw new Error("Failed to fetch vehicle settings.");
    }

    return response.json() as Promise<VehicleSettings>;
  },
  staleTime: 10 * 60 * 1000,
});

export const settingsOptions = (
  region: string | undefined,
  district: string | undefined,
  hub: string | undefined,
) => ({
  queryKey: ["settings", region ?? "All", district ?? "All", hub ?? "All"],
  queryFn: async () => {
    const response = await fetch(
      `/api/vehicles?region=${region ?? "All"}&district=${district ?? "All"}&hub=${hub ?? "All"}`,
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
      throw new Error("Failed to fetch vehicle settings.");
    }

    return response.json() as Promise<VehicleSettings[]>;
  },
  staleTime: 10 * 60 * 1000,
});
