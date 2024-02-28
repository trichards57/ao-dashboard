// -----------------------------------------------------------------------
// <copyright file="users.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useQuery } from "@tanstack/react-query";

import useQueryFunction from "./query-fn";

interface IUserPermissions {
  userId: string;
  canViewVehicles: boolean;
  canEditVehicles: boolean;
  canViewPlaces: boolean;
  canEditVOR: boolean;
  canViewVOR: boolean;
}

// eslint-disable-next-line import/prefer-default-export
export const useUserPermissions = () => {
  const uri = "/api/user/permissions";

  const permissionsQuery = useQueryFunction<IUserPermissions>(uri);

  return useQuery({
    queryKey: ["users", "me"],
    queryFn: permissionsQuery,
    throwOnError: true,
  });
}
