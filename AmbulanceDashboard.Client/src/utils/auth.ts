// -----------------------------------------------------------------------
// <copyright file="auth.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useIsAuthenticated, useMsal } from "@azure/msal-react";

export const clientId = "ae7dee55-3f98-4bda-b5cf-7641de4a1776";
export const tenantId = "91d037fb-4714-4fe8-b084-68c083b8193f";
export const authority = `https://login.microsoftonline.com/${tenantId}`;

export const useIsSjaAuthenticated = () => {
  const { accounts } = useMsal();

  const account = accounts.find((a) => a.tenantId === tenantId);

  return useIsAuthenticated(account);
};
