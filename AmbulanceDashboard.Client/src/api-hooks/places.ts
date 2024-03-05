// -----------------------------------------------------------------------
// <copyright file="places.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useQuery } from "@tanstack/react-query";

export const useDistricts = (region: string) =>
  useQuery({
    queryKey: ["vehicles", "districts", region],
    queryFn: async () => {
      if (region === "All") {
        return [];
      }

      const response = await fetch(`/api/places/${region}/districts`);
      if (!response.ok) {
        throw new Error("Failed to fetch data");
      }

      const result = (await response.json()) as { names: string[] };

      return result.names;
    },
    throwOnError: true,
  });

export const useHubs = (region: string, district: string) =>
  useQuery({
    queryKey: ["vehicles", "hubs", region, district],
    queryFn: async () => {
      if (region === "All" || district === "All") {
        return [];
      }

      const response = await fetch(`/api/places/${region}/${district}/hubs`);
      if (!response.ok) {
        throw new Error("Failed to fetch data");
      }

      const result = (await response.json()) as { names: string[] };

      return result.names;
    },
    throwOnError: true,
  });
