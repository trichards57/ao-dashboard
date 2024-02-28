// -----------------------------------------------------------------------
// <copyright file="config.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import useMutateFunction from "./mutate-fn";
import useQueryFunction from "./query-fn";

interface IVehicleName {
  registration: string;
  callSign: string;
  id: string;
}

interface IVehicle {
  registration: string;
  callSign: string;
  hub: string;
  district: string;
  region: string;
  vehicleType: string;
  forDisposal: boolean;
}

export const useVehicleNames = () => {
  const namesQuery = useQueryFunction<IVehicleName[]>("/api/vehicles/names");

  return useQuery({
    queryKey: ["vehicles", "names"],
    queryFn: namesQuery,
    throwOnError: true,
  });
};

export const useVehicleDetails = (id: string) => {
  const detailsQuery = useQueryFunction<IVehicle>(`/api/vehicles/${id}`);

  return useQuery({
    queryKey: ["vehicles", "details", id],
    queryFn: () => {
      if (id === "") {
        return null;
      }
      return detailsQuery();
    },
    throwOnError: true,
  });
};

export const useMutateVehicleDetails = (id:string) => {
  const detailsMutate = useMutateFunction<IVehicle>("/api/vehicles");
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ["vehicles", "details", id],
    mutationFn: detailsMutate,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["vehicles"] });
    },
  });
};
