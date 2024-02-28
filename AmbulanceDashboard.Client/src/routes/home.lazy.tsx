// -----------------------------------------------------------------------
// <copyright file="home.lazy.tsx" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import {
  Box,
  Card,
  CardContent,
  CardHeader,
  CircularProgress,
  Unstable_Grid2 as Grid,
} from "@mui/material";
import {
  AxisConfig,
  ChartsLegend,
  ChartsTooltip,
  ChartsXAxis,
  ChartsYAxis,
  LinePlot,
  PiePlot,
  PieValueType,
  ResponsiveChartContainer,
} from "@mui/x-charts";
import { Navigate, createLazyFileRoute } from "@tanstack/react-router";
import { formatDate, parseISO } from "date-fns";
import { useCallback, useState } from "react";

import { useHomeStats } from "../api-hooks/home";
import PlaceSelector from "../components/place-selector";
import { useIsSjaAuthenticated } from "../utils/auth";

function Home() {
  const isAuthenticated = useIsSjaAuthenticated();
  const [region, setRegion] = useState("All");
  const [district, setDistrict] = useState("All");
  const [hub, setHub] = useState("All");

  const { data: stats, isLoading } = useHomeStats(region, district, hub);

  const load = useCallback((r:string, d:string, h:string) => {
    setRegion(r);
    setDistrict(d);
    setHub(h);
  }, []);

  if (!isAuthenticated) {
    return <Navigate to="/" />;
  }

  const data: PieValueType[] = [
    {
      id: "available",
      value: stats?.availableVehicles ?? 0,
      color: "#007a53",
      label: "Available",
    },
    {
      id: "vor",
      value: stats?.vorVehicles ?? 0,
      color: "lightGray",
      label: "VOR",
    },
  ];

  const historicData = Object.keys(stats?.pastAvailability ?? {}).map(
    (k) => stats?.pastAvailability[k] ?? 0,
  );

  const dates = Object.keys(stats?.pastAvailability ?? {}).map((k) => formatDate(parseISO(k), "MMM yy"));

  const xAxis: AxisConfig = {
    id: "historic",
    dataKey: "historic",
    data: dates,
    scaleType: "band",
    tickLabelStyle: {
      angle: 90,
      textAnchor: "start",
    },
  };

  return (
    <>
      <PlaceSelector onPlaceChanged={load} />
      <Grid container spacing={2} marginTop={2}>
        <Grid xs={12} sm={6} md={4}>
          <Card>
            <CardHeader title="Vehicle Availability" />
            <CardContent>
              {isLoading ? (
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    height: 200,
                  }}
                >
                  <CircularProgress size={100} />
                </Box>
              ) : (
                <ResponsiveChartContainer
                  height={200}
                  series={[
                    {
                      data,
                      innerRadius: 70,
                      outerRadius: 100,
                      startAngle: -90,
                      endAngle: 90,
                      type: "pie",
                    },
                  ]}
                >
                  <PiePlot />
                  <ChartsTooltip trigger="item" />
                  <ChartsLegend position={{ horizontal: "middle", vertical: "bottom" }} />
                </ResponsiveChartContainer>
              )}
            </CardContent>
          </Card>
        </Grid>
        <Grid xs={12} sm={6} md={8}>
          <Card>
            <CardHeader title="Historic Availability" />
            <CardContent>
              {isLoading ? (
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    height: 200,
                  }}
                >
                  <CircularProgress size={100} />
                </Box>
              ) : (
                <ResponsiveChartContainer
                  height={200}
                  series={[
                    {
                      data: historicData,
                      type: "line",
                      label: "Vehicles Available",
                      id: "historic",
                      color: "#007a53",
                      xAxisKey: "historic",
                    },
                  ]}
                  xAxis={[xAxis]}
                >
                  <LinePlot />
                  <ChartsXAxis position="bottom" />
                  <ChartsYAxis label="Vehicles" position="left" />
                </ResponsiveChartContainer>
              )}
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
