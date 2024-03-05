// -----------------------------------------------------------------------
// <copyright file="config.lazy.tsx" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { Search as SearchIcon } from "@mui/icons-material";
import {
  Alert,
  AlertColor,
  Autocomplete,
  Button,
  FormControl,
  FormControlLabel,
  Unstable_Grid2 as Grid,
  InputAdornment,
  InputLabel,
  NativeSelect,
  Paper,
  Snackbar,
  Switch,
  TextField,
  Typography,
} from "@mui/material";
import { Navigate, createLazyFileRoute } from "@tanstack/react-router";
import { useEffect, useMemo, useReducer, useState } from "react";

import {
  IVehicleSettings,
  useMutateVehicleDetails,
  useVehicleDetails,
  useVehicleNames,
} from "../../api-hooks/config";
import { useUserPermissions } from "../../api-hooks/users";

interface ISnackBarState {
  show: boolean;
  message: string;
  severity: AlertColor;
}

type SnackBarAction =
  | { type: "show"; message: string; severity: AlertColor }
  | { type: "hide" };

function snackbarReducer(
  state: ISnackBarState,
  action: SnackBarAction,
): ISnackBarState {
  switch (action.type) {
    case "show":
      return {
        show: true,
        message: action.message,
        severity: action.severity,
      };
    case "hide":
      return {
        ...state,
        show: false,
      };
    default:
      return state;
  }
}

type VehicleAction =
  | {
      type: "call-sign" | "region" | "district" | "hub" | "vehicle-type";
      payload: string;
    }
  | { type: "for-disposal"; payload: boolean }
  | { type: "load"; payload: IVehicleSettings };

interface IVehicleState {
  region: string;
  district: string;
  hub: string;
  callSign: string;
  vehicleType: string;
  forDisposal: boolean;
}

function formReducer(
  state: IVehicleState,
  action: VehicleAction,
): IVehicleState {
  switch (action.type) {
    case "call-sign":
      return {
        ...state,
        callSign: action.payload,
      };
    case "region":
      return {
        ...state,
        region: action.payload,
      };

    case "district":
      return {
        ...state,
        district: action.payload,
      };
    case "hub":
      return {
        ...state,
        hub: action.payload,
      };
    case "vehicle-type":
      return {
        ...state,
        vehicleType: action.payload,
      };
    case "for-disposal":
      return {
        ...state,
        forDisposal: action.payload,
      };
    case "load":
      return {
        region: action.payload.region,
        district: action.payload.district,
        hub: action.payload.hub,
        callSign: action.payload.callSign,
        vehicleType: action.payload.vehicleType,
        forDisposal: action.payload.forDisposal,
      };

    default:
      return state;
  }
}

function VehicleConfiguration() {
  const [selectedVehicle, setSelectedVehicle] = useState({
    id: "",
    label: "",
  } as { id: string; label: string });

  const [snackbarState, dispatchSnackbar] = useReducer(snackbarReducer, {
    show: false,
    message: "",
    severity: "info" as AlertColor,
  });
  const [formState, dispatchForm] = useReducer(formReducer, {
    region: "",
    district: "",
    hub: "",
    callSign: "",
    vehicleType: "",
    forDisposal: false,
  });

  const { data: names, isLoading } = useVehicleNames();
  const nameOptions = useMemo(
    () =>
      [...(names ?? [])]
        .sort((a, b) => a.registration.localeCompare(b.registration))
        .map((n) => ({
          id: n.id,
          label: `${n.registration} (${n.callSign})`,
        })) ?? [],
    [names],
  );
  const { data: details } = useVehicleDetails(selectedVehicle?.id ?? "");
  const {
    mutate: saveVehicle,
    isSuccess: saveSuccess,
    isError: saveError,
    reset: resetSave,
  } = useMutateVehicleDetails(selectedVehicle?.id ?? "");
  const { data: permissions, isLoading: permissionsLoading } =
    useUserPermissions();

  useEffect(() => {
    if (details) {
      dispatchForm({ type: "load", payload: details });
    }
  }, [details]);

  useEffect(() => {
    if (saveSuccess) {
      dispatchSnackbar({
        type: "show",
        message: "Vehicle saved",
        severity: "success",
      });
      setSelectedVehicle({ id: "", label: "" });
      resetSave();
    }
  }, [saveSuccess, resetSave]);

  useEffect(() => {
    if (saveError) {
      dispatchSnackbar({
        type: "show",
        message: "There was a problem saving the vehicle.",
        severity: "error",
      });
      resetSave();
    }
  }, [saveError, resetSave]);

  if (!permissionsLoading && !permissions?.canEditVehicles) {
    return <Navigate to="/home" />;
  }

  const callSignValid = formState.callSign.length > 0;
  const districtValid = formState.district.length > 0;
  const hubValid = formState.hub.length > 0;
  const allValid = callSignValid && districtValid && hubValid;

  const save = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (selectedVehicle.id.length > 0 && details && allValid) {
      saveVehicle({
        registration: details.registration,
        ...formState,
      });
    }
  };

  return (
    <>
      <Typography variant="h4" gutterBottom>
        Vehicle Settings
      </Typography>
      <Paper>
        <Autocomplete
          id="vehicle-search"
          sx={{ width: "100%" }}
          disableClearable
          disabled={isLoading}
          options={nameOptions}
          renderInput={(params) => (
            <TextField
              // eslint-disable-next-line react/jsx-props-no-spreading
              {...params}
              label="Find vehicle"
              placeholder="Find vehicle"
              InputProps={{
                ...params.InputProps,
                type: "search",
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
              }}
            />
          )}
          value={selectedVehicle}
          onChange={(_, v) => setSelectedVehicle(v ?? "")}
          isOptionEqualToValue={(option, value) => option.id === value.id}
        />
      </Paper>
      {details && (
        <Paper sx={{ marginTop: "0.5rem", padding: "1rem" }}>
          <form noValidate onSubmit={save}>
            <Grid container spacing={2}>
              <Grid xs={12}>
                <TextField
                  fullWidth
                  disabled
                  label="Registration"
                  value={details.registration}
                  variant="standard"
                  id="registration"
                />
              </Grid>
              <Grid xs={12}>
                <TextField
                  fullWidth
                  label="Call Sign"
                  value={formState.callSign}
                  onChange={(e) =>
                    dispatchForm({ type: "call-sign", payload: e.target.value })
                  }
                  error={!callSignValid}
                  helperText={!callSignValid ? "Call Sign is required" : ""}
                  id="call-sign"
                  variant="standard"
                />
              </Grid>
              <Grid xs={12}>
                <FormControl fullWidth>
                  <InputLabel id="region-label" variant="standard">
                    Region
                  </InputLabel>
                  <NativeSelect
                    fullWidth
                    value={formState.region}
                    onChange={(e) =>
                      dispatchForm({ type: "region", payload: e.target.value })
                    }
                    id="region"
                  >
                    <option value="Unknown">Unknown</option>
                    <option value="NorthEast">North East</option>
                    <option value="NorthWest">North West</option>
                    <option value="EastOfEngland">East of England</option>
                    <option value="WestMidlands">West Midlands</option>
                    <option value="EastMidlands">East Midlands</option>
                    <option value="London">London</option>
                    <option value="SouthWest">South West</option>
                    <option value="SouthEast">South East</option>
                  </NativeSelect>
                </FormControl>
              </Grid>
              <Grid xs={12}>
                <TextField
                  fullWidth
                  label="District"
                  value={formState.district}
                  onChange={(e) =>
                    dispatchForm({ type: "district", payload: e.target.value })
                  }
                  error={!districtValid}
                  helperText={!districtValid ? "District is required" : ""}
                  id="district"
                  variant="standard"
                />
              </Grid>
              <Grid xs={12}>
                <TextField
                  fullWidth
                  label="Hub"
                  value={formState.hub}
                  onChange={(e) =>
                    dispatchForm({ type: "hub", payload: e.target.value })
                  }
                  error={!hubValid}
                  helperText={!hubValid ? "Hub is required" : ""}
                  id="hub"
                  variant="standard"
                />
              </Grid>
              <Grid xs={12}>
                <FormControl fullWidth>
                  <InputLabel
                    id="vehicle-type-label"
                    htmlFor="vehicle-type"
                    variant="standard"
                  >
                    Vehicle Type
                  </InputLabel>
                  <NativeSelect
                    fullWidth
                    value={formState.vehicleType}
                    onChange={(e) =>
                      dispatchForm({
                        type: "vehicle-type",
                        payload: e.target.value,
                      })
                    }
                    id="vehicle-type"
                  >
                    <option value="Other">Other</option>
                    <option value="FrontLineAmbulance">Front Line</option>
                    <option value="AllWheelDrive">All Wheel Drive</option>
                    <option value="OffRoadAmbulance">Off Road Ambulance</option>
                  </NativeSelect>
                </FormControl>
              </Grid>
              <Grid xs={12}>
                <FormControlLabel
                  control={
                    <Switch
                      checked={formState.forDisposal}
                      onChange={(e) =>
                        dispatchForm({
                          type: "for-disposal",
                          payload: e.target.checked,
                        })
                      }
                      id="for-disposal"
                    />
                  }
                  label="Marked for Disposal"
                />
              </Grid>
              <Grid xs={12} sx={{ textAlign: "center" }}>
                <Button type="submit" variant="contained" disabled={!allValid}>
                  Save
                </Button>
              </Grid>
            </Grid>
          </form>
        </Paper>
      )}
      <Snackbar
        open={snackbarState.show}
        autoHideDuration={6000}
        onClose={() => dispatchSnackbar({ type: "hide" })}
      >
        <Alert
          onClose={() => dispatchSnackbar({ type: "hide" })}
          severity={snackbarState.severity}
          variant="filled"
          sx={{ width: "100%" }}
        >
          {snackbarState.message}
        </Alert>
      </Snackbar>
    </>
  );
}

// eslint-disable-next-line import/prefer-default-export
export const Route = createLazyFileRoute("/vehicles/config")({
  component: VehicleConfiguration,
});
