import { useMutation, useQueryClient } from "@tanstack/react-query";

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
        throw new Error("Failed to update vehicle.");
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["vehicle"] });
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
      throw new Error("Failed to fetch vehicle settings.");
    }

    return response.json() as Promise<VehicleSettings>;
  },
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
      throw new Error("Failed to fetch vehicle settings.");
    }

    return response.json() as Promise<VehicleSettings[]>;
  },
});
