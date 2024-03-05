// -----------------------------------------------------------------------
// <copyright file="home.lazy.tsx" company="Tony Richards">
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
} from "@mui/material";
import { createLazyFileRoute } from "@tanstack/react-router";
import { Suspense, lazy, useReducer } from "react";

import { useHomeStats } from "../api-hooks/home";
import Loading from "../components/loading";
import {
  IPlaceAction,
  IPlaceState,
  PlaceSelector,
} from "../components/place-selector";
import { useRequireAuth } from "../hooks/auth";

const VehiclePie = lazy(() => import("../components/vehicle-pie"));
const VehiclePlot = lazy(() => import("../components/vehicle-plot"));

function homeReducer(place: IPlaceState, action: IPlaceAction): IPlaceState {
  switch (action.type) {
    case "region":
      return {
        ...place,
        region: action.region,
        district: action.region === "All" ? "All" : place.district,
        hub: action.region === "All" ? "All" : place.hub,
      };
    case "district":
      return {
        ...place,
        district: action.district,
        hub: action.district === "All" ? "All" : place.hub,
      };
    case "hub":
      return {
        ...place,
        hub: action.hub,
      };
    default:
      return place;
  }
}

function Home() {
  const [place, dispatchPlace] = useReducer(homeReducer, {
    region: "All",
    district: "All",
    hub: "All",
  });

  const { data: stats, isLoading } = useHomeStats(
    place.region,
    place.district,
    place.hub,
  );

  useRequireAuth();

  return (
    <>
      <PlaceSelector place={place} dispatch={dispatchPlace} />
      <Grid container spacing={2} marginTop={2}>
        <Grid xs={12} sm={6} md={4}>
          <Card>
            <CardHeader title="Vehicle Availability" />
            <CardContent>
              <Suspense fallback={<Loading />}>
                <VehiclePie
                  isLoading={isLoading}
                  availableVehicles={stats?.availableVehicles ?? 0}
                  vorVehicles={stats?.vorVehicles ?? 0}
                />
              </Suspense>
            </CardContent>
          </Card>
        </Grid>
        <Grid xs={12} sm={6} md={8}>
          <Card>
            <CardHeader title="Historic Availability" />
            <CardContent>
              <Suspense fallback={<Loading />}>
                <VehiclePlot
                  isLoading={isLoading}
                  pastAvailability={stats?.pastAvailability ?? {}}
                />
              </Suspense>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </>
  );
}

// eslint-disable-next-line import/prefer-default-export
export const Route = createLazyFileRoute("/home")({
  component: Home,
});
