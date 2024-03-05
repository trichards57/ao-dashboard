import {
  AxisConfig,
  ChartsXAxis,
  ChartsYAxis,
  LinePlot,
  ResponsiveChartContainer,
} from "@mui/x-charts";
import { formatDate, parseISO } from "date-fns";

import Loading from "./loading";

interface IVehiclePlotProps {
  isLoading: boolean;
  pastAvailability: Record<string, number>;
}

export default function VehiclePlot({
  isLoading,
  pastAvailability,
}: IVehiclePlotProps) {
  if (isLoading) {
    return <Loading />;
  }

  const historicData = Object.keys(pastAvailability).map(
    (k) => pastAvailability[k] ?? 0,
  );

  const dates = Object.keys(pastAvailability).map((k) =>
    formatDate(parseISO(k), "MMM yy"),
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

  return (
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
  );
}
