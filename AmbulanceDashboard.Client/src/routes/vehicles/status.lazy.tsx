// -----------------------------------------------------------------------
// <copyright file="status.lazy.tsx" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { Chip, Paper } from "@mui/material";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { Navigate, createLazyFileRoute } from "@tanstack/react-router";
import { parseISO } from "date-fns";
import { useCallback, useState } from "react";

import { IVehicle, useVehicleStatuses } from "../../api-hooks/status";
import PlaceSelector from "../../components/place-selector";
import { useIsSjaAuthenticated } from "../../utils/auth";

function VehicleStatus() {
  const isAuthenticated = useIsSjaAuthenticated();
  const [region, setRegion] = useState("All");
  const [district, setDistrict] = useState("All");
  const [hub, setHub] = useState("All");
  const { data: status, isLoading } = useVehicleStatuses(region, district, hub);

  const load = useCallback((r: string, d: string, h: string) => {
    setRegion(r);
    setDistrict(d);
    setHub(h);
  }, []);

  if (!isAuthenticated) {
    return <Navigate to="/" />;
  }

  const columns: GridColDef<IVehicle>[] = [
    { field: "callSign", headerName: "Callsign" },
    { field: "registration", headerName: "Registration" },
    {
      field: "isVor",
      headerName: "Is VOR?",
      type: "boolean",
      renderCell: (params) => (params.value ? <Chip color="error" label="Yes" /> : <Chip color="success" label="No" />),
    },
    {
      field: "dueBack",
      headerName: "Expected Back",
      type: "date",
      valueGetter: (d) => (d.value == null ? null : parseISO(d.value)),
    },
    {
      field: "summary",
      headerName: "Summary",
      flex: 1,
    },
  ];

  return (
    <>
      <PlaceSelector onPlaceChanged={load} />
      <Paper sx={{ marginTop: "0.5rem" }}>
        <DataGrid
          loading={isLoading}
          columns={columns}
          rows={status ?? []}
          initialState={{
            pagination: {
              paginationModel: {
                pageSize: 25,
              },
            },
          }}
          disableRowSelectionOnClick
          getRowId={(row) => row.registration}
        />
      </Paper>
    </>
  );
}

// eslint-disable-next-line import/prefer-default-export
export const Route = createLazyFileRoute("/vehicles/status")({
  component: VehicleStatus,
});
