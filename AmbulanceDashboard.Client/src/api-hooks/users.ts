// -----------------------------------------------------------------------
// <copyright file="users.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useQuery } from "@tanstack/react-query";
import { useAccount, useMsal } from "@azure/msal-react";

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
  const { accounts, instance } = useMsal();
  const account = useAccount(accounts[0] || {});

  const uri = "/api/user/permissions";

  return useQuery({
    queryKey: ["users", "me"],
    queryFn: async () => {
      if (account) {
        const authResponse = await instance.acquireTokenSilent({
          account,
          scopes: ["ae7dee55-3f98-4bda-b5cf-7641de4a1776/.default"],
        });
        const response = await fetch(uri, {
          headers: { Authorization: `Bearer ${authResponse.accessToken}` },
        });
        if (!response.ok) {
          throw new Error("Failed to fetch data");
        }

        return await response.json() as IUserPermissions;
      }

      return {
        userId: "",
        canViewVehicles: false,
        canEditVehicles: false,
        canViewPlaces: false,
        canEditVOR: false,
        canViewVOR: false,
      };
    },
    throwOnError: true,
  });
}
