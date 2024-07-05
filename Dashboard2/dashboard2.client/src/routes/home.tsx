import { createFileRoute, redirect, useSearch } from "@tanstack/react-router";
import PlacePicker from "../components/place-picker";
import { statisticsOptions } from "../queries/vor-queries";
import { useSuspenseQuery } from "@tanstack/react-query";
import { Line, Pie } from "react-chartjs-2";
import "chart.js/auto";
import validatePlace from "../support/validate-place";

function dateLabel(d: string) {
  const date = new Date(d);

  const months = [
    "Jan",
    "Feb",
    "Mar",
    "Apr",
    "May",
    "Jun",
    "Jul",
    "Aug",
    "Sep",
    "Oct",
    "Nov",
    "Dec",
  ];
  const month = months[date.getMonth()];
  const year = date.getFullYear() - 2000;

  return `${month} ${year}`;
}

const Home = () => {
  const { region, district, hub } = useSearch({ from: "/home" }) as {
    region: string;
    district: string;
    hub: string;
  };
  const { data } = useSuspenseQuery(statisticsOptions(region, district, hub));

  const availabilityData = {
    labels: ["Available", "VOR"],
    datasets: [
      {
        label: "Vehicle Status",
        data: [data.availableVehicles, data.vorVehicles],
        backgroundColor: ["#007a53", "#97999b"],
      },
    ],
  };
  const availabilityOptions = {
    responsive: true,
    circumference: 180,
    rotation: -90,
    cutout: "75%", // Adjust this to change the thickness of the arc
    plugins: {
      legend: {
        display: false,
      },
    },
    aspectRatio: 1, // Keeps the chart as a circle on all screen sizes
  };

  const historicData = {
    labels: Object.keys(data.pastAvailability).map(dateLabel),
    datasets: [
      {
        label: "Available Vehicles",
        data: Object.values(data.pastAvailability),
        fill: false,
      },
    ],
  };

  const historicOptions = {
    responsive: true,
    plugins: {
      legend: {
        display: false,
      },
    },
  };

  return (
    <>
      <PlacePicker />
      <div className="columns">
        <div className="column">
          <h2 className="subtitle is-5">Vehicle Availability</h2>
          <div className="vehicle-availability-container">
            <Pie
              data={availabilityData}
              options={availabilityOptions}
              id="availability-chart"
            />
          </div>
        </div>
        <div className="column">
          <h2 className="subtitle is-5">Historic Availability</h2>
          <div className="historic-availability-container">
            <Line
              data={historicData}
              options={historicOptions}
              id="historic-data-chart"
            />
          </div>
        </div>
      </div>
    </>
  );
};

export const Route = createFileRoute("/home")({
  beforeLoad: ({ context }) => {
    if (!context.loggedIn) {
      throw redirect({
        to: "/",
      });
    }
  },
  validateSearch: validatePlace,
  loaderDeps: ({ search: { region, district, hub } }) => ({
    region,
    district,
    hub,
  }),
  loader: ({ deps, context }) => {
    return context.queryClient.ensureQueryData(
      statisticsOptions(deps.region, deps.district, deps.hub),
    );
  },
  component: Home,
});
