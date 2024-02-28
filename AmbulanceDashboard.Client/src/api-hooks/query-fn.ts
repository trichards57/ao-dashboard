// -----------------------------------------------------------------------
// <copyright file="query-fn.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useAccount, useMsal } from "@azure/msal-react";

export default function useQueryFunction<T>(uri: string) {
  const { accounts, instance } = useMsal();
  const account = useAccount(accounts[0] || {});

  return async () => {
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

      return await response.json() as T;
    }

    throw new Error("Not logged in");
  };
}
