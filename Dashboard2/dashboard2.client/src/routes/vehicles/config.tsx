import { createFileRoute, redirect, useSearch } from "@tanstack/react-router";
import validatePlace from "../../support/validate-place";
import PlacePicker from "../../components/place-picker";
import { settingsOptions } from "../../queries/vehicle-queries";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useState } from "react";
import PagePicker from "../../components/page-picker";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPen } from "@fortawesome/free-solid-svg-icons";
import { regionToString } from "../../components/region-converter";

const PageSize = 10;

const VehicleConfig = () => {
  const { region, district, hub } = useSearch({ from: "/vehicles/config" }) as {
    region: string;
    district: string;
    hub: string;
  };
  const { data } = useSuspenseQuery(settingsOptions(region, district, hub));
  const [page, setPage] = useState(0);

  const itemsToDisplay = (data ?? [])
    .sort((a, b) => a.callSign.localeCompare(b.callSign))
    .slice(page * PageSize, (page + 1) * PageSize);
  const showPagination = data.length > PageSize;

  return (
    <>
      <h1 className="title">Vehicle Setup</h1>
      <PlacePicker />
      <div className="table-container">
        <table className="table is-striped is-fullwidth">
          <thead>
            <tr>
              <th className="edit-col"></th>
              <th className="call-sign-col">Call Sign</th>
              <th className="reg-col">Registration</th>
              <th>Region</th>
              <th>District</th>
              <th>Hub</th>
            </tr>
          </thead>
          <tbody>
            {itemsToDisplay.length > 0 &&
              itemsToDisplay.map((i) => (
                <tr key={i.id}>
                  <td className="edit-col">
                    <a href={`/vehicles/edit/${i.id}`}>
                      <FontAwesomeIcon icon={faPen} />
                    </a>
                  </td>
                  <td className="call-sign-col">{i.callSign}</td>
                  <td className="reg-col">{i.registration}</td>
                  <td className="place-col">{regionToString(i.region)}</td>
                  <td className="place-col">{i.district}</td>
                  <td className="place-col">{i.hub}</td>
                </tr>
              ))}

            {itemsToDisplay.length === 0 && (
              <tr>
                <td colSpan={5} className="no-vehicles subtitle is-6">
                  No Vehicles
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
      {showPagination && (
        <PagePicker
          page={page}
          pages={Math.ceil(data.length / PageSize)}
          setPage={setPage}
        />
      )}
    </>
  );
};

export const Route = createFileRoute("/vehicles/config")({
  validateSearch: validatePlace,
  loaderDeps: ({ search: { region, district, hub } }) => ({
    region,
    district,
    hub,
  }),
  loader: ({ deps, context }) => {
    return context.queryClient.ensureQueryData(
      settingsOptions(deps.region, deps.district, deps.hub),
    );
  },
  component: VehicleConfig,
  beforeLoad: ({ context }) => {
    if (!context.loggedIn) {
      throw redirect({
        to: "/",
      });
    }
    if (!context.canEditVehicles) {
      throw redirect({
        to: "/home",
      });
    }
  },
});
