// -----------------------------------------------------------------------
// <copyright file="index.lazy.tsx" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import {
  Card,
  CardContent,
  CardHeader,
  Unstable_Grid2 as Grid,
  Typography,
} from "@mui/material";
import { Navigate, createLazyFileRoute } from "@tanstack/react-router";

import { useIsAuthenticated } from "../api-hooks/users";
import msLogo from "../assets/ms-sign-in.svg";

function Index() {
  const { data: isAuthenticated, isLoading: authLoading } =
    useIsAuthenticated();

  if (!authLoading && isAuthenticated) {
    return <Navigate to="/home" />;
  }

  return (
    <Card>
      <CardHeader title="Welcome" />
      <CardContent>
        <Typography variant="body1">
          Welcome to the AO Ambulance Dashboard. You will need to sign in using
          your SJA account to use this application.
        </Typography>
        <Grid container>
          <Grid
            xs={12}
            display="flex"
            justifyContent="center"
            alignItems="center"
          >
            <a href="/MicrosoftIdentity/Account/SignIn">
              <img
                src={msLogo}
                title="Sign in with Microsoft"
                alt="Sign in with Microsoft"
              />
            </a>
          </Grid>
        </Grid>
      </CardContent>
    </Card>
  );
}

// eslint-disable-next-line import/prefer-default-export
export const Route = createLazyFileRoute("/")({
  component: Index,
});
