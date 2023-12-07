import { useMutation } from "@tanstack/react-query";
import { IVehicleConfig } from "./useVehicleConfig";

export default function useSetVehicleConfig() {
  return useMutation({
    mutationFn: async (v: IVehicleConfig) => {
      const response = await fetch("/api/vehicle-settings", {
        method: "POST",
        headers: {
          contentType: "application/json",
        },
        body: JSON.stringify(v),
      });

      if (!response.ok) {
        throw new Error("Could not save vehicle config");
      }
    },
  });
}
