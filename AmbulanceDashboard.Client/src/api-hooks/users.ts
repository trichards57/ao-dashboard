// -----------------------------------------------------------------------
// <copyright file="users.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useQuery } from "@tanstack/react-query";

interface IUserPermissions {
  userId: string;
  canViewVehicles: boolean;
  canEditVehicles: boolean;
  canViewPlaces: boolean;
  canEditVOR: boolean;
  canViewVOR: boolean;
}

export const useUserPermissions = () => {
  const uri = "/api/user/permissions";

  return useQuery({
    queryKey: ["users", "me"],
    queryFn: async () => {
      const response = await fetch(uri);
      if (!response.ok) {
        return {
          userId: "",
          canViewVehicles: false,
          canEditVehicles: false,
          canViewPlaces: false,
          canEditVOR: false,
          canViewVOR: false,
        };
      }

      return (await response.json()) as IUserPermissions;
    },
    throwOnError: true,
  });
};

export const useIsAuthenticated = () => {
  const { data: permissions, isLoading } = useUserPermissions();

  return { data: !!permissions?.userId, isLoading };
};
