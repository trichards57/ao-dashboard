// -----------------------------------------------------------------------
// <copyright file="config.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

interface IVehicleName {
  registration: string;
  callSign: string;
  id: string;
}

export interface IVehicleSettings {
  registration: string;
  callSign: string;
  hub: string;
  district: string;
  region: string;
  vehicleType: string;
  forDisposal: boolean;
}

export const useVehicleNames = () =>
  useQuery({
    queryKey: ["vehicles", "names"],
    queryFn: async () => {
      const response = await fetch("/api/vehicles/names");
      if (!response.ok) {
        throw new Error("Failed to fetch data");
      }

      return (await response.json()) as IVehicleName[];
    },
    throwOnError: true,
  });

export const useVehicleDetails = (id: string) =>
  useQuery({
    queryKey: ["vehicles", "details", id],
    queryFn: async () => {
      if (id === "") {
        return null;
      }
      const response = await fetch(`/api/vehicles/${id}`);
      if (!response.ok) {
        throw new Error("Failed to fetch data");
      }

      return (await response.json()) as IVehicleSettings;
    },
    throwOnError: true,
  });

export const useMutateVehicleDetails = (id: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ["vehicles", "details", id],
    mutationFn: async (data: IVehicleSettings) => {
      const response = await fetch("/api/vehicles", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      });
      if (!response.ok) {
        throw new Error("Failed to save data");
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["vehicles"] });
    },
  });
};
