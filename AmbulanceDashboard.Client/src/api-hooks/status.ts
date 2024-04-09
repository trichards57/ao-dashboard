// -----------------------------------------------------------------------
// <copyright file="status.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useQuery } from "@tanstack/react-query";

export interface IVehicleStatus {
  region: string;
  district: string;
  hub: string;
  registration: string;
  callSign: string;
  summary: string;
  isVor: boolean;
  dueBack: string;
}

// eslint-disable-next-line import/prefer-default-export
export const useVehicleStatuses = (
  region: string,
  district: string,
  hub: string,
) => {
  let uri = `/api/vors/byPlace?region=${region}`;

  if (district !== "All") {
    uri += `&district=${district}`;
  }
  if (hub !== "All") {
    uri += `&hub=${hub}`;
  }

  return useQuery({
    queryKey: ["vehicles", "status", region, district, hub],
    queryFn: async () => {
      const response = await fetch(uri);
      if (!response.ok) {
        throw new Error("Failed to fetch data");
      }

      return (await response.json()) as IVehicleStatus[];
    },
    throwOnError: true,
  });
};
