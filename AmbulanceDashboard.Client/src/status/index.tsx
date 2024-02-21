import { DataGrid, GridColDef } from "@mui/x-data-grid";
import Layout from "../layout";
import PlaceSelector from "../place-selector";
import { useCallback, useState } from "react";
import { Alert, AlertColor, Chip, Paper, Snackbar } from "@mui/material";
import useVehicleStatus, { IVehicle } from "./hooks";
import { parseISO } from "date-fns";

export default function VehicleStatus() {
  const [showSnackbar, setShowSnackbar] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState("");
  const [snackvarSeverity, setSnackbarSeverity] = useState(
    "info" as AlertColor
  );
  const displayError = useCallback((message: string, severity: AlertColor) => {
    setSnackbarMessage(message);
    setSnackbarSeverity(severity);
    setShowSnackbar(true);
  }, []);
  const { status, isLoading, load } = useVehicleStatus(displayError);

  const columns: GridColDef<IVehicle>[] = [
    { field: "callSign", headerName: "Callsign" },
    { field: "registration", headerName: "Registration" },
    {
      field: "isVor",
      headerName: "Is VOR?",
      type: "boolean",
      renderCell: (params) =>
        params.value ? <Chip color="error" label="Yes" /> : <Chip color="success" label="No" />,
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
    <Layout>
      <PlaceSelector onPlaceChanged={load} onError={displayError} />
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
      <Snackbar
        open={showSnackbar}
        autoHideDuration={6000}
        onClose={() => setShowSnackbar(false)}
      >
        <Alert
          onClose={() => setShowSnackbar(false)}
          severity={snackvarSeverity}
          variant="filled"
          sx={{ width: "100%" }}
        >
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </Layout>
  );
}
