// -----------------------------------------------------------------------
// <copyright file="index.lazy.tsx" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { useMsal } from "@azure/msal-react";
import {
  Button,
  Card,
  CardContent,
  CardHeader,
  Unstable_Grid2 as Grid,
  Typography,
} from "@mui/material";
import { Navigate, createLazyFileRoute } from "@tanstack/react-router";

import msLogo from "../assets/ms-sign-in.svg";
import { useIsSjaAuthenticated } from "../utils/auth";

function Index() {
  const isAuthenticated = useIsSjaAuthenticated();
  const { instance } = useMsal();

  if (isAuthenticated) {
    return <Navigate to="/home" />;
  }

  return (
    <Card>
      <CardHeader title="Welcome" />
      <CardContent>
        <Typography variant="body1">
          Welcome to the AO Ambulance Dashboard. You will need to sign in
          using your SJA account to use this application.
        </Typography>
        <Grid container>
          <Grid
            xs={12}
            display="flex"
            justifyContent="center"
            alignItems="center"
          >
            <Button
              onClick={() => instance.loginRedirect({
                scopes: ["offline_access", "User.Read"],
              })}
            >
              <img src={msLogo} title="Sign in with Microsoft" alt="Sign in with Microsoft" />
            </Button>
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
