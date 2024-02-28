// -----------------------------------------------------------------------
// <copyright file="places.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useQuery } from "@tanstack/react-query";

import useQueryFunction from "./query-fn";

export const useDistricts = (region: string) => {
  const districtsQuery = useQueryFunction<{ names: string[] }>(`/api/places/${region}/districts`);

  return useQuery({
    queryKey: ["vehicles", "districts", region],
    queryFn: async () => {
      if (region === "All") {
        return [];
      }

      const result = await districtsQuery();

      return result.names;
    },
    throwOnError: true,
  });
};

export const useHubs = (region: string, district: string) => {
  const hubsQuery = useQueryFunction<{ names: string[] }>(`/api/places/${region}/${district}/hubs`);

  return useQuery({
    queryKey: ["vehicles", "hubs", region, district],
    queryFn: async () => {
      if (region === "All" || district === "All") {
        return [];
      }

      const result = await hubsQuery();

      return result.names;
    },
    throwOnError: true,
  });
};
