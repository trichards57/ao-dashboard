// -----------------------------------------------------------------------
// <copyright file="home.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useQuery } from "@tanstack/react-query";

interface IStatistics {
  totalVehicles: number;
  availableVehicles: number;
  vorVehicles: number;
  pastAvailability: Record<string, number>;
}

// eslint-disable-next-line import/prefer-default-export
export const useHomeStats = (region: string, district: string, hub: string) =>
  useQuery({
    queryKey: ["vehicles", "stats", region, district, hub],
    queryFn: async () => {
      let uri = `/api/vors/byPlace/stats?region=${region}`;

      if (district !== "All") {
        uri += `&district=${district}`;
      }
      if (hub !== "All") {
        uri += `&hub=${hub}`;
      }

      const response = await fetch(uri);
      if (!response.ok) {
        throw new Error("Failed to fetch data");
      }

      return (await response.json()) as IStatistics;
    },
    throwOnError: true,
  });
