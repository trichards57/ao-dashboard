// -----------------------------------------------------------------------
// <copyright file="index.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useMsal } from "@azure/msal-react";

function Logout() {
  const { instance } = useMsal();

  localStorage.clear();
  instance.logoutRedirect({
    onRedirectNavigate: () => false,
  });

  return (
    <div>
      <p>Logging out...</p>
    </div>
  );
}

export default Logout;
