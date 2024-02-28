// -----------------------------------------------------------------------
// <copyright file="error.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { InteractionRequiredAuthError } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import { Paper, Typography } from "@mui/material";

export default function ErrorDisplay({ error }: Readonly<{ error: Error }>) {
  const { instance } = useMsal();

  if (error instanceof InteractionRequiredAuthError) {
    instance.loginRedirect({
      scopes: ["offline_access", "User.Read"],
    });
  }

  return (
    <Paper sx={{ padding: "1rem" }}>
      <Typography variant="h5">Error Accessing Data</Typography>
      <Typography variant="body1">
        Sorry, there was an error talking to the server.  You can try refreshing the page.
        If that doesn&apos;t work you&apos;ll need to come back later.
      </Typography>
    </Paper>
  );
}
