import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
  useMsal,
} from "@azure/msal-react";
import Layout from "../layout";
import PlaceSelector from "../place-selector";
import {
  Alert,
  AlertColor,
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  CircularProgress,
  Unstable_Grid2 as Grid,
  Snackbar,
  Typography,
} from "@mui/material";
import msLogo from "../assets/ms-sign-in.svg";
import { useCallback, useState } from "react";
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
import { formatDate, parseISO } from "date-fns";
import useVehicleStats from "./hooks";

export default function Home() {
  const { instance } = useMsal();
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
  const { stats, isLoading, load } = useVehicleStats(displayError);

  const data: PieValueType[] = [
    {
      id: "available",
      value: stats?.availableVehicles || 0,
      color: "#007a53",
      label: "Available",
    },
    {
      id: "vor",
      value: stats?.vorVehicles || 0,
      color: "lightGray",
      label: "VOR",
    },
  ];

  const historicData = Object.keys(stats?.pastAvailability ?? {}).map(
    (k) => stats?.pastAvailability[k] ?? 0
  );

  const dates = Object.keys(stats?.pastAvailability ?? {}).map((k) =>
    formatDate(parseISO(k), "MMM yy")
  );

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

  console.log(historicData);
  console.log(xAxis);

  return (
    <Layout>
      <AuthenticatedTemplate>
        <PlaceSelector onPlaceChanged={load} onError={displayError} />
        <Grid container spacing={2} marginTop={2}>
          <Grid xs={12} sm={4}>
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
                        type: "pie"
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
          <Grid xs={12} sm={8}>
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
      </AuthenticatedTemplate>
      <UnauthenticatedTemplate>
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
                  onClick={() =>
                    instance.loginRedirect({
                      scopes: ["offline_access", "User.Read"],
                    })
                  }
                >
                  <img src={msLogo} title="Sign in with Microsoft" />
                </Button>
              </Grid>
            </Grid>
          </CardContent>
        </Card>
      </UnauthenticatedTemplate>
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
