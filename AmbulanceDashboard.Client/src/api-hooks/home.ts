// -----------------------------------------------------------------------
// <copyright file="home.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useQuery } from "@tanstack/react-query";

import useQueryFunction from "./query-fn";

interface IStatistics {
  totalVehicles: number;
  availableVehicles: number;
  vorVehicles: number;
  pastAvailability: Record<string, number>;
}

// eslint-disable-next-line import/prefer-default-export
export const useHomeStats = (region: string, district: string, hub: string) => {
  const homeQuery = useQueryFunction<IStatistics>(`/api/vors/byPlace/stats?region=${region}`);

  return useQuery({
    queryKey: ["vehicles", "stats", region, district, hub],
    queryFn: homeQuery,
    throwOnError: true,
  });
};
