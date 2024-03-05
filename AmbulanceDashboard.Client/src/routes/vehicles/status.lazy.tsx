// -----------------------------------------------------------------------
// <copyright file="status.lazy.tsx" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { Chip, Paper } from "@mui/material";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { createLazyFileRoute } from "@tanstack/react-router";
import { parseISO } from "date-fns";
import { useReducer } from "react";

import { IVehicle, useVehicleStatuses } from "../../api-hooks/status";
import {
  IPlaceAction,
  IPlaceState,
  PlaceSelector,
} from "../../components/place-selector";
import { useRequireAuth } from "../../hooks/auth";

function statusReducer(place: IPlaceState, action: IPlaceAction): IPlaceState {
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

const columns: GridColDef<IVehicle>[] = [
  { field: "callSign", headerName: "Callsign" },
  { field: "registration", headerName: "Registration" },
  {
    field: "isVor",
    headerName: "Is VOR?",
    type: "boolean",
    renderCell: (params) =>
      params.value ? (
        <Chip color="error" label="Yes" />
      ) : (
        <Chip color="success" label="No" />
      ),
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

function VehicleStatus() {
  const [place, dispatchPlace] = useReducer(statusReducer, {
    region: "All",
    district: "All",
    hub: "All",
  });
  const { data: status, isLoading } = useVehicleStatuses(
    place.region,
    place.district,
    place.hub,
  );

  useRequireAuth();

  return (
    <>
      <PlaceSelector place={place} dispatch={dispatchPlace} />
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
