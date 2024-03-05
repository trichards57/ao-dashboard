import {
  ChartsLegend,
  ChartsTooltip,
  PiePlot,
  PieValueType,
  ResponsiveChartContainer,
} from "@mui/x-charts";

import Loading from "./loading";

interface IVehiclePieProps {
  isLoading: boolean;
  availableVehicles: number;
  vorVehicles: number;
}

export default function VehiclePie({
  availableVehicles,
  vorVehicles,
  isLoading,
}: IVehiclePieProps) {
  if (isLoading) {
    return <Loading />;
  }

  const data: PieValueType[] = [
    {
      id: "available",
      value: availableVehicles,
      color: "#007a53",
      label: "Available",
    },
    {
      id: "vor",
      value: vorVehicles,
      color: "lightGray",
      label: "VOR",
    },
  ];

  return (
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
  );
}
