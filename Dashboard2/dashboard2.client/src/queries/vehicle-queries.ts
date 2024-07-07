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

export const settingsOptions = (
  region: string | undefined,
  district: string | undefined,
  hub: string | undefined,
) => ({
  queryKey: ["status", region ?? "All", district ?? "All", hub ?? "All"],
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
