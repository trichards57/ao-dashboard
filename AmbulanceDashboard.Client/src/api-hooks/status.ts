// -----------------------------------------------------------------------
// <copyright file="status.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useQuery } from "@tanstack/react-query";

import useQueryFunction from "./query-fn";

export interface IVehicle {
  region: string;
  district: string;
  hub: string;
  registration: string;
  callSign: string;
  summary: string;
  isVor: boolean;
  dueBack: string;
}

export const useVehicleStatuses = (region: string, district: string, hub: string) => {
  let uri = `/api/vors/byPlace?region=${region}`;

  if (district !== "All") {
    uri += `&district=${district}`;
  }
  if (hub !== "All") {
    uri += `&hub=${hub}`;
  }

  const statusQuery = useQueryFunction<IVehicle[]>(uri);

  return useQuery({
    queryKey: ["vehicles", "status", region, district, hub],
    queryFn: statusQuery,
    throwOnError: true,
  });
};
