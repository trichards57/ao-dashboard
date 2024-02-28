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
  MenuItem,
  Paper,
  Select,
  Snackbar,
  Switch,
  TextField,
  Typography,
} from "@mui/material";
import { Navigate, createLazyFileRoute } from "@tanstack/react-router";
import {
  useCallback, useEffect, useMemo, useState,
} from "react";

import { useMutateVehicleDetails, useVehicleDetails, useVehicleNames } from "../../api-hooks/config";
import { useUserPermissions } from "../../api-hooks/users";
import { useIsSjaAuthenticated } from "../../utils/auth";

function VehicleConfiguration() {
  const isAuthenticated = useIsSjaAuthenticated();
  const [selectedVehicle, setSelectedVehicle] = useState({
    id: "",
    label: "",
  } as { id: string; label: string });
  const [showSnackbar, setShowSnackbar] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState("");
  const [snackbarSeverity, setSnackbarSeverity] = useState(
    "info" as AlertColor,
  );
  const displayError = useCallback((message: string, severity: AlertColor) => {
    setSnackbarMessage(message);
    setSnackbarSeverity(severity);
    setShowSnackbar(true);
  }, []);
  const { data: names, isLoading } = useVehicleNames();
  const nameOptions = useMemo(
    () => [...(names ?? [])]
      .sort((a, b) => a.registration.localeCompare(b.registration))
      .map((n) => ({
        id: n.id,
        label: `${n.registration} (${n.callSign})`,
      })) ?? [],
    [names],
  );
  const { data: details } = useVehicleDetails(selectedVehicle?.id ?? "");
  const {
    mutate: saveVehicle, isSuccess: saveSuccess, isError: saveError, reset: resetSave,
  } = useMutateVehicleDetails(selectedVehicle?.id ?? "");
  const [callSign, setCallSign] = useState("");
  const [region, setRegion] = useState("");
  const [district, setDistrict] = useState("");
  const [hub, setHub] = useState("");
  const [vehicleType, setVehicleType] = useState("");
  const [forDisposal, setForDisposal] = useState(false);
  const { data: permissions, isLoading: permissionsLoading } = useUserPermissions();

  useEffect(() => {
    if (details) {
      setCallSign(details.callSign);
      setRegion(details.region);
      setDistrict(details.district);
      setHub(details.hub);
      setVehicleType(details.vehicleType);
      setForDisposal(details.forDisposal);
    }
  }, [details]);

  useEffect(() => {
    if (saveSuccess) {
      displayError("Vehicle saved", "success");
      setSelectedVehicle({ id: "", label: "" });
      resetSave();
    }
  }, [saveSuccess, displayError, resetSave]);

  useEffect(() => {
    if (saveError) {
      displayError("There was a problem saving the vehicle.", "error");
      resetSave();
    }
  }, [saveError, displayError, resetSave]);

  if (!isAuthenticated) {
    return <Navigate to="/" />;
  }

  if (!permissionsLoading && !permissions?.canEditVehicles) {
    return <Navigate to="/home" />;
  }

  const callSignValid = callSign.length > 0;
  const districtValid = district.length > 0;
  const hubValid = hub.length > 0;
  const allValid = callSignValid && districtValid && hubValid;

  const save = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (selectedVehicle.id.length > 0 && details && allValid) {
      saveVehicle({
        registration: details.registration,
        callSign,
        region,
        district,
        hub,
        vehicleType,
        forDisposal,
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
                />
              </Grid>
              <Grid xs={12}>
                <TextField
                  fullWidth
                  label="Call Sign"
                  value={callSign}
                  onChange={(e) => setCallSign(e.target.value)}
                  error={!callSignValid}
                  helperText={!callSignValid ? "Call Sign is required" : ""}
                  id="call-sign"
                />
              </Grid>
              <Grid xs={12}>
                <FormControl fullWidth>
                  <InputLabel id="region-label">Region</InputLabel>
                  <Select
                    fullWidth
                    labelId="region-label"
                    label="Region"
                    value={region}
                    onChange={(e) => setRegion(e.target.value)}
                    id="region"
                  >
                    <MenuItem value="Unknown">Unknown</MenuItem>
                    <MenuItem value="NorthEast">North East</MenuItem>
                    <MenuItem value="NorthWest">North West</MenuItem>
                    <MenuItem value="EastOfEngland">East of England</MenuItem>
                    <MenuItem value="WestMidlands">West Midlands</MenuItem>
                    <MenuItem value="EastMidlands">East Midlands</MenuItem>
                    <MenuItem value="London">London</MenuItem>
                    <MenuItem value="SouthWest">South West</MenuItem>
                    <MenuItem value="SouthEast">South East</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
              <Grid xs={12}>
                <TextField
                  fullWidth
                  label="District"
                  value={district}
                  onChange={(e) => setDistrict(e.target.value)}
                  error={!districtValid}
                  helperText={!districtValid ? "District is required" : ""}
                  id="district"
                />
              </Grid>
              <Grid xs={12}>
                <TextField
                  fullWidth
                  label="Hub"
                  value={hub}
                  onChange={(e) => setHub(e.target.value)}
                  error={!hubValid}
                  helperText={!hubValid ? "Hub is required" : ""}
                  id="hub"
                />
              </Grid>
              <Grid xs={12}>
                <FormControl fullWidth>
                  <InputLabel id="vehicle-type-label" htmlFor="vehicle-type">
                    Vehicle Type
                  </InputLabel>
                  <Select
                    fullWidth
                    labelId="vehicle-type-label"
                    label="Vehicle Type"
                    value={vehicleType}
                    onChange={(e) => setVehicleType(e.target.value)}
                    id="vehicle-type"
                  >
                    <MenuItem value="Other">Other</MenuItem>
                    <MenuItem value="FrontLineAmbulance">Front Line</MenuItem>
                    <MenuItem value="AllWheelDrive">All Wheel Drive</MenuItem>
                    <MenuItem value="OffRoadAmbulance">
                      Off Road Ambulance
                    </MenuItem>
                  </Select>
                </FormControl>
              </Grid>
              <Grid xs={12}>
                <FormControlLabel
                  control={(
                    <Switch
                      checked={forDisposal}
                      onChange={(e) => setForDisposal(e.target.checked)}
                    />
                  )}
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
        open={showSnackbar}
        autoHideDuration={6000}
        onClose={() => setShowSnackbar(false)}
      >
        <Alert
          onClose={() => setShowSnackbar(false)}
          severity={snackbarSeverity}
          variant="filled"
          sx={{ width: "100%" }}
        >
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </>
  );
}

// eslint-disable-next-line import/prefer-default-export
export const Route = createLazyFileRoute("/vehicles/config")({
  component: VehicleConfiguration,
});
